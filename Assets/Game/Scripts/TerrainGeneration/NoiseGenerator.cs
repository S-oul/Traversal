using Unity.VisualScripting;
using UnityEngine;

public class NoiseGenerator : MonoBehaviour
{
    public int width = 256;
    public int height = 256;
    public float scale = 20.0f;
    public float offsetX = 100f;
    public float offsetY = 100f;

    void Start()
    {
        SpriteRenderer rend = GetComponent<SpriteRenderer>();
        rend.sprite = GeneratePerlinTexture().ConvertTo<Sprite>();
    }

    Texture2D GeneratePerlinTexture()
    {
        Texture2D texture = new Texture2D(width, height);
        Color[] pixels = new Color[width * height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float xCoord = (float)x / width * scale + offsetX;
                float yCoord = (float)y / height * scale + offsetY;

                float sample = Mathf.PerlinNoise(xCoord, yCoord);
                pixels[y * width + x] = new Color(sample, sample, sample);
            }
        }

        texture.SetPixels(pixels);
        texture.Apply();
        return texture;
    }
}