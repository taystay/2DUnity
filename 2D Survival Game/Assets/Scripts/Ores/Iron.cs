using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Iron : OreClass {
    public Iron() {
        name = "iron";
        rarity = 0.08f;
        size = 0.8f;
        maxSpawnHeight = 10;
    }

    public Iron(float bRarity, float bSize) {
        name = "iron";
        rarity = bRarity;
        size = bSize;
        maxSpawnHeight = 10;
    }
}
