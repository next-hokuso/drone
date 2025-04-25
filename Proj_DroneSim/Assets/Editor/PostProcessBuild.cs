#if UNITY_IOS
using UnityEngine;
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;

public static class PostProcessBuild
{
    [PostProcessBuild(99)]
    private static void OnPostProcessBuild(BuildTarget buildTarget, string pathToBuiltProject)
    {
        if ( buildTarget != BuildTarget.iOS ) return;

        // Info.plist設定
        var path = pathToBuiltProject + "/Info.plist";
        var plistDocument = new PlistDocument();
        plistDocument.ReadFromFile(path);

        // SKAdNetworkItems設定
        PlistElementArray items;
        bool isAdNetworkConfigured = false;
        if (plistDocument.root.values.ContainsKey("SKAdNetworkItems"))
        {
            items = plistDocument.root["SKAdNetworkItems"].AsArray();
            if (items.values.Count > 0)
            {
                isAdNetworkConfigured = true;
            }
        }
        else
        {
            items = plistDocument.root.CreateArray("SKAdNetworkItems");
        }
        if (!isAdNetworkConfigured)
        {
            foreach (string id in AdNetworkIds)
            {
                PlistElementDict item = items.AddDict();
                item.SetString("SKAdNetworkIdentifier", id);
            }
        }

        plistDocument.root.SetString(key: "FacebookClientToken", val: "933b429f778129bf563c15bc45bfc367");
        plistDocument.root.SetString(key: "NSUserTrackingUsageDescription", val: "Your data will be used to deliver personalized ads and analyze usage.");
        plistDocument.WriteToFile(path);

        // プロジェクト設定
        // Bitcode無効化
        string projectPath = PBXProject.GetPBXProjectPath(pathToBuiltProject);
        PBXProject pbxProject = new PBXProject();
        pbxProject.ReadFromString(File.ReadAllText(projectPath));
        string targetGuid = pbxProject.GetUnityMainTargetGuid();
        pbxProject.SetBuildProperty(targetGuid, "ENABLE_BITCODE", "NO");
        File.WriteAllText(projectPath, pbxProject.WriteToString());

        // Facebook SDK 対応
        using (StreamWriter sw = File.AppendText( pathToBuiltProject + "/Podfile" ))
        {
            sw.WriteLine("post_install do |installer|");
            sw.WriteLine("  installer.pods_project.targets.each do |target|");
            sw.WriteLine("    if target.respond_to?(:product_type) and target.product_type == \"com.apple.product-type.bundle\"");
            sw.WriteLine("      target.build_configurations.each do |config|");
            sw.WriteLine("        config.build_settings['CODE_SIGNING_ALLOWED'] = 'NO'");
            sw.WriteLine("      end\n    end\n  end\nend");
        }
    }

    // https://developers.is.com/ironsource-mobile/ios/managing-skadnetwork-ids/
    private static string[] AdNetworkIds = new string[] {
        "97r2b46745.skadnetwork",
        "3rd42ekr43.skadnetwork",
        "22mmun2rn5.skadnetwork",
        "a8cz6cu7e5.skadnetwork",
        "252b5q8x7y.skadnetwork",
        "9nlqeag3gk.skadnetwork",
        "f7s53z58qe.skadnetwork",
        "2u9pt9hc89.skadnetwork",
        "prcb7njmu6.skadnetwork",
        "4fzdc2evr5.skadnetwork",
        "s39g8k73mm.skadnetwork",
        "t38b2kh725.skadnetwork",
        "5l3tpt7t6e.skadnetwork",
        "6yxyv74ff7.skadnetwork",
        "578prtvx9j.skadnetwork",
        "32z4fx6l9h.skadnetwork",
        "8s468mfl3y.skadnetwork",
        "5tjdwbrq8w.skadnetwork",
        "su67r6k2v3.skadnetwork",
        "kbmxgpxpgc.skadnetwork",
        "7fmhfwg9en.skadnetwork",
        "mqn7fxpca7.skadnetwork",
        "44jx6755aq.skadnetwork",
        "hs6bdukanm.skadnetwork",
        "zq492l623r.skadnetwork",
        "av6w8kgt66.skadnetwork",
        "k674qkevps.skadnetwork",
        "yclnxrl5pm.skadnetwork",
        "424m5254lk.skadnetwork",
        "v72qych5uu.skadnetwork",
        "f38h382jlk.skadnetwork",
        "cstr6suwn9.skadnetwork",
        "9t245vhmpl.skadnetwork",
        "7ug5zh24hu.skadnetwork",
        "kbd757ywx3.skadnetwork",
        "4468km3ulz.skadnetwork",
        "e5fvkxwrpn.skadnetwork",
        "3qy4746246.skadnetwork",
        "wzmmz9fp6w.skadnetwork",
        "m8dbw4sv7c.skadnetwork",
        "ppxm28t8ap.skadnetwork",
        "qqp299437r.skadnetwork",
        "2fnua5tdw4.skadnetwork",
        "3qcr597p9d.skadnetwork",
        "3sh42y64q3.skadnetwork",
        "5a6flpkh64.skadnetwork",
        "9rd848q2bz.skadnetwork",
        "c6k4g5qg8m.skadnetwork",
        "klf5c3l5u5.skadnetwork",
        "n6fk4nfna4.skadnetwork",
        "p78axxw29g.skadnetwork",
        "uw77j35x4d.skadnetwork",
        "ydx93a7ass.skadnetwork",
        "v9wttpbfk9.skadnetwork",
        "n38lu8286q.skadnetwork",
        "dbu4b84rxf.skadnetwork",
        "lr83yxwka7.skadnetwork",
        "238da6jt44.skadnetwork",
        "v79kvwwj4g.skadnetwork",
        "w9q455wk68.skadnetwork",
        "5lm9lj6jb7.skadnetwork",
        "mp6xlyr22a.skadnetwork",
        "mlmmfzh3r3.skadnetwork",
        "glqzh8vgby.skadnetwork",
        "4pfyvq9l8r.skadnetwork",
        "4dzt52r2t5.skadnetwork",
        "x44k69ngh6.skadnetwork",
        "294l99pt4k.skadnetwork",
        "wg4vff78zm.skadnetwork",
        "zmvfpc5aq8.skadnetwork",
        "tl55sbb4fm.skadnetwork",
        "4w7y6s5ca2.skadnetwork",
        "488r3q3dtq.skadnetwork",
        "f73kdq92p3.skadnetwork",
        "a2p9lx4jpn.skadnetwork",
    };
}

#endif