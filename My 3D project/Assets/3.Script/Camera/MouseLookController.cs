using UnityEngine;
using System.Collections;

public class MouseLookController : MonoBehaviour
{
    [Header("==== 기본 설정 ====")]
    public Transform playerBody;
    public Transform cameraPivot;
    public float mouseSensitivity = 100f;
    public float clampAngle = 80f;

    [Header("==== 카메라 줌 설정 ====")]
    [SerializeField] private float normalFOV = 60f;    // 기본 시야각
    [SerializeField] private float zoomFOV = 40f;       // 줌인 시야각
    [SerializeField] private float zoomInDuration = 0.3f;  // 줌 인 걸리는 시간
    [SerializeField] private float zoomOutDuration = 0.3f; // 줌 아웃 걸리는 시간
    [SerializeField] private float zoomStayDuration = 0.2f; // 줌 유지 시간

    private float xRotation = 0f;
    private Camera cam;
    private bool isZooming = false;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        cam = Camera.main;
        cam.fieldOfView = normalFOV;

        // 시작할 때 플레이어 회전 초기화 (정면)
        playerBody.rotation = Quaternion.Euler(0f, 0f, 0f);
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -clampAngle, clampAngle);
        cameraPivot.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        playerBody.Rotate(Vector3.up * mouseX);
    }

    // ===== 외부 호출용 =====
    public void ZoomOnce()
    {
        if (!isZooming)
            StartCoroutine(ZoomRoutine());
    }

    // ===== 줌 루틴 =====
    private IEnumerator ZoomRoutine()
    {
        isZooming = true;

        float elapsedTime = 0f;
        float startFOV = cam.fieldOfView;

        // === 줌 인 (부드러운 가속/감속) ===
        while (elapsedTime < zoomInDuration)
        {
            float t = Mathf.SmoothStep(0f, 1f, elapsedTime / zoomInDuration);
            cam.fieldOfView = Mathf.Lerp(startFOV, zoomFOV, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        cam.fieldOfView = zoomFOV;

        // === 줌 유지 ===
        yield return new WaitForSeconds(zoomStayDuration);

        elapsedTime = 0f;
        startFOV = cam.fieldOfView;

        // === 줌 아웃 (부드러운 가속/감속) ===
        while (elapsedTime < zoomOutDuration)
        {
            float t = Mathf.SmoothStep(0f, 1f, elapsedTime / zoomOutDuration);
            cam.fieldOfView = Mathf.Lerp(startFOV, normalFOV, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        cam.fieldOfView = normalFOV;

        isZooming = false;
    }
}