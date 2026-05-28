package com.slowwalk.app.sync

sealed class SyncState {
    data object Idle : SyncState()
    data object Syncing : SyncState()
    data class Done(val synced: Int) : SyncState()
    data object Error : SyncState()
}
