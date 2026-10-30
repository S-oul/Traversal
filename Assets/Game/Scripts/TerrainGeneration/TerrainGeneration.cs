using UnityEngine;

public class TerrainGeneration : MonoBehaviour
{
    private const int GRID_SIZE = 10;  
    
    [Range(-1000000,1000000)]
    [SerializeField] private int _seed = 0;
    [SerializeField] private float _space = 0.05f;
    [SerializeField] private float _scale = 1;
    [SerializeField] GameObject _groundStack;

    GameObject grid = null;

    public int seed => _seed;

    public int gridSize => GRID_SIZE;

    public void Generate()
    {
        if(grid != null) DestroyImmediate(grid);

        grid = new GameObject("Grid");
        //grid.transform.SetParent(transform,false);
        
        Texture2D noise = GeneratePerlinTexture();
        Color[] pixels = noise.GetPixels();

        for(int x = 0; x < gridSize; x++){
         for(int y = 0; y < gridSize; y++)
        {
            float height = pixels[y*gridSize+x].r;
            print(height);
            GroundStack g = Instantiate(_groundStack,grid.transform).GetComponent<GroundStack>();
            g.transform.localPosition = new Vector3(x+_space*x, 0, y+_space*y);
            g.Level = Mathf.FloorToInt(height*5);
        }}
    }
    public void Generate(int newSeed)
    {
        _seed=newSeed;
        Generate();
    }

    Texture2D GeneratePerlinTexture()
    {
        Texture2D texture = new Texture2D(gridSize, gridSize);
        Color[] pixels = new Color[gridSize * gridSize];

        for (int y = 0; y < 10; y++)
        {
            for (int x = 0; x < gridSize; x++)
            {
                float xCoord = (float)x / gridSize * _scale + seed;
                float yCoord = (float)y / gridSize * _scale + seed;

                float sample = Mathf.PerlinNoise(xCoord, yCoord);
                pixels[y * gridSize + x] = new Color(sample, sample, sample);
            }
        }

        texture.SetPixels(pixels);
        texture.Apply();
        return texture;
    }
}
