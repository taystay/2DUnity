using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coal : OreClass {
    public Coal() {
        name = "coal";
        rarity = 0.1f;
        size = 0.7f;
        maxSpawnHeight = 5;
    }

    public Coal(float bRarity, float bSize) {
        name = "coal";
        rarity = bRarity;
        size = bSize;
        maxSpawnHeight = 5;
    }
}
