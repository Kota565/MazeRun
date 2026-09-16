using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

/// <summary>
/// 本編の UpgradeMenuController を整理した強化メニュー管理クラス。
/// ・タブ切り替え（通常 / 特殊 / タイトル）
/// ・強化項目の選択と実行
/// ・UI の更新（ステータス・コスト・色）
/// </summary>
public class UpgradeMenuController : MonoBehaviour
{
    // -------------------------
    // パネル
    // -------------------------
    [Header("Panels")]
    [SerializeField] private GameObject normalPanel;
    [SerializeField] private GameObject specialPanel;

    // -------------------------
    // タブ UI
    // -------------------------
    [Header("Tab UI")]
    [SerializeField] private RectTransform cursorTab;
    [SerializeField] private Button[] tabButtons; // 0:Normal, 1:Special, 2:Title
    private int tabIndex = 0;

    // -------------------------
    // 強化項目 UI
    // -------------------------
    [Header("Option UI")]
    [SerializeField] private RectTransform cursorOption;
    private Button[] currentOptions;
    private int optionIndex = 0;

    // -------------------------
    // メニュー
    // -------------------------
    [Header("Menu")]
    [SerializeField] private GameObject menuUI;
    public PlayerStats stats;
    public static bool isOpen = false;
    private bool selectingTab = true;

    // -------------------------
    // ステータス表示
    // -------------------------
    [Header("Status Texts")]
    [SerializeField] private TextMeshProUGUI speedText;
    [SerializeField] private TextMeshProUGUI staminaText;
    [SerializeField] private TextMeshProUGUI coinValueText;
    [SerializeField] TextMeshProUGUI menuCoinText;

    // -------------------------
    // 通常強化タイトル
    // -------------------------
    [Header("Option Titles")]
    [SerializeField] private TextMeshProUGUI optionSpeedTitle;
    [SerializeField] private TextMeshProUGUI optionCoinTitle;
    [SerializeField] private TextMeshProUGUI optionStaminaTitle;

    // -------------------------
    // 特殊強化タイトル
    // -------------------------
    [Header("Special Titles")]
    [SerializeField] private TextMeshProUGUI specialCompassTitle;
    [SerializeField] private TextMeshProUGUI specialMagnetTitle;

    // -------------------------
    // コスト表示
    // -------------------------
    [Header("Special Cost Texts")]
    [SerializeField] private TextMeshProUGUI specialCompassCostText;
    [SerializeField] private TextMeshProUGUI specialMagnetCostText;

    [Header("Option Cost Texts")]
    [SerializeField] private TextMeshProUGUI optionSpeedCostText;
    [SerializeField] private TextMeshProUGUI optionCoinCostText;
    [SerializeField] private TextMeshProUGUI optionStaminaCostText;

    void Start()
    {
        isOpen = false;
        menuUI.SetActive(false);
        SwitchTab(false); // 最初は通常タブ
        
    }

    void Update()
    {
        // メニュー開閉
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isOpen = !isOpen;
            menuUI.SetActive(isOpen);

            if (isOpen)
            {
                selectingTab = true;
                tabIndex = 0;
                cursorTab.position = tabButtons[tabIndex].transform.position;
                UpdateStatusUI();
            }
        }

        if (!isOpen) return;

        // -------------------------
        // タブ選択モード
        // -------------------------
        if (selectingTab)
        {
            if (Input.GetKeyDown(KeyCode.S)) SelectTab(-1);
            if (Input.GetKeyDown(KeyCode.W)) SelectTab(+1);

            if (Input.GetKeyDown(KeyCode.Space))
            {

                //タイトルタブ（tabIndex == 2）ならタイトルへ戻る
                if (tabIndex == 2)
                {
                    SceneManager.LoadScene("TitleScene");
                    return;
                }

                SwitchTab(tabIndex == 1);

                selectingTab = false;

                optionIndex = 0;
                
                MoveOptionCursor();
            }

            return;
        }

