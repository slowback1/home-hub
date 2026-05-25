@dashboard
Feature: Dashboard

  @dashboard-empty-state
  Scenario: Empty dashboard shows first-run hero
    Given I am on the dashboard
    Then I should see the first-run hero panel
    And the hero should have an add widget button

  @dashboard-add-widget
  Scenario: Add a widget to an empty slot
    Given I am on the dashboard
    When I click the add widget button on slot 0
    Then the widget picker modal should open
    When I select a widget from the picker
    Then the modal should close
    And the widget should appear in slot 0

  @dashboard-widget-persists
  Scenario: Widget assignment persists across page reloads
    Given slot 0 has a widget assigned
    When I reload the dashboard
    Then slot 0 should still show the same widget

  @dashboard-enter-edit-mode
  Scenario: Entering edit mode shows remove controls
    Given slot 0 has a widget assigned
    When I click the Edit Dashboard button
    Then each occupied slot should show a remove button
    And I should see a Done button

  @dashboard-remove-widget
  Scenario: Remove a widget in edit mode
    Given I am in edit mode with slot 0 occupied
    When I click the remove button on slot 0
    Then slot 0 should revert to an empty placeholder
    And the change should be saved automatically

  @dashboard-exit-edit-mode
  Scenario: Exiting edit mode hides remove controls
    Given I am in edit mode
    When I click the Done button
    Then the remove buttons should no longer be visible
