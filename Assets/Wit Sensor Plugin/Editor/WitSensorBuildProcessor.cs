using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using UnityEditor.iOS.Xcode.Extensions;

namespace WitSensor.Editor
{
    public static class WitSensorBuildProcessor
    {
        [PostProcessBuild]
        public static void OnPostProcessBuild(BuildTarget buildTarget, string buildPath)
        {
            if (buildTarget == BuildTarget.iOS)
            {
                // For info.plist
                string plistPath = buildPath + "/Info.plist";
                PlistDocument plist = new();
                plist.ReadFromFile(plistPath);
                PlistElementDict rootDict = plist.root;

                rootDict.SetString("NSBluetoothAlwaysUsageDescription", "For connecting bluetooth devices");

                File.WriteAllText(plistPath, plist.WriteToString());

                // For build settings
                string projectPath = PBXProject.GetPBXProjectPath(buildPath);
                PBXProject project = new();
                project.ReadFromString(File.ReadAllText(projectPath));

                string mainTargetGuid = project.GetUnityMainTargetGuid();
                string unityFrameworkTargetGuid = project.GetUnityFrameworkTargetGuid();

                // Make WitSDK.framework "Embed & Sign"
                string witSdkFilePath = "Wit Sensor Plugin/Runtime/iOS/WitSDK.framework";
                string witSdkFileGuid = project.AddFile(witSdkFilePath, "Frameworks/" + witSdkFilePath, PBXSourceTree.Sdk);
                PBXProjectExtensions.AddFileToEmbedFrameworks(project, unityFrameworkTargetGuid, witSdkFileGuid);

                project.AddBuildProperty(unityFrameworkTargetGuid, "LIBRARY_SEARCH_PATHS", "/usr/lib/swift");
                project.AddBuildProperty(unityFrameworkTargetGuid, "LD_RUNPATH_SEARCH_PATHS", "/usr/lib/swift");
                project.SetBuildProperty(mainTargetGuid, "ENABLE_BITCODE", "NO");
                project.SetBuildProperty(unityFrameworkTargetGuid, "ENABLE_BITCODE", "NO");

                project.WriteToFile(projectPath);
            }
        }
    }
}
