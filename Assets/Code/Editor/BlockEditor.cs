using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Block))]
public class BlockEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Block block = (Block)target;

        GUILayout.Space(10);

        if (GUILayout.Button("Set Static"))
            block.SetStatic();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("<-"))
            block.RotateEditor(true);
        if (GUILayout.Button("->"))
            block.RotateEditor(false);
        GUILayout.EndHorizontal();

        GUILayout.Space(10);

        if (GUILayout.Button("^"))
            block.MoveEditor(new Vector3Int(0,0,1));
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("<"))
            block.MoveEditor(Vector3Int.left);
        if (GUILayout.Button(">"))
            block.MoveEditor(Vector3Int.right);
        GUILayout.EndHorizontal();
        if (GUILayout.Button("v"))
            block.MoveEditor(new Vector3Int(0,0,-1));
    }
}
