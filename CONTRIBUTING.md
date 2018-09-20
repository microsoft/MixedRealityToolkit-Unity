# Contributing

The Mixed Reality Toolkit welcomes contributions from the community. For things like fixing typos and small bug fixes, please follow the following contribution guidelines.

>For feature branch contribution guidelines see the [Feature Contribution Process](./Feature_Contribution_Process.md).

If you have any questions, please reach out on the [HoloLens forums](https://forums.hololens.com/) or the [HoloDevelopers slack](https://holodevelopers.slack.com/).

# Process

1. [Open an issue](https://github.com/Microsoft/MixedRealityToolkit-Unity/issues/new/choose)
    - If you are implementing something from our backlog, you do not need to file a new proposal.
2. Implement the change, including any needed demo scenes and/or unit tests.
3. Start a pull request & address any questions or change requests.
4. Merge.

# Proposals

If your change is more than a simple fix or a substantial change, please don't just create a big pull request.

Instead, start by [opening a proposal](https://github.com/Microsoft/MixedRealityToolkit-Unity/issues?q=is%3Aopen+is%3Aissue+label%3AProposal) describing the problem you want to solve and how you plan to approach the problem. This will let us have a brief discussion about the problem and, hopefully, identify some potential pitfalls before any work is started.

If you're proposing a completely new feature (or a new platform support) please follow the [Feature Contribution Process](./Feature_Contribution_Process.md).

>Note:  If you wish to work on something that already exists on our backlog, you can use that work item as your proposal. Be sure to also comment on the task notifying maintainers that you're working towards completing it.

# Implementation

1. Fork the repository. Click on the "Fork" button on the top right of the page and follow the flow.
2. Create a branch in your fork (off of the [mrtk_development](https://github.com/microsoft/mixedrealitytoolkit-unity/tree/mrtk_development) branch) to make it easier for you to isolate your fork.
3. Instructions for getting the project building and running the tests are in the [README](README.md). 
4. Make small and frequent commits that include tests which could be a unity scene showing usage of your feature.
5. Make sure that all the tests continue to pass.
6. Follow the [Coding Guidelines](/CodingGuidelines.md).
7. Ensure the code and feature(s) are documented as describred in the [Documentation Guidelines](/DocumentationGuidelines.md).
8. Ensure the code works as intended on all [platforms](#supported-platforms).
    - For Windows UWP projects, your code must be [WACK compliant](https://developer.microsoft.com/en-us/windows/develop/app-certification-kit). To do this, generate a Visual Studio solution, right click on project; Store -> Create App Packages. Follow the prompts and run WACK tests. Make sure they all succeed.
9. Update the [documentation](#update-documentation) with additional information as needed.

## Update Documentation

The Mixed Reality Toolkit provides the following forms of documentation.

### APIs

This documentation is generated from the product code and is reviewed as part of **all** pull requests.

### Conceptual

Conceptual documentation is hosted on https://docs.microsoft.com/en-us/windows/mixed-reality. Please submit our changes via Pull Request at https://github.com/MicrosoftDocs/mixed-reality.

### Readme files

As part of your pull request, please update (or create) the Readme markdown file in the appropriate feature folder. This will allow GitHub users to gain a high-level understanding of your new feature.

## Supported Platforms

The Mixed Reality Toolkit supports the following mixed reality (AR/VR/XR) platforms:

- Windows Standalone
    - OpenVR
- Windows Mixed Reality
    - Immersive devices
    - Microsoft HoloLens

# Pull request

Start a GitHub pull request to merge your topic branch into the [mrtk_development](https://github.com/microsoft/mixedrealitytoolkit-unity/tree/mrtk_development) branch. 
> If you are a Microsoft employee and are not a member of the [Microsoft organization on GitHub](https://github.com/Microsoft), please link your Microsoft and GitHub accounts on corpnet by visiting [Open Source at Microsoft](https://opensource.microsoft.com/) before you start your pull request. There's some process stuff you'll need to do ahead of time.

If you haven't contributed to a Microsoft project before, you may be asked to sign a [contribution license agreement](https://cla.microsoft.com/). 
A comment in the PR will let you know if you do.

The project maintainers will review your changes. We aim to review all changes within three business days.
Address any review comments, push to your topic branch, and post a comment letting us know that there's new stuff to review.

# Merge

If the pull request review goes well, a project maintainer will merge your changes. Thank you for helping improve the Mixed Reality Toolkit!