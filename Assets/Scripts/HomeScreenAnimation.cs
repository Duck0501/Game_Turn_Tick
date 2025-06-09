using UnityEngine;
using DG.Tweening;
using System.Collections;

public class HomeScreenAnimation : MonoBehaviour
{
    [SerializeField] private RectTransform logo, buttonPlay, buttonHelp; 

    private Vector2 logoInitialPos, buttonPlayInitialPos, buttonHelpInitialPos; 
    void Start()
    {
        if (logo != null)
        {
            logoInitialPos = logo.anchoredPosition;
            logo.anchoredPosition = new Vector2(logoInitialPos.x, 1000f); 
        }
        if (buttonPlay != null)
        {
            buttonPlayInitialPos = buttonPlay.anchoredPosition;
            buttonPlay.anchoredPosition = new Vector2(buttonPlayInitialPos.x, 1000f);
        }
        if (buttonHelp != null)
        {
            buttonHelpInitialPos = buttonHelp.anchoredPosition;
            buttonHelp.anchoredPosition = new Vector2(buttonHelpInitialPos.x, 1000f);
        }
        StartCoroutine(AnimateElements());
    }

    IEnumerator AnimateElements()
    {
        if (logo != null)
        {
            logo.DOAnchorPosY(logoInitialPos.y, 0.7f).SetEase(Ease.OutBounce); 
        }
        yield return new WaitForSeconds(0.1f);
        if (buttonPlay != null)
        {
            buttonPlay.DOAnchorPosY(buttonPlayInitialPos.y, 0.5f).SetEase(Ease.OutBounce);
        }
        yield return new WaitForSeconds(0.1f);
        if (buttonHelp != null)
        {
            buttonHelp.DOAnchorPosY(buttonHelpInitialPos.y, 0.5f).SetEase(Ease.OutBounce);
        }
    }
}