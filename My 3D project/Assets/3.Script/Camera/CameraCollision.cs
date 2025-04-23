using UnityEngine;

public class CameraCollision : MonoBehaviour
{
    public Transform target; // 따라갈 대상 (예: CameraPivot)
    public float distance = 3f;
    public float minDistance = 0.5f;
    public float smoothSpeed = 10f;
    public float sphereRadius = 0.2f;
    public LayerMask collisionLayers;

    private Vector3 currentPosition;

    void LateUpdate()
    {
        Vector3 desiredPosition = target.position - target.forward * distance;

        // 충돌 체크 (SphereCast 사용)
        RaycastHit hit;
        bool isHit = Physics.SphereCast(
            target.position,
            sphereRadius,
            -target.forward,
            out hit,
            distance,
            collisionLayers
        );

        float targetDistance = isHit ? Mathf.Clamp(hit.distance, minDistance, distance) : distance;
        Vector3 finalPosition = target.position - target.forward * targetDistance;

        // 부드럽게 이동
        currentPosition = Vector3.Lerp(transform.position, finalPosition, Time.deltaTime * smoothSpeed);
        transform.position = currentPosition;

        // 항상 타겟을 바라봄
        transform.LookAt(target);
    }
}