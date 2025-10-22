using System;
using System.Threading;
using Cysharp.Threading.Tasks;//�̰� UniTask using�̴�.
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Cysharp.Threading.Tasks.CompilerServices;

public class UniTaskManager : MonoBehaviour
{
    public Button delayButton;
    public Button delayFrameButton;
    public Button yieldButton;
    public Button nextFrameButton;

    public Button sequentialButton;
    public Button whenAllButton;
    public Button whenAnyButton;

    public Button loadResourceButton;
    public Button loadWithProgressBarButton;
    public Button cancelButton;

    public Button UpdateButton;
    public Button FixedUpdateButton;
    public Button LateUpdateButton;

    public Button destroyTokenButton;
    public Button timeOutTokenButton;
    public Button linkedTokenButton;
    public Button cancelSection5Button;

    public Button fadeInButton;
    public Button fadeOutButton;
    public Button AnimationButton;
    public Button waitForInputButton;

    public TextMeshProUGUI[] sectionTexts;

    public Slider progressBar1;
    public Slider progressBar2;
    public Slider progressBar3;

    public Slider loadingProgressBar;

    public Slider section5ProgressBar;

    private CancellationTokenSource section5Cts;

    public CanvasGroup fadePanel;

    public Transform animatedCube;

    private void Start()
    {
        delayButton.onClick.AddListener(() => OnDelayClicked().Forget());
        delayFrameButton.onClick.AddListener(() => OnDelayFrameClicked().Forget());
        yieldButton.onClick.AddListener(() => OnYieldClicked().Forget());
        nextFrameButton.onClick.AddListener(() => OnNextFrameClicked().Forget());

        sequentialButton.onClick.AddListener(() => OnSequentialClicked().Forget());
        whenAllButton.onClick.AddListener(() => OnWhenAllClicked().Forget());
        whenAnyButton.onClick.AddListener(() => OnWhenAnyClicked().Forget());

        loadResourceButton.onClick.AddListener(() => OnResourceLoadClicked().Forget());
        loadWithProgressBarButton.onClick.AddListener(() => OnLoadWithProgressBarClicked().Forget());
        cancelButton.onClick.AddListener(() => OnCancelClicked());

        UpdateButton.onClick.AddListener(() => OnUpdateClicked().Forget());
        FixedUpdateButton.onClick.AddListener(() => OnFixedUpdateClicked().Forget());
        LateUpdateButton.onClick.AddListener(() => OnLateUpdateClicked().Forget());

        destroyTokenButton.onClick.AddListener(() => OnDestroyTokenClicked().Forget());
        timeOutTokenButton.onClick.AddListener(() => OnTimeOutClicked().Forget());
        linkedTokenButton.onClick.AddListener(() => OnClickLinkedCtsClicked().Forget());
        cancelSection5Button.onClick.AddListener(() => OnCancelSection5Clicked());

        fadeInButton.onClick.AddListener(() => OnFadeInClicked().Forget());
        fadeOutButton.onClick.AddListener(() => OnFadeOutClicked().Forget());
        AnimationButton.onClick.AddListener(() => OnAnimationClicked().Forget());
        waitForInputButton.onClick.AddListener(() => OnWaitKeyClicked().Forget());

        ResetProgressBars();
    }

    private void UpdateSectionText(int section, string msg)
    {
        var log = $"[Section {section - 1}] {msg}"; ;
        sectionTexts[section - 1].text = log;
        Debug.Log(log);
    }

    //Section 1
    //��ȯ ���� �񵿱� �Լ�
    private async UniTaskVoid OnDelayClicked()
    {
        UpdateSectionText(1, "OnDelayClicked()");

        await UniTask.Delay(2000);

        UpdateSectionText(1, "2�� ��� �Ϸ�");
    }

    private async UniTaskVoid OnDelayFrameClicked()
    {
        UpdateSectionText(1, "OnDelayFrameClicked()");

        int startFrame = Time.frameCount;

        await UniTask.DelayFrame(60);
        int endFrame = Time.frameCount;

        UpdateSectionText(1, $"{endFrame - startFrame}������ ��� �Ϸ�");
    }

