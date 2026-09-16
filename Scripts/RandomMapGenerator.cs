using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 穴掘り法で迷路を生成し、壁・プレイヤー・ゴール・コイン・特殊アイテムを配置します。
/// また、入口から出口までのルートを計算し、LineRendererで描画します。
/// </summary>
public class MazeDigGenerator : MonoBehaviour
{
    // -------------------------
    // プレハブ類
    // -------------------------
    [Header("迷路オブジェクト")]
    [SerializeField] private GameObject wallPrefab;   // 壁のプレハブ
    [SerializeField] private GameObject goalPrefab;   // ゴールのプレハブ
    [SerializeField] private GameObject playerPrefab; // プレイヤーのプレハブ
    
    [Header("コイン・特殊アイテム")]
    [SerializeField] private GameObject coinPrefab;        // コインのプレハブ
    [SerializeField] private Transform coinParent;         // コインの親オブジェクト
    [SerializeField] private int coinCount = 1000;         // コイン数
    [SerializeField] private GameObject specialItemPrefab; // 特殊アイテム
    [SerializeField] private int specialItemCount = 30;    // 特殊アイテム数

    // -------------------------
    // 迷路設定
    // -------------------------
    [Header("迷路設定")]
    [SerializeField] private int cellSize = 3; // 1セルの大きさ
    private int[,] maze;                       // 迷路データ（0=PATH,1=WALL）
    private int cellsX;                        // X方向のセル数
    private int cellsY;                        // Y方向のセル数
    private Vector3 origin;                    // 迷路の左下座標

    const int PATH = 0;
    const int WALL = 1;

    // -------------------------
    // 外部連携
    // -------------------------
    [SerializeField] private FollowCamera cameraManager;          // カメラ追従
    [SerializeField] private CompassController compassController; // コンパス

    // -------------------------
    // 経路表示
    // -------------------------
    [Header("ルート表示")]
    public LineRenderer lineRenderer;    // 経路ライン   
    private List<Vector2Int> debugPath; // 経路データ
    public bool debugShowPath = true;    // Gizmo表示ON/OFF

    // 入口・出口
    Vector2Int entrancePos;
    Vector2Int exitPos;

    public static MazeDigGenerator Instance;
    
void Awake()
{
    Instance = this;
}


    void Start()
    {

        if (coinParent != null)
        {
            Destroy(coinParent.gameObject);
        }
        GameObject obj = new GameObject("CoinManager");
        obj.transform.rotation = Quaternion.identity;
        coinParent = obj.transform;

        InitGrid();              // グリッド初期化
        GenerateMaze();          // 穴掘り法で迷路生成
        CreateEntranceAndExit(); // 入口と出口を作成
        BuildMazeVisual();       // 壁を配置
        SpawnPlayer();           // プレイヤー配置
        SpawnGoal();             // ゴール配置
        SpawnCoins();            // コイン配置
        SpawnSpecialItems();     // 特殊アイテム配置

        // 経路計算（入口→出口）
        debugPath = FindPath(entrancePos,exitPos);
        DrawPathLine();
        lineRenderer.enabled = false; // 初期は非表示
        OnDrawGizmos();

    }

    // =========================================================
    // グリッド初期化（Plane のサイズからセル数を決める）
    // =========================================================
    private void InitGrid()
    {
        Renderer rend = GetComponent<Renderer>();
        float planeWidth = rend.bounds.size.x;
        float planeHeight = rend.bounds.size.z;

        cellsX = Mathf.FloorToInt(planeWidth / cellSize);
        cellsY = Mathf.FloorToInt(planeHeight / cellSize);

        // 奇数に揃える（穴掘り法の仕様）
        if (cellsX % 2 == 0) cellsX -= 1;
        if (cellsY % 2 == 0) cellsY -= 1;

        maze = new int[cellsX, cellsY];

        // 左下の原点を計算
        origin = transform.position - new Vector3(
            planeWidth / 2f,
            0,
            planeHeight / 2f
        );

        // 外周は PATH、それ以外は WALL
        for (int y = 0; y < cellsY; y++)
        {
            for (int x = 0; x < cellsX; x++)
            {
                maze[x,y]= (x == 0 || y == 0 || x == cellsX - 1 || y == cellsY - 1)
                    ? PATH : WALL;
            }
        }
    }

    // =========================================================
    // 穴掘り法（非再帰版）
    // =========================================================
    private void GenerateMaze()
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

            // ランダムシャッフル
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

    // =========================================================
    // 壁を配置する
    // =========================================================
    private void BuildMazeVisual()
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

    // =========================================================
    // 入口と出口を作成
    // =========================================================
    private void CreateEntranceAndExit()
    {
        // 入口は中央
        entrancePos = new Vector2Int(cellsX / 2, cellsY / 2);
        Open4Cells(entrancePos.x, entrancePos.y);

        // 出口は4隅からランダム
        List<Vector2Int> corners = new List<Vector2Int>()
        {
            new Vector2Int(1, 1),
            new Vector2Int(cellsX - 2, 1),
            new Vector2Int(1, cellsY - 2),
            new Vector2Int(cellsX - 2, cellsY - 2)
        };

        var rnd = new System.Random();
        exitPos = corners[rnd.Next(corners.Count)];

        Open4Cells(exitPos.x, exitPos.y);
    }

    private void Open4Cells(int x, int y)
    {
        SetPathSafe(x, y);
        SetPathSafe(x + 1, y);
        SetPathSafe(x, y + 1);
        SetPathSafe(x + 1, y + 1);
    }

    private void SetPathSafe(int x, int y)
    {
        if (x >= 0 && y >= 0 && x < cellsX && y < cellsY)
            maze[x, y] = PATH;
    }

