# PRD: [Feature Name]

## Status

`Draft` <!-- Draft | Review | Approved | Superseded -->

## Overview

_One paragraph summarizing what this is and why it matters._

## Problem Statement

_What problem does this solve? Who is affected and how severely? What is the cost of not solving it?_

## Goals

- _Goal 1_
- _Goal 2_

## Non-Goals

- _Explicitly out of scope item 1_
- _Explicitly out of scope item 2_

## User Stories / Use Cases

_Describe the key scenarios from the user's perspective._

- **As a** [user type], **I want to** [action], **so that** [outcome].

## E2E Scenarios

<!-- If this feature has user-visible behaviour, draft Gherkin scenarios here.
     Leave this section empty or omit it for backend-only / infrastructure PRDs. -->

```gherkin
@<domain-tag>
Feature: <Feature Name>

  @<scenario-slug>
  Scenario: <Scenario title>
    Given ...
    When ...
    Then ...
```

## Proposed Solution

_High-level description of the solution. What will be built?_

## Technical Approach

_Key architectural decisions, technology choices, and design constraints. Reference any decisions reached during the planning session._

### Key Decisions

| Decision | Chosen Approach | Rationale |
|----------|----------------|-----------|
| _e.g. Data storage_ | _e.g. PostgreSQL_ | _e.g. Already in use, fits relational model_ |

### Dependencies

_External services, libraries, or internal systems this relies on._

## Open Questions

_Unresolved items that need answers before or during implementation._

- [ ] _Question 1_
- [ ] _Question 2_

## Out of Scope

_Anything explicitly deferred to a future iteration._

## Success Metrics

_How will we know this is working?_

- _Metric 1 (e.g. p95 latency < 200ms)_
- _Metric 2 (e.g. error rate < 0.1%)_

## Timeline / Milestones

_TBD_

<!-- 
| Milestone | Target Date |
|-----------|------------|
| Design complete | YYYY-MM-DD |
| Implementation complete | YYYY-MM-DD |
| Shipped | YYYY-MM-DD |
-->
