# Copyright (c) Microsoft Corporation.
# Licensed under the MIT License.
#
# A script that will write out the list of authors to a MarkDown file for committing to the MRTK repo.
#
# Uses pygithub. Requires a GitHub PAT.

import argparse
from github import Github
from github.Repository import Repository

AUTHORS_PAGE_HEADER = "# Authors\n\nThe Microsoft Mixed Reality Toolkit is a collaborative project containing contributions from individuals around the world. Our sincere thanks to all who have, and continue to contribute."

args_parser = argparse.ArgumentParser()
args_parser.add_argument('--repo', type=str, required=True)
args_parser.add_argument('--file_header', type=str)
args_parser.add_argument('--file_location', type=str)
args_parser.add_argument('--pat', type=str, required=True)
args = args_parser.parse_args()

def get_all_authors(github_repo: Repository) -> str:
    authors = github_repo.get_contributors()
    # Sort alphabetically by name or login if no name is present
    authors = sorted(authors, key=lambda author: str.lower(author.login) if author.name is None else str.lower(author.name))
    author_list = ""
    for author in authors:
        # Filter the MRTK 🤖 from the list
        if author.login == "mrtk-bld" or "[bot]" in author.login:
            continue
        if author.name is None:
            author_list += "- " + author.login + "\n"
        else:
            author_list += "- " + author.name + " (" + author.login + ")\n"
    return author_list

def main(args):
    github_access = Github(args.pat)
    github_repo = github_access.get_repo(args.repo)
    author_list = get_all_authors(github_repo)

    f = open("Authors.md" if args.file_location is None else args.file_location, "w", encoding="utf-8")
    f.write((AUTHORS_PAGE_HEADER if args.file_header is None else args.file_header) + "\n\n" + author_list)
    f.close()

if __name__ == '__main__':
    main(args)
