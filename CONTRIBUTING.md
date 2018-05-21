# Contributing

HoloToolkit welcomes contributions from the community. 
If you have any questions, please reach out on the [HoloLens forums](https://forums.hololens.com/).

# Process

1. [Make a proposal](https://github.com/Microsoft/HoloToolkit-Unity/issues) (either new, or for one of the elements in our backlog)
2. Identify the appropriate development branch
3. Implement the proposal and its tests.
4. Ensure that your implementation meets the [coding guidelines](CodingGuidelines.md).
5. Start a pull request & address comments.
6. Merge.

# Proposal

For things like fixing typos and small bug fixes, you can skip this step.

If your change is more than a simple fix, please don't just create a big pull request. 
Instead, start by [opening an issue](https://github.com/Microsoft/HoloToolkit-Unity/issues) describing the problem you want to solve and how you plan to approach the problem. 
This will let us have a brief discussion about the problem and, hopefully, identify some potential pitfalls before too much time is spent.

Note:  If you wish to work on something that already exists on our backlog, you can use that work item as your proposal.  

# Identifing the development branch

The master branch does not accept direct pull requests. Each change must be submitted through an appropriate developement branch. Each upcoming MRTK release has its own development branch (ex: may18_dev).

To identify the appropriate development branch:

1. Navigate to the MRTK [Projects](https://github.com/Microsoft/MixedRealityToolkit-Unity/projects) page
2. Find the next Mixed Reality Toolkit project (generally, by closest date)
3. If the **Status** is not **Accepting changes**, select the next release
4. Note the **Development branch** for Pull Request submission.

Any pull request made directly to master will be retargetted to a development branches by a project maintainer.

NOTE: A project maintainer may retarget your PR to a different development branch based on the release priorities.

# Implementation

1. Fork the repository. Click on the "Fork" button on the top right of the page and follow the flow.
2. If your work needs more time, the consider branching off of master else just code in your fork.
3. Instructions for getting the project building and running the tests are in the [README](https://github.com/Microsoft/HoloToolkit-Unity/blob/master/README.md). 
4. Make small and frequent commits that include tests which could be a unity scene showing usage of your feature.
5. Make sure that all the tests continue to pass.
6. Ensure the code is [WACK compliant](https://developer.microsoft.com/en-us/windows/develop/app-certification-kit). To do this, generate a Visual Studio solution, right click on project; Store -> Create App Packages. Follow the prompts and run WACK tests. Make sure they all succeed.
7. Ensure you update the [README](https://github.com/Microsoft/HoloToolkit-Unity/blob/master/README.md) with additional documentation as needed.
8. Also update the [HoloToolkit-Unity wiki](https://github.com/Microsoft/HoloToolkit-Unity/wiki) if you think it will be useful for other developers.

# Pull request

Start a GitHub pull request to merge your topic branch into the release's development branch (as identified above). 
(If you are a Microsoft employee and are not a member of the [Microsoft organization on GitHub](https://github.com/Microsoft) yet, please link your Microsoft and GitHub accounts on corpnet by visiting [Open Source at Microsoft](https://opensource.microsoft.com/) before you start your pull request. There's some process stuff you'll need to do ahead of time.)
If you haven't contributed to a Microsoft project before, you may be asked to sign a [contribution license agreement](https://cla.microsoft.com/). 
A comment in the PR will let you know if you do.

The project maintainers will review your changes. We aim to review all changes within three business days.
Address any review comments, force push to your topic branch, and post a comment letting us know that there's new stuff to review.

# Merge

If the pull request review goes well, a project maintainer will merge your changes. Thank you for helping improve the Mixed Reality Toolkit!
