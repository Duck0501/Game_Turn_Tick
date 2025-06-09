using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine.UI;

public class GamePlay : MonoBehaviour
{
    [Header("Bars and Pivots")]
    [SerializeField] private Transform blackBar, greenBar, pivot1, pivot2;

    [Header("Gameplay Elements")]
    [SerializeField] private Transform[] circles;
    [SerializeField] private GameObject[] bells;
    [SerializeField] private List<GameObject> specialCircles;

    [Header("Sprites")]
    [SerializeField] private Sprite defaultSprite, changedSprite;
    [SerializeField] private Sprite defaultCircleSprite, specialCircleSprite;

    [Header("Audio")]
    [SerializeField] private AudioSource rotateSound, bellSound;

    [Header("UI")]
    [SerializeField] private Image timerImage;
    [SerializeField] private Text timerText;

    [Header("Timing & Settings")]
    [SerializeField] private float timeLimit = 30f;
    [SerializeField] private float rotateSpeed = 120f;
    [SerializeField] private float stopThreshold = 0.012f;
    [SerializeField] private float stopThresholds = 1f;
    [SerializeField] private float angleTolerance = 1f;
    [SerializeField] private float positionTolerance = 0.1f;
    [SerializeField] private float bellPassDelay = 1f;
    [SerializeField] private float trailFadeDuration = 0.5f;

    [Header("Visual Effects")]
    [SerializeField] private SpriteRenderer blackBarSprite;
    [SerializeField] private Color afterimageColor = new Color(1f, 1f, 1f, 0.5f);
    [SerializeField] private int poolSize = 50;

    [Header("Initial States")]
    [SerializeField] private List<bool> specialCircleInitialStates;
    [SerializeField] private List<bool> bellInitialStates;

    private Transform currentPivot;
    private bool isRotating = false, wasRotatingLastFrame = false, gameEnded = false;
    private int bellPassCount = 0;
    private float lastBellPassTime = -1f, timeRemaining;
    private bool isTimeMode = false;

    private Queue<GameObject> afterImagePool = new Queue<GameObject>();
    private Dictionary<GameObject, bool> circleSpriteStates = new Dictionary<GameObject, bool>();
    private Dictionary<GameObject, bool> bellSpriteStates = new Dictionary<GameObject, bool>();

    void Start()
    {
        SetupGameMode();
        InitializeAfterImagePool();
        InitializeSpecialCircles();
        InitializeBells();
    }

    void Update()
    {
        if (gameEnded) return;

        HandleTimer();
        HandleMouseInput();
        HandleRotation();
        CheckWinCondition();
    }

    private void SetupGameMode()
    {
        isTimeMode = PlayerPrefs.GetInt("GameMode", 0) == 0;
        timeRemaining = timeLimit;
        timerImage?.gameObject.SetActive(isTimeMode);
        timerText?.gameObject.SetActive(isTimeMode);

        if (rotateSound != null) rotateSound.loop = true;
    }

