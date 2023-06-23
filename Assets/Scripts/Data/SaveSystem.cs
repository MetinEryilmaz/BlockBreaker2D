using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem
{
    public static void SaveData()
    {
        GameData data = DataManager.Instance.data;

        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/BlockBreaker2D.metin";
        FileStream stream = new FileStream(path, FileMode.Create);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static void LoadData()
    {
        string path = Application.persistentDataPath + "/BlockBreaker2D.metin";

        if(File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            GameData data = formatter.Deserialize(stream) as GameData;

            DataManager.Instance.data = data;
        }
    }
}
