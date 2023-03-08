using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class GlobalGameStateManager : MonoBehaviour
{
    public static GlobalGameStateManager Instance { get; private set; }

    static int MAX_LEVEL = 8;
    public bool[] clearedLevels;

    public int curLevel = 0;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadData();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Called per frame
    void Update() {
        
    }

    public void LoadData()
    {
        string filePath = Application.persistentDataPath + "/clearedLevels.dat";
        if (File.Exists(filePath))
        {
            BinaryReader reader = new BinaryReader(File.Open(filePath, FileMode.Open));
            int length = reader.ReadInt32();
            clearedLevels = new bool[length];
            for (int i = 0; i < length; i++)
            {
                clearedLevels[i] = reader.ReadBoolean();
            }
            reader.Close();
        }
        else
        {
            clearedLevels = new bool[MAX_LEVEL];
        }
    }

    public void SaveData()
    {
        BinaryWriter writer = new BinaryWriter(File.Open(Application.persistentDataPath + "/clearedLevels.dat", FileMode.Create));
        writer.Write(clearedLevels.Length);
        for (int i = 0; i < clearedLevels.Length; i++)
        {
            writer.Write(clearedLevels[i]);
        }
        writer.Close();
    }
}
