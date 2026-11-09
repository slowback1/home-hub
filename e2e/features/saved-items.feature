@saved-items
Feature: Saved Items

  @saved-items-create
  Scenario: Create a saved item
    Given the SAVED_ITEMS_ENABLED feature flag is enabled
    When I visit the Saved Items page
    And I create a saved item named "5lb sack of potatoes"
    Then I see a saved item named "5lb sack of potatoes"

  @saved-items-view
  Scenario: Seeded saved items are listed on the page
    Given the SAVED_ITEMS_ENABLED feature flag is enabled
    And a saved item named "5lb sack of potatoes" exists
    When I visit the Saved Items page
    Then I see a saved item named "5lb sack of potatoes"

  @saved-items-edit
  Scenario: Edit a saved item inline
    Given the SAVED_ITEMS_ENABLED feature flag is enabled
    And a saved item named "5lb sack of potatoes" exists
    When I visit the Saved Items page
    And I edit the saved item "5lb sack of potatoes" to "10lb sack of potatoes"
    Then I see a saved item named "10lb sack of potatoes"
    And I do not see a saved item named "5lb sack of potatoes"

  @saved-items-empty-state
  Scenario: Page shows empty state when no saved items exist
    Given the SAVED_ITEMS_ENABLED feature flag is enabled
    When I visit the Saved Items page
    Then I see the saved items empty state

  @saved-items-feature-flag-hidden
  Scenario: Saved Items is hidden when feature flag is disabled
    Given the SAVED_ITEMS_ENABLED feature flag is disabled
    When I am on the dashboard
    Then the Saved Items nav item is not visible in the sidebar
