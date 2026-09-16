using UnityEngine;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// 本編のRecordUIをタイトル画面用に簡略化した記録表示クラス。
/// タイトル画面でTabキーを押すと記録パネルを開閉
/// RecordManagerからトップ5の記録を読み込んで表示
/// <summary>
public class TitleRecordUI : MonoBehaviour
{
    [Header("記録パネル")]
    [SerializeField] private GameObject recordPanel;        // 記録パネル本体        
    [SerializeField] private TextMeshProUGUI[] recordTexts; // 記録表示用テキスト（5つ）

    private bool isOpen = false; // パネルが開いているかどうか

    void Start()
    {
        recordPanel.SetActive(false);

        // 記録を読み込む
        List<float> records = RecordManager.LoadRecords();

        // 記録をテキストに反映
        for (int i = 0; i < recordTexts.Length; i++)
        {
            if (i < records.Count)
            {
                float totalSec = records[i];

                int minutes = Mathf.FloorToInt(totalSec / 60f); // 分
                float seconds = totalSec % 60f;                 // 秒
                // 小数2桁で表示
                recordTexts[i].text = $"{i+1}. {minutes}m {seconds:00.00}s ";
            }    
            else
            {
                // 記録がない場合
                recordTexts[i].text = $"{i+1}. ---";
            }    
        }
    }

    void Update()
    {
        // Tabキーで表示／非表示を切り替え
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            isOpen = !isOpen;
            recordPanel.SetActive(isOpen);
        }
    }
}
