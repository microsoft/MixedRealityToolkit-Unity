# Contributing

HoloToolkit welcomes contributions from the community. 
If you have any questions, please reach out on the [HoloLens forums](https://forums.hololens.com/).

# Process

1. [Make a proposal](https://github.com/Microsoft/HoloToolkit-Unity/issues) (either new, or for one of the elements in our backlog)
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

1. Fork the repository. Click on the "Fork" button on the top right of the page and follow the flow.
2. If your work needs more time, the consider branching off of master else just code in your fork.
3. Instructions for getting the project building and running the tests are in the [README](https://github.com/Microsoft/HoloToolkit-Unity/blob/master/README.md). 
4. Make small and frequent commits that include tests which could be a unity scene showing usage of your feature.
5. Make sure that all the tests continue to pass.
6. Ensure the code is [WACK compliant](https://developer.microsoft.com/en-us/windows/develop/app-certification-kit). To do this, generate a Visual Studio solution, right click on project; Store -> Create App Packages. Follow the prompts and run WACK tests. Make sure they all succeed.
7. Ensure you update the [README](https://github.com/Microsoft/HoloToolkit-Unity/blob/master/README.md) with additional documentation as needed.
8. Also update the [HoloToolkit-Unity wiki](https://github.com/Microsoft/HoloToolkit-Unity/wiki) if you think it will be useful for other developers.

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
(If you are a Microsoft employee and are not a member of the [Microsoft organization on GitHub](https://github.com/Microsoft) yet, please link your Microsoft and GitHub accounts on corpnet by visiting [Open Source at Microsoft](https://opensource.microsoft.com/) before you start your pull request. There's some process stuff you'll need to do ahead of time.)
If you haven't contributed to a Microsoft project before, you may be asked to sign a [contribution license agreement](https://cla.microsoft.com/). 
A comment in the PR will let you know if you do.

The project maintainers will review your changes. We aim to review all changes within three business days.
Address any review comments, force push to your topic branch, and post a comment letting us know that there's new stuff to review.

# Merge

If the pull request review goes well, a project maintainer will merge your changes. Thank you for helping improve HoloToolkit!
