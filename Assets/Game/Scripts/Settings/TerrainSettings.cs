using UnityEngine;

[System.Serializable]
public class TerrainConfig
{
    public float scale;
    public float heightMultiplier;

    public static TerrainConfig Basic => new TerrainConfig
    {
        scale = 1.6f,
        heightMultiplier = 1f,
    };

    public static RiversTerrainConfig RiversTerrain => RiversTerrainConfig.Rivers;
    public static RidgesTerrainConfig RidgesTerrain => RidgesTerrainConfig.Ridges;
    public static DeepForestTerrainConfig DeepForestTerrain => DeepForestTerrainConfig.DeepForest;
}

public class RiversTerrainConfig : TerrainConfig
{
    
    [Header("River Channel")]
    public float riverScale;
    public float riverThreshold;
    public float riverWidth;
    public float bankWidth;
    public float riverCarveDepth;
    public float halfStep;
    
    public static RiversTerrainConfig Rivers => new RiversTerrainConfig
    {
        scale = .2f,
        heightMultiplier = 1f,
        riverScale = 1f,
        riverThreshold = 0.4f,
        riverWidth = 0.04f,
        bankWidth = 0.2f,
        riverCarveDepth = 0.6f,
        halfStep = .3f
    };
}
public class RidgesTerrainConfig : TerrainConfig
{
    
    [Header("Ridges Channel")]
    public float ridgeScale;
    public float ridgeThreshold;
    public float ridgeWidth;
    public float bankWidth;
    public float ridgeCarveDepth;
    public float halfStep;
    
    public static RidgesTerrainConfig Ridges => new RidgesTerrainConfig
    {
        scale = .2f,
        heightMultiplier = 1f,
        ridgeScale = 1f,
        ridgeThreshold = 0.4f,
        ridgeWidth = 0.05f,
        bankWidth = 0.2f,
        ridgeCarveDepth = 0.6f,
        halfStep = .3f
    };
}
public class DeepForestTerrainConfig : TerrainConfig
{
    
    [Header("Forest Channel")]
    public float noiseFrequency;
    public float noiseCutoff;
    public int knightModulo;
    
    public static DeepForestTerrainConfig DeepForest => new DeepForestTerrainConfig
    {
        scale = .2f,
        heightMultiplier = 1f,

        noiseFrequency = .4f,
        noiseCutoff = .5f,
        knightModulo = 5
    };
}