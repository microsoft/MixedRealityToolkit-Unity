# アーキテクチャの全体像

このセクションでは、MRTK のアーキテクチャの全体像について説明します。なお、アーキテクチャのドキュメント全体を通して以下の内容を理解することを目的としています。

- MRTK の各構成要素と、それらの接続方法
- Untiy にはない、MRTK が取り入れている設計思想
- 入力システムといった大掛かりな仕組みがどのように動作するのか

このセクションでは、MRTK の動作方法を説明することは意図しておらず、MRTK がどのような構造で作られているのか、またそのような構造となった理由を説明すること目的としています。

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

## MRTK のコンポーネントの大まかな分類

MRTK は、Mixed Reality（MR）体験を迅速に実現するためのツールコレクションであり、独自のランタイムや拡張方法、設定方法に関する機能を含むアプリケーションフレームワークでもあります。

MRTK は以下の図のとおりにおおまかに分類することができます。

![Architecture Overview Diagram](../../Documentation/Images/Architecture/MRTK_Architecture.png)

MRTK には、他の MRTK のコンポーネントにはほとんど依存しないユーティリティも含まれています（例えばビルドツール、ソルバー、オーディオインフルエンサー、スムージングユーティリティ、ラインレンダラーなど）。

残りのアーキテクチャのドキュメントでは、フレームワークとランタイムの説明からはじめて、入力システムのようなより複雑な仕組みを段階的に説明します。 目次を参照し残りのアーキテクチャの概要を深掘りしてみてください。

