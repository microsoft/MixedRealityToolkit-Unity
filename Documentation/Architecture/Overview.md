# Architecture overview

The goal of this architectural overview is to give the reader an overall survey of what is
contained within the MRTK. After reading through the architecture documentation, the reader should understand:
documentation, the reader should understand:

-  the large pieces of MRTK and how they connect,
- the concepts that MRTK introduces that may not exist in vanilla Unity,
- how some of the larger systems (such as Input) work.

This section isn't intended to teach the audience how to do things, but rather how things
are structured and why.

## Many audiences, one toolkit

The MRTK doesn't have a single, uniform audience - it's been written to support use cases
ranging from first time hackathons, to people who are building complex, shared experiences
for enterprise. Some code and APIs may have been written that have optimized for one more
than the other (i.e. some parts of the MRTK seem more optimized for "one click configure"),
but it's important to note that some of those are more for historical and resourcing
reasons. As the MRTK evolves, the features that get built should be designed to scale to
support the range of use cases.

It's also important to note that the MRTK has requirements to gracefully scale across VR
and AR experiences - it should be easy to build applications that gracefully
fallback in behavior when deployed on a HoloLens 2 OR a HoloLens 1, and it should be
simple to build application that target OpenVR and WMR (and other platforms). While at
times the team may focus a particular iteration on a particular system or platform, the
long term goal is to build a wide range of support for wherever people are building
mixed reality experiences.

## High level breakdown

The MRTK is both a collection of tools for getting mixed reality (MR) experiences of
the ground quickly, and also an application framework with opinions on its own runtime,
how it should be extended, and how it should be configured. 

At a high level the MRTK can be broken down in the following ways:

![Architecture Overview Diagram](../../Documentation/Images/Architecture/MRTK_Architecture.png)

The MRTK also contains another set of grab-bag utilities that have little to no
dependencies on the rest of the MRTK (to list a few: build tools, solvers, audio
influencers, smoothing utilities, and line renderers)

The rest of the architecture documentation will build bottom up, starting from the framework
and runtime, building up to more interesting and complex systems like input. Please reference the
table of contents to the side to dive into the rest of the architectural overview.
