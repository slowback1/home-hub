@weather
Feature: Weather Widget

  @weather-displays-current-conditions
  Scenario: Weather page displays current conditions using mock provider
    Given the weather provider is "mock"
    When I navigate to the weather page
    Then I should see a temperature value
    And I should see a condition description
    And I should see a humidity value
    And I should see a wind speed value

  @weather-unavailable-state
  Scenario: Weather page shows unavailable message when the backend returns an error
    Given the weather provider is "mock"
    When I navigate to the weather page and the weather API returns an error
    Then I should see a "Weather unavailable" message

  @weather-feature-flag-hidden
  Scenario: Weather page is not accessible when the feature flag is disabled
    Given the "WEATHER_ENABLED" feature flag is disabled
    When I navigate to the weather page
    Then I should be redirected or see a not-found state
