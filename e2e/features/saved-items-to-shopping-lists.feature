@saved-items-to-shopping-lists
Feature: Saved Items on Shopping Lists

  @saved-items-to-shopping-lists-pick
  Scenario: Pick a saved item via typeahead and it lands on the list check-off-able
    Given the SHOPPING_LISTS_ENABLED feature flag is enabled
    And the SAVED_ITEMS_ENABLED feature flag is enabled
    And a saved item named "Milk" exists
    When I visit the Shopping Lists page
    And I create a shopping list named "Groceries"
    And I type "mi" in the add-item input of the "Groceries" list
    Then I see a saved item suggestion named "Milk"
    When I pick the saved item suggestion "Milk" in the "Groceries" list
    Then the "Groceries" list shows item "Milk"
    When I toggle item "Milk" in the "Groceries" list
    Then item "Milk" in the "Groceries" list is crossed out

  @saved-items-to-shopping-lists-flag-off
  Scenario: No saved item suggestions when SAVED_ITEMS_ENABLED is off
    Given the SHOPPING_LISTS_ENABLED feature flag is enabled
    And the SAVED_ITEMS_ENABLED feature flag is disabled
    And a saved item named "Milk" exists
    When I visit the Shopping Lists page
    And I create a shopping list named "Groceries"
    And I type "mi" in the add-item input of the "Groceries" list
    Then I do not see a saved item suggestion named "Milk"
    When I add item "mi" to the "Groceries" list
    Then the "Groceries" list shows item "mi"
