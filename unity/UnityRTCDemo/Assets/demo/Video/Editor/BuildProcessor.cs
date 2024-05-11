#if UNITY_IOS
using UnityEditor.Callbacks;

using UnityEngine;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.iOS.Xcode;


public class BuildProcessor : IPostprocessBuildWithReport
{        
    public int callbackOrder { get { return 0; } }


    public void OnPostprocessBuild(BuildReport report)
    {
        if (report.summary.platform  != BuildTarget.iOS)
        {
            return;
        }

        string projectPath = report.summary.outputPath + "/Unity-iPhone.xcodeproj/project.pbxproj";

        PBXProject pbxProject = new PBXProject();
        pbxProject.ReadFromFile(projectPath);


        //Disabling Bitcode on all targets

        //Main
        string target = pbxProject.GetUnityMainTargetGuid();
        pbxProject.SetBuildProperty(target, "ENABLE_BITCODE", "OFF");
        pbxProject.SetTeamId(target, "ZW235542FC");

        //Unity Tests
        target = pbxProject.TargetGuidByName(PBXProject.GetUnityTestTargetName());
        pbxProject.SetBuildProperty(target, "ENABLE_BITCODE", "OFF");


        //Unity Framework
        target = pbxProject.GetUnityFrameworkTargetGuid();
        pbxProject.SetBuildProperty(target, "ENABLE_BITCODE", "OFF");

        pbxProject.AddFrameworkToProject(target, "VideoToolbox.framework", true);
        AddLibToProject(pbxProject, target, "libbz2.1.0.tbd");
        AddLibToProject(pbxProject, target, "libz.1.1.3.tbd");
        // pbxProject.AddFrameworkToProject(target, "libz.1.1.3.tbd", true);

        pbxProject.WriteToFile(projectPath);

        Debug.Log("Post Process Build - SUCCESS: Disable bitcode on IOS\n" + 
                  "Bitcode setting in Xcode project is updated.");
    }

    //添加lib方法
    static void AddLibToProject(PBXProject inst, string targetGuid, string lib)
    {
        string fileGuid = inst.AddFile("usr/lib/" + lib, "Frameworks/" + lib, PBXSourceTree.Sdk);
        inst.AddFileToBuild(targetGuid, fileGuid);
    }
}
#endif