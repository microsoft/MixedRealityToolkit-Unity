# Pull Requests

## Prerequisites
If you haven't contributed to a Microsoft project before, you may be asked to sign a [contribution license agreement](https://cla.microsoft.com/). 
A comment in the PR will let you know if you do.

> [!IMPORTANT]
> If you are a Microsoft employee and are not a member of the [Microsoft organization on GitHub](https://github.com/Microsoft), please link your Microsoft and GitHub accounts on corpnet by visiting [Open Source at Microsoft](https://opensource.microsoft.com/) before you start your pull request. There's some process stuff you'll need to do ahead of time.

## Creating a Pull Request
When you are ready to submit a pull request, [create a pull request](https://github.com/microsoft/MixedRealityToolkit-Unity/compare/mrtk_development...mrtk_development?expand=1) targeting the [mrtk_development](https://github.com/microsoft/mixedrealitytoolkit-unity/tree/mrtk_development) branch.

Read the pull request guidelines and ensure your pull request meets the guidelines.

In particular,
* Make sure you reference any Issue / Feature Request or Task the PR relates to.
* Validate that you are only checking in files / changes related to the PR.
* Check your documentation is up to date and included (unless submitted in a previous PR).
* If adding a new feature, check that tests are included to validate the feature (see [UnitTests](UnitTests.md)).
* If fixing a bug, write a test to catch the bug. This is to make sure your fix does not regress in the future.
* Read all of the guidelines below for additional information about 

The project maintainers will review your changes. We aim to review all changes within three business days. Please address any review comments, push to your topic branch, and post a comment letting us know that there's new stuff to review.

> [!NOTE]
> All PR's submitted to the project will also be vetted according to the [MRTK coding standards guide](CodingGuidelines.md), so please review these before submitting your PR to ensure a smooth process.

## Pull Request Guidelines
These guidelines are based experience reviewing prior MRTK pull requests. We considered both pull requests that were quickly accepted, and pull requests that took a long time or did not make it in. Following these guidelines will ensure your pull request is reviewed and completed quickly. 

These guidelines are based off of the [Google's engineering practices](https://google.github.io/eng-practices/review/developer/small-cls.html).

### Keep your pull requests small
**Reason**
Smaller PRs are reviewed more quickly and thoroughly, are less likely to introduce bugs, easier to roll back, and easier to merge.

**Notes**
Pull requests should be small enough that an engineer could review it in under 30 minutes, then split the CL into several smaller CLs that build on each other

Try to make a minimal change that addresses just one thing.

If you must create a large PR, split it into several PRs that go into either your local branch, or a feature branch of MRTK.

Aim to re-use components instead of adding new ones.

Make sure any assets (e.g. fbx, obj files) are small.

### Tests should be added in the same PR as your fix / feature, except for emergencies
**Reason**
Tests are the best way to ensure changes do not regress existing code, but it is also easy to forget about tests as people write new features. Requiring that they go in with your PR are a great way to ensure that tests get written.

**Notes**
Every feature and bugfix should have tests associated with it.

If you do not have the expertise or time to write a test, create an issue to write the tests, and mark them for “Consider for Current Iteration”.

### Documentation should be added in the same pull request as a fix / feature
**Reason**
Most developers look first at documentation, not code, when understanding how to use a feature. Ensuring documentation is up to date make it much easier for people to consume MRTK. Requiring documentation goes in with a pull request is a great way to ensure you write documentation for each feature.

**Notes**
Ensure every public field, method, property has [triple slash summary comments](https://dotnet.github.io/docfx/spec/triple_slash_comments_spec.html) so our docfx site can generate descriptions for fields / methods.

If needed, update markdown files in Documentation folder.

### Pull request descriptions should clearly and completely describe changes
**Reason**
Clear and complete descriptions of pull requests ensure reviewers understand what they are reviewing.

**Notes**
If adding features that contain UX, add an image / gif of the feature you are changing. Here is a good example: https://github.com/microsoft/MixedRealityToolkit-Unity/pull/4532

Another suggestion is to have a gif of Before and After, for example in this pull request: https://github.com/microsoft/MixedRealityToolkit-Unity/pull/5896

ScreenToGif is a great tool for generating gifs from screen captures.
