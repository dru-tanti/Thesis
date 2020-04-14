using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapGenerator), true)]
public class MapGeneratorEditor : Editor {
    // public override void OnInspectorGUI() {
    //     base.OnInspectorGUI();

    //     MapGenerator generator = target as MapGenerator;

    //     if (GUILayout.Button("Clear Map")) {
    //         generator.Clear();
    //     }

    //     if (GUILayout.Button("Generate Map")) {
    //         generator.GenerateMap();
    //     }
    // }
}