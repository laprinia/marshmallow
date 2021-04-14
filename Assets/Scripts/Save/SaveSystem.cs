using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;


public static class SaveSystem
{
    private static string filePath = Application.persistentDataPath + "/playerData.txt";

    public static void SavePlayer(GameObject gameObject)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream fileStream = new FileStream(filePath, FileMode.Create);
        PlayerData data = new PlayerData(gameObject);
        formatter.Serialize(fileStream, data);
        fileStream.Close();
    }

    public static PlayerData LoadPlayer()
    {
        if (File.Exists(filePath))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream fileStream = new FileStream(filePath, FileMode.Open);
            PlayerData playerData = formatter.Deserialize(fileStream) as PlayerData;
            fileStream.Close();
            return playerData;
        }
        else
        {
            Debug.LogError("Save file not found at " + filePath);
            return null;
        }
    }
}