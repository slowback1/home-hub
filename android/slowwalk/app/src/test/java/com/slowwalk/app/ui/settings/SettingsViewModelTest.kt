package com.slowwalk.app.ui.settings

import org.junit.Assert.assertEquals
import org.junit.Assert.assertFalse
import org.junit.Assert.assertNull
import org.junit.Assert.assertTrue
import org.junit.Test

class SettingsViewModelTest {

    @Test
    fun `initial state has empty url and idle connection status`() {
        val state = SettingsUiState()
        assertEquals("", state.url)
        assertEquals(ConnectionStatus.Idle, state.connectionStatus)
        assertFalse(state.isTesting)
    }

    @Test
    fun `empty url fails validation`() {
        assertFalse(SettingsUiState().isUrlValid)
    }

    @Test
    fun `non-empty url passes validation`() {
        val state = SettingsUiState(url = "http://192.168.1.1:5000")
        assertTrue(state.isUrlValid)
    }

    @Test
    fun `ConnectionStatus sealed values are distinct`() {
        val idle = ConnectionStatus.Idle
        val success = ConnectionStatus.Success
        val failure = ConnectionStatus.Failure("err")

        assertTrue(idle != success)
        assertTrue(idle != failure)
        assertEquals("err", (failure as ConnectionStatus.Failure).message)
    }

    @Test
    fun `savedMessage resets after being set`() {
        val state = SettingsUiState(savedMessage = "Saved")
        assertEquals("Saved", state.savedMessage)
        val cleared = state.copy(savedMessage = null)
        assertNull(cleared.savedMessage)
    }
}
