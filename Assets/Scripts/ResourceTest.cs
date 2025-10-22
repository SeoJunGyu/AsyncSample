using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.CompilerServices;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResourceTest : MonoBehaviour
{
    public Slider ProgressBar1;
    public Slider ProgressBar2;
    public Slider ProgressBar3;
    public Slider ProgressBar4;

    public Button loadingBtn;
    public Button cancelBtn;

    public TextMeshProUGUI loadingText;

    private void Start()
    {
        loadingBtn.onClick.AddListener(() => OnResourceLoadClicked().Forget() );
        cancelBtn.onClick.AddListener(() => OnCancelLoadingClicked());

        ResetProgressBars();
        loadingText.text = "Ready";
    }

    private void ResetProgressBars()
    {
        ProgressBar1.value = 0;
        ProgressBar2.value = 0;
        ProgressBar3.value = 0;
        ProgressBar4.value = 0;
    }

    private async UniTaskVoid OnResourceLoadClicked()
    {
        loadCts?.Cancel();
        loadCts?.Dispose();
        loadCts = new CancellationTokenSource();

        ResetProgressBars();
        loadingText.text = "Loading...";

        try
        {
            using (var timeoutCts = new CancellationTokenSource())
            using (var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(loadCts.Token, timeoutCts.Token))
            {
                timeoutCts.CancelAfter(TimeSpan.FromSeconds(10f));

                await OnParaller(linkedCts.Token);

                loadingText.text = "All resources loaded!";
            }
        }
        catch (OperationCanceledException)
        {
            if (loadCts.IsCancellationRequested)
            {
                loadingText.text = "Loading cancelled";
            }
            else
            {
                loadingText.text = "Loading timeout!";
            }
            ResetProgressBars();
        }
    }

    private CancellationTokenSource loadCts;
    public async UniTask OnParaller(CancellationToken ct)
    {
        ResetProgressBars();

        var loadTask1 = LoadResourceProgress("img1", ProgressBar2, ct);
        var loadTask2 = LoadResourceProgress("img2", ProgressBar3, ct);
        var loadTask3 = LoadResourceProgress("img3", ProgressBar4, ct);

        var progressTask = TotalProgress(ct);

        await UniTask.WhenAll(loadTask1, loadTask2, loadTask3);



        ProgressBar1.value = 1f;
        ProgressBar2.value = 1f;
        ProgressBar3.value = 1f;
        ProgressBar4.value = 1f;
    }

    public void OnCancelLoadingClicked()
    {
        loadCts?.Cancel();
        loadCts?.Dispose();
        loadCts = new CancellationTokenSource();
    }

    private async UniTask TotalProgress(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            float totalProgress = (ProgressBar2.value + ProgressBar3.value + ProgressBar4.value) / 3f;
            ProgressBar1.value = totalProgress;

            if (totalProgress >= 0.99f)
            {
                ProgressBar1.value = 1f;
                break;
            }

            await UniTask.Yield(PlayerLoopTiming.Update, ct);
        }
    }

    private async UniTask<Sprite> LoadResourceProgress(string resourcePath, Slider progressBar, CancellationToken ct)
    {
        progressBar.value = 0f;

        var request = Resources.LoadAsync<Sprite>(resourcePath);

        while (!request.isDone)
        {
            ct.ThrowIfCancellationRequested();

            progressBar.value = request.progress;

            await UniTask.Yield(PlayerLoopTiming.Update, ct);
        }

        progressBar.value = 1f;

        var sprite = request.asset as Sprite;
        var gameObject = new GameObject("LoadedSprite");
        var spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite;
        return sprite;
    }
}