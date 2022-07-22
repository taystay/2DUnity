using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGeneration : MonoBehaviour
{
    [Header("Tile Atlas")]
    public TileAtlas tileAtlas;
    public float seed;

    public BiomeClass[] biomes;

    [Header("Biomes")]
    public float biomeFreq;
    public Gradient biomeGradient;
    public GradientColorKey[] biomeColorKey;
    public GradientAlphaKey[] biomeAlphaKey;
    public Texture2D biomeMap;

    [Header("Generation Settings")]
    public int chunkSize = 20;
    public int worldSize = 100;
    public int heightAddition = 25;

    private GameObject[] worldChunks;
    private List<Vector2> worldTiles = new List<Vector2>();
    public int[,] biomeList;

    private void OnValidate() {
        SetGradient();
        DrawTextures();
    }

    private void Start() {
        GetBiomes();
        SetGradient();
        DrawTextures();
        GetTileBiome();
        CreateChunks();
        GenerateTerrain();
    }

    public void GetBiomes() {
        /*biomes[0] = new Grassland();
        biomes[1] = new Forest();
        biomes[2] = new Tundra();
        biomes[3] = new Desert();*/
        for (int i = 0; i < biomes.Length; i++) {
            biomes[i].ores[0] = new Coal();
            biomes[i].ores[1] = new Iron();
            biomes[i].ores[2] = new Gold();
            biomes[i].ores[3] = new Diamond();
        }
    }

    public void SetGradient() {
        biomeGradient = new Gradient();
        biomeGradient.mode = GradientMode.Fixed;

        //Gradient color keys
        biomeColorKey = new GradientColorKey[4];
        biomeColorKey[0].color = biomes[0].biomeColor;
        biomeColorKey[1].color = biomes[1].biomeColor;
        biomeColorKey[2].color = biomes[2].biomeColor;
        biomeColorKey[3].color = biomes[3].biomeColor;

        //Gradient color time keys
        biomeColorKey[0].time = 0.25f;
        biomeColorKey[1].time = 0.50f;
        biomeColorKey[2].time = 0.75f;
        biomeColorKey[3].time = 1.0f;

        //Gradient alpha keys
        biomeAlphaKey = new GradientAlphaKey[2];
        biomeAlphaKey[0].alpha = 1.0f;
        biomeAlphaKey[1].alpha = 1.0f;

        //Gradient time keys
        biomeAlphaKey[0].time = 0.0f;
        biomeAlphaKey[1].time = 1.0f;

        biomeGradient.SetKeys(biomeColorKey, biomeAlphaKey);
    }

    public int GetCurrTileBiome(int x, int y) {
        int biome = -1;

        //Debug.Log("biome map: " + biomeMap.GetPixel(x, y).r + " " + biomeMap.GetPixel(x, y).g + " " + biomeMap.GetPixel(x, y).b);

        if ((Mathf.Abs(biomeMap.GetPixel(x, y).r - biomes[0].biomeColor.r) < 0.1f) && (Mathf.Abs(biomeMap.GetPixel(x, y).g - biomes[0].biomeColor.g) < 0.1f) && (Mathf.Abs(biomeMap.GetPixel(x, y).b - biomes[0].biomeColor.b) < 0.1f)) {
            biome = 0;
        } else if ((Mathf.Abs(biomeMap.GetPixel(x, y).r - biomes[1].biomeColor.r) < 0.1f) && (Mathf.Abs(biomeMap.GetPixel(x, y).g - biomes[1].biomeColor.g) < 0.1f) && (Mathf.Abs(biomeMap.GetPixel(x, y).b - biomes[1].biomeColor.b) < 0.1f)) {
            biome = 1;
        } else if ((Mathf.Abs(biomeMap.GetPixel(x, y).r - biomes[2].biomeColor.r) < 0.1f) && (Mathf.Abs(biomeMap.GetPixel(x, y).g - biomes[2].biomeColor.g) < 0.1f) && (Mathf.Abs(biomeMap.GetPixel(x, y).b - biomes[2].biomeColor.b) < 0.1f)) {
            biome = 2;
        } else if ((Mathf.Abs(biomeMap.GetPixel(x, y).r - biomes[3].biomeColor.r) < 0.1f) && (Mathf.Abs(biomeMap.GetPixel(x, y).g - biomes[3].biomeColor.g) < 0.1f) && (Mathf.Abs(biomeMap.GetPixel(x, y).b - biomes[3].biomeColor.b) < 0.1f)) {
            biome = 3;
        }

        return biome;
    }

    public void GetTileBiome() {
        biomeList = new int[worldSize, worldSize];
        /*Debug.Log("biome grass: " + biomes[0].biomeColor.r + " " + biomes[0].biomeColor.g + " " + biomes[0].biomeColor.b);
        Debug.Log("biome forest: " + biomes[1].biomeColor.r + " " + biomes[1].biomeColor.g + " " + biomes[1].biomeColor.b);
        Debug.Log("biome tundra: " + biomes[2].biomeColor.g + " " + biomes[2].biomeColor.g + " " + biomes[2].biomeColor.b);
        Debug.Log("biome desert: " + biomes[3].biomeColor.r + " " + biomes[3].biomeColor.g + " " + biomes[3].biomeColor.b);*/
        for (int x = 0; x < worldSize; x++) {
            for (int y = 0; y < worldSize; y++) {
                biomeList[x, y] = GetCurrTileBiome(x, y);
                //Debug.Log(x + " " + y + " " + biomeList[x, y]);
            }
        }
    }

    public void DrawTextures() {
        biomeMap = new Texture2D(worldSize, worldSize);
        DrawBiomeTexture();

        for (int i = 0; i < biomes.Length; i++) {
            biomes[i].caveNoiseTexture = new Texture2D(worldSize, worldSize);
            for (int j = 0; j < biomes[i].ores.Length; j++) {
                biomes[i].ores[j].spreadTexture = new Texture2D(worldSize, worldSize);
            }

            seed = Random.Range(-10000, 10000);
            //cave
            GenerateNoiseTexture(biomes[i].caveFreq, biomes[i].surfaceVal, biomes[i].caveNoiseTexture);
            //ores
            for (int j = 0; j < biomes[i].ores.Length; j++) {
                GenerateNoiseTexture(biomes[i].ores[j].rarity, biomes[i].ores[j].size, biomes[i].ores[j].spreadTexture);
            }
        }
    }

    public void DrawBiomeTexture() {
        for (int x = 0; x < biomeMap.width; x++) {
            for (int y = 0; y < biomeMap.height; y++) {
                //X and Y
                //float v = Mathf.PerlinNoise((x + seed) * biomeFreq, (y + seed) * biomeFreq);
                //Just X
                float v = Mathf.PerlinNoise((x + seed) * biomeFreq, (x + seed) * biomeFreq);
                Color col = biomeGradient.Evaluate(v);
                biomeMap.SetPixel(x, y, col);
            }
        }

        biomeMap.Apply();
    }

    public void CreateChunks() {
        int numChunks = worldSize / chunkSize;
        worldChunks = new GameObject[numChunks];
        for (int i = 0; i < numChunks; i++) {
            GameObject newChunk = new GameObject {
                name = "Chunk " + i.ToString()
            };
            newChunk.transform.parent = transform;
            worldChunks[i] = newChunk;
        }
    }
    
    public void GenerateTerrain() {
        Sprite[] tileSprite;
        for (int x = 0; x < worldSize; x++) {
            int curBiome = biomeList[x, 0];
            float height = Mathf.PerlinNoise((x + seed) * biomes[curBiome].terrFreq, seed * biomes[curBiome].terrFreq) * biomes[curBiome].heightMultiplier + heightAddition;
            for (int y = 0; y < height; y++) {
                curBiome = biomeList[x, y];
                if (y < height - biomes[curBiome].dirtLayerHeight) {
                    if(curBiome == -1) {
                        tileSprite = tileAtlas.stone.tileSprites;
                    } else {
                        tileSprite = biomes[curBiome].tileAtlas.stone.tileSprites;
                    }

                    if (biomes[curBiome].ores[0].spreadTexture.GetPixel(x, y).r > 0.5f && height - y > biomes[curBiome].ores[0].maxSpawnHeight)
                        tileSprite = tileAtlas.coal.tileSprites;
                    if (biomes[curBiome].ores[1].spreadTexture.GetPixel(x, y).r > 0.5f && height - y > biomes[curBiome].ores[1].maxSpawnHeight)
                        tileSprite = tileAtlas.iron.tileSprites;
                    if (biomes[curBiome].ores[2].spreadTexture.GetPixel(x, y).r > 0.5f && height - y > biomes[curBiome].ores[2].maxSpawnHeight)
                        tileSprite = tileAtlas.gold.tileSprites;
                    if (biomes[curBiome].ores[3].spreadTexture.GetPixel(x, y).r > 0.5f && height - y > biomes[curBiome].ores[3].maxSpawnHeight)
                        tileSprite = tileAtlas.diamond.tileSprites;
                } else if (y < height - 1) {
                    if (curBiome == -1) {
                        tileSprite = tileAtlas.dirt.tileSprites;
                    }
                    else {
                        tileSprite = biomes[curBiome].tileAtlas.dirt.tileSprites;
                    }
                } else {
                    if (curBiome == -1) {
                        tileSprite = tileAtlas.grass.tileSprites;
                    }
                    else {
                        tileSprite = biomes[curBiome].tileAtlas.grass.tileSprites;
                    }
                }

                if(biomes[curBiome].generateCave) {
                    if (biomes[curBiome].caveNoiseTexture.GetPixel(x, y).r > 0.5f) {
                        PlaceTile(tileSprite, x, y);
                    }
                } else {
                    PlaceTile(tileSprite, x, y);
                }

                if (y >= height - 1 && curBiome != 2 && curBiome != 3) {
                    int t = Random.Range(0, biomes[curBiome].treeChance);
                    if (t == 1) {
                        if(worldTiles.Contains(new Vector2(x, y))) 
                            GenerateTree(x, y + 1, curBiome);
                    } else {
                        if(Random.Range(0, biomes[curBiome].tallGrassChance) > 7)
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

    public void GenerateTree(int x, int y, int curBiome) {
        int treeH = Random.Range(biomes[curBiome].minTreeHeight, biomes[curBiome].maxTreeHeight);
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
        if (worldTiles.Contains(new Vector2Int(x, y)))
            return;

        GameObject newTile = new GameObject();

        float chunkCoord = Mathf.Round(x / chunkSize);
        newTile.transform.parent = worldChunks[(int)chunkCoord].transform;

        newTile.AddComponent<SpriteRenderer>();

        int spriteIndex = Random.Range(0, tileSprite.Length);
        newTile.GetComponent<SpriteRenderer>().sprite = tileSprite[spriteIndex];

        newTile.name = tileSprite[0].name;
        newTile.transform.position = new Vector2(x + 0.5f, y + 0.5f);

        worldTiles.Add(newTile.transform.position - (Vector3.one * 0.5f));
    }
}