        // -------------------------
        // 強化項目選択モード
        // -------------------------
        if (Input.GetKeyDown(KeyCode.W)) MoveOption(-1);
        if (Input.GetKeyDown(KeyCode.S)) MoveOption(+1);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            ExecuteOption(optionIndex);
        }

        // A / D でタブ選択に戻る
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D))
        {
            selectingTab = true;
            cursorTab.position = tabButtons[tabIndex].transform.position;
        }
    }

    // -------------------------
    // タブ切り替え
    // -------------------------
    public void SwitchTab(bool special)
    {
    //タイトルタブならパネルを全部消す
    if (tabIndex == 2) 
    {
        normalPanel.SetActive(false);
        specialPanel.SetActive(false);
        currentOptions = new Button[0]; 
        return;
    }
        normalPanel.SetActive(!special);
        specialPanel.SetActive(special);

        Transform panel = special ? specialPanel.transform : normalPanel.transform;
        currentOptions = panel.GetComponentsInChildren<Button>();

        optionIndex = 0;
        MoveOptionCursor();
    }

    // -------------------------
    //タブカーソル移動
    // -------------------------
    void SelectTab(int dir)
    {
        tabIndex += dir;

        if (tabIndex < 0) tabIndex = tabButtons.Length - 1;
        if (tabIndex >= tabButtons.Length) tabIndex = 0;

        cursorTab.position = tabButtons[tabIndex].transform.position;
    }

    // -------------------------
    // 強化項目カーソル移動
    // -------------------------
    void MoveOption(int dir)
    {
        optionIndex += dir;

        if (optionIndex < 0) optionIndex = currentOptions.Length - 1;
        if (optionIndex >= currentOptions.Length) optionIndex = 0;

        MoveOptionCursor();
    }

    void MoveOptionCursor()
    {
        cursorOption.position = currentOptions[optionIndex].transform.position;
    }

    // -------------------------
    // 強化実行
    // -------------------------
    void ExecuteOption(int i)
    {
        // -------------------------
        // 通常強化
        // -------------------------
        if (normalPanel.activeSelf)
        {
            int cost = GetUpgradeCost(
                i == 0 ? stats.moveSpeedLevel :
                i == 1 ? stats.maxStaminaLevel :
                stats.coinValueLevel
            );


            if (stats.coins < cost)
            {
                return;
            }

            stats.coins -= cost;
            SEManager.Instance.PlaySE_Upgrade();

           switch (i)
           {
                case 0: // 通常スピード
                stats.moveSpeed += 0.5f;
                stats.moveSpeedLevel++;
                break;

                case 1: // 最大スタミナ
                stats.maxStaminaBonus += 1f;
                stats.maxStaminaLevel++;
                break;

                case 2: // コイン価値
                stats.coinValue += 1;
                stats.coinValueLevel++;
                break;
            }

            stats.RebalanceStats();
            UpdateStatusUI();
            return;
        }
        
        // -------------------------
        // 特殊強化（固定コスト）
        // -------------------------
        int specialCost = 30;

        if ((i == 0 && stats.hasCompass) ||
            (i == 1 && stats.hasMagnet))
        {
            return;
        }

        if (stats.coins < specialCost)
        {
            return;
        }

        stats.coins -= specialCost;
        SEManager.Instance.PlaySE_Upgrade();

        switch (i)
        {
            case 0: stats.hasCompass = true; break;
            case 1: stats.hasMagnet = true; break;
        }
        UpdateStatusUI();
    
    }

    // =========================================================
    // UI 更新
    // =========================================================
    void UpdateStatusUI()
    {
        // ステータス表示
        speedText.text = $"Speed: {stats.moveSpeed:F1} (Lv {stats.moveSpeedLevel})";
        coinValueText.text = $"Coin Value: {stats.coinValue:F1} (Lv {stats.coinValueLevel})";
        staminaText.text = $"Max Stamina: {stats.maxStamina:F1} (Lv {stats.maxStaminaLevel})";
        menuCoinText.text = $"Coins: {stats.coins:F1}";

        //コスト計算
        int speedCost   = GetUpgradeCost(stats.moveSpeedLevel);
        int coinCost    = GetUpgradeCost(stats.coinValueLevel);
        int staminaCost = GetUpgradeCost(stats.maxStaminaLevel);

        optionSpeedCostText.text   = $"Cost: {speedCost}";
        optionCoinCostText.text    = $"Cost: {coinCost}";
        optionStaminaCostText.text = $"Cost: {staminaCost}";

        //コスト不足なら赤、足りていれば白
        optionSpeedTitle.color   = (stats.coins < speedCost)   ? Color.red : Color.white;
        optionCoinTitle.color    = (stats.coins < coinCost)    ? Color.red : Color.white;
        optionStaminaTitle.color = (stats.coins < staminaCost) ? Color.red : Color.white;

        optionSpeedCostText.color   = (stats.coins < speedCost)   ? Color.red : Color.white;
        optionCoinCostText.color    = (stats.coins < coinCost)    ? Color.red : Color.white;
        optionStaminaCostText.color = (stats.coins < staminaCost) ? Color.red : Color.white;

        // スペシャルのコスト（固定）
        int specialCost = 30;

        // Compass
        if (stats.hasCompass)
        {
            specialCompassTitle.text = "Compass (Unlocked)";
            specialCompassTitle.color = Color.gray;
            specialCompassCostText.text = "";
        }
        else
        {
            specialCompassTitle.text = "Compass";
            specialCompassTitle.color = (stats.coins < specialCost) ? Color.red : Color.white;
            specialCompassCostText.text = $"Cost: {specialCost}";
            specialCompassCostText.color = (stats.coins < specialCost) ? Color.red : Color.white;
        }

        // Magnet
        if (stats.hasMagnet)
        {
            specialMagnetTitle.text = "Magnet (Unlocked)";
            specialMagnetTitle.color = Color.gray;
            specialMagnetCostText.text = "";
        }
        else
        {
            specialMagnetTitle.text = "Magnet";
            specialMagnetTitle.color = (stats.coins < specialCost) ? Color.red : Color.white;
            specialMagnetCostText.text = $"Cost: {specialCost}";
            specialMagnetCostText.color = (stats.coins < specialCost) ? Color.red : Color.white;
        }

    }

    // =========================================================
    // コスト計算（指数）
    // =========================================================
    int GetUpgradeCost(int level)
    {
        return Mathf.RoundToInt(Mathf.Pow(2, level));
    }
}
