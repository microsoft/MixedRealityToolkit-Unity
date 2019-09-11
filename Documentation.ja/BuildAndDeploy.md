# MRTK のビルドとデプロイ
アプリケーションを HoloLens, Android, iOS などのデバイス上でスタンドアロンアプリとして実行するには、Unity プロジェクトでビルドとデプロイのステップが必要です。MRTK を使ったアプリケーションのビルドとデプロイ方法は、他の Unity アプリケーションのビルドとデプロイ方法と同様です。MRTK 特有の方法はありません。HoloLens 向けに Unity アプリケーションをビルド、デプロイする方法の詳細なステップは、以下をお読みください。他のプラットフォーム向けのビルドについては、 [Publishing Builds](https://docs.unity3d.com/Manual/PublishingBuilds.html) をご確認ください。

### HoloLens 1 または HoloLens 2 (UWP) への、MRTK のビルドとデプロイ
Hololens 1 または Hololens 2 (UWP) へビルドとデプロイする方法の説明は、[building your application to device](https://docs.microsoft.com/en-us/windows/mixed-reality/mrlearning-base-ch1#build-your-application-to-your-device) をご覧ください。

**ヒント:** WMR (Windows Mixed Reality), HoloLens 1, HoloLens 2 向けにビルドする際は、ビルド設定の 「Target SDK Version」 
と 「Minimum Platform Version」 を以下の画像のように設定することをおすすめします。

![](../Documentation/Images/getting_started/BuildWindow.png)

その他の設定は違っていることもあります。（例えば、Build Configuration, Architecture, Build Type やその他いくつかの設定は、
Visual Studio のソリューションでいつでも変更可能です。）

「Target SDK Version」 のドロップダウンに 「10.0.18362.0」 が含まれていることを確認してください。もし存在しない場合は、
[最新の Windows SDK](https://developer.microsoft.com/en-us/windows/downloads/windows-10-sdk) のインストールが必要です。
