using System.Drawing.Printing;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TerrainGeneration))]
public class GenerateTerrainButton : Editor
{
    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Generate"))
        {
            var c = (TerrainGeneration)target;
            c.Generate();
        }
        if (GUILayout.Button("Generate With Random Seed"))
        {
            var c = (TerrainGeneration)target;
            c.Generate(Random.Range(-100000,100000));
        }

        DrawDefaultInspector();
    }
}
