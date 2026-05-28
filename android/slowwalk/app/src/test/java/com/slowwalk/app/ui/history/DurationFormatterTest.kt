package com.slowwalk.app.ui.history

import org.junit.Assert.assertEquals
import org.junit.Test

class DurationFormatterTest {

    @Test
    fun `zero seconds formats as 0 min`() {
        assertEquals("0 min", formatDuration(0))
    }

    @Test
    fun `under 60 minutes formats as X min`() {
        assertEquals("5 min", formatDuration(300))
        assertEquals("23 min", formatDuration(1380))
        assertEquals("59 min", formatDuration(3540))
    }

    @Test
    fun `exactly 60 minutes formats as 1h 0m`() {
        assertEquals("1h 0m", formatDuration(3600))
    }

    @Test
    fun `over 60 minutes formats as Xh Ym`() {
        assertEquals("1h 4m", formatDuration(3840))
        assertEquals("2h 30m", formatDuration(9000))
    }
}
