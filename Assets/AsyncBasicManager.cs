using UnityEngine;
using UnityEngine.UI;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using System;

public class AsyncBasicManager : MonoBehaviour
{
    public TextMeshProUGUI section1StatusText;
    public TextMeshProUGUI section2StatusText;
    public TextMeshProUGUI section3StatusText;
    public TextMeshProUGUI section4StatusText;
    public TextMeshProUGUI section5StatusText;
    public TextMeshProUGUI timeOutValue;

    public Slider slider1;
    public Slider slider2;
    public Slider slider3;
    public Slider timeOutSlider;
    public Slider cancelPrograssbar;

    public Transform cubeObjTr;

    private void Start()
    {
        InitTimeOutSlider();
    }

    private void OnDestroy()
    {
        cancelCts?.Cancel();
        cancelCts?.Dispose();
    }

    //Section 1 동기 비동기 테스트
    private void UpdateSection1Text(string msg)
    {
        section1StatusText.text = msg;
        Debug.Log($"[Section1] {msg}");
    }

    public void OnSyncDownloadButtonClicked()
    {
        UpdateSection1Text("OnSyncDownloadButtonClicked: Start");

        Thread.Sleep(3000);

        UpdateSection1Text("OnSyncDownloadButtonClicked: End");
    }

    //async void는 원래는 쓰면 안되는 함수다.
    //쓰는 경우 : 이벤트에 의해 호출되는 함수 -> 계속 호출하지않고 대기하는 경우
    public async void OnAsyncDownloadButtonClicked()
    {
        UpdateSection1Text("OnAsyncDownloadButtonClicked: Start");

        await Task.Delay(3000); //코루틴 WaitForSecond와 동일한 기능이다.

        UpdateSection1Text("OnAsyncDownloadButtonClicked: End");
    }

    //Section 2 딜레이 캔슬 테스트
    private void UpdateSection2Text(string msg)
    {
        section2StatusText.text = msg;
        Debug.Log($"[Section2] {msg}");
    }

    public async void OnDelayClicked(int seconds)
    {
        UpdateSection2Text($"대기중: {seconds} 초...");

        for(int i = seconds; i > 0; --i)
        {
            UpdateSection2Text($"남은 시간: {i} 초...");
            await Task.Delay(1000);

        }

        UpdateSection2Text($"대기 완료");
    }

    //캔슬 테스트
    private CancellationTokenSource delayCts = new CancellationTokenSource();
    public async void OnCancellableDelayClicked()
    {
        delayCts?.Cancel();
        delayCts?.Dispose();
        delayCts = new CancellationTokenSource();

        //캔슬 방법 1 : 예외 처리
        try
        {
            for (int i = 10; i > 0; --i)
            {
                delayCts.Token.ThrowIfCancellationRequested(); //이걸 호출했다고 캔슬되는게 아니라, 

                UpdateSection2Text($"남은 시간: {i} 초...");
                await Task.Delay(1000, delayCts.Token);

            }

            UpdateSection2Text($"10초 대기 완료");
        }
        catch(OperationCanceledException) //캔슬 예외처리 매개변수
        {
            UpdateSection2Text($"10초 대기 취소");
        }
    }

    public void OnCancelDelayClicked()
    {
        delayCts?.Cancel();
        delayCts?.Dispose();
        delayCts = new CancellationTokenSource();
    }

    //Section 3 병렬실행 테스트
    private void ResetProgressBars()
    {
        slider1.value = 0;
        slider2.value = 0;
        slider3.value = 0;
    }

    public async void OnSequencialDownloadClicked()
    {
        ResetProgressBars();
        UpdateSection3Text("순차 다운로드 시작");

        float startTime = Time.time;

        await FakeDownloadAsync(slider1, 1, 2000);
        await FakeDownloadAsync(slider2, 2, 2000);
        await FakeDownloadAsync(slider3, 3, 2000);

        float elapsed = Time.time - startTime; //지나간 시간이 나온다.

        UpdateSection3Text($"순차 다운로드 끝: {elapsed} 초");
    }

    public async void OnParallerDownloadClicked()
    {
        ResetProgressBars();
        UpdateSection3Text("병렬 다운로드 시작");

        float startTime = Time.time;

        Task task1 = FakeDownloadAsync(slider1, 1, 2000);
        Task task2 = FakeDownloadAsync(slider2, 2, 2000);
        Task task3 = FakeDownloadAsync(slider3, 3, 2000);

        await Task.WhenAll(task1, task2, task3);

        float elapsed = Time.time - startTime; //지나간 시간이 나온다.

        UpdateSection3Text($"병렬 다운로드 끝: {elapsed} 초");
    }

