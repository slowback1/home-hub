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
import kotlinx.coroutines.launch
import java.util.UUID

private const val CHANNEL_ID = "walk_session"
private const val NOTIFICATION_ID = 1
private const val TIMER_INTERVAL_MS = 1_000L

class StepCounterService : Service(), SensorEventListener {

    inner class LocalBinder : Binder() {
        fun getService(): StepCounterService = this@StepCounterService
    }

    private val binder = LocalBinder()
    private val serviceScope = CoroutineScope(SupervisorJob() + Dispatchers.Main)

    private val _state = MutableStateFlow(WalkState())
    val state: StateFlow<WalkState> = _state.asStateFlow()

    private var sensorManager: SensorManager? = null
    private var baselineSteps: Int = 0
    private var startEpochMillis: Long = 0L
    private var timerJob: Job? = null

    override fun onCreate() {
        super.onCreate()
        createNotificationChannel()
    }

    override fun onBind(intent: Intent?): IBinder = binder

    override fun onStartCommand(intent: Intent?, flags: Int, startId: Int): Int {
        if (intent?.action == ACTION_STOP) {
            stopWalk()
            return START_NOT_STICKY
        }
        startWalk()
        return START_STICKY
    }

    private fun startWalk() {
        startEpochMillis = System.currentTimeMillis()
        sensorManager = getSystemService(SENSOR_SERVICE) as SensorManager
        val sensor = sensorManager?.getDefaultSensor(Sensor.TYPE_STEP_COUNTER)
        sensorManager?.registerListener(this, sensor, SensorManager.SENSOR_DELAY_NORMAL)

        _state.update { WalkState(isActive = true) }

        timerJob = serviceScope.launch {
            while (true) {
                delay(TIMER_INTERVAL_MS)
                val elapsed = (System.currentTimeMillis() - startEpochMillis) / 1_000L
                _state.update { it.copy(elapsedSeconds = elapsed) }
                updateNotification()
            }
        }

        startForeground(NOTIFICATION_ID, buildNotification())
    }

    fun stopWalk() {
        timerJob?.cancel()
        sensorManager?.unregisterListener(this)
        val finalState = _state.value

        serviceScope.launch(Dispatchers.IO) {
            val db = SlowWalkDatabase.getInstance(applicationContext)
            db.walkSessionDao().insertOrReplace(
                WalkSessionEntity(
                    clientId = UUID.randomUUID().toString(),
                    startedAt = startEpochMillis,
                    durationSeconds = finalState.elapsedSeconds.toInt(),
                    stepCount = finalState.steps,
                )
            )
        }

        _state.update { WalkState() }
        stopForeground(STOP_FOREGROUND_REMOVE)
        stopSelf()
    }

    override fun onSensorChanged(event: SensorEvent?) {
        event ?: return
        if (event.sensor.type != Sensor.TYPE_STEP_COUNTER) return
        val total = event.values[0].toInt()
        if (baselineSteps == 0 && total > 0) baselineSteps = total
        val delta = if (baselineSteps > 0) total - baselineSteps else 0
        _state.update { it.copy(steps = delta.coerceAtLeast(0)) }
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
    }
}
