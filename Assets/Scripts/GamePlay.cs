using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine.UI;

public class GamePlay : MonoBehaviour
{
    [SerializeField] private Transform blackBar; // Thanh đen
    [SerializeField] private Transform greenBar; // Thanh xanh (mục tiêu)
    [SerializeField] private Transform pivot1;   // Chấm 1
    [SerializeField] private Transform pivot2;   // Chấm 2
    [SerializeField] private Transform[] circles; // Danh sách các hình tròn
    [SerializeField] private GameObject[] bells;  // Danh sách các chuông
    [SerializeField] private Sprite defaultSprite; // Sprite mặc định cho chuông
    [SerializeField] private Sprite changedSprite; // Sprite khi đi qua chuông
    [SerializeField] private Sprite defaultCircleSprite; // Sprite mặc định cho hình tròn đặc biệt
    [SerializeField] private Sprite specialCircleSprite; // Sprite đặc biệt cho hình tròn
    [SerializeField] private AudioSource rotateSound; // Âm thanh khi quay
    [SerializeField] private float rotateSpeed = 100f; // Tốc độ xoay (độ/giây)
    [SerializeField] private float stopThreshold = 0.012f; // Ngưỡng để coi là "giữa hình tròn hoặc chuông"
    [SerializeField] private float stopThresholds = 1f;
    [SerializeField] private float angleTolerance = 1f; // Sai số góc để kiểm tra trùng khớp
    [SerializeField] private float positionTolerance = 0.1f;
    [SerializeField] private float bellPassDelay = 1f; // Thời gian tối thiểu giữa các lần đổi sprite (1 giây)
    [SerializeField] private float trailFadeDuration = 0.5f; // Thời gian fade ra của trail khi dừng
    [SerializeField] private SpriteRenderer blackBarSprite; // SpriteRenderer của thanh đen
    [SerializeField] private Color afterimageColor = new Color(1f, 1f, 1f, 0.5f); // Màu bóng mờ
    [SerializeField] private Image timerImage; // Hình ảnh hiển thị trong chế độ Time
    [SerializeField] private Text timerText; // Văn bản hiển thị thời gian đếm ngược
    [SerializeField] private float timeLimit = 30f; // Thời gian đếm ngược (giây)
    [SerializeField] private List<GameObject> specialCircles; // Danh sách các GameObject hình tròn đặc biệt
    [SerializeField] private List<bool> specialCircleInitialStates; // Trạng thái sprite ban đầu cho hình tròn đặc biệt
    [SerializeField] private List<bool> bellInitialStates; // Trạng thái sprite ban đầu cho chuông (true: changedSprite, false: defaultSprite)

    private Transform currentPivot; // Trọng tâm hiện tại
    private bool isRotating = false;
    private int bellPassCount = 0; // Đếm số lần đi qua chuông (giữ lại nhưng không dùng để đổi sprite)
    private float lastBellPassTime = -1f; // Thời điểm lần đi qua chuông cuối cùng
    private bool gameEnded = false;
    private Queue<GameObject> afterImagePool = new Queue<GameObject>(); // Pool cho bóng mờ
    private int poolSize = 50; // Kích thước pool
    private bool isTimeMode = false; // Chế độ Time (true) hoặc No Time (false)
    private float timeRemaining; // Thời gian còn lại trong chế độ Time
    private bool wasRotatingLastFrame = false; // Trạng thái xoay của khung trước
    private Dictionary<GameObject, bool> circleSpriteStates = new Dictionary<GameObject, bool>(); // Trạng thái sprite của hình tròn đặc biệt
    private Dictionary<GameObject, bool> bellSpriteStates = new Dictionary<GameObject, bool>(); // Trạng thái sprite của chuông

