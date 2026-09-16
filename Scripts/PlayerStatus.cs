using UnityEngine;

/// <summary>
/// プレイヤーの能力値（移動・スタミナ・コイン・特殊スキル・強化）を管理します。
/// </summary>
public class PlayerStats : MonoBehaviour
{
    [Header("移動系")]
    [SerializeField] public float moveSpeed = 5.0f;        // 通常速度（強化対象）
    [HideInInspector] public float runSpeed = 8.0f;        // 走り速度（自動計算）
    [HideInInspector] public float tiredWalkSpeed = 3.0f;  // 疲労時の歩き速度（自動計算）

    [Header("スタミナ系")]
    [SerializeField] public float maxStamina = 5.0f;       // 基本最大スタミナ
    [SerializeField] public float maxStaminaBonus = 0f;    // 強化で増えるスタミナ量
    [HideInInspector] public float runStaminaCost = 1.0f;  // 走り時の消費（固定）
    [HideInInspector] public float staminaRecovery = 1.0f; // 回復速度（自動計算）
    [HideInInspector] public float stamina;                // 現在のスタミナ

    [Header("ゲーム進行")]
    public float timeElapsed = 0f; // 経過時間（ゴール判定で使用）
    public int coinValue = 1;   // コイン1枚の価値（強化対象）
    public int coins = 0;        // 所持コイン数

    [Header("特殊スキル")]
    public bool hasCompass = false; // コンパス取得済みか
    public bool hasMagnet = false;  // マグネット取得済みか

    public float magnetRange = 5f;  // 吸引範囲
    public float magnetSpeed = 10f; // 吸引速度
 
    public float magnetDuration = 10f; // 発動時間
    public float magnetCooldown = 5f;  // クールタイム

    public bool magnetActive = false;      // 今発動中か
    public float magnetTimer = 0f;         // 発動時間の残り
    public float magnetCooldownTimer = 0f; // クールタイム残り

    [Header("強化レベル")]
    public int moveSpeedLevel = 0;  // 移動速度の強化レベル
    public int coinValueLevel = 0;  // コイン価値の強化レベル
    public int maxStaminaLevel = 0; // 最大スタミナの強化レベル
    
    void Awake()
    {
        // ゲーム開始時はスタミナを最大値にする
        stamina = maxStamina;
    }

    /// <summary>
    /// 強化内容に応じて各ステータスを再計算
    /// </summary>
    public void RebalanceStats()
    {
        // 走り速度は通常速度の1.4倍
        runSpeed = moveSpeed * 1.4f;

        // 疲れ歩き速度は通常速度の60%
        tiredWalkSpeed = moveSpeed * 0.6f;

        // 最大スタミナ（基本5 + ボーナス）
        maxStamina = 5.0f + maxStaminaBonus;

        // 回復速度は最大スタミナに応じて上昇
        staminaRecovery = 1.0f + (maxStaminaBonus * 0.15f);

        // スタミナ消費は固定
        runStaminaCost = 1.0f;
    }
}
