using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreTracker : MonoBehaviour {

    [SerializeField]
    private Text score;
    [SerializeField]
    private Text scorePopup;

    void Start()
    {
        score.text = MatchingManager.Instance.Score + "";
        MatchingManager.Instance.onScoreChanged += OnScoreChanged;
    }

    void OnScoreChanged(int currentScore, int byAmount)
    {
        score.text = currentScore + "";

            LeanTween.alphaText(scorePopup.rectTransform, 1f, 0.5f).setFrom(0f).setOnComplete(() => {
                LeanTween.alphaText(scorePopup.rectTransform, 0f, 0.5f).setDelay(1f);
            });
            scorePopup.text = (byAmount < 0 ? "" : "+") + byAmount;
        
    }
}
