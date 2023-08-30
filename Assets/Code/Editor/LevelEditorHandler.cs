using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LevelEditorMb))]
public class LevelEditorHandler : Editor
{
    private int selected = 0;
    LevelEditorMb editor;

    public override void OnInspectorGUI()
    {
        editor = (LevelEditorMb)target;

        DrawDefaultInspector();

        string[] options = new string[]
        {
         "Grassland", "Desert", "Arctic", "Mars", "City", "Factory"
        };
        selected = EditorGUILayout.Popup("Biome:", selected, options);

        if (GUILayout.Button("Set"))
            editor.SetBiome(selected);

        GUILayout.Space(10);

        if (GUILayout.Button("ScreenShot"))
            editor.ScreenShot();
    }
}
