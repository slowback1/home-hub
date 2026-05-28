package com.slowwalk.app.ui.walk

data class WalkState(
    val isActive: Boolean = false,
    val steps: Int = 0,
    val elapsedSeconds: Long = 0L,
) {
    val formattedElapsed: String
        get() {
            val minutes = elapsedSeconds / 60
            val seconds = elapsedSeconds % 60
            return "%02d:%02d".format(minutes, seconds)
        }
}