    private async UniTaskVoid OnYieldClicked()
    {
        UpdateSectionText(1, "OnYieldClicked()");

        int startFrame = Time.frameCount;

        await UniTask.Yield(); //������Ʈ�� �� ������ ���
        int endFrame = Time.frameCount;

        UpdateSectionText(1, $"Yield �Ϸ�: ����({startFrame}) ��({endFrame})");
    }

    private async UniTaskVoid OnNextFrameClicked()
    {
        UpdateSectionText(1, "OnNextFrameClicked()");

        int startFrame = Time.frameCount;

        await UniTask.NextFrame(); //������Ʈ�� �� ������ ���
        int endFrame = Time.frameCount;

        UpdateSectionText(1, $"NextFrame �Ϸ�: ����({startFrame}) ��({endFrame})");
    }

    //Section 2
    private void ResetProgressBars()
    {
        progressBar1.value = 0f;
        progressBar2.value = 0f;
        progressBar3.value = 0f;
        loadingProgressBar.value = 0f;
    }

    private async UniTask FakeLoadAsync(Slider progressBar, int ms)
    {
        int steps = 20;
        int delayPerStep = ms / steps;

        for(int i = 0; i <= steps; i++)
        {
            progressBar.value = (float)i / steps; //�����̵� ����
            await UniTask.Delay(delayPerStep); //���
        }
    }

    private async UniTaskVoid OnSequentialClicked()
    {
        ResetProgressBars();
        UpdateSectionText(2, "OnSequentialClicked");

        float startTime = Time.time;

        await FakeLoadAsync(progressBar1, 2000);
        await FakeLoadAsync(progressBar2, 2500);
        await FakeLoadAsync(progressBar3, 1500);

        float elapsed = Time.time - startTime;

        UpdateSectionText(2, $"���� ���� �Ϸ�: {elapsed}��");
    }

    private async UniTaskVoid OnWhenAllClicked()
    {
        ResetProgressBars();
        UpdateSectionText(2, "OnWhenAllClicked");

        float startTime = Time.time;

        await UniTask.WhenAll
            (
                FakeLoadAsync(progressBar1, 2000),
                FakeLoadAsync(progressBar2, 2500),
                FakeLoadAsync(progressBar3, 1500)
            );

        float elapsed = Time.time - startTime;

        UpdateSectionText(2, $"WhenAll ���� �Ϸ�: {elapsed}��");
    }

    private async UniTaskVoid OnWhenAnyClicked()
    {
        ResetProgressBars();
        UpdateSectionText(2, "OnWhenAnyClicked");

        float startTime = Time.time;

        //���� ���� ����� ���α׷����� �ε����� ��ȯ ���� �� �ִ�. 0 1 2 ����
        int index = await UniTask.WhenAny
            (
                FakeLoadAsync(progressBar1, 2000),
                FakeLoadAsync(progressBar2, 2500),
                FakeLoadAsync(progressBar3, 1500)
            );

        float elapsed = Time.time - startTime;

        UpdateSectionText(2, $"WhenAny ���� �Ϸ� {index} : {elapsed}��");
    }

    //Section 3
    private async UniTaskVoid OnResourceLoadClicked()
    {
        UpdateSectionText(3, "OnResourceLoadClicked");

        var prefab = await Resources.LoadAsync<GameObject>("RotatingCube").ToUniTask() as GameObject;

        UpdateSectionText(3, "���ҽ� �ε� �Ϸ�");
        Instantiate(prefab);
    }

    private void OnDestroy()
    {
        loadCts?.Cancel();
        loadCts?.Dispose();
    }

    private void OnCancelClicked()
    {
        if(loadCts != null && !loadCts.IsCancellationRequested)
        {
            loadCts.Cancel();
            loadCts.Dispose();
            loadCts = null;
        }
        else
        {

        }
    }

