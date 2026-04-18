# Delete Superseded Draft Workflows

## Status

`done` <!-- pending | in-progress | done -->

## Description

Remove the three legacy workflow files under `frontend/deployment/github_actions/` that pre-date this pipeline. They target S3 deployment (out of scope) and will never be run again. Deleting them avoids confusion about which workflows are canonical.

## Acceptance Criteria

- [ ] `frontend/deployment/github_actions/run-unit-tests.yml` is deleted
- [ ] `frontend/deployment/github_actions/deploy-to-s3.yml` is deleted
- [ ] `frontend/deployment/github_actions/README.md` is deleted
- [ ] No other files in the repo are modified

## Notes

See PRD §Files Changed. The `frontend/deployment/github_actions/` directory itself may be removed if it becomes empty.
