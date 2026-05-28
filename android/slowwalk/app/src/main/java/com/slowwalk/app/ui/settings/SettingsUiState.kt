package com.slowwalk.app.ui.settings

data class SettingsUiState(
    val url: String = "",
    val connectionStatus: ConnectionStatus = ConnectionStatus.Idle,
    val isTesting: Boolean = false,
    val savedMessage: String? = null,
) {
    val isUrlValid: Boolean get() = url.isNotBlank()
}

sealed class ConnectionStatus {
    data object Idle : ConnectionStatus()
    data object Success : ConnectionStatus()
    data class Failure(val message: String) : ConnectionStatus()
}
