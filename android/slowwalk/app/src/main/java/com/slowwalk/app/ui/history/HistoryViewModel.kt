package com.slowwalk.app.ui.history

import androidx.lifecycle.ViewModel
import androidx.lifecycle.ViewModelProvider
import androidx.lifecycle.viewModelScope
import com.slowwalk.app.data.local.dao.WalkSessionDao
import com.slowwalk.app.data.local.entity.WalkSessionEntity
import kotlinx.coroutines.flow.SharingStarted
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.flow.map
import kotlinx.coroutines.flow.stateIn
import kotlinx.coroutines.launch

class HistoryViewModel(private val dao: WalkSessionDao) : ViewModel() {

    val sessions: StateFlow<List<WalkSessionEntity>> = dao.observePending()
        .map { list -> list.sortedByDescending { it.startedAt } }
        .stateIn(viewModelScope, SharingStarted.WhileSubscribed(5_000), emptyList())

    fun delete(clientId: String) {
        viewModelScope.launch {
            dao.deleteByClientId(clientId)
        }
    }

    class Factory(private val dao: WalkSessionDao) : ViewModelProvider.Factory {
        override fun <T : ViewModel> create(modelClass: Class<T>): T {
            @Suppress("UNCHECKED_CAST")
            return HistoryViewModel(dao) as T
        }
    }
}
