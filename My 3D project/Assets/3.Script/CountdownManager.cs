using UnityEngine;
using TMPro;
using System.Collections;
using System;

public class CountdownManager : MonoBehaviour
{
    [SerializeField] private TMP_Text countdownText;

    public Action OnCountdownFinished; //  카운트 끝났을 때 호출할 콜백

    public void StartCountdown()
    {
        StartCoroutine(CountdownRoutine());
    }

    private IEnumerator CountdownRoutine()
    {
        if (countdownText != null)
            countdownText.gameObject.SetActive(true);

        string[] countdown = { "3", "2", "1", "START!" };

        foreach (var count in countdown)
        {
            if (countdownText != null)
                countdownText.text = count;

            yield return new WaitForSecondsRealtime(1f); //  Realtime로 바꾼다
        }

        if (countdownText != null)
            countdownText.gameObject.SetActive(false);

        OnCountdownFinished?.Invoke();
    }
}