    private void UpdateSection3Text(string msg)
    {
        section3StatusText.text = msg;
        Debug.Log($"[Section3] {msg}");
    }

    //비동기 함수는 뒤에 Async를 붙이는게 관례이다.
    private async Task FakeDownloadAsync(Slider progressbar, int index, int durationMs)
    {
        int steps = 20; //한번에 몇 ms를 진행할거냐
        int delayPerStep = durationMs / steps; //즉 20단계로 진행

        for(int i = 0; i < steps; ++i)
        {
            progressbar.value = (float)i / steps;
            await Task.Delay(delayPerStep);
        }

        progressbar.value = 1f;

        Debug.Log($"[Section3] 파일 {index} 다운로드 완료");
    }

    //Section 4
    private void UpdateSection4Text(string msg)
    {
        section4StatusText.text = msg;
        Debug.Log($"[Section4] {msg}");
    }

    private void InitTimeOutSlider()
    {
        timeOutSlider.minValue = 1f;
        timeOutSlider.maxValue = 5f;
        timeOutSlider.value = 3f;
        OnTimeOutSliderChanged(timeOutSlider.value);

        timeOutSlider.onValueChanged.AddListener(OnTimeOutSliderChanged);
    }

    private void OnTimeOutSliderChanged(float value)
    {
        timeOutValue.text = $"{value} 초";
    }

    public async void OnTimeOutDownloadClicked()
    {
        UpdateSection4Text("다운로드 시작");

        Task downloadTask = Task.Delay(4000);
        Task timeOutTask = Task.Delay((int)timeOutSlider.value * 1000);

        Task completedTask = await Task.WhenAny(downloadTask, timeOutTask);

        if(completedTask == downloadTask)
        {
            UpdateSection4Text("다운로드 완료");
        }
        else
        {
            UpdateSection4Text("타임 아웃");

        }
    }

    //Section 5
    private void UpdateSection5Text(string msg)
    {
        section5StatusText.text = msg;
        Debug.Log($"[Section3] {msg}");
    }

    public async void OnSafeCodeClicked()
    {
        UpdateSection5Text("OnSafeCodeClicked");

        await Task.Delay(1000);

        cubeObjTr.position += Vector3.up * 0.5f;

        UpdateSection5Text("큐브 이동 완료");
    }

    public async void OnUnsafeCodeClicked()
    {
        UpdateSection5Text("OnUnsafeCodeClicked");

        await Task.Run(() => 
        {
            Thread.Sleep(1000); //비동기함수에서 호출되는 메인 슬립이기 때문에 비동기 내부에서만 정지한다.
            cubeObjTr.position += Vector3.up * 0.5f; //오류 발생
        });

        UpdateSection5Text("큐브 이동 완료");
    }

    //Section 6
    private CancellationTokenSource cancelCts = new CancellationTokenSource();

    public async void OnCancelationTokenClicked()
    {
        cancelCts?.Cancel();
        cancelCts?.Dispose();
        cancelCts = new CancellationTokenSource();

        //캔슬 방법 1 : 예외 처리
        cancelPrograssbar.value = 0;
        try
        {
            await FakeDownloadCancelAsync(cancelPrograssbar, 2000, cancelCts.Token);
            await Task.Delay(1000, delayCts.Token);
        }
        catch (OperationCanceledException) //캔슬 예외처리 매개변수
        {
            UpdateSection2Text($"다운로드 취소");
        }
    }

    private async Task FakeDownloadCancelAsync(Slider progressbar, int durationMs, CancellationToken ct)
    {
        int steps = 20; //한번에 몇 ms를 진행할거냐
        int delayPerStep = durationMs / steps; //즉 20단계로 진행

        for (int i = 0; i < steps; ++i)
        {
            ct.ThrowIfCancellationRequested(); //취소 확인

            progressbar.value = (float)i / steps;
            await Task.Delay(delayPerStep, ct);
        }

        progressbar.value = 1f;
    }

    public void OnCancelSection6Clicked()
    {
        cancelCts?.Cancel();
        cancelCts?.Dispose(); //사용할 수 없는 상태로 변경
        cancelCts = new CancellationTokenSource();
    }
}
