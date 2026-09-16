using UnityEngine;

/// <summary>
/// ゲーム内のSE（コイン取得・強化音）を管理するクラス。
/// シングルトン化して、どのスクリプトからでも簡単に再生できるようにしています。
/// </summary>
public class SEManager : MonoBehaviour
{
    [Header("SE AudioSource")]
    [SerializeField] private AudioSource se_Coin;    // コイン取得音
    [SerializeField] private AudioSource se_Upgrade; // 強化時の音

    public static SEManager Instance { get; private set; }

    // Startメソッドよりも早いタイミングで呼び出される
    private void Awake()
    {
        // シングルトンパターン
        // もしInstanceがまだ作られてないなら
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            // すでに存在する場合は自分を破棄（重複防止）
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// コイン取得音を再生します。
    /// </summary>
    public void PlaySE_Coin()
    {
        if (se_Coin != null)
        se_Coin.Play();
    }

    /// <summary>
    /// 強化時の音を再生します。
    /// </summary>
    public void PlaySE_Upgrade()
    {
        if (se_Upgrade != null)
        se_Upgrade.Play();
    }
    
}
