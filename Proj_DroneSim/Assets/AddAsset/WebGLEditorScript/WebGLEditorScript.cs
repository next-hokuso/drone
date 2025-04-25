using UnityEditor;
public class WebGLEditorScript
{
#if UNITY_EDITOR
    [MenuItem("WebGL/Enable Embedded Resources")]
    public static void EnableErrorMessageTesting()
    {
        // PlayerSettings.SetPropertyBool("useEmbeddedResources", false, BuildTargetGroup.WebGL);
    }
#endif
}
