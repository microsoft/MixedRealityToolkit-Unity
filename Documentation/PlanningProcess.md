# Mixed Reality Toolkit Planning process

Before each milestone, the MRTK team will prioritize features to implement and bugs to fix. A
milestone will be comprised of one or more iterations, resulting in an official MRTK release.

Items will be distributed between iterations based on resource availablility and customer impact.
Each iteration item will have a definition for what it means for it to be "done".

## What is the definition of done?

Each type of work item will have a set of criteria that define being done, which are listed in
the following sections.

### Bugs

- The reported issue is addressed and resolved
- The fix has been manually verified
- Performance expectations have been met and maintained
- Automated regression test, if possible, is implemented
- Test automation passes

### Features

- The feature functions as expected and addresses the customer feedback
- Documentation (API and conceptual) has been written and peer reviewed
- As appropriate, default profiles have been created and/or updated
- The feature has been manually tested and verified, including Performance
- Automated tests, where possible, have been implemented
- Test automation passes

## Customer feedback
A key part of the planning process is feedback from MRTK customers and contributors. The format
in which this feedback includes, but is not limited to GitHub issue ratings (ex: thumbs up / down),
forum posts and face to face meetings (ex: hackathons).

The feedback from customers and contributors will be merged with any business needs and requirements
to finalize the iteration plans.

## Iteration timeline

Iterations last approximately four weeks (approximately a calendar month) and are divided into coding
and stabilization phases.

- Weeks 1 - 3: Coding
- Week 4: Stabilization and release

Note: Triage occurs throughout the iteration.

### Coding
During the first three weeks of an iteration, the MRTK team is focused on fixing bugs and implementing
approved features.

### Stabilization

At the start of stabilization, a new branch is created (off of the mrtk_development branch). Formalized
release testing (also called "build call") and targeted bug fixes will be performed in this branch.

During stabilization, the mrtk_development branch remains open for work that spans iterations.

### Release

At the end of stabilization, the branch will be merged into mrtk_development. If the iteration results
in an official release, the stabilization branch will also be merged into mrtk_release.

### Triage

During planning and as issues are reported, items will be assigned a priority. Priorities include,
in priority order:

- Urgency-Now
    - Highest priority
    - Issue added to the current iteration
    - Fix as soon as possible
- Urgency-Soon
    - Issue typically assigned to the next iteration
- 0: Backlog
    - Issue to be scheduled in a future planning session

