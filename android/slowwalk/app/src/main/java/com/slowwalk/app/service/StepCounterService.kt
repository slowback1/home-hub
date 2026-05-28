package com.slowwalk.app.service

import android.app.Notification
import android.app.NotificationChannel
import android.app.NotificationManager
import android.app.PendingIntent
import android.app.Service
import android.content.Intent
import android.hardware.Sensor
import android.hardware.SensorEvent
import android.hardware.SensorEventListener
import android.hardware.SensorManager
import android.os.Binder
import android.os.IBinder
import androidx.core.app.NotificationCompat
import com.slowwalk.app.MainActivity
import com.slowwalk.app.data.local.SlowWalkDatabase
import com.slowwalk.app.data.local.entity.InProgressSessionEntity
import com.slowwalk.app.data.local.entity.WalkSessionEntity
import com.slowwalk.app.ui.walk.WalkState
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.Job
import kotlinx.coroutines.SupervisorJob
import kotlinx.coroutines.delay
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.flow.asStateFlow
import kotlinx.coroutines.flow.update
import kotlinx.coroutines.isActive
import kotlinx.coroutines.launch
import java.util.UUID

private const val CHANNEL_ID = "walk_session"
private const val NOTIFICATION_ID = 1
private const val TIMER_INTERVAL_MS = 1_000L
private const val CHECKPOINT_INTERVAL_MS = 30_000L

class StepCounterService : Service(), SensorEventListener {

    inner class LocalBinder : Binder() {
        fun getService(): StepCounterService = this@StepCounterService
    }

    private val binder = LocalBinder()
    private val serviceScope = CoroutineScope(SupervisorJob() + Dispatchers.Main)

    private val _state = MutableStateFlow(WalkState())
    val state: StateFlow<WalkState> = _state.asStateFlow()

    private var sensorManager: SensorManager? = null
    private var sensorBaseline: Int = 0
    private var accumulatedSteps: Int = 0
    private var startEpochMillis: Long = 0L
    private var sessionClientId: String = ""
    private var timerJob: Job? = null
    private var checkpointJob: Job? = null

    override fun onCreate() {
        super.onCreate()
        createNotificationChannel()
    }

    override fun onBind(intent: Intent?): IBinder = binder

    override fun onStartCommand(intent: Intent?, flags: Int, startId: Int): Int {
        when (intent?.action) {
            ACTION_STOP -> stopWalk()
            ACTION_RESUME -> {
                val accumulated = intent.getIntExtra(EXTRA_ACCUMULATED_STEPS, 0)
                val elapsed = intent.getIntExtra(EXTRA_ELAPSED_SECONDS, 0)
                val clientId = intent.getStringExtra(EXTRA_CLIENT_ID) ?: UUID.randomUUID().toString()
                startWalk(
                    resumedAccumulatedSteps = accumulated,
                    resumedElapsedSeconds = elapsed,
                    clientId = clientId,
                    isResumed = true,
                )
            }
            else -> startWalk()
        }
        return START_STICKY
    }

    private fun startWalk(
        resumedAccumulatedSteps: Int = 0,
        resumedElapsedSeconds: Int = 0,
        clientId: String = UUID.randomUUID().toString(),
        isResumed: Boolean = false,
    ) {
        startEpochMillis = System.currentTimeMillis() - resumedElapsedSeconds * 1_000L
        sessionClientId = clientId
        accumulatedSteps = resumedAccumulatedSteps
        sensorBaseline = 0

        sensorManager = getSystemService(SENSOR_SERVICE) as SensorManager
        val sensor = sensorManager?.getDefaultSensor(Sensor.TYPE_STEP_COUNTER)
        sensorManager?.registerListener(this, sensor, SensorManager.SENSOR_DELAY_NORMAL)

        _state.update { WalkState(isActive = true, steps = resumedAccumulatedSteps, isResumed = isResumed) }

        timerJob = serviceScope.launch {
            while (isActive) {
                delay(TIMER_INTERVAL_MS)
                val elapsed = (System.currentTimeMillis() - startEpochMillis) / 1_000L
                _state.update { it.copy(elapsedSeconds = elapsed) }
                updateNotification()
            }
        }

        checkpointJob = serviceScope.launch(Dispatchers.IO) {
            while (isActive) {
                delay(CHECKPOINT_INTERVAL_MS)
                saveCheckpoint()
            }
        }

        startForeground(NOTIFICATION_ID, buildNotification())
    }

