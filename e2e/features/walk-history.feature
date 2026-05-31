@walk-history
Feature: Walk Session History

  @walk-history-widget-recent-sessions
  Scenario: Dashboard widget shows 3 most recent walk sessions
    Given the WALK_SESSION_HISTORY_ENABLED feature flag is enabled
    And the walk session history widget is on the dashboard
    And there are walk sessions synced from the Android app
    When I am on the dashboard
    Then I see up to 3 sessions in the walk history widget
    And each widget row shows the session date, step count, and duration

  @walk-history-widget-empty
  Scenario: Dashboard widget shows empty state when no sessions exist
    Given the WALK_SESSION_HISTORY_ENABLED feature flag is enabled
    And the walk session history widget is on the dashboard
    When I am on the dashboard
    Then the walk history widget shows an empty state message

  @walk-history-list-page
  Scenario: List page shows all walk sessions sorted by date descending
    Given the WALK_SESSION_HISTORY_ENABLED feature flag is enabled
    And there are walk sessions synced from the Android app
    When I visit the Walk History page
    Then I see all synced walk sessions in the list
    And the sessions are sorted with the most recent session first
    And each row shows the session date, step count, and duration

  @walk-history-list-empty
  Scenario: List page shows empty state when no sessions exist
    Given the WALK_SESSION_HISTORY_ENABLED feature flag is enabled
    When I visit the Walk History page
    Then I see the walk history empty state

  @walk-history-feature-flag-hidden
  Scenario: Walk History is hidden when feature flag is disabled
    Given the WALK_SESSION_HISTORY_ENABLED feature flag is disabled
    When I am on the dashboard
    Then the Walk History nav item is not visible in the sidebar
    And the walk history widget is not available in the widget picker
