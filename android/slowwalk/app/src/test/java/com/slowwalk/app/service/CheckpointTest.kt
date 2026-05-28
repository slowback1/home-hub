package com.slowwalk.app.service

import com.slowwalk.app.data.local.entity.InProgressSessionEntity
import org.junit.Assert.assertEquals
import org.junit.Test

class CheckpointTest {

    @Test
    fun `InProgressSessionEntity stores all checkpoint fields`() {
        val checkpoint = InProgressSessionEntity(
            clientId = "abc-123",
            startedAt = 1_000_000L,
            accumulatedSteps = 250,
            elapsedSeconds = 120,
            sensorBaselineAtLastCheckpoint = 5000,
        )
        assertEquals("abc-123", checkpoint.clientId)
        assertEquals(1_000_000L, checkpoint.startedAt)
        assertEquals(250, checkpoint.accumulatedSteps)
        assertEquals(120, checkpoint.elapsedSeconds)
        assertEquals(5000, checkpoint.sensorBaselineAtLastCheckpoint)
    }
}