    void Start()
    {
        // Kiểm tra chế độ chơi
        isTimeMode = PlayerPrefs.GetInt("GameMode", 0) == 0;

        // Khởi tạo thời gian đếm ngược
        if (isTimeMode)
        {
            timeRemaining = timeLimit;
            if (timerImage != null) timerImage.gameObject.SetActive(true);
            if (timerText != null) timerText.gameObject.SetActive(true);
        }
        else
        {
            if (timerImage != null) timerImage.gameObject.SetActive(false);
            if (timerText != null) timerText.gameObject.SetActive(false);
        }

        // Đặt âm thanh quay lặp liên tục
        if (rotateSound != null)
        {
            rotateSound.loop = true; // Lặp âm thanh khi quay
        }

        // Khởi tạo pool cho bóng mờ
        for (int i = 0; i < poolSize; i++)
        {
            GameObject afterImage = new GameObject("AfterImage");
            afterImage.SetActive(false);
            SpriteRenderer sr = afterImage.AddComponent<SpriteRenderer>();
            sr.sprite = blackBarSprite.sprite;
            sr.sortingOrder = blackBarSprite.sortingOrder - 1;
            afterImagePool.Enqueue(afterImage);
        }

        // Khởi tạo trạng thái sprite cho các hình tròn đặc biệt
        for (int i = 0; i < specialCircles.Count; i++)
        {
            GameObject specialCircle = specialCircles[i];
            if (specialCircle != null)
            {
                SpriteRenderer circleSprite = specialCircle.GetComponent<SpriteRenderer>();
                if (circleSprite != null)
                {
                    // Lấy trạng thái ban đầu từ specialCircleInitialStates (nếu có), mặc định là false
                    bool initialState = i < specialCircleInitialStates.Count ? specialCircleInitialStates[i] : false;
                    circleSprite.sprite = initialState ? specialCircleSprite : defaultCircleSprite;
                    circleSpriteStates[specialCircle] = initialState; // Lưu trạng thái ban đầu
                }
            }
        }

        // Khởi tạo trạng thái sprite cho các chuông
        for (int i = 0; i < bells.Length; i++)
        {
            GameObject bell = bells[i];
            if (bell != null)
            {
                SpriteRenderer bellSprite = bell.GetComponent<SpriteRenderer>();
                if (bellSprite != null)
                {
                    // Lấy trạng thái ban đầu từ bellInitialStates (nếu có), mặc định là false
                    bool initialState = i < bellInitialStates.Count ? bellInitialStates[i] : false;
                    bellSprite.sprite = initialState ? changedSprite : defaultSprite;
                    bellSpriteStates[bell] = initialState; // Lưu trạng thái ban đầu
                }
            }
        }
    }

