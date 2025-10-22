using System;
using System.Threading;
using Cysharp.Threading.Tasks;//이게 UniTask using이다.
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
    //반환 없는 비동기 함수
    private async UniTaskVoid OnDelayClicked()
    {
        UpdateSectionText(1, "OnDelayClicked()");

        await UniTask.Delay(2000);

        UpdateSectionText(1, "2초 대기 완료");
    }

    private async UniTaskVoid OnDelayFrameClicked()
    {
        UpdateSectionText(1, "OnDelayFrameClicked()");

        int startFrame = Time.frameCount;

        await UniTask.DelayFrame(60);
        int endFrame = Time.frameCount;

        UpdateSectionText(1, $"{endFrame - startFrame}프레임 대기 완료");
    }

    private async UniTaskVoid OnYieldClicked()
    {
        UpdateSectionText(1, "OnYieldClicked()");

        int startFrame = Time.frameCount;

        await UniTask.Yield(); //업데이트의 한 프레임 대기
        int endFrame = Time.frameCount;

        UpdateSectionText(1, $"Yield 완료: 시작({startFrame}) 끝({endFrame})");
    }

    private async UniTaskVoid OnNextFrameClicked()
    {
        UpdateSectionText(1, "OnNextFrameClicked()");

        int startFrame = Time.frameCount;

        await UniTask.NextFrame(); //업데이트의 한 프레임 대기
        int endFrame = Time.frameCount;

        UpdateSectionText(1, $"NextFrame 완료: 시작({startFrame}) 끝({endFrame})");
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
            progressBar.value = (float)i / steps; //슬라이드 진행
            await UniTask.Delay(delayPerStep); //대기
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

        UpdateSectionText(2, $"순차 실행 완료: {elapsed}초");
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

        UpdateSectionText(2, $"WhenAll 실행 완료: {elapsed}초");
    }

    private async UniTaskVoid OnWhenAnyClicked()
    {
        ResetProgressBars();
        UpdateSectionText(2, "OnWhenAnyClicked");

        float startTime = Time.time;

        //가장 먼저 종료된 프로그래스의 인덱스를 반환 받을 수 있다. 0 1 2 순서
        int index = await UniTask.WhenAny
            (
                FakeLoadAsync(progressBar1, 2000),
                FakeLoadAsync(progressBar2, 2500),
                FakeLoadAsync(progressBar3, 1500)
            );

        float elapsed = Time.time - startTime;

        UpdateSectionText(2, $"WhenAny 실행 완료 {index} : {elapsed}초");
    }

    //Section 3
    private async UniTaskVoid OnResourceLoadClicked()
    {
        UpdateSectionText(3, "OnResourceLoadClicked");

        var prefab = await Resources.LoadAsync<GameObject>("RotatingCube").ToUniTask() as GameObject;

        UpdateSectionText(3, "리소스 로딩 완료");
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
            UpdateSectionText(3, "로딩 시작");

            loadingProgressBar.value = 0f;

            for (int i = 0; i < 100; i++)
            {
                loadCts.Token.ThrowIfCancellationRequested(); //취소 들어오면 예외 발생

                loadingProgressBar.value = i / 100f;

                UpdateSectionText(3, $"로딩: {i}%");

                await UniTask.Delay(50, cancellationToken: loadCts.Token);
            }
        }
        catch (OperationCanceledException)
        {
            UpdateSectionText(3, "로딩 취소");
        }

        UpdateSectionText(3, "리소스 로딩 완료");
    }

    //Section 4
    private async UniTaskVoid OnUpdateClicked()
    {
        UpdateSectionText(4, "OnUpdateClicked");

        for(int i = 0; i < 3; i++)
        {
            await UniTask.Yield(PlayerLoopTiming.Update);
            UpdateSectionText(4, $"업데이트 프레임 {Time.frameCount}");
        }

        UpdateSectionText(4, "업데이트 타이밍 테스트 끝");
    }

    private async UniTaskVoid OnFixedUpdateClicked()
    {
        UpdateSectionText(4, "OnFixedUpdateClicked");

        for (int i = 0; i < 3; i++)
        {
            await UniTask.Yield(PlayerLoopTiming.FixedUpdate);
            UpdateSectionText(4, $"픽스드 업데이트 프레임 {Time.time}"); //어차피 픽스드 업데이트는 업데이트 프레임이기에 의미가 없다.
        }

        UpdateSectionText(4, "픽스드 업데이트 타이밍 테스트 끝");
    }

    private async UniTaskVoid OnLateUpdateClicked()
    {
        UpdateSectionText(4, "OnLateUpdateClicked");

        for (int i = 0; i < 3; i++)
        {
            await UniTask.Yield(PlayerLoopTiming.LastTimeUpdate);
            UpdateSectionText(4, $"레이트 업데이트 프레임 {Time.frameCount}");
        }

        UpdateSectionText(4, "레이트 업데이트 타이밍 테스트 끝");
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
            UpdateSectionText(5, "테스크 완료");
        }
        catch (OperationCanceledException)
        {
            UpdateSectionText(5, "취소");
        }
    }

    private async UniTask OnTimeOutClicked()
    {
        UpdateSectionText(5, "OnTimeOutClicked");

        var cts = new CancellationTokenSource();
        cts.CancelAfterSlim(TimeSpan.FromSeconds(3)); //지정된 시간 후 자동으로 취소시키는 함수

        try
        {
            await LongTaskAsync(cts.Token);
        }
        catch (OperationCanceledException)
        {
            UpdateSectionText(5, "타임 아웃 취소");
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
                UpdateSectionText(5, "수동 취소");
            }
            else if(timeOutCts.IsCancellationRequested)
            {
                UpdateSectionText(5, "타임아웃 취소");
            }
            else
            {
                UpdateSectionText(5, "OnDestroy 취소");
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
            UpdateSectionText(5, "테스크 없음");
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
            float t = elapsed / duration; //선형보간에 사용할 시간 값

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
            float t = elapsed / duration; //선형보간에 사용할 시간 값

            target.anchoredPosition = Vector2.Lerp(from, to, t);
            await UniTask.Yield();
        }

        target.anchoredPosition = to;
    }

    //duration ; 돌리는 시간
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

        target.rotation = Quaternion.Euler(0f, 0f, from); //원상태로 복귀
    }

    private async UniTaskVoid OnFadeInClicked()
    {
        UpdateSectionText(6, "OnFadeInClicked");

        await FadeAsync(fadePanel, 0f, 1f, 0.5f);

        UpdateSectionText(6, "페이드 인 완료");
    }

    private async UniTaskVoid OnFadeOutClicked()
    {
        UpdateSectionText(6, "OnFadeOutClicked");

        await FadeAsync(fadePanel, 1f, 0f, 0.5f);

        UpdateSectionText(6, "페이드 아웃 완료");
    }

    private async UniTaskVoid OnAnimationClicked()
    {
        UpdateSectionText(6, "OnAnimationClicked");

        var rectTr = animatedCube as RectTransform;
        var originalPos = rectTr.anchoredPosition;

        await MoveToAsync(rectTr, originalPos + Vector2.up * 50f, 0.5f); //위로 이동
        UpdateSectionText(6, "1. 위로 이동");

        await RotateToAsync(animatedCube, 360f, 0.5f); //회전
        UpdateSectionText(6, "2. 회전");

        await MoveToAsync(rectTr, originalPos, 0.5f); //원래 위치로 이동
        UpdateSectionText(6, "3. 원위치로 이동");
    }

    private async UniTaskVoid OnWaitKeyClicked()
    {
        UpdateSectionText(6, "OnWaitKeyClicked");
        await UniTask.WaitUntil(() => Input.anyKey); //아무 키 입력까지 대기
        UpdateSectionText(6, "아무 키 입력 완료");
    }
}
