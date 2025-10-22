using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AutoSaveAndDebouncing : MonoBehaviour
{
    private enum SaveType
    {
        Auto,
        Manual,
        Debounced
    }

    public Button saveBtn;
    public TMP_InputField field;

    private string str = "";

    private bool isSaving = false;

    private DateTime lastSavedTime;

    private CancellationTokenSource autoSaveCts;
    private CancellationTokenSource debounceCts;

    [SerializeField] private float autoSaveInterval = 5f;
    [SerializeField] private float debounceDelay = 3f;

    private async UniTask ThirtySecondAutoSave()
    {
        while (true)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(30));
        }
    }
}
