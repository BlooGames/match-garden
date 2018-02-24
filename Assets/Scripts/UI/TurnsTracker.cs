using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnsTracker : MonoBehaviour {

    [SerializeField]
    private Text turns;
    [SerializeField]
    private Text turnsPopup;

    void Start () {
        turns.text = MatchingManager.Instance.RemainingTurns + "";
        MatchingManager.Instance.onTurnsChanged += OnTurnsChanged;
	}
	
	void OnTurnsChanged(int remainingTurns, int byAmount, bool specialEvent)
    {
        turns.text = remainingTurns + "";

        if (specialEvent)
        {
            LeanTween.alphaText(turnsPopup.rectTransform, 1f, 0.5f).setFrom(0f).setOnComplete(() => {
                LeanTween.alphaText(turnsPopup.rectTransform, 0f, 0.5f).setDelay(1f);
            });
            turnsPopup.text = (byAmount < 0 ? "" : "+") + byAmount;
        }
    }
}