    void Update()
    {
        if (gameEnded) return;

        // Cập nhật thời gian đếm ngược trong chế độ Time
        if (isTimeMode)
        {
            timeRemaining -= Time.deltaTime;
            if (timerText != null)
            {
                timerText.text = Mathf.CeilToInt(timeRemaining).ToString();
            }
            if (timeRemaining <= 0)
            {
                gameEnded = true;
                PlayerPrefs.SetString("LastLevel", SceneManager.GetActiveScene().name);
                SceneManager.LoadScene("Lose");
                return;
            }
        }

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
                    if (rotateSound != null) rotateSound.Play();
                }
            }
        }

        // Kiểm tra khi bắt đầu xoay để đổi sprite các hình tròn đặc biệt
        if (isRotating && !wasRotatingLastFrame)
        {
            foreach (GameObject specialCircle in specialCircles)
            {
                if (specialCircle != null)
                {
                    float distanceToPivot = Vector2.Distance(currentPivot.position, specialCircle.transform.position);
                    if (distanceToPivot >= stopThreshold)
                    {
                        UpdateSpecialCircleSprite(specialCircle);
                    }
                }
            }
        }

        // Xoay thanh đen theo chiều kim đồng hồ
        if (isRotating && currentPivot != null)
        {
            float rotation = rotateSpeed * Time.deltaTime;
            blackBar.RotateAround(currentPivot.position, Vector3.forward, -rotation); // Dấu "-" để xoay theo chiều kim đồng hồ

            // Tạo bóng mờ
            CreateAfterImage();

            // Kiểm tra nếu chấm không phải trọng tâm nằm giữa một trong các hình tròn thì dừng
            CheckCircleAlignment();

            // Kiểm tra nếu chấm đi qua chuông
            CheckBellPass();
        }
        else
        {
            // Dừng âm thanh quay khi không xoay
            if (rotateSound != null && rotateSound.isPlaying)
            {
                rotateSound.Stop();
            }
        }

        // Cập nhật trạng thái xoay cho khung tiếp theo
        wasRotatingLastFrame = isRotating;

        // Kiểm tra xem thanh đen có trùng với thanh xanh không
        CheckWinCondition();
    }

    // Tạo hiệu ứng bóng mờ
    private void CreateAfterImage()
    {
        if (afterImagePool != null && afterImagePool.Count > 0)
        {
            GameObject afterImage = afterImagePool.Dequeue();
            afterImage.SetActive(true);
            afterImage.transform.position = blackBar.position;
            afterImage.transform.rotation = blackBar.rotation;
            afterImage.transform.localScale = blackBar.localScale;
            SpriteRenderer sr = afterImage.GetComponent<SpriteRenderer>();
            sr.color = afterimageColor;

            // Fade bóng mờ và đưa lại vào pool
            sr.DOFade(0f, trailFadeDuration).OnComplete(() =>
            {
                afterImage.SetActive(false);
                afterImagePool.Enqueue(afterImage);
            });
        }
    }

    // Kiểm tra nếu chấm nằm giữa một trong các hình tròn
    private bool IsPivotInCircle(Transform pivot)
    {
        for (int i = 0; i < circles.Length; i++)
        {
            Transform circle = circles[i];
            float distance = Vector2.Distance(pivot.position, circle.position);
            if (distance < stopThreshold)
            {
                GameObject circleObj = circle.gameObject;
                if (specialCircles.Contains(circleObj))
                {
                    SpriteRenderer circleSprite = circleObj.GetComponent<SpriteRenderer>();
                    if (circleSprite != null && circleSprite.sprite != defaultCircleSprite)
                    {
                        return false; // Không cho phép dừng nếu sprite không phải mặc định
                    }
                }
                return true;
            }
        }
        return false;
    }

    // Kiểm tra nếu chấm không phải trọng tâm nằm giữa một trong các hình tròn thì dừng
    private void CheckCircleAlignment()
    {
        for (int i = 0; i < circles.Length; i++)
        {
            Transform circle = circles[i];
            float distanceToCircle1 = Vector2.Distance(pivot1.position, circle.position);
            float distanceToCircle2 = Vector2.Distance(pivot2.position, circle.position);

            // Nếu chấm 1 không phải trọng tâm và nằm giữa hình tròn
            if (currentPivot != pivot1 && distanceToCircle1 < stopThreshold)
            {
                GameObject circleObj = circle.gameObject;
                if (specialCircles.Contains(circleObj))
                {
                    SpriteRenderer circleSprite = circleObj.GetComponent<SpriteRenderer>();
                    if (circleSprite != null && circleSprite.sprite != defaultCircleSprite)
                    {
                        continue; // Bỏ qua hình tròn đặc biệt nếu sprite không phải mặc định
                    }
                }
                isRotating = false;
                currentPivot = pivot1; // Chuyển trọng tâm sang chấm 1
                break;
            }
            // Nếu chấm 2 không phải trọng tâm và nằm giữa hình tròn
            else if (currentPivot != pivot2 && distanceToCircle2 < stopThreshold)
            {
                GameObject circleObj = circle.gameObject;
                if (specialCircles.Contains(circleObj))
                {
                    SpriteRenderer circleSprite = circleObj.GetComponent<SpriteRenderer>();
                    if (circleSprite != null && circleSprite.sprite != defaultCircleSprite)
                    {
                        continue; // Bỏ qua hình tròn đặc biệt nếu sprite không phải mặc định
                    }
                }
                isRotating = false;
                currentPivot = pivot2; // Chuyển trọng tâm sang chấm 2
                break;
            }
        }
    }

    // Cập nhật sprite hình tròn đặc biệt
    private void UpdateSpecialCircleSprite(GameObject specialCircle)
    {
        if (specialCircle == null) return;

        SpriteRenderer circleSprite = specialCircle.GetComponent<SpriteRenderer>();
        if (circleSprite != null)
        {
            bool currentState = circleSpriteStates.ContainsKey(specialCircle) ? circleSpriteStates[specialCircle] : false;
            circleSpriteStates[specialCircle] = !currentState; // Toggle trạng thái
            circleSprite.sprite = circleSpriteStates[specialCircle] ? specialCircleSprite : defaultCircleSprite;
        }
    }

    // Kiểm tra nếu chấm đi qua chuông
    private void CheckBellPass()
    {
        if (bells.Length == 0) return; // Không có chuông trong scene, thoát sớm

        // Kiểm tra nếu đã đủ thời gian từ lần đổi sprite trước
        if (Time.time - lastBellPassTime < bellPassDelay) return;

        foreach (GameObject bell in bells)
        {
            if (bell == null) continue; // Bỏ qua nếu chuông không tồn tại

            float distanceToPivot1 = Vector2.Distance(pivot1.position, bell.transform.position);
            float distanceToPivot2 = Vector2.Distance(pivot2.position, bell.transform.position);

            // Nếu chấm 1 hoặc chấm 2 đi qua chuông
            if (distanceToPivot1 < stopThresholds || distanceToPivot2 < stopThresholds)
            {
                bellPassCount++; // Tăng số lần đi qua (giữ lại nhưng không dùng để đổi sprite)
                lastBellPassTime = Time.time; // Cập nhật thời điểm
                SpriteRenderer bellSprite = bell.GetComponent<SpriteRenderer>();
                if (bellSprite != null)
                {
                    // Toggle trạng thái sprite của chuông này
                    bool currentState = bellSpriteStates.ContainsKey(bell) ? bellSpriteStates[bell] : false;
                    bellSpriteStates[bell] = !currentState;
                    bellSprite.sprite = bellSpriteStates[bell] ? changedSprite : defaultSprite;
                }
                break; // Thoát vòng lặp sau khi xử lý một chuông
            }
        }
    }

    // Kiểm tra điều kiện thắng
    private void CheckWinCondition()
    {
        // Kiểm tra góc
        float angleDifference = Mathf.DeltaAngle(blackBar.rotation.eulerAngles.z, greenBar.rotation.eulerAngles.z);
        bool isAngleMatched = Mathf.Abs(angleDifference) < angleTolerance;

        // Kiểm tra vị trí
        float positionDifference = Vector3.Distance(blackBar.position, greenBar.position);
        bool isPositionMatched = positionDifference < positionTolerance;

        // Nếu scene có chuông, cần tất cả chuông ở trạng thái changedSprite để thắng
        if (bells.Length > 0)
        {
            bool allBellsChanged = true;
            foreach (GameObject bell in bells)
            {
                if (bell != null)
                {
                    SpriteRenderer bellSprite = bell.GetComponent<SpriteRenderer>();
                    if (bellSprite != null && bellSprite.sprite != changedSprite)
                    {
                        allBellsChanged = false;
                        break;
                    }
                }
            }
            if (isAngleMatched && isPositionMatched && allBellsChanged)
            {
                gameEnded = true;
                string currentLevel = SceneManager.GetActiveScene().name;
                PlayerPrefs.SetString("LastLevel", currentLevel);
                if (isTimeMode)
                {
                    PlayerPrefs.SetFloat("Score", timeRemaining); // Lưu thời gian còn lại làm điểm
                    string highScoreKey = "HighScore_" + currentLevel;
                    float highScore = PlayerPrefs.GetFloat(highScoreKey, 0f);
                    if (timeRemaining > highScore)
                    {
                        PlayerPrefs.SetFloat(highScoreKey, timeRemaining); // Cập nhật điểm cao nhất cho level
                    }
                }
                SceneManager.LoadScene("Win");
            }
        }
        // Nếu scene không có chuông, chỉ cần góc và vị trí khớp
        else
        {
            if (isAngleMatched && isPositionMatched)
            {
                gameEnded = true;
                string currentLevel = SceneManager.GetActiveScene().name;
                PlayerPrefs.SetString("LastLevel", currentLevel);
                if (isTimeMode)
                {
                    PlayerPrefs.SetFloat("Score", timeRemaining); // Lưu thời gian còn lại làm điểm
                    string highScoreKey = "HighScore_" + currentLevel;
                    float highScore = PlayerPrefs.GetFloat(highScoreKey, 0f);
                    if (timeRemaining > highScore)
                    {
                        PlayerPrefs.SetFloat(highScoreKey, timeRemaining); // Cập nhật điểm cao nhất cho level
                    }
                }
                SceneManager.LoadScene("Win");
            }
        }
    }
}