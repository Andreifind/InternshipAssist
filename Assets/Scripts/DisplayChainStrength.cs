using TMPro;
using UnityEngine;

public class DisplayChainStrength : MonoBehaviour
{
    public TextMeshProUGUI Text;

    public void ShowChainStrength(int strength)
    {
        Text.text = $"{strength}";
    }
}
