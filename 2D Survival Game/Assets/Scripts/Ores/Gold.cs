using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gold : OreClass {
    public Gold() {
        name = "gold";
        rarity = 0.07f;
        size = 0.85f;
        maxSpawnHeight = 25;
    }

    public Gold(float bRarity, float bSize) {
        name = "gold";
        rarity = bRarity;
        size = bSize;
        maxSpawnHeight = 25;
    }
}
