using System.Collections.Generic;
using Game.Scripts;
using UnityEngine;

public class TerrainGeneration : MonoBehaviour
{
    private const int GRID_SIZE = 10;
    private const int MAX_GRID_HEIGHT = 17;

    public bool GenerateRandomAtStart = true;
    public bool Enforce2GapLimits = true;
    public bool EnforceMinimumHeigh = true;
    
    [SerializeField] TerrainGenerationType _type;
    [SerializeReference] private TerrainConfig _currentSettings;
    [SerializeField] private int _seed = 0;
    [SerializeField] private float _space = 0.0f;
    [SerializeField] GameObject _groundStack;
    [SerializeField] private Texture2D _noise;
    [SerializeField] private Texture2D _secondNoise;
    [SerializeField] private Texture2D _thirdNoise;
    [SerializeField] private BoardManager _boardManager;
    [SerializeField] private Material team1Material;
    [SerializeField] private Material team2Material;
    GameObject board = null;

    public int seed => _seed;

#region Accessors
    public TerrainGenerationType Type { get => _type; set => _type = value; }
    public Texture2D Noise { get => _noise; set => _noise = value; }
    public Texture2D SecondNoise { get => _secondNoise; set => _secondNoise = value; }
    public TerrainConfig CurrentSettings { get => _currentSettings; set => _currentSettings = value; }
    public Texture2D ThirdNoise { get => _thirdNoise; set => _thirdNoise = value; }


    #endregion

