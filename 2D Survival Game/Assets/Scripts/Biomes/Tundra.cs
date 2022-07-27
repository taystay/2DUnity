using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tundra : BiomeClass
{
    public Tundra() {
        name = "Tundra";
        biomeColor = new Color(255f / 255f, 255f / 255f, 255f / 255f);
        caveFreq = 0.08f;
        terrFreq = 0.3f;
        dirtLayerHeight = 10;
        generateCave = true;
        surfaceVal = 0.4f;
        heightMultiplier = 20;
        treeChance = 0;
        minTreeHeight = 0;
        maxTreeHeight = 0;
        tallGrassChance = 0;

        ores = new OreClass[4];
        oresRarity[3] = 0.15f;
        oresSize[3] = 0.85f;
    }
}
