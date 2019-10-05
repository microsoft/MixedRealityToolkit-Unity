# Controllers (コントローラー)

コントローラーは、[**input providers (入力プロバイダー)**](InputProviders.md) によって自動的に作成および破棄されます。各コントローラー タイプは、入力値のデータ タイプ (Digital, Single Axis, Dual Axis, Six Dof, ...) を示す *axis type (軸タイプ)* によって定義される *physical inputs (物理入力)* が多数あり、また入力元を示す *input type (入力タイプ)* (Button Press, Trigger, Thumb Stick, Spatial Pointer, ...) があります。物理的な入力は、Mixed Reality Toolkit コンポーネントの *Input System Profile* の下の **Controller Input Mapping Profile** で *input actions (入力アクション)* にマッピングされます。

<img src="../../Documentation/Images/Input/ControllerInputMapping.png" style="max-width:100%;">