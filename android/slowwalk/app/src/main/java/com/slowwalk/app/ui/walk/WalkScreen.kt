package com.slowwalk.app.ui.walk

import android.content.ComponentName
import android.content.Context
import android.content.Intent
import android.content.ServiceConnection
import android.os.IBinder
import androidx.compose.animation.core.RepeatMode
import androidx.compose.animation.core.animateFloat
import androidx.compose.animation.core.infiniteRepeatable
import androidx.compose.animation.core.rememberInfiniteTransition
import androidx.compose.animation.core.tween
import androidx.compose.foundation.background
import androidx.compose.foundation.layout.Arrangement
import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.Spacer
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.height
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.layout.size
import androidx.compose.foundation.shape.CircleShape
import androidx.compose.material3.Button
import androidx.compose.material3.MaterialTheme
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.runtime.DisposableEffect
import androidx.compose.runtime.LaunchedEffect
import androidx.compose.runtime.collectAsState
import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.remember
import androidx.compose.runtime.setValue
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.alpha
import androidx.compose.ui.platform.LocalContext
import androidx.compose.ui.unit.dp
import com.slowwalk.app.service.StepCounterService
import kotlinx.coroutines.flow.MutableStateFlow

@Composable
fun WalkScreen(onActiveChanged: (Boolean) -> Unit = {}) {
    val context = LocalContext.current
    var service by remember { mutableStateOf<StepCounterService?>(null) }
    val fallbackState = remember { MutableStateFlow(WalkState()) }
    val walkState by (service?.state ?: fallbackState).collectAsState(initial = WalkState())

    LaunchedEffect(walkState.isActive) {
        onActiveChanged(walkState.isActive)
    }

    DisposableEffect(Unit) {
        val connection = object : ServiceConnection {
            override fun onServiceConnected(name: ComponentName, binder: IBinder) {
                service = (binder as StepCounterService.LocalBinder).getService()
            }
            override fun onServiceDisconnected(name: ComponentName) {
                service = null
            }
        }
        val intent = Intent(context, StepCounterService::class.java)
        context.bindService(intent, connection, Context.BIND_AUTO_CREATE)
        onDispose { context.unbindService(connection) }
    }

    Column(
        modifier = Modifier.fillMaxSize().padding(24.dp),
        horizontalAlignment = Alignment.CenterHorizontally,
        verticalArrangement = Arrangement.Center,
    ) {
        if (walkState.isActive) {
            Text("${walkState.steps}", style = MaterialTheme.typography.displayLarge)
            Text("steps", style = MaterialTheme.typography.bodyLarge)
            Spacer(Modifier.height(8.dp))
            Text(walkState.formattedElapsed, style = MaterialTheme.typography.headlineMedium)
            Spacer(Modifier.height(32.dp))
            Button(onClick = { service?.stopWalk() }) {
                Text("Stop Walk")
            }
        } else {
            Button(onClick = {
                val intent = Intent(context, StepCounterService::class.java)
                context.startForegroundService(intent)
            }) {
                Text("Start Walk")
            }
        }
    }
}

@Composable
fun PulsingDot(modifier: Modifier = Modifier) {
    val infiniteTransition = rememberInfiniteTransition(label = "pulse")
    val alpha by infiniteTransition.animateFloat(
        initialValue = 1f,
        targetValue = 0.2f,
        animationSpec = infiniteRepeatable(tween(800), RepeatMode.Reverse),
        label = "dot_alpha",
    )
    Box(
        modifier = modifier
            .size(8.dp)
            .alpha(alpha)
            .background(MaterialTheme.colorScheme.primary, CircleShape),
    )
}
