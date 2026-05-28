package com.slowwalk.app.data.local.dao

import androidx.room.Dao
import androidx.room.Insert
import androidx.room.OnConflictStrategy
import androidx.room.Query
import com.slowwalk.app.data.local.entity.WalkSessionEntity
import kotlinx.coroutines.flow.Flow

@Dao
interface WalkSessionDao {

    @Insert(onConflict = OnConflictStrategy.REPLACE)
    suspend fun insertOrReplace(session: WalkSessionEntity)

    @Query("SELECT * FROM sessions WHERE serverSyncedAt IS NULL")
    fun observePending(): Flow<List<WalkSessionEntity>>

    @Query("SELECT * FROM sessions WHERE serverSyncedAt IS NULL")
    suspend fun getPending(): List<WalkSessionEntity>

    @Query("DELETE FROM sessions WHERE clientId = :clientId")
    suspend fun deleteByClientId(clientId: String)

    @Query("UPDATE sessions SET serverSyncedAt = :syncedAt WHERE clientId = :clientId")
    suspend fun markSynced(clientId: String, syncedAt: Long)
}
