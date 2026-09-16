using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// タイトル画面の演出とゲーム開始処理を管理するクラス。
/// </summary>
public class TitleManager : MonoBehaviour
{
    [Header("UI設定")]
    [SerializeField] private GameObject missionText; // ミッションテキスト
    [SerializeField] private float delay = 1f;       // 表示後にゲーム開始するまでの待ち時間

    private bool started = false; // すでに開始処理が走ったかどうか

    void Start()
    {
        missionText.SetActive(false);
    }

    void Update()
    {
        if (!started && Input.GetKeyDown(KeyCode.Space))
        {
            started = true;

            missionText.SetActive(true);

            // ゲーム開始処理へ
            StartCoroutine(StartGame());
        }
    }

    /// <summary>
    /// 少し待ってからゲームシーンへ移動するコルーチン。
    /// </summary>
    IEnumerator StartGame()
    {
        yield return new WaitForSeconds(delay);
        
        // ゲームシーンへ移動
        SceneManager.LoadScene("GameScene");
    }
}
