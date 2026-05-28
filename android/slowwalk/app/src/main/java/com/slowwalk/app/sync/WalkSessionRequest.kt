package com.slowwalk.app.sync

data class WalkSessionRequest(
    val clientId: String,
    val startedAt: String,
    val durationSeconds: Int,
    val stepCount: Int,
)
