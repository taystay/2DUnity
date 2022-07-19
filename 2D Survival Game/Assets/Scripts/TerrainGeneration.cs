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

    [Header("Noise Settings")]
    public float caveFreq = 0.05f;
    public float terrFreq = 0.05f;
    public float seed;
    public Texture2D caveNoiseTexture;

    [Header("Ore Settings")]
    public float coalR;
    public float coalS;
    public float ironR, ironS;
    public float goldR, goldS;
    public float diamondR, diamondS;
    public Texture2D coalT;
    public Texture2D ironT;
    public Texture2D goldT;
    public Texture2D diamondT;

    private GameObject[] worldChunks;
    private List<Vector2> worldTiles = new List<Vector2>();

    private void OnValidate() {
        if (caveNoiseTexture == null) {
            caveNoiseTexture = new Texture2D(worldSize, worldSize);
            coalT = new Texture2D(worldSize, worldSize);
            ironT = new Texture2D(worldSize, worldSize);
            goldT = new Texture2D(worldSize, worldSize);
            diamondT = new Texture2D(worldSize, worldSize);
        }

        seed = Random.Range(-10000, 10000);
        //cave
        GenerateNoiseTexture(caveFreq, surfaceVal, caveNoiseTexture);
        //ores
        GenerateNoiseTexture(coalR, coalS, coalT);
        GenerateNoiseTexture(ironR, ironS, ironT);
        GenerateNoiseTexture(goldR, goldS, goldT);
        GenerateNoiseTexture(diamondR, diamondS, diamondT);
    }

    private void Start() {
        if (caveNoiseTexture == null) {
            caveNoiseTexture = new Texture2D(worldSize, worldSize);
            coalT = new Texture2D(worldSize, worldSize);
            ironT = new Texture2D(worldSize, worldSize);
            goldT = new Texture2D(worldSize, worldSize);
            diamondT = new Texture2D(worldSize, worldSize);
        }

        seed = Random.Range(-10000, 10000);
        //cave
        GenerateNoiseTexture(caveFreq, surfaceVal, caveNoiseTexture);
        //ores
        GenerateNoiseTexture(coalR, coalS, coalT);
        GenerateNoiseTexture(ironR, ironS, ironT);
        GenerateNoiseTexture(goldR, goldS, goldT);
        GenerateNoiseTexture(diamondR, diamondS, diamondT);

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
                Sprite tileSprite;
                if (y < height - dirtLayerHeight) {
                    if(coalT.GetPixel(x, y).r > 0.5f)
                        tileSprite = tileAtlas.coal.tileSprite;
                    else if (ironT.GetPixel(x, y).r > 0.5f)
                        tileSprite = tileAtlas.iron.tileSprite;
                    else if (goldT.GetPixel(x, y).r > 0.5f)
                        tileSprite = tileAtlas.gold.tileSprite;
                    else if (diamondT.GetPixel(x, y).r > 0.5f)
                        tileSprite = tileAtlas.diamond.tileSprite;
                    else 
                        tileSprite = tileAtlas.stone.tileSprite;
                } else if (y < height - 1) {
                    tileSprite = tileAtlas.dirt.tileSprite;
                } else {
                    tileSprite = tileAtlas.grass.tileSprite;
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
                PlaceTile(tileAtlas.logBase.tileSprite, x, y + h);
            } else {
                PlaceTile(tileAtlas.log.tileSprite, x, y + h);
            }
        }

        for (int j = 0; j < 3; j++) {
            if(j < 2) {
                PlaceTile(tileAtlas.leaf.tileSprite, x, y + treeH + j);
                PlaceTile(tileAtlas.leaf.tileSprite, x + 1, y + treeH + j);
                PlaceTile(tileAtlas.leaf.tileSprite, x - 1, y + treeH + j);
            } else {
                PlaceTile(tileAtlas.leaf.tileSprite, x, y + treeH + j);
            }
        }
    }

    public void PlaceTile(Sprite tileSprite, int x, int y) {
        GameObject newTile = new GameObject();

        float chunkCoord = Mathf.Round(x / chunkSize) * chunkSize;
        chunkCoord /= chunkSize;
        newTile.transform.parent = worldChunks[(int)chunkCoord].transform;

        newTile.AddComponent<SpriteRenderer>();
        newTile.GetComponent<SpriteRenderer>().sprite = tileSprite;
        newTile.name = tileSprite.name;
        newTile.transform.position = new Vector2(x + 0.5f, y + 0.5f);

        worldTiles.Add(newTile.transform.position - (Vector3.one * 0.5f));
    }
}
