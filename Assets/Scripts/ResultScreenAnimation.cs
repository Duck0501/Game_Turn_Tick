using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ResultScreenAnimation : MonoBehaviour
{
    [SerializeField] private RectTransform panel, img, img1;
    [SerializeField] private Button buttonHome, buttonPlay, buttonReset;

    private Vector2 panelInitialPos, imgInitialPos, imgInitialPos1, buttonHomeInitialPos, buttonPlayInitialPos, buttonResetInitialPos; 

    void Start()
    {
        if (panel != null)
        {
            panelInitialPos = panel.anchoredPosition;
            panel.anchoredPosition = new Vector2(panelInitialPos.x + 1000f, panelInitialPos.y); 
        }
        if (img != null)
        {
            imgInitialPos = img.anchoredPosition;
            img.anchoredPosition = new Vector2(imgInitialPos.x + 1000f, imgInitialPos.y);
        }
        if (img1 != null)
        {
            imgInitialPos1 = img1.anchoredPosition;
            img1.anchoredPosition = new Vector2(imgInitialPos1.x + 1000f, imgInitialPos1.y);
        }
        if (buttonHome != null)
        {
            buttonHomeInitialPos = buttonHome.GetComponent<RectTransform>().anchoredPosition;
            buttonHome.GetComponent<RectTransform>().anchoredPosition = new Vector2(buttonHomeInitialPos.x + 1000f, buttonHomeInitialPos.y);
        }
        if (buttonPlay != null)
        {
            buttonPlayInitialPos = buttonPlay.GetComponent<RectTransform>().anchoredPosition;
            buttonPlay.GetComponent<RectTransform>().anchoredPosition = new Vector2(buttonPlayInitialPos.x + 1000f, buttonPlayInitialPos.y);
        }
        if (buttonReset != null)
        {
            buttonResetInitialPos = buttonReset.GetComponent<RectTransform>().anchoredPosition;
            buttonReset.GetComponent<RectTransform>().anchoredPosition = new Vector2(buttonResetInitialPos.x + 1000f, buttonResetInitialPos.y);
        }
        AnimateEntry();
    }

    void AnimateEntry()
    {
        if (panel != null)
        {
            panel.DOAnchorPosX(panelInitialPos.x, 0.5f).SetEase(Ease.OutQuad);
        }
        if (img != null)
        {
            img.DOAnchorPosX(imgInitialPos.x, 0.5f).SetEase(Ease.OutQuad);
        }
        if (img1 != null)
        {
            img1.DOAnchorPosX(imgInitialPos1.x, 0.5f).SetEase(Ease.OutQuad);
        }
        if (buttonHome != null)
        {
            buttonHome.GetComponent<RectTransform>().DOAnchorPosX(buttonHomeInitialPos.x, 0.5f).SetEase(Ease.OutQuad);
        }
        if (buttonPlay != null)
        {
            buttonPlay.GetComponent<RectTransform>().DOAnchorPosX(buttonPlayInitialPos.x, 0.5f).SetEase(Ease.OutQuad);
        }
        if (buttonReset != null)
        {
            buttonReset.GetComponent<RectTransform>().DOAnchorPosX(buttonResetInitialPos.x, 0.5f).SetEase(Ease.OutQuad);
        }
    }
}