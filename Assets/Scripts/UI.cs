using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    public Button buttonPlay, buttonHelp, buttonToHome, buttonReset, buttonNext;
    public Button buttonTime, buttonNoTime;
    public List<Button> levelButtons;

    private void Start()
    {
        Screen.SetResolution(Display.main.systemWidth, Display.main.systemHeight, true);

        AssignBasicButton(buttonPlay, "Mode");
        AssignBasicButton(buttonHelp, "Help");
        AssignBasicButton(buttonToHome, "Home");

        AssignGameModeButton(buttonTime, 1);
        AssignGameModeButton(buttonNoTime, 0);

        AssignResetButton(buttonReset);
        AssignNextButton(buttonNext);

        AssignLevelButtons();
    }

    private void AssignBasicButton(Button button, string sceneName)
    {
        if (button != null)
            button.onClick.AddListener(() => SceneManager.LoadScene(sceneName));
    }

    private void AssignGameModeButton(Button button, int mode)
    {
        if (button != null)
        {
            button.onClick.AddListener(() =>
            {
                PlayerPrefs.SetInt("GameMode", mode);
                SceneManager.LoadScene("Level");
            });
        }
    }

    private void AssignResetButton(Button button)
    {
        if (button != null)
        {
            button.onClick.AddListener(() =>
            {
                string currentScene = SceneManager.GetActiveScene().name;

                if (currentScene == "Win" || currentScene == "Lose")
                {
                    string lastLevel = PlayerPrefs.GetString("LastLevel", "Level 1");
                    SceneManager.LoadScene(lastLevel);
                }
                else
                {
                    SceneManager.LoadScene(currentScene);
                }
            });
        }
    }

    private void AssignNextButton(Button button)
    {
        if (button != null)
        {
            button.onClick.AddListener(() =>
            {
                if (SceneManager.GetActiveScene().name != "Win") return;

                string lastLevel = PlayerPrefs.GetString("LastLevel", "Level 1");
                string nextLevel = GetNextLevel(lastLevel);

                SceneManager.LoadScene(nextLevel);
            });
        }
    }

    private void AssignLevelButtons()
    {
        for (int i = 0; i < levelButtons.Count; i++)
        {
            int levelIndex = i + 1;
            if (levelButtons[i] != null)
            {
                string levelName = $"Level {levelIndex}";
                levelButtons[i].onClick.AddListener(() => SceneManager.LoadScene(levelName));
            }
        }
    }

    private string GetNextLevel(string currentLevel)
    {
        if (currentLevel.StartsWith("Level "))
        {
            int levelNumber;
            if (int.TryParse(currentLevel.Substring(6), out levelNumber))
            {
                if (levelNumber >= 1 && levelNumber < 15)
                    return $"Level {levelNumber + 1}";
                else
                    return "Home";
            }
        }
        return "Level 1";
    }
}
