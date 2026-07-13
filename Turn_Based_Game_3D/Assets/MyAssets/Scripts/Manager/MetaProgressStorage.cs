using System;
using System.IO;
using UnityEngine;

namespace Assets.MyAssets.Scripts.Manager
{

/// <summary>
/// MetaProgressData를 Application.persistentDataPath에 JSON 파일로 저장/로드하는 헬퍼.
/// </summary>
public static class MetaProgressStorage
{
    private const string FileName = "meta_progress.json";

    private static string FilePath => Path.Combine(Application.persistentDataPath, FileName);

    public static MetaProgressData Load()
    {
        if (!File.Exists(FilePath)) return new MetaProgressData();

        try
        {
            string json = File.ReadAllText(FilePath);
            return JsonUtility.FromJson<MetaProgressData>(json) ?? new MetaProgressData();
        }
        catch (Exception ex)
        {
            Debug.LogError($"MetaProgressData 로드 실패, 기본값으로 대체: {ex.Message}");
            return new MetaProgressData();
        }
    }

    public static void Save(MetaProgressData data)
    {
        File.WriteAllText(FilePath, JsonUtility.ToJson(data, true));
    }
}

}
