using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public class SerializableShipData
{
    public string HullName;
    public int ShipCost;
    public List<SerializableComponentData> Components;

    public static SerializableShipData LoadFromFile(string filename)
    {
        Debug.Log("Loading ship from file: " + filename);
        if (!Directory.Exists(ShipBuilderController.SAVE_FOLDER))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(filename));
        }
        if (!File.Exists(filename))
        {
            return null;
        }

        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(filename, FileMode.Open);

        SerializableShipData data = (SerializableShipData)formatter.Deserialize(stream);

        stream.Close();

        return data;
    }

    public void SaveToFile(string filename)
    {
        FileStream stream = new FileStream(filename+".ship", FileMode.OpenOrCreate);
        BinaryFormatter formatter = new BinaryFormatter();
        formatter.Serialize(stream, this);
        stream.Close();
    }
}

[Serializable]
public class SerializableComponentData
{
    public SerializableVector3 Position;
    public SerializableVector3 Rotation;
    public string ComponentName;
}

[Serializable]
public struct SerializableVector3
{
    public float x;
    public float y;
    public float z;

    public SerializableVector3(float rX, float rY, float rZ)
    {
        x = rX;
        y = rY;
        z = rZ;
    }

    public SerializableVector3(Vector3 rValue)
    {
        x = rValue.x;
        y = rValue.y;
        z = rValue.z;
    }

    public override string ToString()
    {
        return String.Format("[{0}, {1}, {2}]", x, y, z);
    }

    public static implicit operator Vector3(SerializableVector3 rValue)
    {
        return new Vector3(rValue.x, rValue.y, rValue.z);
    }

    public static implicit operator SerializableVector3(Vector3 rValue)
    {
        return new SerializableVector3(rValue.x, rValue.y, rValue.z);
    }
}