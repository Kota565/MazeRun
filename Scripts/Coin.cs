using UnityEngine;

/// <summary>
/// コインの回転・浮遊・吸引・取得処理を行います。
/// </summary>
public class Coin : MonoBehaviour
{
    [Header("見た目の動き")]
    [SerializeField] private float rotationSpeed = 180f;  // コインの回転速度
    [SerializeField] private float floatMoveSpeed = 1f;   // 浮遊の揺れる速さ
    [SerializeField] private float floatAmplitude = 0.2f; // 浮遊の揺れる高さ

    [Header("プレイヤー参照")]
    [SerializeField] private Transform playerTransform; // プレイヤーの Transform
    private PlayerStats playerStats;                    // プレイヤーのステータス

    private Vector3 basePosition;  // 浮遊の基準位置
    private float floatTime = 0f;  // 浮遊アニメーション用の時間
 
    
    void Start()
    {
        basePosition = transform.position;

        // プレイヤーをタグから探す
        GameObject p = GameObject.FindWithTag("Player");
        if (p != null)
        {
            playerTransform = p.transform;
            playerStats = p.GetComponent<PlayerStats>();
        }
    }

    void Update()
    {
        // 回転
        transform.Rotate(rotationSpeed * Time.deltaTime, 0f, 0f);

        // 浮遊アニメーション（Sin波で上下に揺れる)
        floatTime += Time.deltaTime * floatMoveSpeed;
        float y = Mathf.Sin(floatTime) * floatAmplitude;
        transform.position = basePosition + new Vector3(0, y, 0);

        if (playerTransform == null || playerStats == null) return;

        // マグネット発動中のみ吸引
        if (!playerStats.hasMagnet || !playerStats.magnetActive) return;

        float dist = Vector3.Distance(transform.position, playerTransform.position);
        if (dist < playerStats.magnetRange)
        {
            Vector3 dir = (playerTransform.position - transform.position).normalized;
            transform.position += dir * playerStats.magnetSpeed * Time.deltaTime;

            // 吸引で動いた位置を新しい基準位置にする
            basePosition = transform.position;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerStats stats = other.GetComponent<PlayerStats>();
        if (stats != null)
        {
            stats.coins += stats.coinValue;
            SEManager.Instance.PlaySE_Coin();
            Destroy(gameObject); 
        }
    }
    
}
