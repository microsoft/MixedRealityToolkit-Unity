# issue-sync-action
Used to sync issues and comments from one repository to another, for example for use in internal roadmap publication.
**Please note: you need to set a GITHUB_TOKEN environmental variable that is authorized to read from the source repository as well as write new issues to the target repository. It is recommended to use the integrated GitHub Actions secrets vault for managing this secret.**

This supports different options:
```yml
only_sync_on_label:
  description: 'If set, will only sync on issues with a label of this text'
  required: false
repo_source:
  description: 'Org/Repo slug for the source repository. Will default to action launch repo if not set.'
  required: false
repo_target:
  description: 'Org/repo slug for the target repository.'
  required: true
only_sync_main_issue:
  description: 'Will exclude the syncing of comments.'
  required: false
  default: "false"
 ```
 
Here is a usage example:
```yml

on:
  issues:
  issue_comment:

env:
  GITHUB_TOKEN: ${{ secrets.GH_TOKEN_FOR_BOTH_REPOS }}

- name: Run the typescript action
  uses: Maximisch/issue-sync-action
  with:
    repo_target: "MyOrg/public-roadmap" # The target repository
    only_sync_on_label: "publicise" # Only syncs issues with this label set
    only_sync_main_issue: true # Excludes comments
```
