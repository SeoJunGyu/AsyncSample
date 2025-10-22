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

    //Section 1 ���� �񵿱� �׽�Ʈ
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

    //async void�� ������ ���� �ȵǴ� �Լ���.
    //���� ��� : �̺�Ʈ�� ���� ȣ��Ǵ� �Լ� -> ��� ȣ�������ʰ� ����ϴ� ���
    public async void OnAsyncDownloadButtonClicked()
    {
        UpdateSection1Text("OnAsyncDownloadButtonClicked: Start");

        await Task.Delay(3000); //�ڷ�ƾ WaitForSecond�� ������ ����̴�.

        UpdateSection1Text("OnAsyncDownloadButtonClicked: End");
    }

    //Section 2 ������ ĵ�� �׽�Ʈ
    private void UpdateSection2Text(string msg)
    {
        section2StatusText.text = msg;
        Debug.Log($"[Section2] {msg}");
    }

    public async void OnDelayClicked(int seconds)
    {
        UpdateSection2Text($"�����: {seconds} ��...");

        for(int i = seconds; i > 0; --i)
        {
            UpdateSection2Text($"���� �ð�: {i} ��...");
            await Task.Delay(1000);

        }

        UpdateSection2Text($"��� �Ϸ�");
    }

    //ĵ�� �׽�Ʈ
    private CancellationTokenSource delayCts = new CancellationTokenSource();
    public async void OnCancellableDelayClicked()
    {
        delayCts?.Cancel();
        delayCts?.Dispose();
        delayCts = new CancellationTokenSource();

        //ĵ�� ��� 1 : ���� ó��
        try
        {
            for (int i = 10; i > 0; --i)
            {
                delayCts.Token.ThrowIfCancellationRequested(); //�̰� ȣ���ߴٰ� ĵ���Ǵ°� �ƴ϶�, 

                UpdateSection2Text($"���� �ð�: {i} ��...");
                await Task.Delay(1000, delayCts.Token);

            }

            UpdateSection2Text($"10�� ��� �Ϸ�");
        }
        catch(OperationCanceledException) //ĵ�� ����ó�� �Ű�����
        {
            UpdateSection2Text($"10�� ��� ���");
        }
    }

    public void OnCancelDelayClicked()
    {
        delayCts?.Cancel();
        delayCts?.Dispose();
        delayCts = new CancellationTokenSource();
    }

    //Section 3 ���Ľ��� �׽�Ʈ
    private void ResetProgressBars()
    {
        slider1.value = 0;
        slider2.value = 0;
        slider3.value = 0;
    }

    public async void OnSequencialDownloadClicked()
    {
        ResetProgressBars();
        UpdateSection3Text("���� �ٿ�ε� ����");

        float startTime = Time.time;

        await FakeDownloadAsync(slider1, 1, 2000);
        await FakeDownloadAsync(slider2, 2, 2000);
        await FakeDownloadAsync(slider3, 3, 2000);

        float elapsed = Time.time - startTime; //������ �ð��� ���´�.

        UpdateSection3Text($"���� �ٿ�ε� ��: {elapsed} ��");
    }

    public async void OnParallerDownloadClicked()
    {
        ResetProgressBars();
        UpdateSection3Text("���� �ٿ�ε� ����");

        float startTime = Time.time;

        Task task1 = FakeDownloadAsync(slider1, 1, 2000);
        Task task2 = FakeDownloadAsync(slider2, 2, 2000);
        Task task3 = FakeDownloadAsync(slider3, 3, 2000);

        await Task.WhenAll(task1, task2, task3);

        float elapsed = Time.time - startTime; //������ �ð��� ���´�.

        UpdateSection3Text($"���� �ٿ�ε� ��: {elapsed} ��");
    }

    private void UpdateSection3Text(string msg)
    {
        section3StatusText.text = msg;
        Debug.Log($"[Section3] {msg}");
    }

    //�񵿱� �Լ��� �ڿ� Async�� ���̴°� �����̴�.
    private async Task FakeDownloadAsync(Slider progressbar, int index, int durationMs)
    {
        int steps = 20; //�ѹ��� �� ms�� �����Ұų�
        int delayPerStep = durationMs / steps; //�� 20�ܰ�� ����

        for(int i = 0; i < steps; ++i)
        {
            progressbar.value = (float)i / steps;
            await Task.Delay(delayPerStep);
        }

        progressbar.value = 1f;

        Debug.Log($"[Section3] ���� {index} �ٿ�ε� �Ϸ�");
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
        timeOutValue.text = $"{value} ��";
    }

    public async void OnTimeOutDownloadClicked()
    {
        UpdateSection4Text("�ٿ�ε� ����");

        Task downloadTask = Task.Delay(4000);
        Task timeOutTask = Task.Delay((int)timeOutSlider.value * 1000);

        Task completedTask = await Task.WhenAny(downloadTask, timeOutTask);

        if(completedTask == downloadTask)
        {
            UpdateSection4Text("�ٿ�ε� �Ϸ�");
        }
        else
        {
            UpdateSection4Text("Ÿ�� �ƿ�");

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

        UpdateSection5Text("ť�� �̵� �Ϸ�");
    }

    public async void OnUnsafeCodeClicked()
    {
        UpdateSection5Text("OnUnsafeCodeClicked");

        await Task.Run(() => 
        {
            Thread.Sleep(1000); //�񵿱��Լ����� ȣ��Ǵ� ���� �����̱� ������ �񵿱� ���ο����� �����Ѵ�.
            cubeObjTr.position += Vector3.up * 0.5f; //���� �߻�
        });

        UpdateSection5Text("ť�� �̵� �Ϸ�");
    }

    //Section 6
    private CancellationTokenSource cancelCts = new CancellationTokenSource();

    public async void OnCancelationTokenClicked()
    {
        cancelCts?.Cancel();
        cancelCts?.Dispose();
        cancelCts = new CancellationTokenSource();

        //ĵ�� ��� 1 : ���� ó��
        cancelPrograssbar.value = 0;
        try
        {
            await FakeDownloadCancelAsync(cancelPrograssbar, 2000, cancelCts.Token);
            await Task.Delay(1000, delayCts.Token);
        }
        catch (OperationCanceledException) //ĵ�� ����ó�� �Ű�����
        {
            UpdateSection2Text($"�ٿ�ε� ���");
        }
    }

    private async Task FakeDownloadCancelAsync(Slider progressbar, int durationMs, CancellationToken ct)
    {
        int steps = 20; //�ѹ��� �� ms�� �����Ұų�
        int delayPerStep = durationMs / steps; //�� 20�ܰ�� ����

        for (int i = 0; i < steps; ++i)
        {
            ct.ThrowIfCancellationRequested(); //��� Ȯ��

            progressbar.value = (float)i / steps;
            await Task.Delay(delayPerStep, ct);
        }

        progressbar.value = 1f;
    }

    public void OnCancelSection6Clicked()
    {
        cancelCts?.Cancel();
        cancelCts?.Dispose(); //����� �� ���� ���·� ����
        cancelCts = new CancellationTokenSource();
    }
}
