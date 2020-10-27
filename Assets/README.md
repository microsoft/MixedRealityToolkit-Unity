# mrtk_development_holospaces

mrtk_development_holospaces is a custom branch of mrtk_development

note: mrtk_development is a fork of https://github.com/microsoft/MixedRealityToolkit-Unity.git

mrtk_development is always upto date with the upstream branch.

Try to avoid modifying this branch(mrtk_development_holospaces) as much as possible. If its a bug or required feature, please do it
locally and then create pull request to upstream branch.

Also recommend to track locall changes here on this document.

#fix 27/10/20 
Line renderer points are not exposed
namespace Microsoft.MixedReality.Toolkit.Utilities
{
    public class MixedRealityLineRenderer : BaseMixedRealityLineRenderer
    {
	+/// <summary>
        +/// Gets the LineRenderer points
        +/// </summary>
        +public Vector3[] Positions
        +{
        +    get => positions;
        +}
    }
}
	