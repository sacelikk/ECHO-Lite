using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public int width = 10;
    public int height = 10;
    public float tileSize = 1f;

    public Vector2Int startPosition = new Vector2Int(0, 0);
    public Vector2Int goalPosition = new Vector2Int(9, 9);

    private HashSet<Vector2Int> walls = new HashSet<Vector2Int>();
    private Sprite tileSprite;

    private void Start()
    {
        CreateTileSprite();
        CreateWalls();
        CreateGrid();
        CreateGoal();
        CreateAgent();
        SetupCamera();
    }

    private void CreateTileSprite()
    {
        // 1x1 piksel tek renkli bir doku oluşturulup PPU 1 olarak atanır.
        // Böylece 1 birimlik nesneler ekranda tam boyutunda görünür.
        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, Color.white);
        texture.Apply();

        tileSprite = Sprite.Create(
            texture,
            new Rect(0, 0, 1, 1),
            new Vector2(0.5f, 0.5f),
            1f // pixelsPerUnit
        );
    }

    private void CreateWalls()
    {
        walls.Clear();

        walls.Add(new Vector2Int(2, 0));
        walls.Add(new Vector2Int(2, 1));
        walls.Add(new Vector2Int(2, 2));

        walls.Add(new Vector2Int(4, 3));
        walls.Add(new Vector2Int(5, 3));
        walls.Add(new Vector2Int(6, 3));

        walls.Add(new Vector2Int(6, 4));
        walls.Add(new Vector2Int(6, 5));

        walls.Add(new Vector2Int(3, 6));
        walls.Add(new Vector2Int(4, 6));
        walls.Add(new Vector2Int(5, 6));
    }

    private Vector3 GridToWorld(Vector2Int position)
    {
        float x = (position.x - (width - 1) / 2f) * tileSize;
        float y = (position.y - (height - 1) / 2f) * tileSize;

        return new Vector3(x, y, 0f);
    }

    private GameObject CreateSquare(
        string objectName,
        Vector2Int position,
        Color color,
        float scale,
        int sortingOrder)
    {
        GameObject square = new GameObject(objectName);

        square.transform.position = GridToWorld(position);
        square.transform.localScale = new Vector3(scale, scale, 1f);

        SpriteRenderer renderer = square.AddComponent<SpriteRenderer>();
        renderer.sprite = tileSprite;
        renderer.color = color;
        renderer.sortingOrder = sortingOrder;

        return square;
    }

    private void CreateGrid()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2Int position = new Vector2Int(x, y);

                Color color = walls.Contains(position)
                    ? new Color(0.9f, 0.15f, 0.12f)
                    : new Color(0.82f, 0.85f, 0.9f);

                CreateSquare(
                    "Tile_" + x + "_" + y,
                    position,
                    color,
                    tileSize * 0.96f,
                    0
                );
            }
        }
    }

    private void CreateGoal()
    {
        CreateSquare(
            "Goal",
            goalPosition,
            new Color(0.1f, 1f, 0.2f),
            tileSize * 0.88f,
            5
        );
    }

    private void CreateAgent()
    {
        if (!IsWalkable(startPosition))
        {
            Debug.LogError("ECHO: Başlangıç konumu geçersiz!");
            return;
        }

        GameObject agent = CreateSquare(
            "Agent",
            startPosition,
            Color.blue,
            tileSize * 0.76f,
            10
        );

        QLearningAgent controller = agent.AddComponent<QLearningAgent>();
        controller.Initialize(this, startPosition);
    }

    public bool IsWalkable(Vector2Int position)
    {
        if (position.x < 0 || position.x >= width ||
            position.y < 0 || position.y >= height)
        {
            return false;
        }

        return !walls.Contains(position);
    }

    public bool IsGoal(Vector2Int position)
    {
        return position == goalPosition;
    }

    public Vector3 GetWorldPosition(Vector2Int position)
    {
        return GridToWorld(position);
    }

    private void SetupCamera()
    {
        Camera mainCamera = Camera.main;

        if (mainCamera == null)
        {
            Debug.LogWarning("ECHO: Main Camera bulunamadı!");
            return;
        }

        mainCamera.transform.position = new Vector3(0f, 0f, -10f);
        mainCamera.orthographic = true;
        mainCamera.orthographicSize = 5.5f;
        mainCamera.backgroundColor = new Color(0.12f, 0.14f, 0.18f);
        mainCamera.clearFlags = CameraClearFlags.SolidColor;
    }
}