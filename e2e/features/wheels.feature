@wheels
Feature: Wheels

  @wheels-create
  Scenario: Create a wheel with items
    Given the WHEEL_PICKER_ENABLED feature flag is enabled
    When I visit the Wheels page
    And I create a wheel named "Dinner" with items "Pizza, Tacos, Sushi"
    Then I see a wheel named "Dinner" in the manage list
    And the "Dinner" wheel shows 3 items

  @wheels-edit
  Scenario: Edit a wheel's name and items
    Given the WHEEL_PICKER_ENABLED feature flag is enabled
    And a wheel named "Dinner" exists with items "Pizza, Tacos"
    When I visit the Wheels page
    And I edit the "Dinner" wheel to be named "Dinner Options" with items "Pizza, Tacos, Ramen"
    Then I see a wheel named "Dinner Options" in the manage list
    And the "Dinner Options" wheel shows 3 items

  @wheels-delete
  Scenario: Delete a wheel
    Given the WHEEL_PICKER_ENABLED feature flag is enabled
    And a wheel named "Dinner" exists with items "Pizza, Tacos"
    When I visit the Wheels page
    And I delete the "Dinner" wheel
    Then I do not see a wheel named "Dinner" in the manage list

  @wheels-empty-state
  Scenario: Page shows empty state when no wheels exist
    Given the WHEEL_PICKER_ENABLED feature flag is enabled
    When I visit the Wheels page
    Then I see the wheels empty state

  @wheels-spin
  Scenario: Spin a saved wheel and see a result from its items
    Given the WHEEL_PICKER_ENABLED feature flag is enabled
    And a wheel named "Dinner" exists with items "Pizza, Tacos, Sushi"
    When I visit the Wheels page
    And I select the "Dinner" wheel in the spin section
    And I click Spin
    Then I see a spin result that is one of "Pizza, Tacos, Sushi"

  @wheels-spin-empty-disabled
  Scenario: Spin is disabled for a wheel with no items
    Given the WHEEL_PICKER_ENABLED feature flag is enabled
    And a wheel named "Empty" exists with no items
    When I visit the Wheels page
    And I select the "Empty" wheel in the spin section
    Then the Spin button is disabled

  @wheels-widget-quick-spin
  Scenario: Quick-spin a wheel from the dashboard widget
    Given the WHEEL_PICKER_ENABLED feature flag is enabled
    And the wheels widget is on the dashboard
    And a wheel named "Dinner" exists with items "Pizza, Tacos, Sushi"
    When I am on the dashboard
    And I select the "Dinner" wheel in the wheels widget
    And I click Spin in the wheels widget
    Then I see a widget spin result that is one of "Pizza, Tacos, Sushi"

  @wheels-feature-flag-hidden
  Scenario: Wheels is hidden when feature flag is disabled
    Given the WHEEL_PICKER_ENABLED feature flag is disabled
    When I am on the dashboard
    Then the Wheels nav item is not visible in the sidebar
    And the wheels widget is not available in the widget picker
