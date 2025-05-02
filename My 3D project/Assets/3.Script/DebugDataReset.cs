using UnityEngine;

public class DebugDataReset : MonoBehaviour
{
    void Start()
    {
        if (!PlayerPrefs.HasKey("ResetOnce"))
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.SetInt("ResetOnce", 1); // 다시 초기화되지 않도록 플래그 저장
            PlayerPrefs.Save();
            Debug.Log("🎯 빌드 환경에서 PlayerPrefs 전체 초기화됨 (최초 1회)");
        }
    }
}