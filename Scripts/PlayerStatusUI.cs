using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// プレイヤーの状態（スタミナ・タイマー・コイン・マグネット）をUIに反映します。
/// </summary>
public class StatusUI : MonoBehaviour
{
    public PlayerStats playerStats; // プレイヤーのステータス

    [Header("スタミナUI")]
    [SerializeField] private Image staminaFill; // スタミナバーの画像

    [Header("タイマーUI")]
    [SerializeField] private TextMeshProUGUI timerText; // 経過時間の表示

    [Header("コインUI")]
    public TextMeshProUGUI coinText; // 所持コイン数の表示

    [Header("マグネットUI")]
    public Image magnetGauge; // マグネット発動・クールタイムゲージ

    void Update()
    {
        UpdateStaminaUI();
        UpdateTimerUI();
        UpdateCoinUI();
        UpdateMagnetUI();
    }
    /// <summary>
    /// スタミナバーの量と色を更新します。
    /// </summary>
    private void UpdateStaminaUI()
    {
        // スタミナを0〜1に正規化
        float normalized = playerStats.stamina / playerStats.maxStamina;
        staminaFill.fillAmount = normalized;

        // 色の定義
        Color green = new Color(0f, 1f, 0f);
        Color yellow = new Color(1f, 1f, 0f);
        Color red = new Color(1f, 0f, 0f);

        // スタミナ量に応じて色補間
        Color barColor;
        if (normalized > 0.5f)
        {
            float c = (normalized - 0.5f) / 0.5f;
            barColor = Color.Lerp(yellow, green, c);
        }
        else
        {
            float c = normalized / 0.5f;
            barColor = Color.Lerp(red, yellow, c);
        }

        staminaFill.color = barColor;
    }

    /// <summary>
    /// 経過時間を「00:00.00」形式で表示します。
    /// </summary>
    private void UpdateTimerUI()
    {
        float t = playerStats.timeElapsed;

        int minutes = Mathf.FloorToInt(t / 60f);
        float seconds = t % 60f;

        timerText.text = $"{minutes:00}:{seconds:00.00}";
    }

    /// <summary>
    /// 所持コイン数を表示します。
    /// </summary>
    private void UpdateCoinUI()
    {
        coinText.text = $"Coins: {playerStats.coins}";
    }

    /// <summary>
    /// マグネットの発動状態・クールタイムをゲージに反映します。
    /// </summary>
    private void UpdateMagnetUI()
    {
        // 未取得なら非表示
        if (!playerStats.hasMagnet)
        {
            magnetGauge.gameObject.SetActive(false);
            return;
        }

        magnetGauge.gameObject.SetActive(true);

        // 発動中 → 残り時間をゲージに反映
        if (playerStats.magnetActive)
        {
            magnetGauge.fillAmount = playerStats.magnetTimer / playerStats.magnetDuration;
            return;
        }

        // クールタイム中 → 回復ゲージを表示
        if (playerStats.magnetCooldownTimer > 0f)
        {
            magnetGauge.fillAmount = 1f - (playerStats.magnetCooldownTimer / playerStats.magnetCooldown);
            return;
        }

        //クールタイム後の時間の端数を消します
        magnetGauge.fillAmount = 1f; 

    }
}
