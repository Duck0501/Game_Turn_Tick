using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class ButtonEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private float scaleFactor = 1.1f; // Tỷ lệ phóng to (110% kích thước ban đầu)
    [SerializeField] private float scaleDuration = 0.2f; // Thời gian chuyển đổi (giây)

    private Vector3 originalScale; // Kích thước ban đầu của button
    private RectTransform buttonTransform; // RectTransform của button

    void Start()
    {
        // Lấy RectTransform và lưu kích thước ban đầu
        buttonTransform = GetComponent<RectTransform>();
        originalScale = buttonTransform.localScale;
    }

    // Khi chuột di vào button
    public void OnPointerEnter(PointerEventData eventData)
    {
        // Phóng to button với hiệu ứng mượt mà
        buttonTransform.DOScale(originalScale * scaleFactor, scaleDuration).SetEase(Ease.OutQuad);
    }

    // Khi chuột rời khỏi button
    public void OnPointerExit(PointerEventData eventData)
    {
        // Trở lại kích thước ban đầu với hiệu ứng mượt mà
        buttonTransform.DOScale(originalScale, scaleDuration).SetEase(Ease.OutQuad);
    }
}