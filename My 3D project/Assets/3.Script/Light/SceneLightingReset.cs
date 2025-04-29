using UnityEngine;
using UnityEngine.Rendering;

public class SceneLightingReset : MonoBehaviour
{
    void Start()
    {
        // ✅ 기본 환경 조명 모드 설정
        RenderSettings.ambientMode = AmbientMode.Skybox;
        RenderSettings.ambientLight = Color.white;
        RenderSettings.defaultReflectionMode = DefaultReflectionMode.Skybox;
        DynamicGI.UpdateEnvironment();
        
    }
}