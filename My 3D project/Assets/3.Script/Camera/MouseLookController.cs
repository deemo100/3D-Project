using UnityEngine;

public class MouseLookController : MonoBehaviour
{
    public Transform playerBody;       // 캐릭터 (Y축 회전용)
    public Transform cameraPivot;      // 카메라 상하회전 중심 (빈 오브젝트)
    public float mouseSensitivity = 100f;
    public float clampAngle = 80f;

    private float xRotation = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;  // 마우스 고정
        Cursor.visible = false;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // 마우스 Y → 위아래 회전 (카메라만)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -clampAngle, clampAngle);
        cameraPivot.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // 마우스 X → 좌우 회전 (플레이어 자체 회전)
        playerBody.Rotate(Vector3.up * mouseX);
    }
}