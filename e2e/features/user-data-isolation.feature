@user-switching @user-data-isolation
Feature: Per-user data isolation

  @bookmarks-isolation-between-users
  Scenario: A bookmark created as Alex is invisible to Sam
    Given I have selected the user "Alex"
    When I add a bookmark named "Alex Bookmark"
    Then I should see the bookmark "Alex Bookmark"
    When I switch to the user "Sam"
    Then I should not see the bookmark "Alex Bookmark"

  @wheels-isolation-between-users
  Scenario: A wheel created as Alex is invisible to Sam
    Given I have selected the user "Alex"
    When I add a wheel named "Alex Wheel" with items "One, Two"
    Then I should see the wheel "Alex Wheel"
    When I switch to the user "Sam"
    Then I should not see the wheel "Alex Wheel"

  @activity-pick-isolation-between-users
  Scenario: Each user sees their own current activity pick
    Given an activity pick named "Alex Pick" is seeded for the user "Alex"
    And an activity pick named "Sam Pick" is seeded for the user "Sam"
    When I visit the home page
    And I select the user "Alex"
    And I navigate to the activity display page
    Then I should see "Alex Pick" displayed as the current pick
    When I switch to the user "Sam"
    And I navigate to the activity display page
    Then I should see "Sam Pick" displayed as the current pick
