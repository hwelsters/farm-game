using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.IO;

public class SaveLoad : MonoBehaviour
{
    public static void Save <T> (T objectToSave, string key)
    {
        string path = Application.persistentDataPath + "/saves/";
        Directory.CreateDirectory(path);

        BinaryFormatter binaryFormatter = new BinaryFormatter();


        using (FileStream fileStream = new FileStream(path + key + ".txt", FileMode.Create))
        {
            binaryFormatter.Serialize(fileStream, objectToSave);
        }
    }

    public static T Load <T> (string key)
    {
        string path = Application.persistentDataPath + "/saves/";

        BinaryFormatter binaryFormatter = new BinaryFormatter();

        T returnValue = default(T);

        using (FileStream fileStream = new FileStream(path + key + ".txt", FileMode.Open))
        {
            returnValue = (T)binaryFormatter.Deserialize(fileStream);
        }

        return returnValue;
    }

    public static bool SaveExists (string key)
    {
        string path = Application.persistentDataPath + "/saves/" + key + ".txt";
        return File.Exists(path);
    }

    public static void SeriouslyDeleteAllFiles()
    {
        string path = Application.persistentDataPath + "/saves/";
        DirectoryInfo directory = new DirectoryInfo(path);
        directory.Delete();
        Directory.CreateDirectory(path);

    }
}
