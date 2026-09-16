using UnityEngine;

/// <summary>
/// 特殊アイテムがプレイヤーに付与する効果をまとめたクラス。
/// ランダムで 1 つの効果を発動します。
/// </summary>
public static class SpecialItemEffects
{
    /// <summary>
    /// ランダムで特殊効果を 1 つ発動する。
    /// </summary>
    public static void ApplyRandomEffect(PlayerStats stats)
    {
        int r = Random.Range(0, 3);

        switch (r)
        {
            case 0:
                // コイン再生成（迷路全体のコインをリセット）
                MazeDigGenerator.Instance.RespawnCoins();
                Debug.Log("特殊アイテム：コイン再生成");
                break;

            case 1:
                // スタミナ全回復
                stats.stamina = stats.maxStamina;
                Debug.Log("特殊アイテム：スタミナ全回復");
                break;

            case 2:
                // スピードアップ
                stats.moveSpeed += 2f;
                // PlayerStats 内の RebalanceStats() を 5秒後に呼び出す 
                stats.Invoke("RebalanceStats", 5f); 

                Debug.Log("特殊アイテム：スピードアップ");
                break;
        }
    }
}
