using UnityEngine;

public class LevelLoader : MonoBehaviour
{
    [System.Serializable]
    public class LevelData
    {
        public int level;
        public string type;
        public int duration;
        public int layers;
        public int shelves;
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
