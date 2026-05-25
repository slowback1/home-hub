@bookmarks
Feature: Bookmarks

  @view-bookmarks-page
  Scenario: View bookmarks page shows saved bookmarks as cards
    Given I have saved bookmarks
    When I navigate to the bookmarks page
    Then I should see each bookmark displayed as a card with its title and favicon

  @add-bookmark-happy-path
  Scenario: Add a bookmark with title, URL, and description
    Given I am on the bookmarks page
    When I open the add bookmark modal and submit a URL, name, and description
    Then the new bookmark card should appear on the page

  @add-bookmark-url-normalization
  Scenario: URL without a protocol is normalized to https
    Given I am on the bookmarks page
    When I add a bookmark with the URL "github.com" and no protocol
    Then the bookmark should be saved with the URL "https://github.com"

  @edit-bookmark
  Scenario: Edit a bookmark name and description
    Given I have a saved bookmark
    When I open the edit modal and change the name and description
    Then the card should reflect the updated values

  @delete-bookmark-confirmed
  Scenario: Delete a bookmark after confirming the dialog
    Given I have a saved bookmark
    When I click delete and confirm the confirmation dialog
    Then the bookmark card should no longer appear on the page

  @delete-bookmark-cancelled
  Scenario: Cancel delete confirmation leaves the bookmark intact
    Given I have a saved bookmark
    When I click delete and cancel the confirmation dialog
    Then the bookmark card should still appear on the page

  @star-bookmark
  Scenario: Star a bookmark toggles its starred state
    Given I have a saved bookmark that is not starred
    When I click the star button on the card
    Then the bookmark should be marked as starred

  @search-bookmarks
  Scenario: Search filters visible bookmark cards
    Given I have multiple saved bookmarks
    When I type a search term that matches only one bookmark
    Then only the matching bookmark card should be visible

  @bookmarks-empty-state
  Scenario: Empty state is shown when no bookmarks exist
    Given I have no saved bookmarks
    When I navigate to the bookmarks page
    Then I should see an empty state message
