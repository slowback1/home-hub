@admin
Feature: Enhanced System Config

  @system-config-select-renders-as-dropdown
  Scenario: A select-type config field renders as a dropdown
    Given I navigate to the admin system config page
    Then the "Provider" field in the "Weather" section should render as a dropdown
    And the dropdown should contain "Mock" and "Open Weather Map" as options

  @system-config-select-saves
  Scenario: Changing a select dropdown saves the new value
    Given I navigate to the admin system config page
    When I change the "Provider" dropdown to "Open Weather Map"
    Then the "Provider" field should display "Open Weather Map"
    And I should see a success toast

  @system-config-section-headers
  Scenario: Namespaces appear as Title Case section headers
    Given I navigate to the admin system config page
    Then I should see a "Weather" section header

  @system-config-key-labels
  Scenario: Config keys appear as Title Case labels
    Given I navigate to the admin system config page
    Then I should see a "Zip Code" label in the "Weather" section
    And I should see a "Api Key" label in the "Weather" section
