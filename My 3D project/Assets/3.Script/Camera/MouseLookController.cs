using UnityEngine;
using System.Collections;

public class MouseLookController : MonoBehaviour
{
    [Header("==== 기본 설정 ====")]
    [SerializeField] private Transform playerBody;
    [SerializeField] private Transform cameraPivot;
    [SerializeField] private float mouseSensitivity = 100f;
    [SerializeField] private float clampAngle = 80f;

    [Header("==== 카메라 줌 설정 ====")]
    [SerializeField] private float normalFOV = 60f;
    [SerializeField] private float zoomFOV = 40f;
    [SerializeField] private float zoomInDuration = 0.3f;
    [SerializeField] private float zoomOutDuration = 0.3f;
    [SerializeField] private float zoomStayDuration = 0.2f;

    private float xRotation = 0f;
    private Camera cam;
    private bool isZooming = false;

    private void Start()
    {
        InitCamera();
    }

    private void Update()
    {
        if (playerBody == null || cameraPivot == null)
            return;

        HandleMouseLook();
    }

    private void InitCamera()
    {
        cam = Camera.main;
        if (cam == null)
        {
            Debug.LogError("Camera.main을 찾을 수 없습니다. 카메라가 존재하는지 확인하세요.");
            return;
        }

        cam.fieldOfView = normalFOV;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        playerBody.rotation = Quaternion.identity;
    }

    private void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -clampAngle, clampAngle);

        cameraPivot.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);
    }

    public void ZoomOnce()
    {
        if (!isZooming && cam != null)
            StartCoroutine(ZoomRoutine());
    }

    private IEnumerator ZoomRoutine()
    {
        isZooming = true;

        yield return LerpFOV(cam.fieldOfView, zoomFOV, zoomInDuration);

        yield return new WaitForSeconds(zoomStayDuration);

        yield return LerpFOV(cam.fieldOfView, normalFOV, zoomOutDuration);

        isZooming = false;
    }

    private IEnumerator LerpFOV(float from, float to, float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float t = Mathf.SmoothStep(0f, 1f, elapsedTime / duration);
            cam.fieldOfView = Mathf.Lerp(from, to, t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        cam.fieldOfView = to;
    }

    // 외부에서 민감도 조정할 수 있도록 프로퍼티 제공
    public float MouseSensitivity
    {
        get => mouseSensitivity;
        set => mouseSensitivity = Mathf.Max(0f, value);
    }
}