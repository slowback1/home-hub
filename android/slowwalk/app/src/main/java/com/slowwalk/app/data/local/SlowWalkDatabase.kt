package com.slowwalk.app.data.local

import android.content.Context
import androidx.room.Database
import androidx.room.Room
import androidx.room.RoomDatabase
import com.slowwalk.app.data.local.dao.InProgressSessionDao
import com.slowwalk.app.data.local.dao.SettingDao
import com.slowwalk.app.data.local.dao.WalkSessionDao
import com.slowwalk.app.data.local.entity.InProgressSessionEntity
import com.slowwalk.app.data.local.entity.SettingEntity
import com.slowwalk.app.data.local.entity.WalkSessionEntity

@Database(
    entities = [WalkSessionEntity::class, SettingEntity::class, InProgressSessionEntity::class],
    version = 2,
    exportSchema = false,
)
abstract class SlowWalkDatabase : RoomDatabase() {
    abstract fun walkSessionDao(): WalkSessionDao
    abstract fun settingDao(): SettingDao
    abstract fun inProgressSessionDao(): InProgressSessionDao

    companion object {
        @Volatile private var instance: SlowWalkDatabase? = null

        fun getInstance(context: Context): SlowWalkDatabase =
            instance ?: synchronized(this) {
                instance ?: Room.databaseBuilder(
                    context.applicationContext,
                    SlowWalkDatabase::class.java,
                    "slowwalk.db",
                ).fallbackToDestructiveMigration().build().also { instance = it }
            }
    }
}
