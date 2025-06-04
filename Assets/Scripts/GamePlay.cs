using UnityEngine;
using UnityEngine.SceneManagement;

public class GamePlay : MonoBehaviour
{
    [SerializeField] private Transform blackBar; // Thanh đen
    [SerializeField] private Transform greenBar; // Thanh xanh (mục tiêu)
    [SerializeField] private Transform pivot1;   // Chấm 1
    [SerializeField] private Transform pivot2;   // Chấm 2
    [SerializeField] private Transform[] circles; // Danh sách các hình tròn
    [SerializeField] private float rotateSpeed = 90f; // Tốc độ xoay (độ/giây)
    [SerializeField] private float stopThreshold = 0.012f; // Ngưỡng để coi là "giữa hình tròn"
    [SerializeField] private float angleTolerance = 1f; // Sai số góc để kiểm tra trùng khớp
    [SerializeField] private float positionTolerance = 0.1f;

    private Transform currentPivot; // Trọng tâm hiện tại
    private bool isRotating = false;

    void Update()
    {
        // Phát hiện chuột nhấn vào chấm
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.GetRayIntersection(ray);

            if (hit.collider != null)
            {
                Transform clickedPivot = null;
                if (hit.collider.transform == pivot1)
                {
                    clickedPivot = pivot1;
                }
                else if (hit.collider.transform == pivot2)
                {
                    clickedPivot = pivot2;
                }

                // Kiểm tra nếu chấm được nhấn nằm giữa hình tròn thì đặt làm trọng tâm và xoay
                if (clickedPivot != null && IsPivotInCircle(clickedPivot))
                {
                    currentPivot = clickedPivot;
                    isRotating = true;
                }
            }
        }

        // Xoay thanh đen theo chiều kim đồng hồ
        if (isRotating && currentPivot != null)
        {
            float rotationAmount = rotateSpeed * Time.deltaTime;
            blackBar.RotateAround(currentPivot.position, Vector3.forward, -rotationAmount); // Dấu "-" để xoay theo chiều kim đồng hồ

            // Kiểm tra nếu chấm không phải trọng tâm nằm giữa một trong các hình tròn thì dừng
            CheckCircleAlignment();
        }

        // Kiểm tra xem thanh đen có trùng với thanh xanh không
        CheckWinCondition();
    }

    // Kiểm tra nếu chấm nằm giữa một trong các hình tròn
    private bool IsPivotInCircle(Transform pivot)
    {
        foreach (Transform circle in circles)
        {
            float distance = Vector2.Distance(pivot.position, circle.position);
            if (distance < stopThreshold)
            {
                return true;
            }
        }
        return false;
    }

    // Kiểm tra nếu chấm không phải trọng tâm nằm giữa một trong các hình tròn thì dừng
    private void CheckCircleAlignment()
    {
        foreach (Transform circle in circles) // Duyệt qua từng hình tròn trong danh sách
        {
            float distanceToCircle1 = Vector2.Distance(pivot1.position, circle.position);
            float distanceToCircle2 = Vector2.Distance(pivot2.position, circle.position);

            // Nếu chấm 1 không phải trọng tâm và nằm giữa hình tròn
            if (currentPivot != pivot1 && distanceToCircle1 < stopThreshold)
            {
                isRotating = false;
                currentPivot = pivot1; // Chuyển trọng tâm sang chấm 1
                break; // Thoát vòng lặp khi đã dừng
            }
            // Nếu chấm 2 không phải trọng tâm và nằm giữa hình tròn
            else if (currentPivot != pivot2 && distanceToCircle2 < stopThreshold)
            {
                isRotating = false;
                currentPivot = pivot2; // Chuyển trọng tâm sang chấm 2
                break; // Thoát vòng lặp khi đã dừng
            }
        }
    }

    // Kiểm tra điều kiện thắng
    private void CheckWinCondition()
    {
        // Kiểm tra góc
        float angleDifference = Mathf.Abs(blackBar.rotation.eulerAngles.z - greenBar.rotation.eulerAngles.z);
        bool isAngleMatched = (angleDifference < angleTolerance || Mathf.Abs(angleDifference - 360) < angleTolerance);

        // Kiểm tra vị trí
        float positionDifference = Vector2.Distance(blackBar.position, greenBar.position);
        bool isPositionMatched = positionDifference < positionTolerance;

        // Nếu cả góc và vị trí đều khớp, chuyển sang scene Win
        if (isAngleMatched && isPositionMatched)
        {
            SceneManager.LoadScene("Win");
        }
    }
}