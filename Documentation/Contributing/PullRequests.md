# Pull requests

## Prerequisites

If you haven't contributed to a Microsoft project before, you may be asked to sign a [contribution license agreement](https://cla.microsoft.com/).
A comment in the PR will let you know if you do.

> [!IMPORTANT]
> If you are a Microsoft employee and are not a member of the [Microsoft organization on GitHub](https://github.com/Microsoft), please link your Microsoft and GitHub accounts on corpnet by visiting [Open Source at Microsoft](https://opensource.microsoft.com/) before you start your pull request. There's some process stuff you'll need to do ahead of time.

## Creating a pull request

When you are ready to submit a pull request, [create a pull request](https://github.com/microsoft/MixedRealityToolkit-Unity/compare/mrtk_development...mrtk_development?expand=1) targeting the [mrtk_development](https://github.com/microsoft/mixedrealitytoolkit-unity/tree/mrtk_development) branch.

Read the guidelines and ensure your pull request meets the guidelines.

* Make sure to reference any Issue / Feature Request or Task the PR relates to.
* Check the pull request contains only files / changes related to the PR.
* Check documentation is up to date and included. Check all public fields have comments.
* If adding a new feature, check that tests are included to validate the feature (see [UnitTests](UnitTests.md)).
* If fixing a bug, write a test to verify the bug fix.

The project maintainers will review your changes. We aim to review all changes within three business days. Please address any review comments, push to your topic branch, and post a comment letting us know that there's new stuff to review.

> [!NOTE]
> All PR's submitted to the project will also be vetted according to the [MRTK coding standards guide](CodingGuidelines.md), so please review these before submitting your PR to ensure a smooth process.

## Pull request guidelines

These guidelines are based off of the [Google's engineering practices](https://google.github.io/eng-practices/review/developer/small-cls.html).

### Keep pull requests small

Smaller PRs are reviewed more quickly and thoroughly, are less likely to introduce bugs, easier to roll back, and easier to merge.

Pull requests should be small enough that an engineer could review it in under 30 minutes. Try to make a minimal change that addresses just one thing. If you must create a large PR, split it into several PRs that go into either your local branch, or a feature branch of MRTK. Avoid adding new assets (e.g. fbx, obj files) and instead aim to re-use existing assets.

### Tests should be added in the same PR as your fix / feature, except for emergencies

Tests are the best way to ensure changes do not regress existing code, but it is also easy to forget about tests when submitting pull requests. Requiring that they go in with your PR are a great way to ensure that tests get written.

Every feature and bug fix should have tests associated with it. If you do not have the expertise or time to write a test, create an issue to write the tests, and mark them with label **Consider for Current Iteration**.

### Documentation should be added in the same pull request as a fix / feature

Most developers look first at documentation, not code, when understanding how to use a feature. Ensuring documentation is up to date makes it much easier for people to consume and rely MRTK.  Documentation should always be bundled with the related pull to ensure items remain up-to-date and consistent.

Ensure every public field, method, property has [triple slash summary comments](https://dotnet.github.io/docfx/spec/triple_slash_comments_spec.html) so our docfx site can generate descriptions for fields / methods. If needed, update markdown files in Documentation folder.

### Pull request descriptions should clearly and completely describe changes

Clear and complete descriptions of pull requests ensure reviewers understand what they are reviewing.

If adding features that contain UX, add an image / gif of the feature you are changing. [Here is a good example](https://github.com/microsoft/MixedRealityToolkit-Unity/pull/4532). Another suggestion is to have a gif of Before and After, [for example in this pull request](https://github.com/microsoft/MixedRealityToolkit-Unity/pull/5896). A tool we recommend for generating gifs from screen captures is [ScreenToGif](https://www.screentogif.com/).
