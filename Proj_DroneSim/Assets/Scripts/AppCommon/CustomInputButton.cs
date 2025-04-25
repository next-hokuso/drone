//https://note.com/hikohiro/n/nc41855007f4e
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class CustomInputButton : Button
{
    [SerializeField] public string displayInputText;
}

#if UNITY_EDITOR
[CustomEditor(typeof(CustomInputButton))]
public class CustomButtonEditor : UnityEditor.UI.ButtonEditor
{

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var component = (CustomInputButton)target;

        PropertyField(nameof(component.displayInputText), "DisplayInputText");

        serializedObject.ApplyModifiedProperties();
    }

    private void PropertyField(string property, string label)
    {
        EditorGUILayout.PropertyField(serializedObject.FindProperty(property), new GUIContent(label));
    }
}
#endif