using UnityEngine;

public class LevelLoader : MonoBehaviour
{
    [System.Serializable]
    public class Shelf
    {
        public int position;
        public int slots;
    }

    [System.Serializable]
    public class LevelData
    {
        public int level;
        public string type;
        public int duration;
        public int layers;
        public Lock[] locks;
        public int items;
        public Shelf[] shelves;
    }

    [System.Serializable]
    public class Lock
    {
        public int turns;
        public string status;
    }

    public LevelData LoadLevel(int levelNumber)
    {
        string path = $"Levels/level{levelNumber}";
        TextAsset jsonFile = Resources.Load<TextAsset>(path);

        if (jsonFile == null)
        {
            Debug.LogError($"Level {levelNumber} not found!");
            return null;
        }

        LevelData levelData = JsonUtility.FromJson<LevelData>(jsonFile.text);
        return levelData;
    }
}