    private CancellationTokenSource loadCts;
    private async UniTaskVoid OnLoadWithProgressBarClicked()
    {
        loadCts?.Cancel();
        loadCts?.Dispose();
        loadCts = new CancellationTokenSource();

        UpdateSectionText(3, "OnResourceLoadClicked");

        try
        {
            UpdateSectionText(3, "�ε� ����");

            loadingProgressBar.value = 0f;

            for (int i = 0; i < 100; i++)
            {
                loadCts.Token.ThrowIfCancellationRequested(); //��� ������ ���� �߻�

                loadingProgressBar.value = i / 100f;

                UpdateSectionText(3, $"�ε�: {i}%");

                await UniTask.Delay(50, cancellationToken: loadCts.Token);
            }
        }
        catch (OperationCanceledException)
        {
            UpdateSectionText(3, "�ε� ���");
        }

        UpdateSectionText(3, "���ҽ� �ε� �Ϸ�");
    }

    //Section 4
    private async UniTaskVoid OnUpdateClicked()
    {
        UpdateSectionText(4, "OnUpdateClicked");

        for(int i = 0; i < 3; i++)
        {
            await UniTask.Yield(PlayerLoopTiming.Update);
            UpdateSectionText(4, $"������Ʈ ������ {Time.frameCount}");
        }

        UpdateSectionText(4, "������Ʈ Ÿ�̹� �׽�Ʈ ��");
    }

    private async UniTaskVoid OnFixedUpdateClicked()
    {
        UpdateSectionText(4, "OnFixedUpdateClicked");

        for (int i = 0; i < 3; i++)
        {
            await UniTask.Yield(PlayerLoopTiming.FixedUpdate);
            UpdateSectionText(4, $"�Ƚ��� ������Ʈ ������ {Time.time}"); //������ �Ƚ��� ������Ʈ�� ������Ʈ �������̱⿡ �ǹ̰� ����.
        }

        UpdateSectionText(4, "�Ƚ��� ������Ʈ Ÿ�̹� �׽�Ʈ ��");
    }

    private async UniTaskVoid OnLateUpdateClicked()
    {
        UpdateSectionText(4, "OnLateUpdateClicked");

        for (int i = 0; i < 3; i++)
        {
            await UniTask.Yield(PlayerLoopTiming.LastTimeUpdate);
            UpdateSectionText(4, $"����Ʈ ������Ʈ ������ {Time.frameCount}");
        }

        UpdateSectionText(4, "����Ʈ ������Ʈ Ÿ�̹� �׽�Ʈ ��");
    }

    //Section 5
    public async UniTask LongTaskAsync(CancellationToken ct)
    {
        UpdateSectionText(5, "Long task started... (10s)");
        section5ProgressBar.value = 0;

        for (int i = 0; i <= 100; i++)
        {
            ct.ThrowIfCancellationRequested();
            section5ProgressBar.value = i / 100f;

            UpdateSectionText(5, $"Progress: {i}% (Cancellable)");

            await UniTask.Delay(100, cancellationToken: ct);
        }

        UpdateSectionText(5, "Task complete! (100%)");
    }

    private async UniTaskVoid OnDestroyTokenClicked()
    {
        UpdateSectionText(5, "OnDestroyTokenClicked");

        try
        {
            await LongTaskAsync(this.GetCancellationTokenOnDestroy());
            UpdateSectionText(5, "�׽�ũ �Ϸ�");
        }
        catch (OperationCanceledException)
        {
            UpdateSectionText(5, "���");
        }
    }

    private async UniTask OnTimeOutClicked()
    {
        UpdateSectionText(5, "OnTimeOutClicked");

        var cts = new CancellationTokenSource();
        cts.CancelAfterSlim(TimeSpan.FromSeconds(3)); //������ �ð� �� �ڵ����� ��ҽ�Ű�� �Լ�

        try
        {
            await LongTaskAsync(cts.Token);
        }
        catch (OperationCanceledException)
        {
            UpdateSectionText(5, "Ÿ�� �ƿ� ���");
        }
    }

