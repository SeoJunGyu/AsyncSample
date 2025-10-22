using Cysharp.Threading.Tasks;
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
    }

    private void ResetProgressBars()
    {
        ProgressBar1.value = 0;
        ProgressBar2.value = 0;
        ProgressBar3.value = 0;
        ProgressBar4.value = 0;
    }

    private CancellationTokenSource loadCts;
    public async UniTaskVoid OnParaller()
    {
        ResetProgressBars();
        loadCts?.Cancel();
        loadCts?.Dispose();
        loadCts = new CancellationTokenSource();

        try
        {
            float startTime = Time.realtimeSinceStartup;

            var loadTask1 = LoadResourceProgress("img1", ProgressBar2, loadCts.Token);
            var loadTask2 = LoadResourceProgress("img2", ProgressBar3, loadCts.Token);
            var loadTask3 = LoadResourceProgress("img3", ProgressBar4, loadCts.Token);

            var progressTask = TotalProgress(loadCts.Token);

            var sprites = await UniTask.WhenAll(loadTask1, loadTask2, loadTask3);

            float elapsedTime = Time.realtimeSinceStartup - startTime;
            float remainingTime = 2f - elapsedTime;

            if(remainingTime > 0)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(remainingTime), cancellationToken: loadCts.Token);
            }

            ProgressBar1.value = 1f;
            ProgressBar2.value = 1f;
            ProgressBar3.value = 1f;
            ProgressBar4.value = 1f;

        }
        catch (OperationCanceledException)
        {

        }
    }

    public void OnCancelLoadingClicked()
    {
        loadCts?.Cancel();
        loadCts?.Dispose();
        loadCts = new CancellationTokenSource();
    }

    private async UniTask TotalProgress(CancellationToken ct)
    {
        while(ProgressBar2.value < 0.99f ||
            ProgressBar3.value < 0.99f ||
            ProgressBar4.value < 0.99f)
        {
            ct.ThrowIfCancellationRequested();

            float totalProgress = (ProgressBar2.value + ProgressBar3.value + ProgressBar4.value) / 3f;
            ProgressBar1.value = totalProgress;

            await UniTask.Yield(PlayerLoopTiming.Update, ct);
        }

        ProgressBar1.value = 1f;
    }

    private async UniTask<Sprite> LoadResourceProgress(string resourcePath, Slider progressBar, CancellationToken ct)
    {
        var request = Resources.LoadAsync<Sprite>(resourcePath);

        while (!request.isDone)
        {
            ct.ThrowIfCancellationRequested();
            progressBar.value = request.progress;
            await UniTask.Yield(PlayerLoopTiming.Update, ct);
        }

        progressBar.value = 1f;

        return request.asset as Sprite;
    }

    private async UniTaskVoid OnResourceLoadClicked()
    {
        var prefab = await Resources.LoadAsync<Sprite>("img1").ToUniTask() as Sprite;
        Instantiate(prefab);
    }
}