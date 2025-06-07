using UnityEngine;
using UnityEngine.UI;

public class WinScreen : MonoBehaviour
{
    [SerializeField] private Image scoreImage; // Hình ảnh hiển thị điểm trong chế độ Time
    [SerializeField] private Text scoreText;   // Văn bản hiển thị điểm
    [SerializeField] private Image highScoreImage; // Hình ảnh hiển thị điểm cao nhất trong chế độ Time
    [SerializeField] private Text highScoreText;   // Văn bản hiển thị điểm cao nhất

    void Start()
    {
        // Kiểm tra chế độ chơi
        bool isTimeMode = PlayerPrefs.GetInt("GameMode", 0) == 0;

        if (isTimeMode)
        {
            // Lấy level hiện tại từ PlayerPrefs
            string currentLevel = PlayerPrefs.GetString("LastLevel", "Level 1");

            // Hiển thị Image và Text trong chế độ Time
            if (scoreImage != null) scoreImage.gameObject.SetActive(true);
            if (scoreText != null)
            {
                scoreText.gameObject.SetActive(true);
                float score = PlayerPrefs.GetFloat("Score", 0f);
                scoreText.text = "Score: " + Mathf.CeilToInt(score).ToString();
            }

            // Hiển thị điểm cao nhất của level hiện tại
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
            // Ẩn Image và Text trong chế độ No Time
            if (scoreImage != null) scoreImage.gameObject.SetActive(false);
            if (scoreText != null) scoreText.gameObject.SetActive(false);
            if (highScoreImage != null) highScoreImage.gameObject.SetActive(false);
            if (highScoreText != null) highScoreText.gameObject.SetActive(false);
        }
    }
}