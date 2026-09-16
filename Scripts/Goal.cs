using UnityEngine;

/// <summary>
/// プレイヤーがゴールに触れたときにクリア処理を呼び出します。
/// </summary>
public class Goal : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // プレイヤーが触れたか判定
        if (other.CompareTag("Player"))
        {
            // プレイヤーのステータス取得
            PlayerStats stats = other.GetComponent<PlayerStats>();

            // 経過時間をクリアタイムとして取得
            float clearTime = stats.timeElapsed;

            // クリア処理を呼び出す
            GameClearController.Instance.ShowClear(clearTime);
        }
    }  

}
