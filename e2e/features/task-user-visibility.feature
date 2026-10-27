@user-switching @task-visibility
Feature: Per-user and shared task visibility

  @personal-task-isolation-between-users
  Scenario: A personal task created as Alex is invisible to Sam
    Given a personal task "Alex Private Chore" is seeded for the user "Alex"
    And I have selected the user "Alex"
    When I visit the Tasks page
    Then I should see the task "Alex Private Chore"
    When I switch to the user "Sam"
    And I visit the Tasks page
    Then I should not see the task "Alex Private Chore"

  @shared-task-visible-to-both-users
  Scenario: A shared task is visible to both users
    Given a shared task "Shared Chore" is seeded
    And I have selected the user "Alex"
    When I visit the Tasks page
    Then I should see the task "Shared Chore"
    When I switch to the user "Sam"
    And I visit the Tasks page
    Then I should see the task "Shared Chore"

  @shared-task-completion-visible-across-users
  Scenario: Completing a shared task as Alex shows it completed for Sam
    Given a shared task "Shared Chore" is seeded
    And I have selected the user "Alex"
    When I visit the Tasks page
    And I click Done on "Shared Chore"
    Then "Shared Chore" is not visible on the Tasks page
    When I switch to the user "Sam"
    And I visit the Tasks page
    Then I should not see the task "Shared Chore"
    And the task "Shared Chore" is completed in the history for the user "Sam"
