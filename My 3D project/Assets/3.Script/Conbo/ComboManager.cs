using UnityEngine;
using TMPro;
using System.Collections;

public class ComboManager : MonoBehaviour
{
    [Header("UI 참조")]
    [SerializeField] private GameObject comboImage = null;
    [SerializeField] private TMP_Text comboText = null;

    [Header("팝 애니메이션 설정")]
    [SerializeField] private float popScale = 1.2f;  // 팝 때 커지는 비율
    [SerializeField] private float popDuration = 0.1f; // 팝 애니메이션 속도

    private int currentCombo = 0;
    private int maxCombo = 0; //  최대 콤보 기록
    private Coroutine popCoroutine;


    private void Start()
    {
        comboText.text = "000"; 
        comboText.gameObject.SetActive(true);
        if (comboImage != null)
            comboImage.SetActive(true);
    }

    public void IncrementCombo(int amount = 1)
    {
        currentCombo += amount;
        comboText.text = currentCombo.ToString("D3");

        if (currentCombo > maxCombo)
        {
            maxCombo = currentCombo;
        }

        PlayPopAnimation();
    }


    public void ResetCombo()
    {
        currentCombo = 0;
        comboText.text = "000"; 
    }

    public int GetMaxCombo()
    {
        return maxCombo;
    }

    
    private void PlayPopAnimation()
    {
        if (popCoroutine != null)
            StopCoroutine(popCoroutine);

        popCoroutine = StartCoroutine(PopAnimationCoroutine());
    }

    private IEnumerator PopAnimationCoroutine()
    {
        Vector3 originalScale = comboText.transform.localScale;
        Vector3 targetScale = originalScale * popScale;

        float timer = 0f;

        // 커지기
        while (timer < popDuration)
        {
            comboText.transform.localScale = Vector3.Lerp(originalScale, targetScale, timer / popDuration);
            timer += Time.deltaTime;
            yield return null;
        }
        comboText.transform.localScale = targetScale;

        timer = 0f;

        // 원래 크기로 돌아오기
        while (timer < popDuration)
        {
            comboText.transform.localScale = Vector3.Lerp(targetScale, originalScale, timer / popDuration);
            timer += Time.deltaTime;
            yield return null;
        }
        comboText.transform.localScale = originalScale;
    }
}