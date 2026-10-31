using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TerrainGeneration))]
public class GenerateTerrainCustomInspector : Editor
{
    TerrainGeneration _refScript;
    void OnEnable()
    {
        _refScript = (TerrainGeneration)target;
        
    }
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
    

        if (GUILayout.Button("Generate"))
        {
            _refScript.Generate();
        }
        if (GUILayout.Button("Generate With Random Seed"))
        {
            _refScript.Generate(Random.Range(-1000000,1000000));
        }

        DrawPropertiesExcluding(serializedObject,"m_Script","_noise","_riverNoise");

        DrawTexure(_refScript.Noise);

        if(_refScript.Type == TerrainGenerationType.Rivers || _refScript.Type == TerrainGenerationType.Ridges){ DrawTexure(_refScript.RiverNoise);}

        serializedObject.ApplyModifiedProperties();
    }

    void DrawTexure(Texture2D text)
    {
        if (text == null) return;
        EditorGUILayout.Space(10);

        float availableWidth = EditorGUIUtility.currentViewWidth - 20f;

        float aspectRatio = (float)text.height / text.width;
        aspectRatio /=3;
        float targetHeight = availableWidth * aspectRatio;

        Rect textureRect = GUILayoutUtility.GetRect(availableWidth, targetHeight);

        GUI.DrawTexture(textureRect, text, ScaleMode.ScaleToFit);
    }
}
