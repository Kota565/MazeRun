using UnityEngine;

/// <summary>
/// UI を点滅させるためのコンポーネント。
/// CanvasGroupのalphaを時間で変化させてフェード点滅を作る。
/// </summary>
public class BlinkUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup; // 点滅させたい UI の透明度を操作する
    [SerializeField] private float blinkspeed = 2f;   // 点滅の速さ

    void Update()
    {
        // Sin波を 0〜1 に変換して透明度として使う
        canvasGroup.alpha = (Mathf.Sin(Time.time * blinkspeed) + 1f) / 2f;
    }
}