    private void InitializeAfterImagePool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject afterImage = new GameObject("AfterImage");
            afterImage.SetActive(false);
            var sr = afterImage.AddComponent<SpriteRenderer>();
            sr.sprite = blackBarSprite.sprite;
            sr.sortingOrder = blackBarSprite.sortingOrder - 1;
            afterImagePool.Enqueue(afterImage);
        }
    }

    private void InitializeSpecialCircles()
    {
        for (int i = 0; i < specialCircles.Count; i++)
        {
            var circle = specialCircles[i];
            if (!circle) continue;

            var sr = circle.GetComponent<SpriteRenderer>();
            if (!sr) continue;

            bool initialState = i < specialCircleInitialStates.Count && specialCircleInitialStates[i];
            sr.sprite = initialState ? specialCircleSprite : defaultCircleSprite;
            circleSpriteStates[circle] = initialState;
        }
    }

    private void InitializeBells()
    {
        for (int i = 0; i < bells.Length; i++)
        {
            var bell = bells[i];
            if (!bell) continue;

            var sr = bell.GetComponent<SpriteRenderer>();
            if (!sr) continue;

            bool initialState = i < bellInitialStates.Count && bellInitialStates[i];
            sr.sprite = initialState ? changedSprite : defaultSprite;
            bellSpriteStates[bell] = initialState;
        }
    }

    private void HandleTimer()
    {
        if (!isTimeMode) return;

        timeRemaining -= Time.deltaTime;
        timerText.text = Mathf.CeilToInt(timeRemaining).ToString();

        if (timeRemaining <= 0)
        {
            gameEnded = true;
            PlayerPrefs.SetString("LastLevel", SceneManager.GetActiveScene().name);
            SceneManager.LoadScene("Lose");
        }
    }

    private void HandleMouseInput()
    {
        if (!Input.GetMouseButtonDown(0)) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.GetRayIntersection(ray);
        if (!hit.collider) return;

        Transform clicked = hit.collider.transform;
        if (clicked == pivot1 || clicked == pivot2)
        {
            if (IsPivotInCircle(clicked))
            {
                currentPivot = clicked;
                isRotating = true;
                rotateSound?.Play();
            }
        }
    }

    private void HandleRotation()
    {
        if (isRotating && !wasRotatingLastFrame)
        {
            foreach (var circle in specialCircles)
                if (circle && Vector2.Distance(currentPivot.position, circle.transform.position) >= stopThreshold)
                    UpdateSpecialCircleSprite(circle);
        }

        if (isRotating && currentPivot != null)
        {
            blackBar.RotateAround(currentPivot.position, Vector3.forward, -rotateSpeed * Time.deltaTime);
            CreateAfterImage();
            CheckCircleAlignment();
            CheckBellPass();
        }
        else if (rotateSound?.isPlaying == true)
        {
            rotateSound.Stop();
        }

        wasRotatingLastFrame = isRotating;
    }

    private void CreateAfterImage()
    {
        if (afterImagePool.Count == 0) return;

        GameObject afterImage = afterImagePool.Dequeue();
        afterImage.SetActive(true);
        afterImage.transform.SetPositionAndRotation(blackBar.position, blackBar.rotation);
        afterImage.transform.localScale = blackBar.localScale;
        var sr = afterImage.GetComponent<SpriteRenderer>();
        sr.color = afterimageColor;

        sr.DOFade(0f, trailFadeDuration).OnComplete(() => {
            afterImage.SetActive(false);
            afterImagePool.Enqueue(afterImage);
        });
    }

    private bool IsPivotInCircle(Transform pivot)
    {
        foreach (var circle in circles)
        {
            float dist = Vector2.Distance(pivot.position, circle.position);
            if (dist < stopThreshold)
            {
                if (specialCircles.Contains(circle.gameObject))
                {
                    var sr = circle.GetComponent<SpriteRenderer>();
                    if (sr && sr.sprite != defaultCircleSprite) return false;
                }
                return true;
            }
        }
        return false;
    }

    private void CheckCircleAlignment()
    {
        foreach (var circle in circles)
        {
            if ((currentPivot != pivot1 && (pivot1.position - circle.position).sqrMagnitude < stopThreshold * stopThreshold) ||
                (currentPivot != pivot2 && (pivot2.position - circle.position).sqrMagnitude < stopThreshold * stopThreshold))
            {
                if (specialCircles.Contains(circle.gameObject))
                {
                    var sr = circle.GetComponent<SpriteRenderer>();
                    if (sr && sr.sprite != defaultCircleSprite) continue;
                }
                isRotating = false;
                currentPivot = (currentPivot == pivot1) ? pivot2 : pivot1;
                break;
            }
        }
    }

    private void UpdateSpecialCircleSprite(GameObject specialCircle)
    {
        if (!specialCircle) return;

        var sr = specialCircle.GetComponent<SpriteRenderer>();
        if (!sr) return;

        bool state = circleSpriteStates.TryGetValue(specialCircle, out bool val) ? val : false;
        state = !state;
        sr.sprite = state ? specialCircleSprite : defaultCircleSprite;
        circleSpriteStates[specialCircle] = state;
    }

    private void CheckBellPass()
    {
        if (Time.time - lastBellPassTime < bellPassDelay) return;

        foreach (var bell in bells)
        {
            if (!bell) continue;

            if (Vector2.Distance(pivot1.position, bell.transform.position) < stopThresholds ||
                Vector2.Distance(pivot2.position, bell.transform.position) < stopThresholds)
            {
                bellPassCount++;
                lastBellPassTime = Time.time;

                var sr = bell.GetComponent<SpriteRenderer>();
                if (sr)
                {
                    bool state = bellSpriteStates.TryGetValue(bell, out bool val) ? val : false;
                    state = !state;
                    sr.sprite = state ? changedSprite : defaultSprite;
                    bellSpriteStates[bell] = state;
                }

                bellSound?.Play();
                break;
            }
        }
    }

    private void CheckWinCondition()
    {
        bool angleMatch = Mathf.Abs(Mathf.DeltaAngle(blackBar.rotation.eulerAngles.z, greenBar.rotation.eulerAngles.z)) < angleTolerance;
        bool positionMatch = Vector3.Distance(blackBar.position, greenBar.position) < positionTolerance;

        bool bellsPassed = true;
        foreach (var bell in bells)
        {
            var sr = bell?.GetComponent<SpriteRenderer>();
            if (sr && sr.sprite != changedSprite)
            {
                bellsPassed = false;
                break;
            }
        }

        if (angleMatch && positionMatch && (bells.Length == 0 || bellsPassed))
        {
            gameEnded = true;
            string level = SceneManager.GetActiveScene().name;
            PlayerPrefs.SetString("LastLevel", level);

            if (isTimeMode)
            {
                PlayerPrefs.SetFloat("Score", timeRemaining);
                string key = "HighScore_" + level;
                float prevHigh = PlayerPrefs.GetFloat(key, 0);
                if (timeRemaining > prevHigh)
                    PlayerPrefs.SetFloat(key, timeRemaining);
            }

            SceneManager.LoadScene("Win");
        }
    }
}