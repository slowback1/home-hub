package com.slowwalk.app.data.local.entity

import androidx.room.Entity
import androidx.room.PrimaryKey

@Entity(tableName = "in_progress_session")
data class InProgressSessionEntity(
    @PrimaryKey val clientId: String,
    val startedAt: Long,
    val accumulatedSteps: Int,
    val elapsedSeconds: Int,
    val sensorBaselineAtLastCheckpoint: Int,
)
