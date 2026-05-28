package com.slowwalk.app.ui.settings

import androidx.compose.foundation.layout.Arrangement
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.Row
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.layout.size
import androidx.compose.material3.Button
import androidx.compose.material3.CircularProgressIndicator
import androidx.compose.material3.MaterialTheme
import androidx.compose.material3.OutlinedTextField
import androidx.compose.material3.SnackbarHostState
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.runtime.LaunchedEffect
import androidx.compose.runtime.collectAsState
import androidx.compose.runtime.getValue
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.unit.dp

@Composable
fun SettingsScreen(
    viewModel: SettingsViewModel,
    snackbarHostState: SnackbarHostState,
) {
    val state by viewModel.state.collectAsState()

    LaunchedEffect(state.savedMessage) {
        state.savedMessage?.let {
            snackbarHostState.showSnackbar(it)
            viewModel.clearSavedMessage()
        }
    }

    Column(
        modifier = Modifier
            .fillMaxSize()
            .padding(24.dp),
        verticalArrangement = Arrangement.spacedBy(16.dp),
    ) {
        Text("Settings", style = MaterialTheme.typography.headlineSmall)

        OutlinedTextField(
            value = state.url,
            onValueChange = viewModel::onUrlChange,
            label = { Text("HomeHub backend URL") },
            placeholder = { Text("http://192.168.1.x:5000") },
            modifier = Modifier.fillMaxWidth(),
            enabled = !state.isTesting,
            singleLine = true,
        )

        Row(horizontalArrangement = Arrangement.spacedBy(12.dp)) {
            Button(onClick = viewModel::save, enabled = !state.isTesting) {
                Text("Save")
            }

            Button(
                onClick = viewModel::testConnection,
                enabled = !state.isTesting,
            ) {
                if (state.isTesting) {
                    CircularProgressIndicator(modifier = Modifier.size(18.dp), strokeWidth = 2.dp)
                } else {
                    Text("Test Connection")
                }
            }
        }

        when (val status = state.connectionStatus) {
            ConnectionStatus.Idle -> Unit
            ConnectionStatus.Success -> Text(
                "Connected",
                color = MaterialTheme.colorScheme.primary,
            )
            is ConnectionStatus.Failure -> Text(
                status.message,
                color = MaterialTheme.colorScheme.error,
            )
        }
    }
}
