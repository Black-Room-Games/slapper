using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SaveSystem
{
    static string fileName = "/appp.yepi";
    static string path = Application.persistentDataPath + fileName;

    public static void SavePlayer(Player player)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);

        PlayerData data = new PlayerData(player);
        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static PlayerData LoadPlayer()
    {
        if(File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            PlayerData data = formatter.Deserialize(stream) as PlayerData;
            stream.Close();
            Debug.Log("Data loaded at: " + path);
            return data;
        }
        else //return default values
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Create);

            PlayerData data = new PlayerData();
            formatter.Serialize(stream, data);
            stream.Close();
            Debug.Log("Data created at: " + path);
            return data;
        }
    }

    public static void DeletePlayer()
    {
        if (File.Exists(Application.persistentDataPath + fileName))
        {
            File.Delete(Application.persistentDataPath + fileName);
            Debug.Log("Data deleted at: " + path);
        }
    }
}