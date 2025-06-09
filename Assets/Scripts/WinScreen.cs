using UnityEngine;
using UnityEngine.UI;

public class WinScreen : MonoBehaviour
{
    [SerializeField] private Image scoreImage, highScoreImage; 
    [SerializeField] private Text scoreText, highScoreText;  

    void Start()
    {
        bool isTimeMode = PlayerPrefs.GetInt("GameMode", 0) == 0;

        if (isTimeMode)
        {
            string currentLevel = PlayerPrefs.GetString("LastLevel", "Level 1");

            if (scoreImage != null) scoreImage.gameObject.SetActive(true);
            if (scoreText != null)
            {
                scoreText.gameObject.SetActive(true);
                float score = PlayerPrefs.GetFloat("Score", 0f);
                scoreText.text = "Score: " + Mathf.CeilToInt(score).ToString();
            }

            if (highScoreImage != null) highScoreImage.gameObject.SetActive(true);
            if (highScoreText != null)
            {
                highScoreText.gameObject.SetActive(true);
                string highScoreKey = "HighScore_" + currentLevel;
                float highScore = PlayerPrefs.GetFloat(highScoreKey, 0f);
                highScoreText.text = "High Score: " + Mathf.CeilToInt(highScore).ToString();
            }
        }
        else
        {
            if (scoreImage != null) scoreImage.gameObject.SetActive(false);
            if (scoreText != null) scoreText.gameObject.SetActive(false);
            if (highScoreImage != null) highScoreImage.gameObject.SetActive(false);
            if (highScoreText != null) highScoreText.gameObject.SetActive(false);
        }
    }
}