using TMPro;
using UnityEngine;

public class StarCounter : MonoBehaviour
{
    public TextMeshProUGUI Text;
    private int _stars = 0;

    public void AddStar()
    {
        _stars++;
        Text.text = $"{_stars}";
    }
}
