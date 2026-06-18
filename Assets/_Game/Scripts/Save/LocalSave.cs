// TOY WAR RUSH - LocalSave.cs
// Low-level local persistence helpers.

using UnityEngine;
using System.IO;

public static class LocalSave
{
    public static void WriteJson<T>(string fileName, T data)
    {
        string path = Path.Combine(Application.persistentDataPath, fileName);
        File.WriteAllText(path, JsonUtility.ToJson(data, true));
    }

    public static T ReadJson<T>(string fileName) where T : new()
    {
        string path = Path.Combine(Application.persistentDataPath, fileName);
        if (!File.Exists(path)) return new T();
        return JsonUtility.FromJson<T>(File.ReadAllText(path));
    }

    public static bool Exists(string fileName) =>
        File.Exists(Path.Combine(Application.persistentDataPath, fileName));
}
