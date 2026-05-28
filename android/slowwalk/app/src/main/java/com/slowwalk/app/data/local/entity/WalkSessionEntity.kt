package com.slowwalk.app.data.local.entity

import androidx.room.Entity
import androidx.room.PrimaryKey

@Entity(tableName = "sessions")
data class WalkSessionEntity(
    @PrimaryKey val clientId: String,
    val startedAt: Long,
    val durationSeconds: Int,
    val stepCount: Int,
    val serverSyncedAt: Long? = null,
)
