using UnityEngine;

/// <summary>
/// 特殊アイテムの見た目（回転・浮遊）と、プレイヤー接触時の効果発動を管理するクラス。
/// </summary>
public class SpecialItem : MonoBehaviour
{
    [Header("見た目の動き設定")]
    [SerializeField] private float rotateSpeed = 180f; // 1秒あたりの回転速度（Y軸）
    [SerializeField] private float floatSpeed = 1f;    // 浮遊の揺れ速度
    [SerializeField] private float floatHeight = 0.2f; // 浮遊の高さ（振れ幅）

    private float floatTimer = 0f; // 浮遊アニメーション用の内部タイマー
    private Vector3 basePos;       // 浮遊の基準位置（初期位置）

    void Start()
    {
        // 初期位置を記録（浮遊の中心）
        basePos = transform.position;
    }

    void Update()
    {
        // 回転
        transform.Rotate(0f, rotateSpeed * Time.deltaTime, 0f);

        // 浮遊
        floatTimer += Time.deltaTime * floatSpeed;
        float y = Mathf.Sin(floatTimer) * floatHeight;

        transform.position = basePos + new Vector3(0, y, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerStats stats = other.GetComponent<PlayerStats>();

        if (stats != null)
        {
            // ランダム効果発動
            SpecialItemEffects.ApplyRandomEffect(stats);

            Destroy(gameObject);
        }
    }
}
