using System;
using UnityEngine;

public class TerrainConfig
{
    public float scale;
    public float heightMultiplier;

    public static TerrainConfig Basic => new TerrainConfig
    {
        scale = 3f,
        heightMultiplier = 1f,
    };

    public static RiversTerrainConfig RiversTerrain => RiversTerrainConfig.Rivers;
    //ridges 
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
        riverWidth = 0.05f,
        bankWidth = 0.12f,
        riverCarveDepth = 0.6f,
        halfStep = .3f
    };
}