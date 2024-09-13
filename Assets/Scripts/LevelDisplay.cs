using TMPro;
using UnityEngine;

public class LevelDisplay : MonoBehaviour
{
    public TextMeshProUGUI Text;

    public void UpdateLevelDisplay(int level)
    {
        Text.text = $"Lv. {level+1}";
    }
}
