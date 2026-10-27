@user-switching @dashboard-user-isolation
Feature: Per-user dashboard layout isolation

  @dashboard-layout-isolation-between-users
  Scenario: Alex's dashboard layout is invisible to Sam and persists across switching and reload
    Given I have selected the user "Alex"
    And the dashboard layout for the user "Alex" is seeded with widgets "tasks, activity"
    When I visit the dashboard
    Then slot 0 should show the widget "tasks"
    And slot 1 should show the widget "activity"
    When I switch to the user "Sam"
    Then I should see the first-run hero panel
    When I switch to the user "Alex"
    Then slot 0 should show the widget "tasks"
    And slot 1 should show the widget "activity"
    When I reload the dashboard
    Then slot 0 should show the widget "tasks"
    And slot 1 should show the widget "activity"
