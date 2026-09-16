using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// プレイヤーの記録（タイム）を PlayerPrefs に保存・読み込みする管理クラス。
/// 上位5件までを保持し、常に速い順に並べます。
/// </summary>
public static class RecordManager
{
    // 保存する記録数の上限
    private const int MaxRecords = 5;

    /// <summary>
    /// 新しい記録（タイム）を追加し、上位5件だけを保存します。
    /// </summary>
    public static void AddRecord(float time)
    {
        // 既存の記録を読み込み
        List<float> records = LoadRecords();

        // 新しい記録を追加
        records.Add(time);

        // 小さい順にソート（速いタイムが上）
        records.Sort();

        // 上位5つだけ残す
        if (records.Count > MaxRecords)
            records = records.GetRange(0, MaxRecords);

        // 保存
        SaveRecords(records);
    }
     
    /// <summary>
    /// PlayerPrefs から記録を読み込み、存在するものだけ返します。
    /// </summary>
    public static List<float> LoadRecords()
    {
        List<float> records = new List<float>();

        for (int i = 0; i < MaxRecords; i++)
        {
            // 記録がない場合は -1 が返る
            float saved = PlayerPrefs.GetFloat("Record" + i, -1f);

            // 有効な記録だけ追加
            if (saved >= 0f)
            {
                records.Add(saved);
            }    
        }
 
        return records;
    }

    /// <summary>
    /// 記録リストを PlayerPrefs に保存します。
    /// 記録が足りない場合は -1 を入れて「空」を表現します。
    /// </summary>
    private static void SaveRecords(List<float> records)
    {
        for (int i = 0; i < MaxRecords; i++)
        {
            if (i < records.Count)
                // 記録がある場合はその値を保存
                PlayerPrefs.SetFloat("Record" + i, records[i]);
            else
                // 記録がない場合は -1 を保存（空のスロット）
                PlayerPrefs.SetFloat("Record" + i, -1f);
        }

        // PlayerPrefs に書き込みを確定
        PlayerPrefs.Save();
    }
}
