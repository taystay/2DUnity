using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGeneration : MonoBehaviour
{
    [Header("Tile Sprites")]
    public TileAtlas tileAtlas;

    [Header("Generation Settings")]
    public int chunkSize = 20;
    public int worldSize = 100;
    public int dirtLayerHeight = 5;
    public bool generateCave = true;
    public int heightAddition = 25;
    public float surfaceVal = 0.25f;
    public float heightMultiplier = 4f;

    [Header("Tree Generation")]
    public int treeChance = 10;
    public int minTreeHeight = 3;
    public int maxTreeHeight = 7;

    [Header("Addons")]
    public int tallGrassChance = 10;

    [Header("Noise Settings")]
    public float caveFreq = 0.05f;
    public float terrFreq = 0.05f;
    public float seed;
    public Texture2D caveNoiseTexture;

    [Header("Ore Settings")]
    public OreClass[] ores;

    private GameObject[] worldChunks;
    private List<Vector2> worldTiles = new List<Vector2>();

    private void OnValidate() {
        caveNoiseTexture = new Texture2D(worldSize, worldSize);
        ores[0].spreadTexture = new Texture2D(worldSize, worldSize);
        ores[1].spreadTexture = new Texture2D(worldSize, worldSize);
        ores[2].spreadTexture = new Texture2D(worldSize, worldSize);
        ores[3].spreadTexture = new Texture2D(worldSize, worldSize);


        seed = Random.Range(-10000, 10000);
        //cave
        GenerateNoiseTexture(caveFreq, surfaceVal, caveNoiseTexture);
        //ores
        GenerateNoiseTexture(ores[0].rarity, ores[0].size, ores[0].spreadTexture);
        GenerateNoiseTexture(ores[1].rarity, ores[1].size, ores[1].spreadTexture);
        GenerateNoiseTexture(ores[2].rarity, ores[2].size, ores[2].spreadTexture);
        GenerateNoiseTexture(ores[3].rarity, ores[3].size, ores[3].spreadTexture);
    }

    private void Start() {
        caveNoiseTexture = new Texture2D(worldSize, worldSize);
        ores[0].spreadTexture = new Texture2D(worldSize, worldSize);
        ores[1].spreadTexture = new Texture2D(worldSize, worldSize);
        ores[2].spreadTexture = new Texture2D(worldSize, worldSize);
        ores[3].spreadTexture = new Texture2D(worldSize, worldSize);

        seed = Random.Range(-10000, 10000);
        //cave
        GenerateNoiseTexture(caveFreq, surfaceVal, caveNoiseTexture);
        //ores
        GenerateNoiseTexture(ores[0].rarity, ores[0].size, ores[0].spreadTexture);
        GenerateNoiseTexture(ores[1].rarity, ores[1].size, ores[1].spreadTexture);
        GenerateNoiseTexture(ores[2].rarity, ores[2].size, ores[2].spreadTexture);
        GenerateNoiseTexture(ores[3].rarity, ores[3].size, ores[3].spreadTexture);

        CreateChunks();
        GenerateTerrain();
    }

    public void CreateChunks() {
        int numChunks = worldSize / chunkSize;
        worldChunks = new GameObject[numChunks];
        for (int i = 0; i < numChunks; i++) {
            GameObject newChunk = new GameObject();
            newChunk.name = "Chunk " + i.ToString();
            newChunk.transform.parent = transform;
            worldChunks[i] = newChunk;
        }
    }

    public void GenerateTerrain() {
        for (int x = 0; x < worldSize; x++) {
            float height = Mathf.PerlinNoise((x + seed) * terrFreq, seed * terrFreq) * heightMultiplier + heightAddition;
            for (int y = 0; y < height; y++) {
                Sprite[] tileSprite;
                if (y < height - dirtLayerHeight) {
                    tileSprite = tileAtlas.stone.tileSprites;

                    if (ores[0].spreadTexture.GetPixel(x, y).r > 0.5f && height - y > ores[0].maxSpawnHeight)
                        tileSprite = tileAtlas.coal.tileSprites;
                    if (ores[1].spreadTexture.GetPixel(x, y).r > 0.5f && height - y > ores[1].maxSpawnHeight)
                        tileSprite = tileAtlas.iron.tileSprites;
                    if (ores[2].spreadTexture.GetPixel(x, y).r > 0.5f && height - y > ores[2].maxSpawnHeight)
                        tileSprite = tileAtlas.gold.tileSprites;
                    if (ores[3].spreadTexture.GetPixel(x, y).r > 0.5f && height - y > ores[3].maxSpawnHeight)
                        tileSprite = tileAtlas.diamond.tileSprites;
                } else if (y < height - 1) {
                    tileSprite = tileAtlas.dirt.tileSprites;
                } else {
                    tileSprite = tileAtlas.grass.tileSprites;
                }

                if(generateCave) {
                    if (caveNoiseTexture.GetPixel(x, y).r > 0.5f) {
                        PlaceTile(tileSprite, x, y);
                    }
                } else {
                    PlaceTile(tileSprite, x, y);
                }

                if (y >= height - 1) {
                    int t = Random.Range(0, treeChance);
                    if (t == 1) {
                        if(worldTiles.Contains(new Vector2(x, y))) 
                            GenerateTree(x, y + 1);
                    } else {
                        if(Random.Range(0, tallGrassChance) > 7)
                            if (worldTiles.Contains(new Vector2(x, y)))
                                PlaceTile(tileAtlas.tallGrass.tileSprites, x, y + 1);
                    }
                }
            }
        }
    }

    public void GenerateNoiseTexture(float frequency, float limit, Texture2D noiseTexture) {
        for (int x = 0; x < noiseTexture.width; x++) {
            for (int y = 0; y < noiseTexture.height; y++) {
                float v = Mathf.PerlinNoise((x + seed) * frequency, (y + seed) * frequency);
                if (v > limit)
                    noiseTexture.SetPixel(x, y, Color.white);
                else
                    noiseTexture.SetPixel(x, y, Color.black);
            }
        }

        noiseTexture.Apply();
    }

    public void GenerateTree(int x, int y) {
        int treeH = Random.Range(minTreeHeight, maxTreeHeight);
        for (int h = 0; h < treeH; h++) {
            if(h == 0) {
                PlaceTile(tileAtlas.logBase.tileSprites, x, y + h);
            } else {
                PlaceTile(tileAtlas.log.tileSprites, x, y + h);
            }
        }

        for (int j = 0; j < 3; j++) {
            if(j < 2) {
                PlaceTile(tileAtlas.leaf.tileSprites, x, y + treeH + j);
                PlaceTile(tileAtlas.leaf.tileSprites, x + 1, y + treeH + j);
                PlaceTile(tileAtlas.leaf.tileSprites, x - 1, y + treeH + j);
            } else {
                PlaceTile(tileAtlas.leaf.tileSprites, x, y + treeH + j);
            }
        }
    }

    public void PlaceTile(Sprite[] tileSprite, int x, int y) {
        GameObject newTile = new GameObject();

        float chunkCoord = Mathf.Round(x / chunkSize) * chunkSize;
        chunkCoord /= chunkSize;
        newTile.transform.parent = worldChunks[(int)chunkCoord].transform;

        newTile.AddComponent<SpriteRenderer>();

        int spriteIndex = Random.Range(0, tileSprite.Length);
        newTile.GetComponent<SpriteRenderer>().sprite = tileSprite[spriteIndex];

        newTile.name = tileSprite[0].name;
        newTile.transform.position = new Vector2(x + 0.5f, y + 0.5f);

        worldTiles.Add(newTile.transform.position - (Vector3.one * 0.5f));
    }
}
