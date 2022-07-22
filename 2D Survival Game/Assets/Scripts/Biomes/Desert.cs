using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Desert : BiomeClass
{
    public Desert() {
        name = "Desert";
        biomeColor = new Color(195f / 255f, 160f / 255f, 0f / 255f);
        caveFreq = 0.05f;
        terrFreq = 0.3f;
        dirtLayerHeight = 10;
        generateCave = true;
        surfaceVal = 0.25f;
        heightMultiplier = 20;
        treeChance = 0;
        minTreeHeight = 0;
        maxTreeHeight = 0;
        tallGrassChance = 0;

        ores = new OreClass[4];
    }
}
