using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.CompilerServices;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class UIAnimationTest : MonoBehaviour
{
    public Button startBtn;
    public Button ResetBtn;
    public Button cancelBtn;

    public CanvasGroup canvas;

    private Vector2 initialPosition;
    private Vector3 initialScale;
    private Quaternion initialRotation;
    private float initialAlpha;

    private CancellationTokenSource animationCts;

    public CanvasGroup fadePanel;
    public Transform animatedCube;

    private void Start()
    {
        SaveState();

        startBtn.onClick.AddListener(() => OnStartClicked().Forget());
        ResetBtn.onClick.AddListener(() => ResetAll());
        cancelBtn.onClick.AddListener(() => OnCancelClicked());
    }

    private async UniTaskVoid OnStartClicked()
    {
        OnCancelClicked();

        var ct = animationCts.Token;

        try
        {
            var rectTr = animatedCube as RectTransform;

            ResetAll();
            fadePanel.alpha = 0f;

            await UniTask.Delay(100, cancellationToken: ct);

            await FadeAsync(fadePanel, 0f, 1f, 1f, ct);

            Vector2 targetPosition = initialPosition + Vector2.right * 200f;
            await MoveToAsync(rectTr, initialPosition, targetPosition, 0.5f, ct);

            Vector3 targetScale = initialScale * 1.5f;
            await ScaleToAsync(animatedCube, initialScale, targetScale, 0.5f, ct);

            float currentAngle = animatedCube.eulerAngles.z;
            await RotateToAsync(animatedCube, currentAngle, currentAngle + 360f, 1f, ct);
        }
        catch (OperationCanceledException)
        {
            
        }
    }

    private async UniTask FadeAsync(CanvasGroup canvasGroup, float from, float to, float duration, CancellationToken ct)
    {
        canvasGroup.alpha = from;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            ct.ThrowIfCancellationRequested();

            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            canvasGroup.alpha = Mathf.Lerp(from, to, t);

            await UniTask.Yield(cancellationToken: ct);
        }

        canvasGroup.alpha = to;
    }

    private async UniTask MoveToAsync(RectTransform target, Vector2 from, Vector2 to, float duration, CancellationToken ct)
    {
        target.anchoredPosition = from;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            ct.ThrowIfCancellationRequested();

            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            target.anchoredPosition = Vector2.Lerp(from, to, t);

            await UniTask.Yield(cancellationToken: ct);
        }

        target.anchoredPosition = to;
    }

    private async UniTask ScaleToAsync(Transform target, Vector3 from, Vector3 to, float duration, CancellationToken ct)
    {
        target.localScale = from;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            ct.ThrowIfCancellationRequested();

            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            target.localScale = Vector3.Lerp(from, to, t);

            await UniTask.Yield(cancellationToken: ct);
        }

        target.localScale = to;
    }

    private async UniTask RotateToAsync(Transform target, float fromAngle, float toAngle, float duration, CancellationToken ct)
    {
        target.rotation = Quaternion.Euler(0f, 0f, fromAngle);
        float elapsed = 0f;

        while (elapsed < duration)
        {
            ct.ThrowIfCancellationRequested();

            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float currentAngle = Mathf.Lerp(fromAngle, toAngle, t);
            target.rotation = Quaternion.Euler(0f, 0f, currentAngle);

            await UniTask.Yield(cancellationToken: ct);
        }

        target.rotation = Quaternion.Euler(0f, 0f, toAngle);
    }

    private void OnResetClicked()
    {
        OnCancelClicked();

        ResetAll();
    }

    private void SaveState()
    {
        var rectTr = animatedCube as RectTransform;
        initialPosition = rectTr.anchoredPosition;
        initialScale = animatedCube.localScale;
        initialRotation = animatedCube.rotation;
        initialAlpha = fadePanel.alpha;
    }

    private void ResetAll()
    {
        var rectTr = animatedCube as RectTransform;
        rectTr.anchoredPosition = initialPosition;
        animatedCube.localScale = initialScale;
        animatedCube.rotation = initialRotation;
        fadePanel.alpha = initialAlpha;
    }

    private void OnCancelClicked()
    {
        animationCts?.Cancel();
        animationCts?.Dispose();
        animationCts = new CancellationTokenSource();
    }
}
