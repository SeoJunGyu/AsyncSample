using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.CompilerServices;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoadingAndFadeInOutTest : MonoBehaviour
{
    public Button SceneLoadingBtn;
    public TextMeshProUGUI progressText;
    public Slider ProgressBar;
    public CanvasGroup fadePanel;

    [SerializeField] float minLoadTime = 2f;

    private void Start()
    {
        SceneLoadingBtn.onClick.AddListener(() => LoadSceneUniTask().Forget());
    }

    public async UniTaskVoid LoadSceneUniTask()
    {
        await FadeOutAsync();

        ProgressBar.value = 0f;

        float StartTime = Time.time;
        float actualProgress = 0f;

        var progress = Progress.Create<float>(x =>
        {
            actualProgress = x;
        });

        var sceneLoadTask = SceneManager.LoadSceneAsync("AsyncTest").ToUniTask(progress);
        
        while(Time.time - StartTime < minLoadTime || !sceneLoadTask.Status.IsCompleted())
        {
            float elapsed = Time.time - StartTime;
            float timeBasedProgress = Mathf.Clamp01(elapsed / minLoadTime);

            float displayProgress = Mathf.Min(timeBasedProgress, Mathf.Clamp01(actualProgress / 0.9f));

            ProgressBar.value = displayProgress;
            progressText.text = $"Loading... {displayProgress * 100f:F1}%";

            await UniTask.Yield();
        }

        ProgressBar.value = 1f;
        progressText.text = $"Loading... 100.0%";

        await sceneLoadTask;

        await FadeInAsync();
    }

    public async UniTask FadeInAsync()
    {
        CanvasGroup canvasGroup = fadePanel;
        float duration = 0.5f;

        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            canvasGroup.alpha = 1f - (t / duration);
            await UniTask.Yield();
        }

        canvasGroup.alpha = 0f;
    }

    public async UniTask FadeOutAsync()
    {
        CanvasGroup canvasGroup = fadePanel;
        float duration = 0.5f;

        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            canvasGroup.alpha = t / duration;
            await UniTask.Yield();
        }

        canvasGroup.alpha = 1f;
    }
}
