package com.slowwalk.app.sync

import com.slowwalk.app.data.local.dao.SettingDao
import com.slowwalk.app.data.local.dao.WalkSessionDao
import com.slowwalk.app.data.network.WalkSessionApi
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.flow.asStateFlow
import retrofit2.Retrofit
import retrofit2.converter.gson.GsonConverterFactory
import java.time.Instant
import java.time.ZoneOffset
import java.time.format.DateTimeFormatter

private val ISO_FORMAT = DateTimeFormatter.ofPattern("yyyy-MM-dd'T'HH:mm:ss'Z'")
    .withZone(ZoneOffset.UTC)

class SyncService(
    private val sessionDao: WalkSessionDao,
    private val settingDao: SettingDao,
) {
    private val _state = MutableStateFlow<SyncState>(SyncState.Idle)
    val state: StateFlow<SyncState> = _state.asStateFlow()

    suspend fun sync() {
        val backendUrl = settingDao.getByKey("backend_url")?.value
        if (backendUrl.isNullOrBlank()) return

        val pending = sessionDao.getPending()
        if (pending.isEmpty()) {
            _state.value = SyncState.Done(0)
            return
        }

        _state.value = SyncState.Syncing

        val api = buildApi(backendUrl)
        var syncedCount = 0

        for (session in pending) {
            val success = runCatching {
                val request = WalkSessionRequest(
                    clientId = session.clientId,
                    startedAt = ISO_FORMAT.format(Instant.ofEpochMilli(session.startedAt)),
                    durationSeconds = session.durationSeconds,
                    stepCount = session.stepCount,
                )
                val response = api.postSession(request)
                response.isSuccessful
            }.getOrElse { false }

            if (success) {
                sessionDao.markSynced(session.clientId, System.currentTimeMillis())
                syncedCount++
            }
        }

        _state.value = if (syncedCount > 0 || pending.all { true }) {
            SyncState.Done(syncedCount)
        } else {
            SyncState.Error
        }
    }

    private fun buildApi(baseUrl: String): WalkSessionApi {
        val url = if (baseUrl.endsWith("/")) baseUrl else "$baseUrl/"
        return Retrofit.Builder()
            .baseUrl(url)
            .addConverterFactory(GsonConverterFactory.create())
            .build()
            .create(WalkSessionApi::class.java)
    }
}
