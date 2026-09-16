using UnityEngine;

/// <summary>
/// ヒントボタンの表示タイミング管理と、ルート表示のON/OFFを行います。
/// </summary>
public class HintController : MonoBehaviour
{
    public MazeDigGenerator maze;   // ルート表示(LineRenderer)を持つ迷路生成スクリプト
    public GameObject hintButton;   // ヒントボタンのUI
    public float hintTime = 900f;   // ヒントが解禁されるまでの時間（秒）

    private float playTime = 0f;    // プレイ時間のカウント
    private bool hintAvailable = false; // ヒントが解禁されたかどうか

    void Start()
    {
        // ゲーム開始時はヒントボタンを非表示
        if (hintButton != null)
            hintButton.SetActive(false);
    }

    void Update()
    {
        // プレイ時間を加算
        playTime += Time.deltaTime;

        // 一定時間経過でヒント解禁 
        if (!hintAvailable && playTime >= hintTime)
        {
            hintAvailable = true;
            hintButton.SetActive(true);
        }

        // Tabキーでルート表示のON/OFF
        if (hintAvailable && Input.GetKeyDown(KeyCode.Tab))
        {
            TogglePathLine();
        }
    }

    // ルート表示(LineRenderer)のON/OFF切り替え
    void TogglePathLine()
    {
        maze.lineRenderer.enabled = !maze.lineRenderer.enabled;
    }
}

