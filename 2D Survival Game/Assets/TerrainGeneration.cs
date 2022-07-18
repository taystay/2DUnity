using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGeneration : MonoBehaviour
{
    [Header("Tile Sprites")]
    public Sprite dirt;
    public Sprite grass;
    public Sprite stone;
    public Sprite log;
    public Sprite logBase;
    public Sprite leaf;

    [Header("Generation Settings")]
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
    public Texture2D noiseTexture;

    private List<Vector2> worldTiles = new List<Vector2>();

    private void Start() {
        seed = Random.Range(-10000, 10000);
        GenerateNoiseTexture();
        GenerateTerrain();
    }

    public void GenerateTerrain() {
        for (int x = 0; x < worldSize; x++) {
            float height = Mathf.PerlinNoise((x + seed) * terrFreq, seed * terrFreq) * heightMultiplier + heightAddition;
            for (int y = 0; y < height; y++) {
                Sprite tileSprite;
                if (y < height - dirtLayerHeight) {
                    tileSprite = stone;
                } else if (y < height - 1) {
                    tileSprite = dirt;
                } else {
                    tileSprite = grass;
                }

                if(generateCave) {
                    if (noiseTexture.GetPixel(x, y).r > surfaceVal) {
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

    public void GenerateNoiseTexture() {
        noiseTexture = new Texture2D(worldSize, worldSize);

        for (int x = 0; x < noiseTexture.width; x++) {
            for (int y = 0; y < noiseTexture.height; y++) {
                float v = Mathf.PerlinNoise((x + seed) * caveFreq, (y + seed) * caveFreq);
                noiseTexture.SetPixel(x, y, new Color(v, v, v));
            }
        }

        noiseTexture.Apply();
    }

    public void GenerateTree(float x, float y) {
        int treeH = Random.Range(minTreeHeight, maxTreeHeight);
        for (int h = 0; h < treeH; h++) {
            if(h == 0) {
                PlaceTile(logBase, x, y + h);
            } else {
                PlaceTile(log, x, y + h);
            }
        }

        for (int j = 0; j < 3; j++) {
            if(j < 2) {
                PlaceTile(leaf, x, y + treeH + j);
                PlaceTile(leaf, x + 1, y + treeH + j);
                PlaceTile(leaf, x - 1, y + treeH + j);
            } else {
                PlaceTile(leaf, x, y + treeH + j);
            }
        }
    }

    public void PlaceTile(Sprite tileSprite, float x, float y) {
        GameObject newTile = new GameObject();
        newTile.transform.parent = transform;
        newTile.AddComponent<SpriteRenderer>();
        newTile.GetComponent<SpriteRenderer>().sprite = tileSprite;
        newTile.name = tileSprite.name;
        newTile.transform.position = new Vector2(x + 0.5f, y + 0.5f);

        worldTiles.Add(newTile.transform.position - (Vector3.one * 0.5f));
    }
}
