@nba
Feature: NBA Games

  @nba-widget-shows-today-games
  Scenario: Widget shows today's upcoming game and hides games past the 3-hour rule
    Given the NBA_SCHEDULE_ENABLED feature flag is enabled
    And NBA games are seeded with deterministic timestamps
    And the nba-games widget is placed in slot 0
    And I am on the dashboard
    Then the NBA widget shows the game row for "Lakers @ Celtics" with its local start time
    And the NBA widget does not show the game row for "Warriors @ Suns"
    And the NBA widget list has a scroll container

  @nba-chevron-navigates-to-nba-page
  Scenario: Chevron in the widget header navigates to the NBA page listing non-disappeared games
    Given the NBA_SCHEDULE_ENABLED feature flag is enabled
    And NBA games are seeded with deterministic timestamps
    And the nba-games widget is placed in slot 0
    And I am on the dashboard
    When I click the chevron on the NBA widget
    Then I am on the NBA page
    And the NBA page lists the game "Lakers @ Celtics"
    And the NBA page lists the game "Bulls @ Knicks"
    And the NBA page does not list the game "Warriors @ Suns"

  @nba-feature-flag-hidden
  Scenario: NBA nav item and widget option are hidden when the flag is disabled
    Given the NBA_SCHEDULE_ENABLED feature flag is disabled
    And a filler widget is placed in slot 1
    And I am on the dashboard
    Then the NBA nav item is not visible in the sidebar
    When I click the Edit Dashboard button
    And I click the add widget button on slot 0
    Then the NBA widget option is not visible in the picker
