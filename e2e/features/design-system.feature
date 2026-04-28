@design-system
Feature: App Shell Navigation

  @sidebar-navigation
  Scenario: Navigate between hub sections via the Sidebar
    Given I am on the home page
    When I click the "Chore / Task Tracker" nav item in the Sidebar
    Then I should be on the task tracker page
    And the "Chore / Task Tracker" nav item should be marked as active

  @sidebar-collapse
  Scenario: Collapse the Sidebar to icon-only mode
    Given I am on the home page
    And the Sidebar is expanded
    When I click the collapse toggle
    Then the Sidebar should collapse to icon-only mode
    And nav item labels should not be visible

  @sidebar-collapse-persists
  Scenario: Collapsed Sidebar state persists across page loads
    Given I am on the home page
    And I have collapsed the Sidebar
    When I reload the page
    Then the Sidebar should still be in icon-only mode

  @dark-theme-applied
  Scenario: App renders in dark theme by default
    Given I navigate to the home page
    Then the app should have the dark theme applied
    And no light theme class should be present on the document