    private async UniTaskVoid OnClickLinkedCtsClicked()
    {
        UpdateSectionText(5, "OnClickLinkedCtsClicked");

        section5Cts?.Cancel();
        section5Cts?.Dispose();
        section5Cts = new CancellationTokenSource();

        var timeOutCts = new CancellationTokenSource();
        timeOutCts.CancelAfterSlim(TimeSpan.FromSeconds(3));

        var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(
                section5Cts.Token,
                timeOutCts.Token,
                this.GetCancellationTokenOnDestroy()
            );

        try
        {
            await LongTaskAsync(linkedCts.Token);
        }
        catch (OperationCanceledException)
        {
            if (section5Cts.IsCancellationRequested)
            {
                UpdateSectionText(5, "���� ���");
            }
            else if(timeOutCts.IsCancellationRequested)
            {
                UpdateSectionText(5, "Ÿ�Ӿƿ� ���");
            }
            else
            {
                UpdateSectionText(5, "OnDestroy ���");
            }
        }
        finally
        {
            timeOutCts?.Cancel();
            timeOutCts?.Dispose();
            section5Cts?.Cancel();
            section5Cts?.Dispose();
            linkedCts?.Dispose();

            section5Cts = new CancellationTokenSource();
        }
    }

    private void OnCancelSection5Clicked()
    {
        if(section5Cts != null && !section5Cts.IsCancellationRequested)
        {
            section5Cts.Cancel();
        }
        else
        {
            UpdateSectionText(5, "�׽�ũ ����");
        }
    }

    //Section 6
    private async UniTask FadeAsync(CanvasGroup canvasGroup, float from, float to, float duration)
    {
        canvasGroup.alpha = from;

        float elapsed = 0f;

        while(elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration; //���������� ����� �ð� ��

            canvasGroup.alpha = Mathf.Lerp(from, to, t);
            await UniTask.Yield();
        }

        canvasGroup.alpha = to;
    }

    private async UniTask MoveToAsync(RectTransform target, Vector2 to, float duration)
    {
        Vector2 from = target.anchoredPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration; //���������� ����� �ð� ��

            target.anchoredPosition = Vector2.Lerp(from, to, t);
            await UniTask.Yield();
        }

        target.anchoredPosition = to;
    }

    //duration ; ������ �ð�
    private async UniTask RotateToAsync(Transform target, float speed, float duration)
    {
        float from = target.eulerAngles.z;
        float elapsed = 0f;
        float currentAngle = from;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            
            currentAngle += speed * Time.deltaTime;
            target.rotation = Quaternion.Euler(0f, 0f, currentAngle);
            await UniTask.Yield();
        }

        target.rotation = Quaternion.Euler(0f, 0f, from); //�����·� ����
    }

    private async UniTaskVoid OnFadeInClicked()
    {
        UpdateSectionText(6, "OnFadeInClicked");

        await FadeAsync(fadePanel, 0f, 1f, 0.5f);

        UpdateSectionText(6, "���̵� �� �Ϸ�");
    }

    private async UniTaskVoid OnFadeOutClicked()
    {
        UpdateSectionText(6, "OnFadeOutClicked");

        await FadeAsync(fadePanel, 1f, 0f, 0.5f);

        UpdateSectionText(6, "���̵� �ƿ� �Ϸ�");
    }

    private async UniTaskVoid OnAnimationClicked()
    {
        UpdateSectionText(6, "OnAnimationClicked");

        var rectTr = animatedCube as RectTransform;
        var originalPos = rectTr.anchoredPosition;

        await MoveToAsync(rectTr, originalPos + Vector2.up * 50f, 0.5f); //���� �̵�
        UpdateSectionText(6, "1. ���� �̵�");

        await RotateToAsync(animatedCube, 360f, 0.5f); //ȸ��
        UpdateSectionText(6, "2. ȸ��");

        await MoveToAsync(rectTr, originalPos, 0.5f); //���� ��ġ�� �̵�
        UpdateSectionText(6, "3. ����ġ�� �̵�");
    }

    private async UniTaskVoid OnWaitKeyClicked()
    {
        UpdateSectionText(6, "OnWaitKeyClicked");
        await UniTask.WaitUntil(() => Input.anyKey); //�ƹ� Ű �Է±��� ���
        UpdateSectionText(6, "�ƹ� Ű �Է� �Ϸ�");
    }
}
