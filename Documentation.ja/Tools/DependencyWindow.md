
# Dependency Window （依存関係ウィンドウ）

Unity ではよく、どのアセットが利用されていて、何がそのアセットを参照しているのかを明らかにするのが難しいことがあります。「Find References in Scene」オプションは、現在のシーンのみに関心がある場合にはうまく動作しますが、Unity プロジェクト全体についてはどうでしょうか？ このような場合に、[Dependency Window（依存関係ウィンドウ）](https://github.com/Microsoft/MixedRealityToolkit-Unity/blob/mrtk_development/Assets/MixedRealityToolkit.Tools/DependencyWindow) が役に立ちます。

Dependency Window は、どのようにアセットが参照され、お互いにどう依存しているかを表示します。依存関係はプロジェクトの YAML ファイルにある GUID をパースすることで計算されます。（ただし、スクリプトからスクリプトへの依存は考慮されません）


### 使い方

Dependency Window を開くには、 *Mixed Reality Toolkit->Utilities->Dependency Window* を選択します。これによってウィンドウが開き、自動的にプロジェクトの依存関係グラフが作られ始めます。依存関係グラフができると、プロジェクトタブ内で、依存関係を見るためにアセットを選択できるようになります。

![](../../Documentation/Images/DependencyWindow/MRTK_Dependency_Window.png)

ウィンドウには、現在選択しているアセットが依存しているアセットのリストと、選択しているアセットに依存しているアセットの階層的なリストが表示されます。もし、現在選択しているアセットに依存しているものが何もなければ、そのアセットをプロジェクトから削除することを検討できます。（ただし、いくつかのアセットは、Shader.Find() のような API を通じてプログラムでロードされるので、依存関係が追跡されないかもしれません）

Dependency Window では、どのアセットからも参照されておらず、削除することを検討できるすべてのアセットのリストも表示できます。

![](../../Documentation/Images/DependencyWindow/MRTK_Dependency_Window_Unreferenced.png)

> [!NOTE]
> Dependency Window を使用中に、アセットの変更、追加、削除があった場合は、最新の結果になるよう依存関係グラフをリフレッシュすることをおすすめします。
