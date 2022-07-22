using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Forest : BiomeClass
{
    public Forest() {
        name = "Forest";
        biomeColor = new Color(0f / 255f, 84f / 255f, 9f / 255f);
        caveFreq = 0.90f;
        terrFreq = 0.27f;
        dirtLayerHeight = 15;
        generateCave = true;
        surfaceVal = 0.25f;
        heightMultiplier = 20;
        treeChance = 6;
        minTreeHeight = 3;
        maxTreeHeight = 7;
        tallGrassChance = 8;

        ores = new OreClass[4];
    }
}
