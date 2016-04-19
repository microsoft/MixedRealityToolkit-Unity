# Contributing

HoloToolkit welcomes contributions from the community.

# Process

1. Make a proposal (either new, or for one of the elements in our backlog)
2. Implement the proposal and its tests.
3. Rebase commits to tell a compelling story.
4. Start a pull request & address comments.
5. Merge.

# Proposal

For things like fixing typos and small bug fixes, you can skip this step.

If your change is more than a simple fix, please don't just create a big pull request. 
Instead, start by [opening an issue](https://github.com/Microsoft/HoloToolkit-Unity/issues) describing the problem you want to solve and how you plan to approach the problem. 
This will let us have a brief discussion about the problem and, hopefully, identify some potential pitfalls before too much time is spent.

Note:  If you wish to work on something that already exists on our backlog, you can use that work item as your proposal.  

# Implementation

1. Fork the repository on GitHub.
2. Start on a new topic branch off of master.
3. Instructions for getting the project building and running the tests are in the README. 
4. Make small and atomic commits that include tests.
5. Make sure that all the tests continue to pass. 

# Rebase commits

The commits in your pull request should tell a story about how the code got from point A to point B. 
Good stories are edited, so you'll want to rebase your commits so that they tell a good story.

Each commit should build and pass all of the tests. 
If you want to add new tests for functionality that's not yet written, ensure the tests are added disabled.

Don't forget to run git diff --check to catch those annoying whitespace changes.
 
Please follow the established [Git convention for commit messages](https://www.git-scm.com/book/en/v2/Distributed-Git-Contributing-to-a-Project#Commit-Guidelines). 
The first line is a summary in the imperative, about 50 characters or less, and should not end with a period. 
An optional, longer description must be preceded by an empty line and should be wrapped at around 72 characters. 
This helps with various outputs from Git or other tools.

You can update message of local commits you haven't pushed yet using git commit --amend or git rebase --interactivewith reword command.

# Pull request

Start a GitHub pull request to merge your topic branch into the [main repository's master branch](https://github.com/Microsoft/HoloToolkit-Unity/tree/master). 
(If you are a Microsoft employee and are not a member of the [Microsoft organization on GitHub](https://github.com/Microsoft) yet, please contact the DDE team via e-mail for instructions before starting your pull request. There's some process stuff you'll need to do ahead of time.)
If you haven't contributed to a Microsoft project before, you may be asked to sign a [contribution license agreement](https://cla.microsoft.com/). 
A comment in the PR will let you know if you do.

The project maintainers will review your changes. We aim to review all changes within three business days.
Address any review comments, force push to your topic branch, and post a comment letting us know that there's new stuff to review.

# Merge

If the pull request review goes well, a project maintainer will merge your changes. Thank you for helping improve HoloToolkit!