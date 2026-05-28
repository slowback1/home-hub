package com.slowwalk.app.data.local.dao

import androidx.room.Dao
import androidx.room.Insert
import androidx.room.OnConflictStrategy
import androidx.room.Query
import com.slowwalk.app.data.local.entity.SettingEntity

@Dao
interface SettingDao {

    @Insert(onConflict = OnConflictStrategy.REPLACE)
    suspend fun upsert(setting: SettingEntity)

    @Query("SELECT * FROM settings WHERE `key` = :key LIMIT 1")
    suspend fun getByKey(key: String): SettingEntity?
}
