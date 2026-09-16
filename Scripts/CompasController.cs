using UnityEngine;

/// <summary>
/// ゴール方向を指すコンパスUIを制御します。
/// </summary>
public class CompassController : MonoBehaviour
{
    [SerializeField] private RectTransform arrowUI; // UIの矢印
    public Transform goal; // ゴールの位置

    private PlayerStats playerStats; // プレイヤーのステータス     
    private Transform player;  // プレイヤーのtransform

    void Update()
    {
        // Playerがまだ見つかっていない場合は探す
        if (player == null)
        {
            GameObject p = GameObject.FindWithTag("Player");
            if (p != null)
            {
                player = p.transform;
                playerStats = p.GetComponent<PlayerStats>();
            }

            // 見つかるまでは矢印を非表示
            arrowUI.gameObject.SetActive(true);
            return;
        }

        // ★ コンパス未取得なら非表示
        if (!playerStats.hasCompass)
        {
            arrowUI.gameObject.SetActive(false);
            return;
        } 

        // コンパス取得済みなら表示
        arrowUI.gameObject.SetActive(true);

        // ゴール方向を計算
        Vector3 dir = goal.position - player.position;
        float angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;

        // UI矢印を回転
        arrowUI.rotation = Quaternion.Euler(0, 0, -angle);
        
    }
}