    void Start()
    {
        if (GenerateRandomAtStart)
        {
            Generate(Random.Range(-1000000,1000000));
        }
        else
        {
            Generate();
        }
    }
    #region Generation
    public void Generate(int newSeed)
    {
        _seed=newSeed;
        Generate();
    }
    public void Generate()
    {
        if(board != null)
        {
            DestroyImmediate(board);
        } 
        board = GameObject.FindGameObjectWithTag("Board");
        if(board != null)
        {
            DestroyImmediate(board);
        } //quickfix to remove

        board = new GameObject("Board");
        board.tag = "Board";
        //board.transform.SetParent(transform,false);

        _boardManager.boardObject = board;
        
        Texture2D finalText = new Texture2D(GRID_SIZE,GRID_SIZE);
        finalText.filterMode = FilterMode.Point;

        switch (_type)
        {
            case TerrainGenerationType.Basic:
                _currentSettings = TerrainConfig.Basic;
                _noise = BasicPerlinTexture();
                finalText = _noise;
            break;

            case TerrainGenerationType.Rivers:
                _currentSettings = TerrainConfig.RiversTerrain;
                _noise = BasicPerlinTexture();
                _secondNoise = RiversPerlinTexture();
                BlendTextures(_noise, _secondNoise, ref finalText,BlendOperation.Sub);
            break;
            case TerrainGenerationType.Ridges:
                _currentSettings = TerrainConfig.RidgesTerrain;
                _noise = BasicPerlinTexture();
                _secondNoise = RidgesPerlinTexture();
                BlendTextures(_noise, _secondNoise, ref finalText,BlendOperation.Add);
            break;
            case TerrainGenerationType.DeepForest:
                _currentSettings = TerrainConfig.DeepForestTerrain;
                _noise = BasicPerlinTexture();
                _secondNoise = DotsPerlinTexture();
                BlendTextures(_noise, _secondNoise, ref finalText,BlendOperation.Add);
            break;
            case TerrainGenerationType.Everest:
                _currentSettings = TerrainConfig.EverestTerrain;
                _noise = CircleTexture();
                finalText = _noise;
                //failed attemps 
                //BlendTextures(_noise, _noise, ref finalText,BlendOperation.Inverse);
            break;
            case TerrainGenerationType.Flat :
                _currentSettings = TerrainConfig.Flat;
                _noise = FlatTexture();
                finalText = _noise;
            break;

        }
        Color[] pixels = finalText.GetPixels();

        int[,] levels = new int[GRID_SIZE, GRID_SIZE];
        for (int x = 0; x < GRID_SIZE; x++)
        {
            for (int y = 0; y < GRID_SIZE; y++)
            {
                float height = Mathf.Max(0,pixels[y * GRID_SIZE + x].r);

                levels[x, y] = Mathf.FloorToInt(Mathf.Lerp(1,MAX_GRID_HEIGHT * (_currentSettings.heightMultiplier/3/*Way to high horrible*/),height));
            }
        }

        if(Enforce2GapLimits) EnforceMaxSlope(ref levels, maxDifference: 2);
        if(EnforceMinimumHeigh) EnforceMinimumGround(ref levels);
        _boardManager.Board = new GroundStack[GRID_SIZE,GRID_SIZE];
        
        for (int x = 0; x < GRID_SIZE; x++)
        {
            for (int y = 0; y < GRID_SIZE; y++)
            {
                GroundStack g = Instantiate(_groundStack, board.transform).GetComponent<GroundStack>();
                g.transform.localPosition = new Vector3(x + _space * x, 0, y + _space * y);
                g.Level = levels[x, y];
                g.GridPos = new Vector2Int(x,y);
                _boardManager.Board[x,y] = g;
            }
        }

      
        _boardManager.SetUpBoard();
        _boardManager.BoardHeights = levels;
        
        for (int i = 0; i < GRID_SIZE; i++)
        {
            GroundStack g = Instantiate(_groundStack, board.transform).GetComponent<GroundStack>();
            g.transform.localPosition = new Vector3(-1 - _space, 0, i + _space * i);
            g.GridPos = new Vector2Int(-1, i);
            g.Level = levels[0, i];
            _boardManager.backBoardExtension1[i] = g;
            
            
            foreach (MeshRenderer mr in g.GetComponentsInChildren<MeshRenderer>())
            {
                mr.material = team2Material;
            }
        }
        
        for (int i = 0; i < GRID_SIZE; i++)
        {
            GroundStack g = Instantiate(_groundStack, board.transform).GetComponent<GroundStack>();
            g.transform.localPosition = new Vector3(11, 0, i + _space * i);
            g.GridPos = new Vector2Int(11, i);
            g.Level = levels[9, i];
            _boardManager.backBoardExtension2[i] = g;
            
            foreach (MeshRenderer meshRenderer in g.GetComponentsInChildren<MeshRenderer>())
            {
                meshRenderer.material = team1Material;
            }
        }

    }
    private void EnforceMinimumGround(ref int[,] gridLevels)
    {
        int minimumLevel = int.MaxValue;
        for (int x = 0; x < GRID_SIZE; x++){
            for (int y = 0; y < GRID_SIZE; y++)
            {
                int currentLevel = gridLevels[x, y];
                minimumLevel = Mathf.Min(currentLevel,minimumLevel);
            }
        }
        if(minimumLevel > 1)
        {
            int shiftDown = minimumLevel - 1;
            for (int x = 0; x < GRID_SIZE; x++)
            {
                for (int y = 0; y < GRID_SIZE; y++)
                {
                    gridLevels[x, y] -= shiftDown;
                }
            }
        }
    }
    private void EnforceMaxSlope(ref int[,] gridLevels, int maxDifference = 2)
    {
        bool changed = true;
        int maxPasses = GRID_SIZE * 10; //avoidnfinite loops
        int pass = 0;

        int[] dx = { 1, -1, 0, 0 };
        int[] dy = { 0, 0, 1, -1 };

        while (changed && pass < maxPasses)
        {
            changed = false;
            pass++;

            for (int x = 0; x < GRID_SIZE; x++) {
                for (int y = 0; y < GRID_SIZE; y++)
                {
                    int currentLevel = gridLevels[x, y];

                    for (int dir = 0; dir < 4; dir++)
                    {
                        int nx = x + dx[dir];
                        int ny = y + dy[dir];

                        if (nx >= 0 && nx < GRID_SIZE && ny >= 0 && ny < GRID_SIZE)
                        {
                            int neighborLevel = gridLevels[nx, ny];

                            if (currentLevel > neighborLevel + maxDifference)
                            {
                                gridLevels[x, y] = currentLevel - 1;
                                changed = true;
                            }
                        }
                    }
                }
            }
        }
    }

#endregion

#region Blends

    private void BlendTextures(Texture2D t1, Texture2D t2, ref Texture2D result, BlendOperation operationType = BlendOperation.Add )
    {
        Color[] P1 = t1.GetPixels();
        Color[] P2 = t2.GetPixels();
        Color[] r = new Color[P1.Length];

        for (int i = 0; i < P1.Length; i++)
        {
            switch (operationType)
            {
                case BlendOperation.Add:
                    r[i] = P1[i] + P2[i];
                break;
                case BlendOperation.Sub:
                    r[i] = P1[i] - P2[i];
                break;
                case BlendOperation.Mult:
                    r[i] = P1[i] * P2[i];
                break;
                case BlendOperation.Inverse:
                    r[i] = P1[i]*-1;
                break;
            }
        }

        result.SetPixels(r);
    }

