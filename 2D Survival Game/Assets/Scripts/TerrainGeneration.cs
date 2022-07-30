using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGeneration : MonoBehaviour
{
    private enum ORES {
        COAL = 0,
        IRON = 1,
        GOLD = 2,
        DIAMOND = 3
    };

    public PlayerController player;
    public CamController cam;
    public GameObject tileDrop;

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
    private List<GameObject> worldTileObjects = new List<GameObject>();
    private List<TileClass> worldTileClasses = new List<TileClass>();

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

        cam.worldSize = worldSize;
        cam.Spawn(new Vector3(player.spawnPos.x, player.spawnPos.y, cam.transform.position.z));
        player.Spawn();
    }

    private void Update() {
        RefreshChunks();
    }

    public void RefreshChunks() {
        for (int i = 0; i < worldChunks.Length; i++) {
            if (Mathf.Abs(((i * chunkSize) + (chunkSize / 2)) - player.transform.position.x) > Camera.main.orthographicSize * 4f)
                worldChunks[i].SetActive(false);
            else
                worldChunks[i].SetActive(true);
        }
    }

    public void GetBiomes() {
        /*biomes[0] = new Grassland();
        biomes[1] = new Forest();
        biomes[2] = new Tundra();
        biomes[3] = new Desert();*/
        for (int i = 0; i < biomes.Length; i++) {
            if(biomes[i].oresRarity[(int)ORES.COAL] != -1) 
                biomes[i].ores[(int)ORES.COAL] = new Coal(biomes[i].oresRarity[(int)ORES.COAL], biomes[i].oresSize[(int)ORES.COAL]);
            else
                biomes[i].ores[(int)ORES.COAL] = new Coal();

            if (biomes[i].oresRarity[(int)ORES.IRON] != -1)
                biomes[i].ores[(int)ORES.IRON] = new Iron(biomes[i].oresRarity[(int)ORES.IRON], biomes[i].oresSize[(int)ORES.IRON]);
            else
                biomes[i].ores[(int)ORES.IRON] = new Iron();

            if (biomes[i].oresRarity[(int)ORES.GOLD] != -1)
                biomes[i].ores[(int)ORES.GOLD] = new Gold(biomes[i].oresRarity[(int)ORES.GOLD], biomes[i].oresSize[(int)ORES.GOLD]);
            else
                biomes[i].ores[(int)ORES.GOLD] = new Gold();

            if (biomes[i].oresRarity[(int)ORES.DIAMOND] != -1)
                biomes[i].ores[(int)ORES.DIAMOND] = new Diamond(biomes[i].oresRarity[(int)ORES.DIAMOND], biomes[i].oresSize[(int)ORES.DIAMOND]);
            else
                biomes[i].ores[(int)ORES.DIAMOND] = new Diamond();
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
        biomeColorKey[0].time = 0.3f;
        biomeColorKey[1].time = 0.6f;
        biomeColorKey[2].time = 0.8f;
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
        float v;
        Color col;
        for (int x = 0; x < biomeMap.width; x++) {
            for (int y = 0; y < biomeMap.height; y++) {
                //X and Y
                v = Mathf.PerlinNoise((x + seed) * biomeFreq, (y + seed) * biomeFreq);
                //Just X
                //float v = Mathf.PerlinNoise((x + seed) * biomeFreq, (x + seed) * biomeFreq);
                col = biomeGradient.Evaluate(v);
                biomeMap.SetPixel(x, y, col);
            }
        }

        biomeMap.Apply(); 
    }

    public void GenerateNoiseTexture(float frequency, float limit, Texture2D noiseTexture) {
        float v;
        for (int x = 0; x < noiseTexture.width; x++) {
            for (int y = 0; y < noiseTexture.height; y++) {
                v = Mathf.PerlinNoise((x + seed) * frequency, (y + seed) * frequency);

                if (v > limit)
                    noiseTexture.SetPixel(x, y, Color.white);
                else
                    noiseTexture.SetPixel(x, y, Color.black);
            }
        }

        noiseTexture.Apply();
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

    public int GetCurrTileBiome(int x, int y) {
        int biome = -1;

        //Debug.Log("biome map: " + biomeMap.GetPixel(x, y).r + " " + biomeMap.GetPixel(x, y).g + " " + biomeMap.GetPixel(x, y).b);

        if ((Mathf.Abs(biomeMap.GetPixel(x, y).r - biomes[0].biomeColor.r) < 0.1f) && (Mathf.Abs(biomeMap.GetPixel(x, y).g - biomes[0].biomeColor.g) < 0.1f) && (Mathf.Abs(biomeMap.GetPixel(x, y).b - biomes[0].biomeColor.b) < 0.1f)) {
            biome = 0;
        }
        else if ((Mathf.Abs(biomeMap.GetPixel(x, y).r - biomes[1].biomeColor.r) < 0.1f) && (Mathf.Abs(biomeMap.GetPixel(x, y).g - biomes[1].biomeColor.g) < 0.1f) && (Mathf.Abs(biomeMap.GetPixel(x, y).b - biomes[1].biomeColor.b) < 0.1f)) {
            biome = 1;
        }
        else if ((Mathf.Abs(biomeMap.GetPixel(x, y).r - biomes[2].biomeColor.r) < 0.1f) && (Mathf.Abs(biomeMap.GetPixel(x, y).g - biomes[2].biomeColor.g) < 0.1f) && (Mathf.Abs(biomeMap.GetPixel(x, y).b - biomes[2].biomeColor.b) < 0.1f)) {
            biome = 2;
        }
        else if ((Mathf.Abs(biomeMap.GetPixel(x, y).r - biomes[3].biomeColor.r) < 0.1f) && (Mathf.Abs(biomeMap.GetPixel(x, y).g - biomes[3].biomeColor.g) < 0.1f) && (Mathf.Abs(biomeMap.GetPixel(x, y).b - biomes[3].biomeColor.b) < 0.1f)) {
            biome = 3;
        }

        return biome;
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
        TileClass tileClass;
        int curBiome;
        float height;
        for (int x = 0; x < worldSize; x++) {
            curBiome = biomeList[x, 0];
            height = Mathf.PerlinNoise((x + seed) * biomes[curBiome].terrFreq, seed * biomes[curBiome].terrFreq) * biomes[curBiome].heightMultiplier + heightAddition;

            if (x == worldSize / 2)
                player.spawnPos = new Vector2(x, height + 1.5f);

            for (int y = 0; y < height; y++) {
                curBiome = biomeList[x, y];
                if (y < height - biomes[curBiome].dirtLayerHeight) {
                    if (curBiome == -1) {
                        tileClass = tileAtlas.stone;
                    } else {
                        tileClass = biomes[curBiome].tileAtlas.stone;
                    }

                    if (biomes[curBiome].ores[0].spreadTexture.GetPixel(x, y).r > 0.5f && height - y > biomes[curBiome].ores[0].maxSpawnHeight)
                        tileClass = tileAtlas.coal;
                    if (biomes[curBiome].ores[1].spreadTexture.GetPixel(x, y).r > 0.5f && height - y > biomes[curBiome].ores[1].maxSpawnHeight)
                        tileClass = tileAtlas.iron;
                    if (biomes[curBiome].ores[2].spreadTexture.GetPixel(x, y).r > 0.5f && height - y > biomes[curBiome].ores[2].maxSpawnHeight)
                        tileClass = tileAtlas.gold;
                    if (biomes[curBiome].ores[3].spreadTexture.GetPixel(x, y).r > 0.5f && height - y > biomes[curBiome].ores[3].maxSpawnHeight)
                        tileClass = tileAtlas.diamond;
                } else if (y < height - 1) {
                    if (curBiome == -1) {
                        tileClass = tileAtlas.dirt;
                    } else {
                        tileClass = biomes[curBiome].tileAtlas.dirt;
                    }
                } else {
                    if (curBiome == -1) {
                        tileClass = tileAtlas.grass;
                    } else {
                        tileClass = biomes[curBiome].tileAtlas.grass;
                    }
                }

                if (biomes[curBiome].generateCave) {
                    if (biomes[curBiome].caveNoiseTexture.GetPixel(x, y).r > 0.5f) {
                        PlaceTile(tileClass, x, y, true);
                    } else if (tileClass.wallVariant != null) {
                        PlaceTile(tileClass.wallVariant, x, y, true);
                    }
                } else {
                    PlaceTile(tileClass, x, y, true);
                }

                if (y >= height - 1) {
                    int t = Random.Range(0, biomes[curBiome].treeChance);
                    if (t == 1) {
                        if (worldTiles.Contains(new Vector2(x, y)))
                            GenerateTree(x, y + 1, curBiome);
                    } else {
                        if (Random.Range(0, biomes[curBiome].tallGrassChance) > 7)
                            if (worldTiles.Contains(new Vector2(x, y)))
                                PlaceTile(biomes[curBiome].tileAtlas.tallGrass, x, y + 1, true);
                    }
                }
            }
        }
    }

    public void GenerateTree(int x, int y, int curBiome) {
        int treeH = Random.Range(biomes[curBiome].minTreeHeight, biomes[curBiome].maxTreeHeight);
        for (int h = 0; h < treeH; h++) {
            if (h == 0) {
                if (curBiome == 3)
                    PlaceTile(biomes[curBiome].tileAtlas.logBase, x, y + h, true);
                else
                    PlaceTile(tileAtlas.logBase, x, y + h, true);
            }
            else {
                if (curBiome == 3)
                    PlaceTile(biomes[curBiome].tileAtlas.log, x, y + h, true);
                else
                    PlaceTile(tileAtlas.log, x, y + h, true);
            }
        }
        if (curBiome != 2 && curBiome != 3) {
            for (int j = 0; j < 3; j++) {
                if (j < 2) {
                    PlaceTile(tileAtlas.leaf, x, y + treeH + j, true);
                    PlaceTile(tileAtlas.leaf, x + 1, y + treeH + j, true);
                    PlaceTile(tileAtlas.leaf, x - 1, y + treeH + j, true);
                }
                else {
                    PlaceTile(tileAtlas.leaf, x, y + treeH + j, true);
                }
            }
        }
    }

    public void CheckTile(TileClass tile, int x, int y) {
        Vector2 blockPos = new Vector2Int(x, y);
        if (!worldTiles.Contains(blockPos)) {
            //place tile if the tile doesn't exist
            PlaceTile(tile, x, y, false);
        }
        else {
            if (!worldTileClasses[worldTiles.IndexOf(blockPos)].isCollidable) {
                //overwrite non-collidable tile
                RemoveTile(x, y);
                PlaceTile(tile, x, y, false);
            }
        }
    }

    public void PlaceTile(TileClass tile, int x, int y, bool nat) {
        bool isCollidable = tile.isCollidable;
        if (x > worldSize || x < 0 || y < 0 || y > worldSize)
            return;

        GameObject newTile = new GameObject();

        int chunkCoord = Mathf.RoundToInt(Mathf.Round(x / chunkSize));
        newTile.transform.parent = worldChunks[chunkCoord].transform;

        newTile.AddComponent<SpriteRenderer>();
        if (isCollidable) {
            newTile.AddComponent<BoxCollider2D>();
            newTile.GetComponent<BoxCollider2D>().size = Vector2.one;
            newTile.tag = "Ground";
        }

        int spriteIndex = Random.Range(0, tile.tileSprites.Length);
        SpriteRenderer newTileSR = newTile.GetComponent<SpriteRenderer>();
        newTileSR.sprite = tile.tileSprites[spriteIndex];
        if (tile.isCollidable)
            newTileSR.sortingOrder = -5;
        else
            newTileSR.sortingOrder = -10;

        if (tile.name.ToUpper().Contains("WALL"))
            newTileSR.color = new Color(0.6f, 0.6f, 0.6f);

        newTile.name = tile.tileSprites[0].name;
        newTile.transform.position = new Vector2(x + 0.5f, y + 0.5f);
        tile.isNatural = nat;

        worldTiles.Add(newTile.transform.position - (Vector3.one * 0.5f));
        worldTileObjects.Add(newTile);
        worldTileClasses.Add(tile);
    }

    public void RemoveTile(int x, int y) {
        if (!worldTiles.Contains(new Vector2Int(x, y)))
            return;

        int tilePos = worldTiles.IndexOf(new Vector2(x, y));

        if(worldTileClasses[tilePos].wallVariant != null && worldTileClasses[tilePos].isNatural) {
            PlaceTile(worldTileClasses[tilePos].wallVariant, x, y, false);
        }

        Destroy(worldTileObjects[tilePos]);
        if(worldTileClasses[tilePos].dropTile) {
            GameObject droppedTile = Instantiate(tileDrop, new Vector2(x, y + 0.2f), Quaternion.identity);
            droppedTile.GetComponent<SpriteRenderer>().sprite = worldTileClasses[tilePos].tileSprites[0];
        }

        worldTileObjects.RemoveAt(tilePos);
        worldTileClasses.RemoveAt(tilePos);
        worldTiles.RemoveAt(tilePos);
    }
}
