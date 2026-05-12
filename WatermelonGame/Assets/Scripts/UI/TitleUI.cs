using TMPro;
using UnityEngine;

public class TitleUI : MonoBehaviour
{
    [SerializeField]
    TMP_Text bestScoreText;

    void OnEnable()
    {
        if (bestScoreText)
            bestScoreText.text = $"Best Score {PlayerPrefs.GetInt("BestScore", 0):N0}";
    }

    public void OnStartButton() => GameManager.Instance?.StartGame();
}
