using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

/// <summary>
/// ゲームクリア時のUI表示とタイトルへの遷移を管理します。
/// </summary>
public class GameClearController : MonoBehaviour
{
    [SerializeField] private GameObject clearUI;            // クリア画面のパネル
    [SerializeField] private TextMeshProUGUI clearMessage;  // "STAGE CLEAR!" のテキスト
    [SerializeField] private TextMeshProUGUI clearTime;     // クリアタイム表示
    [SerializeField] private TextMeshProUGUI returnMessage; // タイトルへ戻る案内

    private bool isCleared = false;             // クリア状態かどうか
    public static GameClearController Instance; // シングルトン

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        clearUI.SetActive(false); // ゲーム開始時は非表示
    }

    /// <summary> 
    /// ゲームクリア時に呼び出され、UI表示と記録保存を行います。
    /// </summary>
    public void ShowClear(float time)
    {
        isCleared = true;

        // 記録を保存
        RecordManager.AddRecord(time);

        // 分と秒に変換
        int minutes = Mathf.FloorToInt(time / 60f);
        float seconds = time % 60f;

        clearUI.SetActive(true);
        clearMessage.text = "STAGE CLEAR!";
        clearTime.text = $"Time: {minutes}m {seconds:0.00}s";
        returnMessage.text = "Press SPACE to return to Title";

        Time.timeScale = 0f; // ゲーム停止
    }

    void Update()
    {
        if (!isCleared) return;

        // SPACEでタイトルへ戻る  
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Time.timeScale = 1f; // 再開
            SceneManager.LoadScene("TitleScene");
        }
    }
}
