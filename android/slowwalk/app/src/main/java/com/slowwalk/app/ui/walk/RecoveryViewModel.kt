package com.slowwalk.app.ui.walk

import androidx.lifecycle.ViewModel
import androidx.lifecycle.ViewModelProvider
import androidx.lifecycle.viewModelScope
import com.slowwalk.app.data.local.dao.InProgressSessionDao
import com.slowwalk.app.data.local.entity.InProgressSessionEntity
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.flow.asStateFlow
import kotlinx.coroutines.launch

class RecoveryViewModel(private val dao: InProgressSessionDao) : ViewModel() {

    private val _checkpoint = MutableStateFlow<InProgressSessionEntity?>(null)
    val checkpoint: StateFlow<InProgressSessionEntity?> = _checkpoint.asStateFlow()

    init {
        viewModelScope.launch {
            _checkpoint.value = dao.get()
        }
    }

    fun discard() {
        viewModelScope.launch {
            dao.delete()
            _checkpoint.value = null
        }
    }

    class Factory(private val dao: InProgressSessionDao) : ViewModelProvider.Factory {
        override fun <T : ViewModel> create(modelClass: Class<T>): T {
            @Suppress("UNCHECKED_CAST")
            return RecoveryViewModel(dao) as T
        }
    }
}
