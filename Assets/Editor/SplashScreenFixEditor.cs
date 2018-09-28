using NUnit.Framework;
using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using UnityEditor;
using UnityEditor.Callbacks;

public class SplashScreenFixEditor : Editor
{
    [PostProcessBuild(2)]
    public static void SplashScreenFix(BuildTarget target, string pathToBuild)
    {
        if (target != BuildTarget.WSAPlayer ||
            PlayerSettings.GetScriptingBackend(BuildTargetGroup.WSA) != ScriptingImplementation.IL2CPP)
        {
            return;
        }

        var vcxProj = Directory.GetFiles(pathToBuild, "*.vcxproj", SearchOption.AllDirectories)
            .Where(project => project.IndexOf("il2cppoutputproject", StringComparison.CurrentCultureIgnoreCase) < 0);
        var projects = vcxProj as string[] ?? vcxProj.ToArray();

        Assert.IsTrue(projects != null);
        Assert.IsTrue(projects.Length == 1);

        var vcxProject = projects[0];
        var projXml = XDocument.Load(vcxProject);
        var pngElements = projXml.Descendants().Where(x =>
        {
            var val = x.FirstAttribute?.Value.Contains(".png");
            return val.HasValue && val.Value;
        });

        foreach (var element in pngElements)
        {
            foreach (var child in element.Elements())
            {
                if (child.Name.ToString().Contains("ExcludeFromResourceIndex"))
                {
                    child.Remove();
                }
            }
        }

        projXml.Save(vcxProject);
    }
}