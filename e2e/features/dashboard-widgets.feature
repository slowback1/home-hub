@dashboard-widgets
Feature: Dashboard Widgets

  @weather-widget-shows-conditions
  Scenario: Weather widget displays current conditions
    Given the weather widget is placed in slot 0
    And I am on the dashboard
    Then I should see the temperature in the weather widget
    And I should see the condition label in the weather widget
    And I should see the humidity in the weather widget
    And I should see the wind speed in the weather widget

  @tasks-widget-shows-due-tasks
  Scenario: Tasks widget shows tasks due today
    Given there are due tasks in the system
    And the tasks widget is placed in slot 0
    And I am on the dashboard
    Then I should see the due task names in the tasks widget

  @tasks-widget-mark-done
  Scenario: Marking a task done from the tasks widget shows an undo toast
    Given there is a due task named "Take out trash"
    And the tasks widget is placed in slot 0
    And I am on the dashboard
    When I mark "Take out trash" as done in the tasks widget
    Then I should see an undo toast

  @tasks-widget-overflow
  Scenario: Tasks widget shows overflow count when more than 5 tasks are due
    Given there are 7 due tasks in the system
    And the tasks widget is placed in slot 0
    And I am on the dashboard
    Then I should see 5 task rows in the tasks widget
    And I should see "and 2 more" in the tasks widget

  @activity-widget-shows-current-pick
  Scenario: Activity widget shows the current activity pick
    Given there is a current activity pick
    And the activity widget is placed in slot 0
    And I am on the dashboard
    Then I should see the activity name in the activity widget

  @audiobook-widget-active-job
  Scenario: Audiobook widget shows an active conversion job
    Given there is an in-progress audiobook job for "my-book.epub"
    And the audiobook widget is placed in slot 0
    And I am on the dashboard
    Then I should see "my-book.epub" in the audiobook widget
    And I should see the "in_progress" status badge in the audiobook widget

  @audiobook-widget-no-jobs
  Scenario: Audiobook widget shows empty state when there are no jobs
    Given there are no audiobook jobs
    And the audiobook widget is placed in slot 0
    And I am on the dashboard
    Then I should see "No conversions yet" in the audiobook widget

  @bookmarks-widget-starred
  Scenario: Bookmarks widget shows starred bookmarks as clickable links
    Given there is a starred bookmark named "GitHub"
    And the bookmarks widget is placed in slot 0
    And I am on the dashboard
    Then I should see "GitHub" as a link in the bookmarks widget

  @bookmarks-widget-fallback
  Scenario: Bookmarks widget falls back to recent bookmarks when none are starred
    Given there are unstarred bookmarks in the system
    And the bookmarks widget is placed in slot 0
    And I am on the dashboard
    Then I should see bookmark links in the bookmarks widget

  @widget-error-state
  Scenario: Weather widget shows an error indicator when its API fails
    Given the weather API is unavailable
    And the weather widget is placed in slot 0
    And I am on the dashboard
    Then I should see the error state in the weather widget
