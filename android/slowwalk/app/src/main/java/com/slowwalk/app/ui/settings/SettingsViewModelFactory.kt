package com.slowwalk.app.ui.settings

import androidx.lifecycle.ViewModel
import androidx.lifecycle.ViewModelProvider
import com.slowwalk.app.data.local.dao.SettingDao

class SettingsViewModelFactory(private val dao: SettingDao) : ViewModelProvider.Factory {
    override fun <T : ViewModel> create(modelClass: Class<T>): T {
        @Suppress("UNCHECKED_CAST")
        return SettingsViewModel(dao) as T
    }
}
