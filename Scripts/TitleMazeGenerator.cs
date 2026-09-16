using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 本編の MazeDigGenerator をタイトル画面用に簡略化した迷路生成クラス。
/// ・穴掘り法で迷路を生成
/// ・壁だけを配置（プレイヤーやゴールは配置しない）
/// タイトル画面の背景迷路として使用する。
public class TitleMazeDigGenerator : MonoBehaviour
{
    [Header("迷路オブジェクト")]
    [SerializeField] private GameObject wallPrefab; // 壁のプレハブ
    
    [Header("迷路設定")]
    [SerializeField] private float cellSize = 2; // 1セルの大きさ
    
    private int[,] maze;    // 迷路データ（0=PATH,1=WALL）
    private int cellsX;     // X方向のセル数
    private int cellsY;     // Y方向のセル数
    private Vector3 origin; // 迷路の左下座標

    const int PATH = 0;
    const int WALL = 1;

    public static TitleMazeDigGenerator Instance;

void Awake()
{
    Instance = this;
}


    void Start()
    {
        InitGrid();        // グリッド初期化
        GenerateMaze();    // 迷路生成
        BuildMazeVisual(); // 壁を配置
    }

    // ---------------------------------------------------------
    // Plane に合わせてグリッドを作る（外周は PATH、それ以外は WALL）
    // ---------------------------------------------------------
    void InitGrid()
    {
        Renderer rend = GetComponent<Renderer>();
        float planeWidth = rend.bounds.size.x;
        float planeHeight = rend.bounds.size.z;

        cellsX = Mathf.FloorToInt(planeWidth / cellSize);
        cellsY = Mathf.FloorToInt(planeHeight / cellSize);

        if (cellsX % 2 == 0) cellsX -= 1;
        if (cellsY % 2 == 0) cellsY -= 1;

        maze = new int[cellsX, cellsY];

        origin = transform.position - new Vector3(
            planeWidth / 2f,
            0,
            planeHeight / 2f
        );

        for (int y = 0; y < cellsY; y++)
        {
            for (int x = 0; x < cellsX; x++)
            {
                if (x == 0 || y == 0 || x == cellsX - 1 || y == cellsY - 1)
                    maze[x, y] = PATH;
                else
                    maze[x, y] = WALL;
            }
        }
    }

    // ---------------------------------------------------------
    // 非再帰版穴掘り法
    // ---------------------------------------------------------
    void GenerateMaze()
    {
        Stack<Vector2Int> stack = new Stack<Vector2Int>();
        System.Random rnd = new System.Random();

        Vector2Int start = new Vector2Int(1, 1);
        maze[start.x, start.y] = PATH;
        stack.Push(start);

        while (stack.Count > 0)
        {
            Vector2Int cell = stack.Pop();

            List<Vector2Int> dirs = new List<Vector2Int>()
            {
                new Vector2Int(1, 0),
                new Vector2Int(-1, 0),
                new Vector2Int(0, 1),
                new Vector2Int(0, -1)
            };

            // シャッフル
            for (int i = 0; i < dirs.Count; i++)
            {
                int r = rnd.Next(i, dirs.Count);
                (dirs[i], dirs[r]) = (dirs[r], dirs[i]);
            }

            foreach (var dir in dirs)
            {
                int nx = cell.x + dir.x * 2;
                int ny = cell.y + dir.y * 2;

                if (nx <= 0 || ny <= 0 || nx >= cellsX - 1 || ny >= cellsY - 1)
                    continue;

                if (maze[nx, ny] == WALL)
                {
                    maze[cell.x + dir.x, cell.y + dir.y] = PATH;
                    maze[nx, ny] = PATH;

                    stack.Push(new Vector2Int(nx, ny));
                }
            }
        }

        // 外周を WALL に戻す
        for (int x = 0; x < cellsX; x++)
        {
            maze[x, 0] = WALL;
            maze[x, cellsY - 1] = WALL;
        }
        for (int y = 0; y < cellsY; y++)
        {
            maze[0, y] = WALL;
            maze[cellsX - 1, y] = WALL;
        }
    }

    // ---------------------------------------------------------
    // WALL を配置する
    // ---------------------------------------------------------
    void BuildMazeVisual()
    {
        for (int y = 0; y < cellsY; y++)
        {
            for (int x = 0; x < cellsX; x++)
            {
                if (maze[x, y] == WALL)
                {
                    Vector3 pos = origin + new Vector3(
                        x * cellSize + cellSize / 2f,
                        0,
                        y * cellSize + cellSize / 2f
                    );

                    Instantiate(wallPrefab, pos, Quaternion.identity, transform);
                }
            }
        }
    }
}
