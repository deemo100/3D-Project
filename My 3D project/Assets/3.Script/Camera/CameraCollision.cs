using UnityEngine;

public class CameraCollision : MonoBehaviour
{
    [Header("Target Settings")]
    [SerializeField] private Transform target;

    [Header("Collision Settings")]
    [SerializeField] private float maxDistance = 3f;
    [SerializeField] private float minDistance = 0.5f;
    [SerializeField] private float sphereRadius = 0.2f;
    [SerializeField] private LayerMask collisionLayers;

    [Header("Movement Settings")]
    [SerializeField] private float smoothSpeed = 10f;

    private Vector3 currentVelocity;
    private RaycastHit hitInfo;

    private void LateUpdate()
    {
        if (target == null)
            return;

        HandleCollision();
        LookAtTarget();
    }

    private void HandleCollision()
    {
        Vector3 desiredPosition = target.position - target.forward * maxDistance;

        bool hasCollision = Physics.SphereCast(
            origin: target.position,
            radius: sphereRadius,
            direction: -target.forward,
            hitInfo: out hitInfo,
            maxDistance: maxDistance,
            layerMask: collisionLayers
        );

        float adjustedDistance = hasCollision 
            ? Mathf.Clamp(hitInfo.distance, minDistance, maxDistance)
            : maxDistance;

        Vector3 targetPosition = target.position - target.forward * adjustedDistance;

        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * smoothSpeed);
    }

    private void LookAtTarget()
    {
        transform.LookAt(target);
    }

    // public 메서드로 타겟 설정 가능
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}