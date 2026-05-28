package com.slowwalk.app.ui.history

fun formatDuration(seconds: Int): String {
    val totalMinutes = seconds / 60
    return if (totalMinutes < 60) {
        "$totalMinutes min"
    } else {
        val hours = totalMinutes / 60
        val minutes = totalMinutes % 60
        "${hours}h ${minutes}m"
    }
}
