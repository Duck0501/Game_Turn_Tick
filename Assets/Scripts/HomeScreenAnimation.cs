using UnityEngine;
using DG.Tweening;
using System.Collections;

public class HomeScreenAnimation : MonoBehaviour
{
    [SerializeField] private RectTransform logo; // Gán logo từ Inspector
    [SerializeField] private RectTransform buttonPlay; // Gán button Play từ Inspector
    [SerializeField] private RectTransform buttonHelp; // Gán button Help từ Inspector

    private Vector2 logoInitialPos; // Lưu vị trí ban đầu của logo
    private Vector2 buttonPlayInitialPos; // Lưu vị trí ban đầu của button Play
    private Vector2 buttonHelpInitialPos; // Lưu vị trí ban đầu của button Help

    void Start()
    {
        // Lưu lại vị trí ban đầu của các đối tượng trước khi di chuyển
        if (logo != null)
        {
            logoInitialPos = logo.anchoredPosition;
            logo.anchoredPosition = new Vector2(logoInitialPos.x, 1000f); // Đặt ngoài màn hình (phía trên)
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

        // Bắt đầu hiệu ứng rơi
        StartCoroutine(AnimateElements());
    }

    IEnumerator AnimateElements()
    {
        // Hiệu ứng rơi cho logo
        if (logo != null)
        {
            logo.DOAnchorPosY(logoInitialPos.y, 0.7f).SetEase(Ease.OutBounce); // Rơi về vị trí ban đầu
        }

        yield return new WaitForSeconds(0.1f); // Đợi trước khi button Play rơi

        // Hiệu ứng rơi cho button Play
        if (buttonPlay != null)
        {
            buttonPlay.DOAnchorPosY(buttonPlayInitialPos.y, 0.5f).SetEase(Ease.OutBounce); // Rơi về vị trí ban đầu
        }

        yield return new WaitForSeconds(0.1f); // Đợi trước khi button Help rơi

        // Hiệu ứng rơi cho button Help
        if (buttonHelp != null)
        {
            buttonHelp.DOAnchorPosY(buttonHelpInitialPos.y, 0.5f).SetEase(Ease.OutBounce); // Rơi về vị trí ban đầu
        }
    }
}