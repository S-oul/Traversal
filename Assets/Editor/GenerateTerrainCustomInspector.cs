using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TerrainGeneration))]
public class GenerateTerrainCustomInspector : Editor
{
    TerrainGeneration _refScript;
    SerializedProperty ConfigProp;
    void OnEnable()
    {
        _refScript = (TerrainGeneration)target;
        ConfigProp = serializedObject.FindProperty("_currentSettings");
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

        DrawPropertiesExcluding(serializedObject,"m_Script","_noise","_secondNoise","_currentSettings","_thirdNoise");
        DrawTexure(_refScript.Noise);

        float min = float.MaxValue;
        foreach(Color c in _refScript.Noise.GetPixels())
        {
            min =Mathf.Min(min,c.r);
        }

        switch(_refScript.Type)
        {
            //to copy cat
            case TerrainGenerationType.Basic:
                
            break;
            case TerrainGenerationType.Rivers:
                DrawTexure(_refScript.SecondNoise);
            break;
            case TerrainGenerationType.Ridges:
                DrawTexure(_refScript.SecondNoise);            
            break;
            case TerrainGenerationType.DeepForest:
                DrawTexure(_refScript.SecondNoise);  
                //DrawTexure(_refScript.ThirdNoise);          
            break;
        }
        using (new EditorGUI.DisabledScope(true))
        {
            //EditorGUILayout.PropertyField(ConfigProp.GetType())
            EditorGUILayout.PropertyField(ConfigProp, includeChildren: true);
        }
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
