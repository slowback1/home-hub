@shopping-lists
Feature: Shopping Lists

  @shopping-lists-create
  Scenario: Create a list and add items
    Given the SHOPPING_LISTS_ENABLED feature flag is enabled
    When I visit the Shopping Lists page
    And I create a shopping list named "Groceries"
    And I add item "Milk" to the "Groceries" list
    And I add item "Eggs" to the "Groceries" list
    Then I see a shopping list named "Groceries"
    And the "Groceries" list shows item "Milk"
    And the "Groceries" list shows item "Eggs"

  @shopping-lists-check-off
  Scenario: Check off an item and it stays visible crossed out
    Given the SHOPPING_LISTS_ENABLED feature flag is enabled
    And a shopping list named "Groceries" exists with items "Milk, Eggs"
    When I visit the Shopping Lists page
    And I toggle item "Milk" in the "Groceries" list
    Then item "Milk" in the "Groceries" list is crossed out
    And item "Eggs" in the "Groceries" list is not crossed out

  @shopping-lists-complete-archives
  Scenario: Completing a list archives it and retains it in the Archived section
    Given the SHOPPING_LISTS_ENABLED feature flag is enabled
    And a shopping list named "Groceries" exists with items "Milk, Eggs"
    When I visit the Shopping Lists page
    And I complete the "Groceries" list
    Then I do not see an active shopping list named "Groceries"
    And I see an archived shopping list named "Groceries"
    And the archived "Groceries" list shows item "Milk"

  @shopping-lists-empty-state
  Scenario: Page shows empty state when no lists exist
    Given the SHOPPING_LISTS_ENABLED feature flag is enabled
    When I visit the Shopping Lists page
    Then I see the shopping lists empty state

  @shopping-lists-feature-flag-hidden
  Scenario: Shopping Lists is hidden when feature flag is disabled
    Given the SHOPPING_LISTS_ENABLED feature flag is disabled
    When I am on the dashboard
    Then the Shopping Lists nav item is not visible in the sidebar
