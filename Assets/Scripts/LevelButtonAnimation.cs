using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;

public class LevelButtonAnimation : MonoBehaviour
{
    [SerializeField] private Button[] levelButtons; // Gán mảng các button từ buttonLevel1 đến buttonLevel15

    void Start()
    {
        // Bắt đầu hiệu ứng lượn sóng
        StartCoroutine(AnimateWaveEffect());
    }

    IEnumerator AnimateWaveEffect()
    {
        for (int i = 0; i < levelButtons.Length; i++)
        {
            if (levelButtons[i] != null)
            {
                // Lưu vị trí ban đầu
                Vector2 initialPosition = levelButtons[i].transform.position;

                // Tính độ trễ dựa trên chỉ số trong mảng (tạo hiệu ứng sóng)
                float delay = i * 0.05f;

                // Tạo chuỗi chuyển động lượn sóng (lặp lại 2 lần)
                Sequence waveSequence = DOTween.Sequence();
                waveSequence.SetDelay(delay);
                waveSequence.Append(levelButtons[i].transform.DOMoveY(initialPosition.y + 70f, 0.3f).SetEase(Ease.OutQuad))
                            .Append(levelButtons[i].transform.DOMoveY(initialPosition.y, 0.3f).SetEase(Ease.InQuad))
                            .OnComplete(() =>
                            {
                                // Đảm bảo trở lại vị trí ban đầu sau khi kết thúc
                                levelButtons[i].transform.DOMoveY(initialPosition.y, 0.1f);
                            });

                yield return null; // Chờ frame tiếp theo để xử lý button tiếp theo
            }
        }
    }
}