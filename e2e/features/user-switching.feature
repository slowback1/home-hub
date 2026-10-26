@user-switching
Feature: User switching

  @user-switcher-first-run
  Scenario: Fresh load shows the user switcher and blocks app content
    Given no user has been selected on this device
    When I visit the home page
    Then I should see the user switcher with the seeded users
    And I should not see the app sidebar

  @user-switcher-select-user
  Scenario: Selecting a user renders the app
    Given no user has been selected on this device
    When I visit the home page
    And I select the user "Alex"
    Then I should see the app sidebar
    And I should not see the user switcher

  @user-switcher-persists-across-reload
  Scenario: The selected user survives a reload
    Given I have selected the user "Sam"
    When I reload the home page
    Then I should see the app sidebar
    And I should not see the user switcher
