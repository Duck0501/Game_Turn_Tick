using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class ButtonEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private float scaleFactor = 1.1f; 
    [SerializeField] private float scaleDuration = 0.2f; 

    private Vector3 originalScale; 
    private RectTransform buttonTransform; 

    void Start()
    {
        buttonTransform = GetComponent<RectTransform>();
        originalScale = buttonTransform.localScale;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        buttonTransform.DOScale(originalScale * scaleFactor, scaleDuration).SetEase(Ease.OutQuad);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        buttonTransform.DOScale(originalScale, scaleDuration).SetEase(Ease.OutQuad);
    }
}