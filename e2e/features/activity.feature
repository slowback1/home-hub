@activity
Feature: Activity Picker

  @add-activity-happy-path
  Scenario: Add a new activity on the config page
    Given I am on the activity config page
    When I add a new activity with name "Play Chess" and weight 3
    Then I should see "Play Chess" in the activity list with weight 3

  @change-activity-weight
  Scenario: Change the weight of an existing activity
    Given I am on the activity config page
    And the activity list contains "Play Chess" with weight 3
    When I change the weight of "Play Chess" to 5
    Then I should see "Play Chess" in the activity list with weight 5

  @delete-activity
  Scenario: Delete an activity from the config page
    Given I am on the activity config page
    And the activity list contains "Play Chess"
    When I delete "Play Chess"
    Then I should not see "Play Chess" in the activity list

  @activity-empty-state
  Scenario: Display page shows placeholder when no pick has been made
    Given there are no activity picks recorded
    When I navigate to the activity display page
    Then I should see a placeholder message prompting me to configure activities

  @activity-display-current-pick
  Scenario: Display page shows the current hourly pick
    Given an activity pick exists for the current hour with name "Play Chess"
    When I navigate to the activity display page
    Then I should see "Play Chess" displayed as the current pick
