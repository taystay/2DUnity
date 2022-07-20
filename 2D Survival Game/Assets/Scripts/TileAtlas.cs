using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TileAtlas", menuName = "Tile Atlas")]
public class TileAtlas : ScriptableObject
{
    [Header("Environment")]
    public TileClass dirt;
    public TileClass grass;
    public TileClass tallGrass;
    public TileClass stone;
    public TileClass log;
    public TileClass logBase;
    public TileClass leaf;
    public TileClass snow;
    public TileClass sand;

    [Header("Ores")]
    public TileClass coal;
    public TileClass iron;
    public TileClass gold;
    public TileClass diamond;
}
