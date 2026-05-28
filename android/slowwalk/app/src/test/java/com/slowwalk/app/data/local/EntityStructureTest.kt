package com.slowwalk.app.data.local

import com.slowwalk.app.data.local.entity.SettingEntity
import com.slowwalk.app.data.local.entity.WalkSessionEntity
import org.junit.Assert.assertEquals
import org.junit.Assert.assertNull
import org.junit.Test

class EntityStructureTest {

    @Test
    fun `WalkSessionEntity defaults serverSyncedAt to null`() {
        val entity = WalkSessionEntity(
            clientId = "test-id",
            startedAt = 1000L,
            durationSeconds = 300,
            stepCount = 500,
        )
        assertNull(entity.serverSyncedAt)
    }

    @Test
    fun `WalkSessionEntity with serverSyncedAt marks as synced`() {
        val entity = WalkSessionEntity(
            clientId = "test-id",
            startedAt = 1000L,
            durationSeconds = 300,
            stepCount = 500,
            serverSyncedAt = 2000L,
        )
        assertEquals(2000L, entity.serverSyncedAt)
    }

    @Test
    fun `SettingEntity stores key and value`() {
        val entity = SettingEntity(key = "backend_url", value = "http://192.168.1.1:5000")
        assertEquals("backend_url", entity.key)
        assertEquals("http://192.168.1.1:5000", entity.value)
    }
}
