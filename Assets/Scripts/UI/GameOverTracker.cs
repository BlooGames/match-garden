using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverTracker : MonoBehaviour {

    [SerializeField]
    private CanvasGroup panel;
    [SerializeField]
    private Text scoreText;
    [SerializeField]
    private Text levelText;
    [SerializeField]
    private Text highLevelText;
    [SerializeField]
    private Text highScoreText;

	void Start ()
    {
        MatchingManager.Instance.onEndGame += OnEndGame;
	}
	
    private void OnEndGame(int score, int level, int highScore, int highLevel, bool beatHighScore)
    {
        Debug.Log("ENDED");
        panel.gameObject.SetActive(true);
        LeanTween.alphaCanvas(panel, 1f, 0.5f).setFrom(0f);
        AudioManager.Instance.PlaySound("GameOver");
        scoreText.text = score + "";
        levelText.text = "Level " + level;
        highScoreText.text = highScore + "";
        highLevelText.text = "Level " + highLevel;
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
