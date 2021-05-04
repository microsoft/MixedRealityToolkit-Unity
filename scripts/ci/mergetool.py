# Copyright (c) Microsoft Corporation.
# Licensed under the MIT License.
#
# A script that will check if any commits exist in the source branch that aren't present in the destination branch.
# Opens a PR if needed and tags it with the passed in label.
#
# Uses pygithub and gitpython. Requires a GitHub PAT.

import argparse
import git
from github import Github
from github.PullRequest import PullRequest
from github.Repository import Repository

PULL_REQUEST_TITLE_TEMPLATE = "Branch synchronization: {0} --> {1}"
PULL_REQUEST_DESCRIPTION = "This is a pull request initiated by an automated process to keep {0} and {1} in sync"

args_parser = argparse.ArgumentParser()
args_parser.add_argument('--repo', type=str, required=True)
args_parser.add_argument('--repo_path', type=str, required=True)
args_parser.add_argument('--source_branch', type=str, required=True)
args_parser.add_argument('--destination_branch', type=str, required=True)
args_parser.add_argument('--label', type=str, required=True)
args_parser.add_argument('--pat', type=str, required=True)
args = args_parser.parse_args()

def get_num_diffs(source_branch: str, destination_branch: str) -> int:
    repo = git.Repo(args.repo_path)
    repo.git.checkout(source_branch)
    repo.git.checkout(destination_branch)
    source_branch_head = repo.heads[args.source_branch]
    common_ancestor = repo.merge_base(source_branch_head, repo.heads[args.destination_branch])
    return len(common_ancestor[0].diff(source_branch_head.commit)) if len(common_ancestor) > 0 else 0

def get_existing_pull_request(github_repo: Repository, search_label: str) -> PullRequest:
    pull_requests = github_repo.get_pulls()
    for pull_request in pull_requests:
        for label in pull_request.labels:
            if label.name == search_label:
                return pull_request
    return None

def create_pull_request(github_repo: Repository, source_branch: str, destination_branch: str, label: str):
    print('Creating pull request...')
    pull_request = github_repo.create_pull(
        title=PULL_REQUEST_TITLE_TEMPLATE.format(source_branch, destination_branch),
        body=PULL_REQUEST_DESCRIPTION.format(source_branch, destination_branch),
        base=destination_branch,
        head=source_branch)
    pull_request.add_to_labels(label)
    print(f'Created pull request {pull_request.url} with label {label}')

def main(args):
    github_access = Github(args.pat)
    github_repo = github_access.get_repo(args.repo)

    num_diffs = get_num_diffs(args.source_branch, args.destination_branch)
    if num_diffs == 0:
        print(f'All commits from [{args.source_branch}] that are already present in [{args.destination_branch}]')
        return

    print(f'There are {num_diffs} files from [{args.source_branch}] that need to be merged into [{args.destination_branch}]')
    print(f'Starting a pull request to merge [{args.source_branch}] into [{args.destination_branch}]')

    print('Checking to see if there is already an outstanding merge to avoid creating duplicate requests.')

    existing_pull_request = get_existing_pull_request(github_repo, args.label)
    if existing_pull_request:
        print(f'Existing pull request {existing_pull_request.url} found, exiting early.')
        return

    create_pull_request(github_repo, args.source_branch, args.destination_branch, args.label)

if __name__ == '__main__':
    main(args)
