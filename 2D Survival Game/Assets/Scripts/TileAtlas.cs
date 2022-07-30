using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TileAtlas", menuName = "Tile Atlas")]
public class TileAtlas : ScriptableObject
{
    [Header("Overworld")]
    public TileClass tallGrass;
    public TileClass log;
    public TileClass logBase;
    public TileClass leaf;

    [Header("UnderWorld")]
    public TileClass stone;
    public TileClass snow;
    public TileClass sand;
    public TileClass dirt;
    public TileClass grass;

    [Header("Ores")]
    public TileClass coal;
    public TileClass iron;
    public TileClass gold;
    public TileClass diamond;
}
