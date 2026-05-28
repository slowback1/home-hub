package com.slowwalk.app

import android.os.Bundle
import androidx.activity.ComponentActivity
import androidx.activity.compose.setContent
import androidx.activity.enableEdgeToEdge
import androidx.compose.foundation.layout.padding
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.DirectionsWalk
import androidx.compose.material.icons.filled.History
import androidx.compose.material.icons.filled.Settings
import androidx.compose.material3.Icon
import androidx.compose.material3.NavigationBar
import androidx.compose.material3.NavigationBarItem
import androidx.compose.material3.Scaffold
import androidx.compose.material3.SnackbarHost
import androidx.compose.material3.SnackbarHostState
import androidx.compose.material3.Text
import androidx.compose.runtime.getValue
import androidx.compose.runtime.remember
import androidx.compose.ui.Modifier
import androidx.lifecycle.viewmodel.compose.viewModel
import androidx.navigation.NavDestination.Companion.hierarchy
import androidx.navigation.NavGraph.Companion.findStartDestination
import androidx.navigation.compose.NavHost
import androidx.navigation.compose.composable
import androidx.navigation.compose.currentBackStackEntryAsState
import androidx.navigation.compose.rememberNavController
import com.slowwalk.app.data.local.SlowWalkDatabase
import com.slowwalk.app.navigation.Route
import com.slowwalk.app.ui.HistoryScreen
import com.slowwalk.app.ui.WalkScreen
import com.slowwalk.app.ui.settings.SettingsScreen
import com.slowwalk.app.ui.settings.SettingsViewModel
import com.slowwalk.app.ui.settings.SettingsViewModelFactory

class MainActivity : ComponentActivity() {
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        enableEdgeToEdge()
        setContent {
            val navController = rememberNavController()
            val navBackStackEntry by navController.currentBackStackEntryAsState()
            val currentDestination = navBackStackEntry?.destination
            val snackbarHostState = remember { SnackbarHostState() }

            val tabs = listOf(
                Triple(Route.Walk, "Walk", Icons.Default.DirectionsWalk),
                Triple(Route.History, "History", Icons.Default.History),
                Triple(Route.Settings, "Settings", Icons.Default.Settings),
            )

            Scaffold(
                snackbarHost = { SnackbarHost(snackbarHostState) },
                bottomBar = {
                    NavigationBar {
                        tabs.forEach { (route, label, icon) ->
                            NavigationBarItem(
                                selected = currentDestination?.hierarchy?.any { it.route == route.route } == true,
                                onClick = {
                                    navController.navigate(route.route) {
                                        popUpTo(navController.graph.findStartDestination().id) {
                                            saveState = true
                                        }
                                        launchSingleTop = true
                                        restoreState = true
                                    }
                                },
                                icon = { Icon(icon, contentDescription = label) },
                                label = { Text(label) },
                            )
                        }
                    }
                }
            ) { innerPadding ->
                NavHost(
                    navController = navController,
                    startDestination = Route.Walk.route,
                    modifier = Modifier.padding(innerPadding),
                ) {
                    composable(Route.Walk.route) { WalkScreen() }
                    composable(Route.History.route) { HistoryScreen() }
                    composable(Route.Settings.route) {
                        val db = SlowWalkDatabase.getInstance(applicationContext)
                        val settingsVm: SettingsViewModel = viewModel(
                            factory = SettingsViewModelFactory(db.settingDao())
                        )
                        SettingsScreen(viewModel = settingsVm, snackbarHostState = snackbarHostState)
                    }
                }
            }
        }
    }
}
