package com.slowwalk.app.ui.walk

import org.junit.Assert.assertEquals
import org.junit.Assert.assertFalse
import org.junit.Assert.assertTrue
import org.junit.Test

class WalkStateTest {

    @Test
    fun `initial walk state is inactive with zero steps and time`() {
        val state = WalkState()
        assertFalse(state.isActive)
        assertEquals(0, state.steps)
        assertEquals(0L, state.elapsedSeconds)
    }

    @Test
    fun `active walk state has isActive true`() {
        val state = WalkState(isActive = true, steps = 100, elapsedSeconds = 60L)
        assertTrue(state.isActive)
        assertEquals(100, state.steps)
        assertEquals(60L, state.elapsedSeconds)
    }

    @Test
    fun `elapsed time format produces mm ss string`() {
        assertEquals("00:00", WalkState().formattedElapsed)
        assertEquals("01:00", WalkState(elapsedSeconds = 60L).formattedElapsed)
        assertEquals("01:30", WalkState(elapsedSeconds = 90L).formattedElapsed)
        assertEquals("10:05", WalkState(elapsedSeconds = 605L).formattedElapsed)
    }
}
