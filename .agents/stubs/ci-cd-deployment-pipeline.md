# CI/CD & Deployment Pipeline

**Status:** stub
**Created:** 2026-04-17

## Summary

Automated build, test, and deployment pipeline that keeps the HomeHub running on the home network with minimal manual intervention.

## Problem / Opportunity

Without a CI/CD pipeline, deploying changes to the home network is a manual, error-prone process. As features accumulate, this becomes a bottleneck — a reliable pipeline enables fast iteration and confidence that merges won't break things.

## Success Looks Like

- A push to the main branch triggers automated tests and, on success, deploys to the home network
- Failed builds/tests block deployment and surface clear feedback
- The pipeline is repeatable and requires no manual steps to ship a change

## Notes & Open Questions

- What is the current deployment target? (bare metal, Docker, VM?)
- Self-hosted CI runner vs. a hosted solution (GitHub Actions, etc.)?
- Should staging/preview environments be in scope, or just production?
- Rollback strategy — automatic or manual?
