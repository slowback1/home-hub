@admin
Feature: Feature Flags Admin UI

  @feature-flags-page-loads
  Scenario: Page loads and displays all feature flags
    Given I navigate to the admin feature flags page
    Then I should see the page heading "Feature Flags"
    And I should see a flag row for "Demo Feature Flag"
    And the toggle for "Demo Feature Flag" should be off

  @feature-flags-toggle-on
  Scenario: Toggling a flag on enables it immediately
    Given I navigate to the admin feature flags page
    And the toggle for "Demo Feature Flag" is off
    When I toggle the switch for "Demo Feature Flag" on
    Then the toggle for "Demo Feature Flag" should be on
    And I should see a success toast

  @feature-flags-toggle-off
  Scenario: Toggling a flag off disables it immediately
    Given I navigate to the admin feature flags page
    And the toggle for "Demo Feature Flag" is on
    When I toggle the switch for "Demo Feature Flag" off
    Then the toggle for "Demo Feature Flag" should be off
    And I should see a success toast

  @feature-flags-tab-navigation
  Scenario: Navigating between admin tabs
    Given I navigate to the admin system config page
    When I click the "Feature Flags" tab
    Then I should be on the admin feature flags page
    When I click the "System Config" tab
    Then I should be on the admin system config page
