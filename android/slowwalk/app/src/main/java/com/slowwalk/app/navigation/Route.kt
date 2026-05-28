package com.slowwalk.app.navigation

sealed class Route(val route: String) {
    data object Walk : Route("walk")
    data object History : Route("history")
    data object Settings : Route("settings")
}
