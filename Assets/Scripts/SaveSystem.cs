using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class SaveData
{
    public string sceneName;
    public float posX;
    public float posY;
}

public static class SaveSystem
{
    public const int SlotCount = 3;

    private static string SavePath(int slot)
    {
        return Path.Combine(Application.persistentDataPath, $"save{slot}.json");
    }

    public static Vector2? PendingSpawnPosition { get; private set; }

    public static bool HasSave(int slot)
    {
        return File.Exists(SavePath(slot));
    }

    public static bool HasAnySave()
    {
        for (int slot = 1; slot <= SlotCount; slot++)
        {
            if (HasSave(slot))
            {
                return true;
            }
        }

        return false;
    }

    public static void Save(int slot, string sceneName, Vector2 position)
    {
        SaveData data = new SaveData
        {
            sceneName = sceneName,
            posX = position.x,
            posY = position.y
        };

        File.WriteAllText(SavePath(slot), JsonUtility.ToJson(data));
    }

    public static SaveData Load(int slot)
    {
        if (!HasSave(slot))
        {
            return null;
        }

        return JsonUtility.FromJson<SaveData>(File.ReadAllText(SavePath(slot)));
    }

    public static void LoadIntoScene(int slot)
    {
        SaveData data = Load(slot);
        if (data == null)
        {
            return;
        }

        PendingSpawnPosition = new Vector2(data.posX, data.posY);
        SceneManager.LoadScene(data.sceneName);
    }

    public static Vector2? ConsumePendingSpawnPosition()
    {
        Vector2? position = PendingSpawnPosition;
        PendingSpawnPosition = null;
        return position;
    }
}