    fun stopWalk() {
        timerJob?.cancel()
        checkpointJob?.cancel()
        sensorManager?.unregisterListener(this)
        val finalState = _state.value

        serviceScope.launch(Dispatchers.IO) {
            val db = SlowWalkDatabase.getInstance(applicationContext)
            db.walkSessionDao().insertOrReplace(
                WalkSessionEntity(
                    clientId = sessionClientId,
                    startedAt = startEpochMillis,
                    durationSeconds = finalState.elapsedSeconds.toInt(),
                    stepCount = finalState.steps,
                )
            )
            db.inProgressSessionDao().delete()
        }

        _state.update { WalkState() }
        stopForeground(STOP_FOREGROUND_REMOVE)
        stopSelf()
    }

    private suspend fun saveCheckpoint() {
        val current = _state.value
        SlowWalkDatabase.getInstance(applicationContext).inProgressSessionDao().save(
            InProgressSessionEntity(
                clientId = sessionClientId,
                startedAt = startEpochMillis,
                accumulatedSteps = current.steps,
                elapsedSeconds = current.elapsedSeconds.toInt(),
                sensorBaselineAtLastCheckpoint = sensorBaseline,
            )
        )
    }

    override fun onSensorChanged(event: SensorEvent?) {
        event ?: return
        if (event.sensor.type != Sensor.TYPE_STEP_COUNTER) return
        val total = event.values[0].toInt()
        if (sensorBaseline == 0 && total > 0) sensorBaseline = total
        val delta = if (sensorBaseline > 0) total - sensorBaseline else 0
        _state.update { it.copy(steps = accumulatedSteps + delta.coerceAtLeast(0)) }
    }

    override fun onAccuracyChanged(sensor: Sensor?, accuracy: Int) = Unit

    private fun createNotificationChannel() {
        val channel = NotificationChannel(
            CHANNEL_ID,
            "Walk Session",
            NotificationManager.IMPORTANCE_LOW,
        )
        getSystemService(NotificationManager::class.java).createNotificationChannel(channel)
    }

    private fun buildNotification(): Notification {
        val openIntent = PendingIntent.getActivity(
            this, 0,
            Intent(this, MainActivity::class.java),
            PendingIntent.FLAG_IMMUTABLE,
        )
        val stopIntent = PendingIntent.getService(
            this, 0,
            Intent(this, StepCounterService::class.java).apply { action = ACTION_STOP },
            PendingIntent.FLAG_IMMUTABLE,
        )
        val s = _state.value
        return NotificationCompat.Builder(this, CHANNEL_ID)
            .setSmallIcon(android.R.drawable.ic_media_play)
            .setContentTitle("Walk in progress")
            .setContentText("${s.steps} steps · ${s.formattedElapsed}")
            .setContentIntent(openIntent)
            .addAction(android.R.drawable.ic_media_pause, "Stop Walk", stopIntent)
            .setOngoing(true)
            .build()
    }

    private fun updateNotification() {
        val manager = getSystemService(NotificationManager::class.java)
        manager.notify(NOTIFICATION_ID, buildNotification())
    }

    companion object {
        const val ACTION_STOP = "com.slowwalk.app.ACTION_STOP_WALK"
        const val ACTION_RESUME = "com.slowwalk.app.ACTION_RESUME_WALK"
        const val EXTRA_ACCUMULATED_STEPS = "accumulated_steps"
        const val EXTRA_ELAPSED_SECONDS = "elapsed_seconds"
        const val EXTRA_CLIENT_ID = "client_id"
    }
}
