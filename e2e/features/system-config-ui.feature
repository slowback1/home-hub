@admin
Feature: System Config Admin

  @system-config-page-loads
  Scenario: Page loads and displays config entries grouped by namespace
    Given I navigate to the admin system config page
    Then I should see a "weather" namespace section
    And I should see a "weather-zip_code" entry with value "10001"
    And I should see an "weather-api_key" entry with a masked value

  @system-config-edit-happy-path
  Scenario: Editing a config value and saving updates the value
    Given I navigate to the admin system config page
    When I click on the "weather-zip_code" value
    Then an inline text input with Save and Cancel buttons should appear
    When I type "90210" and click Save
    Then the "weather-zip_code" entry should display "90210"
    And I should see a success toast

  @system-config-edit-cancel
  Scenario: Cancelling an edit restores the original value
    Given I navigate to the admin system config page
    When I click on the "weather-zip_code" value and type "99999"
    And I click Cancel
    Then the "weather-zip_code" entry should display "10001"

  @system-config-secret-masked
  Scenario: Secret values are masked and can be revealed
    Given I navigate to the admin system config page
    Then the "weather-api_key" value should be masked
    When I click the show toggle on the "weather-api_key" entry
    Then the "weather-api_key" value should display "test-api-key-1"

  @system-config-save-error
  Scenario: A failed save shows an error toast
    Given I navigate to the admin system config page
    When I click on the "weather-zip_code" value and the API returns an error on save
    Then I should see an error toast
    And the input should remain open
