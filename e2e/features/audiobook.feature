@audiobook
Feature: Epub to Audiobook

  @audiobook-submit-job-happy-path
  Scenario: Submit a job and see it progress to completed
    Given I am on the audiobook convert page
    And at least one voice sample exists
    When I upload an EPUB file and select a voice sample
    And I submit the conversion form
    Then a new job appears in the queue with status "queued"
    And the job progresses to "in_progress"
    And the job progresses to "completed"
    And a download button is visible for the completed job

  @audiobook-cancel-queued-job
  Scenario: Cancel a queued job
    Given I am on the audiobook convert page
    And a queued job exists
    When I click cancel on the queued job
    Then the job status changes to "cancelled"

  @audiobook-download-completed-file
  Scenario: Download a completed audiobook file
    Given I am on the audiobook convert page
    And a completed job exists
    When I click download on the completed job
    Then the file download is initiated

  @audiobook-failed-job-shows-error
  Scenario: Failed job displays an error message
    Given I am on the audiobook convert page
    And a voice sample exists
    When I submit a conversion job that will fail
    Then the job status changes to "failed"
    And an error message is visible on the failed job row

  @audiobook-no-voice-samples-disables-form
  Scenario: Upload form is disabled when no voice samples exist
    Given I am on the audiobook convert page
    And no voice samples exist
    Then the conversion form is disabled
    And a message directing me to the Voice Samples tab is visible

  @audiobook-upload-voice-sample
  Scenario: Upload a voice sample
    Given I am on the audiobook voice samples page
    When I upload a WAV file as a voice sample
    Then the new voice sample appears in the list

  @audiobook-delete-voice-sample
  Scenario: Delete a voice sample
    Given I am on the audiobook voice samples page
    And at least one voice sample exists
    When I delete a voice sample
    Then the voice sample is removed from the list

  @audiobook-delete-completed-job
  Scenario: Delete a completed job
    Given I am on the audiobook convert page
    And a completed job exists
    When I click delete on the completed job
    Then the job is removed from the queue list

  @audiobook-delete-failed-job
  Scenario: Delete a failed job
    Given I am on the audiobook convert page
    And a failed job exists
    When I click delete on the failed job
    Then the job is removed from the queue list

  @audiobook-delete-cancelled-job
  Scenario: Delete a cancelled job
    Given I am on the audiobook convert page
    And a cancelled job exists
    When I click delete on the cancelled job
    Then the job is removed from the queue list
