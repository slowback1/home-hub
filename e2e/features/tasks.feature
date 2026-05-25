@tasks
Feature: Chore / Task Tracker

  @tasks-due-list-visibility
  Scenario: Due list shows only tasks whose DoDate is today or earlier
    Given a task "Sweep floors" with DoDate 5 days ago
    And a task "Rake leaves" with DoDate 5 days from now
    When I visit the Tasks page
    Then I see "Sweep floors" in the Due section
    And I do not see "Rake leaves" in the Due section
    And I see "Rake leaves" in the Upcoming section

  @tasks-no-dodate-always-visible
  Scenario: Tasks with no DoDate always appear in the Due section
    Given a task "Buy more soap" with no DoDate
    When I visit the Tasks page
    Then I see "Buy more soap" in the Due section

  @tasks-complete-one-off
  Scenario: Completing a one-off task removes it from the Due list
    Given a task "Plan vacation" with no DoDate and no recurrence
    When I visit the Tasks page
    And I click Done on "Plan vacation"
    Then "Plan vacation" is not visible in the Due section
    And an undo toast is visible

  @tasks-undo-completion
  Scenario: Undoing a completion restores the task to the Due list
    Given a task "Plan vacation" with no DoDate and no recurrence
    When I visit the Tasks page
    And I click Done on "Plan vacation"
    And I click Undo on the toast
    Then I see "Plan vacation" in the Due section

  @tasks-complete-recurring
  Scenario: Completing a recurring task bumps its DoDate and moves it to Upcoming
    Given a recurring task "Sweep floors" with DoDate today and interval 7 days
    When I visit the Tasks page
    And I click Done on "Sweep floors"
    Then "Sweep floors" is not visible in the Due section
    And I see "Sweep floors" in the Upcoming section

  @tasks-create-one-off
  Scenario: Creating a one-off task with a future DoDate places it in Upcoming
    When I visit the Tasks page
    And I open the Add Task modal
    And I fill in the task name "Call dentist" with DoDate 3 days from now
    And I submit the task form
    Then I see "Call dentist" in the Upcoming section
    And "Call dentist" is not visible in the Due section

  @tasks-create-recurring
  Scenario: Creating a recurring task with no DoDate places it immediately in Due
    When I visit the Tasks page
    And I open the Add Task modal
    And I fill in the recurring task name "Take out trash" with interval 7 days
    And I submit the task form
    Then I see "Take out trash" in the Due section

  @tasks-edit-task
  Scenario: Editing a task's name updates it in the list
    Given a task "Sweep flors" with no DoDate
    When I visit the Tasks page
    And I open the Edit modal for "Sweep flors"
    And I update the task name to "Sweep floors" and save
    Then I see "Sweep floors" in the Due section
    And "Sweep flors" is not visible in the Due section

  @tasks-delete-task
  Scenario: Deleting a task from the Edit modal removes it permanently
    Given a task "Old task" with no DoDate
    When I visit the Tasks page
    And I open the Edit modal for "Old task"
    And I delete the task
    Then "Old task" is not visible on the Tasks page
