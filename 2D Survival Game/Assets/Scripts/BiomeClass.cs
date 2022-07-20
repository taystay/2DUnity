using System.Collections;
using System.Collections.Generic;
using UnityEngine; 

[System.Serializable]
public class BiomeClass
{
    public string name;
    public Color biomeColor;

    [Header("Noise Settings")]
    public float caveFreq = 0.05f;
    public float terrFreq = 0.05f;
    public Texture2D caveNoiseTexture;

    [Header("Generation Settings")]
    public int dirtLayerHeight = 5;
    public bool generateCave = true;
    public float surfaceVal = 0.25f;
    public float heightMultiplier = 4f;

    [Header("Tree Generation")]
    public int treeChance = 10;
    public int minTreeHeight = 3;
    public int maxTreeHeight = 7;

    [Header("Addons")]
    public int tallGrassChance = 10;

    [Header("Ore Settings")]
    public OreClass[] ores;
}
