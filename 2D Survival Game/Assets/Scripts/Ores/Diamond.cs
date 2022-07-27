using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Diamond : OreClass {
    public Diamond() {
        name = "diamond";
        rarity = 0.08f;
        size = 0.95f;
        maxSpawnHeight = 25;
    }

    public Diamond(float bRarity, float bSize) {
        name = "diamond";
        rarity = bRarity;
        size = bSize;
        maxSpawnHeight = 25;
    }
}