    public enum BlendOperation
    { 
        Add,
        Sub,
        Mult,
        Inverse,
    }
    #endregion

#region Textures
    Texture2D BasicPerlinTexture()
    {
        Texture2D texture = new Texture2D(GRID_SIZE, GRID_SIZE);
        texture.filterMode = FilterMode.Point;
        Color[] pixels = new Color[GRID_SIZE * GRID_SIZE];

        for (int y = 0; y < GRID_SIZE; y++)
        {
            for (int x = 0; x < GRID_SIZE; x++)
            {
                float xCoord = (float)x / GRID_SIZE * _currentSettings.scale + seed;
                float yCoord = (float)y / GRID_SIZE * _currentSettings.scale + seed;

                float sample = Mathf.PerlinNoise(xCoord, yCoord);
                pixels[y * GRID_SIZE + x] = new Color(sample, sample, sample);
            }
        }

        texture.SetPixels(pixels);
        texture.Apply();
        return texture;
    }
    Texture2D RiversPerlinTexture()
    {
        var _settings = (RiversTerrainConfig)_currentSettings;

        Texture2D texture = new Texture2D(GRID_SIZE, GRID_SIZE);
        texture.filterMode = FilterMode.Point;
        
        Color[] pixels = new Color[GRID_SIZE * GRID_SIZE];

        float seedX = seed % 1000 * 0.137f;
        float seedY = seed / 1000 % 1000 * 0.259f;

        for (int y = 0; y < GRID_SIZE; y++)
        {
            for (int x = 0; x < GRID_SIZE; x++)
            {
                float xCoord = (float)x / GRID_SIZE * _settings.riverScale + seedX;
                float yCoord = (float)y / GRID_SIZE * _settings.riverScale + seedY;

                float rawNoise = Mathf.PerlinNoise(xCoord, yCoord);
                float riversTreshold = Mathf.Abs(rawNoise - _settings.riverThreshold);

                float riverDepth = 0f;

                if (riversTreshold < _settings.riverWidth)
                {
                    float t = 1f - (riversTreshold / _settings.riverWidth);
                    t = Mathf.SmoothStep(0f, 1f, t);
                    riverDepth = Mathf.Lerp(_settings.halfStep, _settings.riverCarveDepth, t);
                }
                else if (riversTreshold < _settings.bankWidth)
                {
                    float t = (riversTreshold - _settings.riverWidth) / (_settings.bankWidth - _settings.riverWidth);
                    t = Mathf.SmoothStep(0f, 1f, t);
                    riverDepth = Mathf.Lerp(_settings.halfStep, 0f, t);
                }

                pixels[y * GRID_SIZE + x] = new Color(riverDepth, riverDepth, riverDepth, 1f);
            }
        }

        texture.SetPixels(pixels);
        texture.Apply();
        return texture;
    }
    Texture2D RidgesPerlinTexture()
    {
        var _settings = (RidgesTerrainConfig)_currentSettings;

        Texture2D texture = new Texture2D(GRID_SIZE, GRID_SIZE);
        texture.filterMode = FilterMode.Point;
        
        Color[] pixels = new Color[GRID_SIZE * GRID_SIZE];

        float seedX = seed % 1000 * 0.137f;
        float seedY = seed / 1000 % 1000 * 0.259f;

        for (int y = 0; y < GRID_SIZE; y++)
        {
            for (int x = 0; x < GRID_SIZE; x++)
            {
                float xCoord = (float)x / GRID_SIZE * _settings.ridgeScale + seedX;
                float yCoord = (float)y / GRID_SIZE * _settings.ridgeScale + seedY;

                float rawNoise = Mathf.PerlinNoise(xCoord, yCoord);
                float riversTreshHold = Mathf.Abs(rawNoise - _settings.ridgeThreshold);

                float riverDepth = 0f;

                if (riversTreshHold < _settings.ridgeWidth)
                {
                    float t = 1f - (riversTreshHold / _settings.ridgeWidth);
                    t = Mathf.SmoothStep(0f, 1f, t);
                    riverDepth = Mathf.Lerp(_settings.halfStep, _settings.ridgeCarveDepth, t);
                }
                else if (riversTreshHold < _settings.bankWidth)
                {
                    float t = (riversTreshHold - _settings.ridgeWidth) / (_settings.bankWidth - _settings.ridgeWidth);
                    t = Mathf.SmoothStep(0f, 1f, t);
                    riverDepth = Mathf.Lerp(_settings.halfStep, 0f, t);
                }

                pixels[y * GRID_SIZE + x] = new Color(riverDepth, riverDepth, riverDepth, 1f);
            }
        }

        texture.SetPixels(pixels);
        texture.Apply();
        return texture;
    }
    Texture2D DotsPerlinTexture()
    {
        Texture2D texture = new Texture2D(GRID_SIZE, GRID_SIZE);
        texture.filterMode = FilterMode.Point;
        Color[] pixels = new Color[GRID_SIZE * GRID_SIZE];
        
        var _settings = (DeepForestTerrainConfig)_currentSettings;

        float seedX = seed % 1000 * 0.137f;
        float seedY = seed / 1000 % 1000 * 0.259f;

        for (int i = 0; i < pixels.Length; i++) pixels[i] = Color.black;

        int offsetX = Mathf.FloorToInt(seedX) % _settings.knightModulo;
        int offsetY = Mathf.FloorToInt(seedY) % _settings.knightModulo;

        for (int y = 0; y < GRID_SIZE; y++)
        {
            for (int x = 0; x < GRID_SIZE; x++)
            {
                float sampleX = (x * _settings.noiseFrequency) + seedX;
                float sampleY = (y * _settings.noiseFrequency) + seedY;
                float noise = Mathf.PerlinNoise(sampleX, sampleY);

                bool isBaseKnight = ((2 * (x + offsetX) + y + offsetY) % _settings.knightModulo) == 0;

                if (isBaseKnight && noise > _settings.noiseCutoff)
                {
                    pixels[y * GRID_SIZE + x] = Color.white;
                }
                else
                {
                    pixels[y * GRID_SIZE + x] = Color.black;
                }
            }
        }

        texture.SetPixels(pixels);
        texture.Apply();
        return texture;
    }
    Texture2D CircleTexture()
    {
        Texture2D texture = new Texture2D(GRID_SIZE, GRID_SIZE);
        texture.filterMode = FilterMode.Point;
        Color[] pixels = new Color[GRID_SIZE * GRID_SIZE];

        Vector2 center = new Vector2((GRID_SIZE - 1) * 0.5f, (GRID_SIZE - 1) * 0.5f);
    
        float maxRadius = (GRID_SIZE +2) * 0.5f;

        for (int y = 0; y < GRID_SIZE; y++)
        {
            for (int x = 0; x < GRID_SIZE; x++)
            {
                float dist = Vector2.Distance(new Vector2(x, y), center);

                float value = Mathf.Clamp01(1.0f - (dist / maxRadius));

                pixels[y * GRID_SIZE + x] = new Color(value, value, value);
            }
        }

        texture.SetPixels(pixels);
        texture.Apply();
        return texture;
    }
    Texture2D FlatTexture()
    {
        Texture2D texture = new Texture2D(GRID_SIZE, GRID_SIZE);
        texture.filterMode = FilterMode.Point;
        Color[] pixels = new Color[GRID_SIZE * GRID_SIZE];

        for (int y = 0; y < GRID_SIZE; y++)
        {
            for (int x = 0; x < GRID_SIZE; x++)
            {
                pixels[y * GRID_SIZE + x] = Color.white;
            }
        }

        texture.SetPixels(pixels);
        texture.Apply();
        return texture;
    }

    // TO COPY CAT
    // Texture2D BASETexture()
    // {
    //     Texture2D texture = new Texture2D(GRID_SIZE, GRID_SIZE);
    //     texture.filterMode = FilterMode.Point;
    //     Color[] pixels = new Color[GRID_SIZE * GRID_SIZE];

    //     for (int y = 0; y < GRID_SIZE; y++)
    //     {
    //         for (int x = 0; x < GRID_SIZE; x++)
    //         {
    //             pixels[y * GRID_SIZE + x] = Color.white;
    //         }
    //     }

    //     texture.SetPixels(pixels);
    //     texture.Apply();
    //     return texture;
    // }

#endregion

}

public enum TerrainGenerationType
{
    Basic,
    Rivers,
    Ridges,
    DeepForest,
    Everest,
    Flat,


}
