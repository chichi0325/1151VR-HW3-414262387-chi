using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpinWheel : MonoBehaviour
{
    public Transform wheelTransform;

    public float maxSpeed = 1200f;
    public float accelerateDuration = 1.5f;
    public float constantDuration = 2.0f;
    public float decelerateDuration = 3.0f;

    private bool isSpinning = false;
    private AudioSource audioSource;

    void Start()
    {
        // 自動取得同一個物件身上的 AudioSource 元件
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        // 檢查滑鼠左鍵 或 螢幕觸控是否被按下
        bool isClicked = false;

        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            isClicked = true;
        }

        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
        {
            isClicked = true;
        }

        if (isClicked && !isSpinning)
        {
            TriggerSpin();
        }
    }

    public void TriggerSpin()
    {
        if (isSpinning) return;
        StartCoroutine(SpinRoutine());
    }

    private IEnumerator SpinRoutine()
    {
        isSpinning = true;

        // 播放旋轉音效
        if (audioSource != null)
        {
            audioSource.Play();
        }

        // 💡 關鍵：隨機化等速階段的時間（例如增減 0 到 2 秒），讓每次停下的位置都不一樣！
        float randomConstantDuration = constantDuration + Random.Range(0f, 2.0f);

        float currentSpeed = 0f;
        float timer = 0f;

        // 1. 加速階段 (順時針)
        while (timer < accelerateDuration)
        {
            timer += Time.deltaTime;
            float t = timer / accelerateDuration;
            currentSpeed = Mathf.Lerp(0f, maxSpeed, t);
            wheelTransform.Rotate(0f, 0f, -currentSpeed * Time.deltaTime);
            yield return null;
        }

        // 2. 等速階段（使用隨機時間）
        timer = 0f;
        while (timer < randomConstantDuration)
        {
            timer += Time.deltaTime;
            wheelTransform.Rotate(0f, 0f, -maxSpeed * Time.deltaTime);
            yield return null;
        }

        // 3. 減速階段
        timer = 0f;
        float startDecelSpeed = maxSpeed;
        while (timer < decelerateDuration)
        {
            timer += Time.deltaTime;
            float t = timer / decelerateDuration;
            currentSpeed = Mathf.Lerp(startDecelSpeed, 0f, t);
            wheelTransform.Rotate(0f, 0f, -currentSpeed * Time.deltaTime);
            yield return null;
        }

        isSpinning = false;
        Debug.Log("輪盤停止轉動！");
    }
}