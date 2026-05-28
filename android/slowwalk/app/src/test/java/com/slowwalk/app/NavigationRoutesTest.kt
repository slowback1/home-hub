package com.slowwalk.app

import com.slowwalk.app.navigation.Route
import org.junit.Assert.assertEquals
import org.junit.Test

class NavigationRoutesTest {

    @Test
    fun `walk route has correct string`() {
        assertEquals("walk", Route.Walk.route)
    }

    @Test
    fun `history route has correct string`() {
        assertEquals("history", Route.History.route)
    }

    @Test
    fun `settings route has correct string`() {
        assertEquals("settings", Route.Settings.route)
    }

    @Test
    fun `all routes are distinct`() {
        val routes = listOf(Route.Walk.route, Route.History.route, Route.Settings.route)
        assertEquals(routes.size, routes.toSet().size)
    }
}
