using UnityEngine;
using DG.Tweening;
using System.Collections;

public class LevelStartAnimation : MonoBehaviour
{
    [SerializeField] private RectTransform panel; // Gán panel (bảng đen) từ Inspector
    [SerializeField] private RectTransform[] childObjects; // Gán các game object con (phép toán, đồng hồ,...) từ Inspector

    private Vector2 panelInitialPos; // Lưu vị trí ban đầu của panel
    private Vector2[] childInitialPos; // Lưu vị trí ban đầu của các game object con

    void Start()
    {
        // Lưu vị trí ban đầu của panel
        if (panel != null)
        {
            panelInitialPos = panel.anchoredPosition;
            panel.anchoredPosition = new Vector2(panelInitialPos.x, panelInitialPos.y + 1000f); // Đặt ngoài màn hình (phía trên)
        }

        // Lưu vị trí ban đầu của các game object con
        if (childObjects != null && childObjects.Length > 0)
        {
            childInitialPos = new Vector2[childObjects.Length];
            for (int i = 0; i < childObjects.Length; i++)
            {
                if (childObjects[i] != null)
                {
                    childInitialPos[i] = childObjects[i].anchoredPosition;
                    childObjects[i].anchoredPosition = new Vector2(childInitialPos[i].x, childInitialPos[i].y + 1000f); // Đặt ngoài màn hình (phía trên)
                }
            }
        }

        // Bắt đầu hiệu ứng rơi
        StartCoroutine(AnimateEntry());
    }

    IEnumerator AnimateEntry()
    {
        // Hiệu ứng rơi cho panel
        if (panel != null)
        {
            panel.DOAnchorPosY(panelInitialPos.y, 1f).SetEase(Ease.OutBounce); // Rơi về vị trí ban đầu
        }

        // Hiệu ứng rơi cho các game object con
        if (childObjects != null && childObjects.Length > 0)
        {
            for (int i = 0; i < childObjects.Length; i++)
            {
                if (childObjects[i] != null)
                {
                    childObjects[i].DOAnchorPosY(childInitialPos[i].y, 1f).SetEase(Ease.OutBounce); // Rơi về vị trí ban đầu
                }
            }
        }

        yield return null;
    }
}