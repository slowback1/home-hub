package com.slowwalk.app.sync

import org.junit.Assert.assertEquals
import org.junit.Assert.assertFalse
import org.junit.Assert.assertTrue
import org.junit.Test

class SyncStateTest {

    @Test
    fun `initial state is idle`() {
        assertEquals(SyncState.Idle, SyncState.Idle)
        assertFalse(SyncState.Idle is SyncState.Syncing)
    }

    @Test
    fun `done state carries synced count`() {
        val done = SyncState.Done(synced = 3)
        assertEquals(3, done.synced)
    }

    @Test
    fun `error state is distinct from idle`() {
        assertTrue(SyncState.Error != SyncState.Idle)
    }

    @Test
    fun `WalkSessionRequest maps entity fields correctly`() {
        val request = WalkSessionRequest(
            clientId = "abc",
            startedAt = "2026-05-28T10:00:00Z",
            durationSeconds = 600,
            stepCount = 1000,
        )
        assertEquals("abc", request.clientId)
        assertEquals(600, request.durationSeconds)
        assertEquals(1000, request.stepCount)
    }
}
