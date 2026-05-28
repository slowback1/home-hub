package com.slowwalk.app.ui.settings

import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.slowwalk.app.data.local.dao.SettingDao
import com.slowwalk.app.data.local.entity.SettingEntity
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.flow.asStateFlow
import kotlinx.coroutines.flow.update
import kotlinx.coroutines.launch
import kotlinx.coroutines.withContext
import java.net.HttpURLConnection
import java.net.URL

private const val KEY_BACKEND_URL = "backend_url"
private const val CONNECT_TIMEOUT_MS = 5_000
private const val READ_TIMEOUT_MS = 5_000
private const val HTTP_OK_MIN = 200
private const val HTTP_OK_MAX = 300

class SettingsViewModel(private val dao: SettingDao) : ViewModel() {

    private val _state = MutableStateFlow(SettingsUiState())
    val state: StateFlow<SettingsUiState> = _state.asStateFlow()

    init {
        viewModelScope.launch {
            val saved = dao.getByKey(KEY_BACKEND_URL)?.value ?: ""
            _state.update { it.copy(url = saved) }
        }
    }

    fun onUrlChange(url: String) {
        _state.update { it.copy(url = url, connectionStatus = ConnectionStatus.Idle) }
    }

    fun save() {
        val url = _state.value.url
        viewModelScope.launch {
            dao.upsert(SettingEntity(key = KEY_BACKEND_URL, value = url))
            _state.update { it.copy(savedMessage = "Saved") }
        }
    }

    fun clearSavedMessage() {
        _state.update { it.copy(savedMessage = null) }
    }

    fun testConnection() {
        val url = _state.value.url
        if (url.isBlank()) {
            _state.update { it.copy(connectionStatus = ConnectionStatus.Failure("URL is required")) }
            return
        }
        _state.update { it.copy(isTesting = true, connectionStatus = ConnectionStatus.Idle) }
        viewModelScope.launch {
            val result = withContext(Dispatchers.IO) {
                runCatching {
                    val connection = URL("$url/healthcheck").openConnection() as HttpURLConnection
                    connection.connectTimeout = CONNECT_TIMEOUT_MS
                    connection.readTimeout = READ_TIMEOUT_MS
                    val code = connection.responseCode
                    connection.disconnect()
                    code in HTTP_OK_MIN until HTTP_OK_MAX
                }.getOrElse { false }
            }
            _state.update {
                it.copy(
                    isTesting = false,
                    connectionStatus = if (result) ConnectionStatus.Success else ConnectionStatus.Failure("Connection failed"),
                )
            }
        }
    }
}
