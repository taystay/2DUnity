using System.Collections;
using UnityEngine;

public class TerrainGeneration : MonoBehaviour
{
    public Sprite tile;
    public int worldSize = 100;
    public float surfaceVal = 0.25f;
    public float caveFreq = 0.08f;
    public float terrFreq = 0.04f;
    public float heightMultiplier = 25;
    public float heightAddition = 25f;
    public float seed;
    public Texture2D noiseTexture;

    private void Start() {
        seed = Random.Range(-10000, 10000);
        GenerateNoiseTexture();
        GenerateTerrain();
    }

    public void GenerateTerrain() {
        for (int x = 0; x < worldSize; x++) {
            float height = Mathf.PerlinNoise((x + seed) * terrFreq, seed * terrFreq) * heightMultiplier + heightAddition;
            for (int y = 0; y < height; y++) {
                if (noiseTexture.GetPixel(x, y).r < surfaceVal) {
                    GameObject newTile = new GameObject(name = "tile");
                    newTile.transform.parent = transform;
                    newTile.AddComponent<SpriteRenderer>();
                    newTile.GetComponent<SpriteRenderer>().sprite = tile;
                    newTile.transform.position = new Vector2(x + 0.5f, y + 0.5f);
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
}
