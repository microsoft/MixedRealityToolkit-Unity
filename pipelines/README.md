# Pipelines

## Each file and their use

### ci-daily

This runs on externally-facing machines daily. It validates the repo on both Unity 2018 and Unity 2019.

### ci-packaging-dontpublish

This runs on externally-facing machines whenever a PR is merged.

### ci-packaging-internal

This runs on internally-facing machines whenever a PR is merged.

### ci-release

This runs on internally-facing machines manually. It packages and signs the packages for public release.

### pr

This runs on externally-facing machines as part of PR validation.
