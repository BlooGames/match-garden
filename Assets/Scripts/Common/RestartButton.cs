using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RestartButton : MonoBehaviour {

    RectTransform rectTransform;
    Button button; 
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        LeanTween.alpha(rectTransform, 0f, 0f);
        LeanTween.textAlpha(rectTransform, 0f, 0f);
        button = GetComponent<Button>();
    }

    public void SetVisible(bool isVisible)
    {
        if (isVisible != button.interactable)
        {
            button.interactable = isVisible;
            enabled = isVisible;
            LeanTween.alpha(rectTransform, isVisible ? 1f : 0f, 0.5f);
            LeanTween.textAlpha(rectTransform, isVisible ? 1f : 0f, 0.5f);
        }
    }
}
