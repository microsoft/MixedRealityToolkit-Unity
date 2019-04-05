# Contributing

The Mixed Reality Toolkit (MRTK) welcomes contributions from the community. Whether it is for a minor change like fixing typos and small bug fixes, or a new feature or component.

For larger submissions, we have drafted contribution guidelines to ensure a smooth process and a good quality of code and documentation, so please be sure to review the  [Feature Contribution guidelines / Process](Feature_Contribution_Process.md).

All changes be they small or large, need to adhere to the [MRTK Coding Standards](CodingGuidelines.md), so please ensure you are familiar with these while developing to avoid delays when the change is being reviewed.

If you have any questions, please reach out on the [HoloLens forums](https://forums.hololens.com/) or the [HoloDevelopers slack](https://holodevelopers.slack.com/). You can easily be granted access to the Slack community via the [automatic invitation sender](https://holodevelopersslack.azurewebsites.net/).

# Submission process
We provide several paths to enable developers to contribute to the Mixed Reality Toolkit, all starting with [creating a new Issue](https://github.com/Microsoft/MixedRealityToolkit-Unity/issues/new/choose)

![](External/ReadMeImages/issue_selection_prompt.png)

From here you can either:

* **Open a new issue** - telling of us a bug or issue with the project you are facing.
> We recommend discussing issues in the [HoloDevelopers slack](https://holodevelopers.slack.com/) channel first to ensure it's an issue with the MRTK.

* **Raise a new Feature request** - some missing feature that you really need, or have even implemented your self that you would like to see added to the project

* **Create a new Task for the Mixed Reality vNext project** - defining a new feature or component for the next generation of the Mixed Reality Toolkit


# Creating proposals

To ensure a smooth process when contributing new fixes or features, it's key that you start your journey by creating one of the issue types listed above.

Start by [opening a proposal](https://github.com/Microsoft/MixedRealityToolkit-Unity/issues/new/choose) describing the change you want to make and how your proposed implementation, or simply the issue you are facing. This will enable us have a brief discussion about the proposal and, hopefully, identify some potential pitfalls before any work is started.

If you're proposing a completely new feature (or a new platform support) please follow the [Feature Contribution Process](Feature_Contribution_Process.md).

>Note:  If you wish to work on something that already exists on our backlog, you can use that work item as your proposal. Be sure to also comment on the task notifying maintainers that you're working towards completing it.

# Beginning development
Working with Git, the contribution process is quite simple (provided you have installed a good Git Client such as TortoiseGit or SourceTree)

> If you are new to to the Git workflow, [check out this tutorial on Pluralsight](https://www.pluralsight.com/blog/software-development/github-tutorial)

To get started, simply follow these steps

1. Fork the repository. Click on the "Fork" button on the top right of the page and follow the flow.
2. Create a branch in your fork (off of the [mrtk_development](https://github.com/microsoft/mixedrealitytoolkit-unity/tree/mrtk_development) branch) to make it easier for you to isolate your fork.
> for the legacy HoloToolkit use the [htk_development](https://github.com/Microsoft/MixedRealityToolkit-Unity/tree/htk_development) branch
3. Instructions for getting the project building and running the tests are in the [README](README.md). 
4. Make **small and frequent** commits that include tests which could be a unity scene showing usage of your feature.
5. Make sure that all the tests continue to pass.
6. Follow the [Coding Guidelines](CodingGuidelines.md).
7. Ensure the code and feature(s) are documented as described in the [Documentation Guidelines](Documentation/DocumentationGuide.md).
8. Ensure the code works as intended on all [platforms](#supported-platforms).
    - For Windows UWP projects, your code must be [WACK compliant](https://developer.microsoft.com/en-us/windows/develop/app-certification-kit). To do this, generate a Visual Studio solution, right click on project; Store -> Create App Packages. Follow the prompts and run WACK tests. Make sure they all succeed.
9. Update the [documentation](#contributing) with additional information as needed.



# Pull request
Once you have created your change, it's time to submit a Pull Request (PR) back to the project.  Please ensure all PR's are small and concise, DO NOT include other files / changes not related to the subject of the PR 
> e.g. Don't update the *projectversion.txt* when you are making changes or adding a button

If you haven't contributed to a Microsoft project before, you may be asked to sign a [contribution license agreement](https://cla.microsoft.com/). 
A comment in the PR will let you know if you do.

> If you are a Microsoft employee and are not a member of the [Microsoft organization on GitHub](https://github.com/Microsoft), please link your Microsoft and GitHub accounts on corpnet by visiting [Open Source at Microsoft](https://opensource.microsoft.com/) before you start your pull request. There's some process stuff you'll need to do ahead of time.

When you are ready:
* Start a GitHub pull request to merge your topic branch targetting the [mrtk_development](https://github.com/microsoft/mixedrealitytoolkit-unity/tree/mrtk_development) branch. 
* Ensure you fill in all details required by the Pull Request template, ensuring you reference any Issue / Feature Request or Task the PR relates to.
* Validate that you are only checking in files / changes related to the PR
* Check your documentation is up to date and included (unless submitted in a previous PR)


The project maintainers will review your changes. We aim to review all changes within three business days.
Please address any review comments, push to your topic branch, and post a comment letting us know that there's new stuff to review.
> All PR's submitted to the project will also be vetted according to the [MRTK coding standards guide](CodingGuidelines.md), so please review these before submitting your PR to ensure a smooth process.

# Merge

If the pull request review goes well, a project maintainer will merge your changes. Thank you for helping improve the Mixed Reality Toolkit!

# Documentation Requirements

The Mixed Reality Toolkit requires the following forms of documentation for any new feature or component.  Also ensure if you are simply patching / fixing an existing feature that the documentation is also updated to match.

### APIs

This documentation is generated from the product code and is reviewed as part of **all** pull requests.

### Conceptual

Conceptual documentation is hosted on https://docs.microsoft.com/en-us/windows/mixed-reality. Please submit your changes via Pull Request at https://github.com/MicrosoftDocs/mixed-reality.

### Readme files

As part of your pull request, please update (or create) the Readme markdown file in the appropriate feature folder. This will allow GitHub users to gain a high-level understanding of your new feature.

## Supported Platforms

The Mixed Reality Toolkit supports the following mixed reality (AR/VR/XR) platforms:

- Windows Standalone
    - OpenVR
- Universal Windows Platform
    - Standalone PC
    - Windows Mixed Reality Immersive devices
    - Microsoft HoloLens
