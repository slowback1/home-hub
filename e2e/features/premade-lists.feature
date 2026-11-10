@premade-lists
Feature: Premade Lists

  @premade-lists-create-with-saved-items
  Scenario: Create a premade list and add saved items to it
    Given the PREMADE_LISTS_ENABLED feature flag is enabled
    And a saved item named "Milk" exists
    And a saved item named "Eggs" exists
    When I visit the Premade Lists page
    And I create a premade list named "Weekly groceries"
    And I add the saved item "Milk" to the premade list "Weekly groceries"
    And I add the saved item "Eggs" to the premade list "Weekly groceries"
    Then the premade list "Weekly groceries" shows saved item "Milk"
    And the premade list "Weekly groceries" shows saved item "Eggs"

  @premade-lists-apply-on-shopping-list
  Scenario: Apply a premade list when creating a shopping list
    Given the SHOPPING_LISTS_ENABLED feature flag is enabled
    And the PREMADE_LISTS_ENABLED feature flag is enabled
    And a premade list named "Weekly groceries" with saved items "Milk, Eggs" exists
    When I visit the Shopping Lists page
    And I create a shopping list named "Shop run" from the premade list "Weekly groceries"
    Then the "Shop run" list shows item "Milk"
    And the "Shop run" list shows item "Eggs"
    And item "Milk" in the "Shop run" list is not crossed out
    And item "Eggs" in the "Shop run" list is not crossed out
    When I toggle item "Milk" in the "Shop run" list
    Then item "Milk" in the "Shop run" list is crossed out

  @premade-lists-flag-off
  Scenario: Premade Lists is hidden when feature flag is disabled
    Given the PREMADE_LISTS_ENABLED feature flag is disabled
    When I am on the dashboard
    Then the Premade Lists nav item is not visible in the sidebar
