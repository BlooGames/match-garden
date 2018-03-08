using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelTracker : MonoBehaviour {

    [SerializeField]
    private Text levelDescription;
    [SerializeField]
    private Image progressBar;
    private MatchingLevel lastLevel;

	void Start () {
        MatchingManager.Instance.onLevelProgressChanged += OnLevelProgressChanged;
	}

    private void OnLevelProgressChanged(MatchingLevel currentLevel, int currentLevelProgress, int scoreToNextLevel)
    {
        UpdateLevelInfo(currentLevel, (float) currentLevelProgress / (float) scoreToNextLevel);
    }

    private void UpdateLevelInfo(MatchingLevel currentLevel, float fillAmount)
    {
        string levelDescriptionString = "Level " + currentLevel.LevelId + ": " + currentLevel.LevelName;
        if (!lastLevel)
        {
            levelDescription.text = levelDescriptionString;
        }

        if (lastLevel && currentLevel != lastLevel)
        {
            LeanTween.value(progressBar.gameObject, (float value, float ratio) => 
            {
                progressBar.fillAmount = value;
            }, progressBar.fillAmount, 1f, 0.5f)
            .setOnComplete(() => {
                levelDescription.text = levelDescriptionString;
                AudioManager.Instance.PlaySound("LevelUp");
                LeanTween.value(progressBar.gameObject, (float value, float ratio) =>
                {
                    progressBar.fillAmount = value;
                }, 0f, fillAmount, 0.5f).setDelay(0.2f);
            });
        }
        else
        {
            LeanTween.value(progressBar.gameObject, (float value, float ratio) =>
            {
                progressBar.fillAmount = value;
            }, progressBar.fillAmount, fillAmount, 0.5f);
        }
        lastLevel = currentLevel;
    }
}
