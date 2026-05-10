@comfyui
Feature: ComfyUI Text-to-Image

  @comfyui-add-workflow
  Scenario: Add a workflow on the config page
    Given I am on the ComfyUI config page
    When I fill in a workflow name and paste a workflow JSON
    And I submit the Add Workflow form
    Then the new workflow should appear in the workflow list

  @comfyui-delete-workflow
  Scenario: Delete a workflow on the config page
    Given a ComfyUI workflow exists
    And I am on the ComfyUI config page
    When I delete the workflow
    Then the workflow should no longer appear in the list

  @comfyui-generate-happy-path
  Scenario: Generate an image from a workflow
    Given a ComfyUI workflow exists
    And I am on the ComfyUI page
    When I select the workflow from the dropdown
    And I enter a prompt
    And I click Generate
    Then I should see a loading state while the image is generating
    And I should see the generated image when complete

  @comfyui-generate-error
  Scenario: Generate shows an error when ComfyUI is unreachable
    Given a ComfyUI workflow exists
    And the ComfyUI stub server is set to fail
    And I am on the ComfyUI page
    When I select the workflow and enter a prompt
    And I click Generate
    Then I should see an inline error message
    And the Generate button should be re-enabled
