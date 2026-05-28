package com.slowwalk.app.data.local.dao

import androidx.room.Dao
import androidx.room.Insert
import androidx.room.OnConflictStrategy
import androidx.room.Query
import com.slowwalk.app.data.local.entity.InProgressSessionEntity

@Dao
interface InProgressSessionDao {

    @Insert(onConflict = OnConflictStrategy.REPLACE)
    suspend fun save(entity: InProgressSessionEntity)

    @Query("SELECT * FROM in_progress_session LIMIT 1")
    suspend fun get(): InProgressSessionEntity?

    @Query("DELETE FROM in_progress_session")
    suspend fun delete()
}
