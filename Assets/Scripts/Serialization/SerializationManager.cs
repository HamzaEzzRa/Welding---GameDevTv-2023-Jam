using UnityEngine;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

public class DataFile
{
    private bool useEncryption;
    private string encryptionKey;

    public DataFile(bool useEncryption, string encryptionKey)
    {
        this.useEncryption = useEncryption;
        this.encryptionKey = encryptionKey;
    }

    public void Save(GameData data, string dataPath, string filename)
    {
        string fullPath = Path.Combine(dataPath, filename);
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
            string dataJSON = JsonUtility.ToJson(data, true);
            if (useEncryption)
            {
                dataJSON = XorCrypt(dataJSON);
            }

            using (FileStream fs = new FileStream(fullPath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(fs))
                {
                    writer.Write(dataJSON);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Saving Error: " + fullPath + "\n" + e);
        }
    }

    public GameData Load(string dataPath, string filename)
    {
        GameData loadedData = null;
        string fullPath = Path.Combine(dataPath, filename);
        
        if (File.Exists(fullPath))
        {
            try
            {
                string dataJSON;
                using (FileStream fs = new FileStream(fullPath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(fs))
                    {
                        dataJSON = reader.ReadToEnd();
                    }
                }
                if (useEncryption)
                {
                    dataJSON = XorCrypt(dataJSON);
                }

                loadedData = JsonUtility.FromJson<GameData>(dataJSON);
            }
            catch (Exception e)
            {
                Debug.LogError("Loading Error: " + fullPath + "\n" + e);
            }
        }

        return loadedData;
    }

    private string XorCrypt(string data)
    {
        string modifiedData = "";
        for (int i = 0; i < data.Length; i++)
        {
            modifiedData += (char)(data[i] ^ encryptionKey[i % encryptionKey.Length]);
        }

        return modifiedData;
    }
}

public class SerializationManager : MonoBehaviour
{
    public static bool IsSaving = false;

    private static bool useEncryption = false;
    private static readonly string encryptionKey = "k+qE4aUy4xDc2r?M4AZVVg#N";

    private static GameData gameData = new GameData();
    private static List<IPersistentObject> persistentObjects = new List<IPersistentObject>();

    private const string DEFAULT_FILENAME = "autosave.json";

    private static DataFile dataFileJSON = new DataFile(useEncryption, encryptionKey);

    public static bool HasSaves(string dataPath="")
    {
        if (dataPath == null || dataPath == "")
        {
            dataPath = Path.Combine(Application.persistentDataPath, "Saves");
        }

        if (!Directory.Exists(dataPath))
        {
            return false;
        }

        return Directory.EnumerateFileSystemEntries(dataPath).Any();
    }

    public static void SaveGame(string filename=DEFAULT_FILENAME, string dataPath="")
    {
        foreach (IPersistentObject persistent in persistentObjects)
        {
            persistent.SaveData(gameData);
        }

        if (dataPath == null || dataPath == "")
        {
            dataPath = Path.Combine(Application.persistentDataPath, "Saves");
        }

        //Debug.Log(
        //    "========== Save ==========\nOrder: " + gameData.currentLevelOrder +
        //    "\nPlayer Position: " + gameData.playerPosition
        //);

        IsSaving = true;
        dataFileJSON.Save(gameData, dataPath, filename);
        IsSaving = false;
    }

    public static void LoadGame(string filename=DEFAULT_FILENAME, string dataPath="")
    {
        if (dataPath == null || dataPath == "")
        {
            dataPath = Path.Combine(Application.persistentDataPath, "Saves");
        }

        gameData = dataFileJSON.Load(dataPath, filename);
        //Debug.Log(
        //    "========== Load ==========\nOrder: " + gameData.currentLevelOrder +
        //    "\nPlayer Position: " + gameData.playerPosition
        //);

        foreach (IPersistentObject persistent in persistentObjects)
        {
            persistent.LoadData(gameData);
        }
    }

    public static void DestroySave(string filename = DEFAULT_FILENAME, string dataPath = "")
    {
        if (dataPath == null || dataPath == "")
        {
            dataPath = Path.Combine(Application.persistentDataPath, "Saves");
        }

        if (HasSaves(dataPath))
        {
            string fullPath = Path.Combine(dataPath, filename);
            File.Delete(fullPath);
        }
    }

    // Call within OnEnable of each IPersistentObject
    public static void AddPersistentObject(IPersistentObject persistent)
    {
        persistentObjects.Add(persistent);
    }

    // Call within OnDisable of each IPersistentObject
    public static void RemovePersistentObject(IPersistentObject persistent)
    {
        persistentObjects.Remove(persistent);
    }
}
