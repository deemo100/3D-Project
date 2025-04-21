using UnityEngine;
using TMPro;

public class ComboManager : MonoBehaviour
{
    [Header("UI 참조")]
    [SerializeField] private GameObject comboImage = null;
    [SerializeField] private TMP_Text comboText = null;

    private int currentCombo = 0;

    private void Start()
    {
        comboText.text = "000"; 
        comboText.gameObject.SetActive(true);
        if (comboImage != null)
            comboImage.SetActive(true);
    }

    /// <summary>
    /// 콤보 증가 처리
    /// </summary>
    public void IncrementCombo(int amount = 1)
    {
        currentCombo += amount;
        comboText.text = currentCombo.ToString("D3"); //  항상 3자리 숫자
    }

    /// <summary>
    /// 콤보 리셋 처리
    /// </summary>
    public void ResetCombo()
    {
        currentCombo = 0;
        comboText.text = "000"; 
    }
}