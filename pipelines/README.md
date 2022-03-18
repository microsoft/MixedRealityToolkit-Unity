# Pipelines

## Each file and their use

### ci-daily

This runs on externally-facing machines daily. It validates the repo on both Unity 2018 and Unity 2019.

### ci-packaging-dontpublish

This runs on externally-facing machines whenever a PR is merged.

### ci-packaging-internal

This runs on internally-facing machines whenever a PR is merged.

### ci-weekly-internal

This runs on internally-facing machines every Monday at 8:00 UTC for compliance tasks.

### docs-binaries

This runs on internally-facing machines whenever a PR is merged. It generates binaries for updating the [MRTK API documentation](https://docs.microsoft.com/dotnet/api/microsoft.mixedreality.toolkit).

### mergetool

This runs on externally-facing machines every morning when enabled, typically during stabilization periods. It checks for the presence of new changes on a specific branch and opens a PR, if necessary, onto a second specific branch.

### packageupmpublic

This runs on externally-facing machines manually to repackage stabilization packages for full release.

### pr

This runs on externally-facing machines as part of PR validation.