    // =========================================================
    // プレイヤー配置
    // =========================================================
    private void SpawnPlayer()
    {
        Vector3 pos = origin + new Vector3(
            entrancePos.x * cellSize + cellSize / 2f,
            1f,
            entrancePos.y * cellSize + cellSize / 2f
        );

        GameObject player = Instantiate(playerPrefab, pos, Quaternion.identity);

        // カメラ追従
        cameraManager.SetTarget(player.transform);

        // UI と強化メニューに Stats を渡す
        FindAnyObjectByType<StatusUI>().playerStats = player.GetComponent<PlayerStats>();
        FindAnyObjectByType<UpgradeMenuController>().stats = player.GetComponent<PlayerStats>();
    }

    // =========================================================
    // ゴール配置
    // =========================================================
    private void SpawnGoal()
    {
        if (goalPrefab == null)
        {
            Debug.LogWarning("goalPrefab が設定されていません");
            return;
        }

        Vector3 pos = origin + new Vector3(
            exitPos.x * cellSize + cellSize / 2f,
            0.5f,
            exitPos.y * cellSize + cellSize / 2f
        );
        Transform g = Instantiate(goalPrefab, pos, Quaternion.identity).transform;
        compassController.goal = g;
    }

    // =========================================================
    // コイン配置
    // =========================================================
    private void SpawnCoins()
    {
        // まず PATH の座標を全部集める
        List<Vector2Int> pathCells = new List<Vector2Int>();
        for (int x = 1; x < cellsX - 1; x++)
        {
            for (int y = 1; y < cellsY - 1; y++)
            {
                if (maze[x, y] == PATH)
                    pathCells.Add(new Vector2Int(x, y));
            }
        }

        var rnd = new System.Random();

        // PATH の中からランダムに選んでコイン生成
        for (int i = 0; i < coinCount; i++)
        {
            var p = pathCells[rnd.Next(pathCells.Count)];

            Vector3 pos = origin + new Vector3(
                p.x * cellSize + cellSize / 2f,
                1,
                p.y * cellSize + cellSize / 2f
            );

            Instantiate(coinPrefab, pos, coinPrefab.transform.rotation, coinParent);
        }
    }

    // =========================================================
    // 特殊アイテム配置
    // =========================================================
    private void SpawnSpecialItems()
    {
        var rnd = new System.Random();

        for (int i = 0; i < specialItemCount; i++)
        {
            while (true)
            {
                int x = rnd.Next(1, cellsX - 1);
                int y = rnd.Next(1, cellsY - 1);

                if (maze[x, y] == PATH)
                {
                    Vector3 pos = origin + new Vector3(
                        x * cellSize + cellSize / 2f,
                        0.5f,
                        y * cellSize + cellSize / 2f
                    );

                    Instantiate(specialItemPrefab, pos, specialItemPrefab.transform.rotation, coinParent);    
                    break;
                }
            }
        }
    }

    // =========================================================
    // コイン再生成
    // =========================================================
    public void RespawnCoins()
    {
        // 既存のコインを全部消す
        foreach (Transform child in coinParent)
            Destroy(child.gameObject);

        // 再生成
        SpawnCoins();
    }

    // =========================================================
    // BFSで入口→出口のルートを計算
    // =========================================================
    private List<Vector2Int> FindPath(Vector2Int start, Vector2Int goal)
    {
        Queue<Vector2Int> q = new Queue<Vector2Int>();
        Dictionary<Vector2Int, Vector2Int> parent = new Dictionary<Vector2Int, Vector2Int>();
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();

        q.Enqueue(start);
        visited.Add(start);

        Vector2Int[] dirs = {
            new Vector2Int(1,0),
            new Vector2Int(-1,0),
            new Vector2Int(0,1),
            new Vector2Int(0,-1)
        };  

        while (q.Count > 0)
        {
            var c = q.Dequeue();
            if (c == goal)
            {
                List<Vector2Int> path = new List<Vector2Int>();
                var cur = goal;

                while (cur != start)
                {
                    path.Add(cur);
                    cur = parent[cur];
                }
                path.Add(start);
                path.Reverse();
                return path;
            }

            foreach (var d in dirs)
            {
                var nx = c.x + d.x;
                var ny = c.y + d.y;

                if (nx < 0 || ny < 0 || nx >= cellsX || ny >= cellsY)
                    continue;

                if (maze[nx, ny] == PATH)
                {
                    var next = new Vector2Int(nx, ny);
                    if (!visited.Contains(next))
                    {
                        visited.Add(next);
                        parent[next] = c;
                        q.Enqueue(next);
                    }
                }
            }
        }

        return null;
    }

    // =========================================================
    // LineRendererで経路を描画
    // =========================================================
    private void DrawPathLine()
    {
        if (debugPath == null || lineRenderer == null) return;

        lineRenderer.positionCount = debugPath.Count;

        for (int i = 0; i < debugPath.Count; i++)
        {
            Vector2Int p = debugPath[i];
            Vector3 pos = origin + new Vector3(
                p.x * cellSize + cellSize / 2f,
                0.2f,
                p.y * cellSize + cellSize / 2f
            );

            lineRenderer.SetPosition(i, pos);
        }
    }

    // =========================================================
    // Sceneビューに赤い点でルート表示(デバッグ用)
    // =========================================================
    void OnDrawGizmos()
    {
        if (!debugShowPath || debugPath == null) return;

        Gizmos.color = Color.red;

        foreach (var p in debugPath)
        {
            Vector3 pos = origin + new Vector3(
                p.x * cellSize + cellSize / 2f,
                0.2f,
                p.y * cellSize + cellSize / 2f
            );

            Gizmos.DrawSphere(pos, 0.3f);
        }
    }

}
