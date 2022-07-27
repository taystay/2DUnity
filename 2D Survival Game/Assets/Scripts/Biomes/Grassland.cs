using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grassland : BiomeClass
{
    public Grassland() {
        name = "Grassland";
        biomeColor = new Color(93f / 255f, 217f / 255f, 91f / 255f);
        caveFreq = 0.95f;
        terrFreq = 0.25f;
        dirtLayerHeight = 10;
        generateCave = true;
        surfaceVal = 0.25f;
        heightMultiplier = 20;
        treeChance = 15;
        minTreeHeight = 2;
        maxTreeHeight = 4;
        tallGrassChance = 3;

        ores = new OreClass[4];
        oresRarity[1] = 0.15f;
        oresSize[1] = 0.8f;
    }
}
