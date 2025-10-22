using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AsyncTimerTest : MonoBehaviour
{
    public Button StopBtn;
    public Button StartBtn;
    public Button ResumeBtn;
    public Button ResetBtn;

    public TextMeshProUGUI timerText;

    private CancellationTokenSource timerCts = new CancellationTokenSource();

    [SerializeField] private int timer = 60;
    [SerializeField] private int setTime = 60;
    private bool IsStart = false;
    private bool IsStop = false;

    private void Start()
    {
        StartBtn.onClick.AddListener(() => OnStartClicked().Forget());
        StopBtn.onClick.AddListener(() => OnStopClicked());
        ResumeBtn.onClick.AddListener(() => OnResumeClicked());
        ResetBtn.onClick.AddListener(() => OnResetClicked());

        ShowTime();
    }

    public void ShowTime()
    {
        timerText.text = $"{Mathf.FloorToInt(timer / 60f):00} : {(Mathf.FloorToInt(timer) % 60f):00}";
    }

    public async UniTaskVoid OnStartClicked()
    {
        if (IsStart || IsStop)
        {
            return;
        }
        IsStart = true;

        try
        {
            for(int i = timer; i >= 0; i--)
            {
                timer--;
                timerCts.Token.ThrowIfCancellationRequested();
                await UniTask.WaitWhile(() => IsStop, cancellationToken: timerCts.Token);

                timerText.text = $"{Mathf.FloorToInt(i / 60f):00} : {(Mathf.FloorToInt(i) % 60f):00}";
                await UniTask.Delay(1000, cancellationToken: timerCts.Token);
            }

            timerText.text = "Time Up!";
        }
        catch (OperationCanceledException)
        {
            Debug.Log("타이머 취소");
        }
    }

    public void OnStopClicked()
    {
        if (!IsStart)
        {
            return;
        }

        IsStop = true;
    }

    public async UniTaskVoid OnResumeClicked()
    {
        if (!IsStop)
        {
            return;
        }

        IsStop = false;

        await UniTask.Delay(1000);

        OnStartClicked();
    }

    public void OnResetClicked()
    {
        IsStart = false;
        IsStop = false;

        timerCts?.Cancel();
        timerCts?.Dispose();
        timerCts = new CancellationTokenSource();

        timer = setTime;
        ShowTime();
    }
}
