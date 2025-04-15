using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // 이동
    private Rigidbody rb;
    public float dashForce = 20f;
    private bool isDashing = false;

    // 점프
    public LayerMask dashBlockLayer;
    public LayerMask groundLayer;
    public Transform groundCheck;

    public float jumpForce = 8f;
    public float groundCheckRadius = 0.1f;

    // 점프 조건 강화
    private float groundedTimer;

    // 점프 관련
    [SerializeField]
    private int jumpCount;
    public int maxJumpCount = 2;

    private bool hasAirDashed = false;
    private float lastJumpTime = -1f;

    // 대시 강화
    private bool isBoostDash = false;
    private bool isRotating = false;
    
    private bool wasGrounded = false;
    
    private bool isGroundBoostReady = false; // 딱 한 번만 사용되도록 따로 분리
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (rb == null)
        {
            Debug.LogError("Rigidbody 컴포넌트가 없습니다!");
        }

        if (groundCheck != null)
        {
            groundCheck.localPosition = new Vector3(0, -0.9f, 0);
        }
        else
        {
            Debug.LogError("groundCheck 오브젝트가 연결되어 있지 않습니다!");
        }
    }
    
    void Update()
    {
        bool isCurrentlyGrounded = IsGrounded();

        if (isCurrentlyGrounded && !wasGrounded)
        {
            Debug.Log(">> 착지! 점프 카운트 리셋");
            jumpCount = 0;
            hasAirDashed = false;
        }

        wasGrounded = isCurrentlyGrounded;
        
        
        // 점프
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (jumpCount < maxJumpCount)
            {
                rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

                if (jumpCount == 0)
                {
                    rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                    Debug.Log("1단 점프 확인");
                    isBoostDash = false; // 1단 점프는 기본 상태
                }
                else if (jumpCount == 1)
                {
                    rb.AddForce(Vector3.up * (jumpForce * 0.8f), ForceMode.Impulse);
                    Debug.Log("2단 점프 확인");

                    isBoostDash = true; // 2단 점프 시 강화 대시
                }

                jumpCount++;
                lastJumpTime = Time.time;
            }
        }

        // 대시 입력
        if (!isDashing)
        {
            Vector3 inputDir = Vector3.zero;

            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) inputDir += transform.forward;
            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) inputDir -= transform.forward;
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) inputDir -= transform.right;
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) inputDir += transform.right;

            inputDir.y = 0f;
            inputDir.Normalize();

            if (inputDir != Vector3.zero && (IsGrounded() || !hasAirDashed) && !isRotating)
            {
                float currentDashForce = dashForce;

                // 지상에서의 강화 대시 적용 후 무효화
                if (isGroundBoostReady && IsGrounded())
                {
                    currentDashForce *= 2f;
                    isGroundBoostReady = false;
                    Debug.Log("F키 지상 강화 대시 사용!");
                }
                // 2단 점프 이후 공중 대시도 강화 적용
                else if (isBoostDash && !IsGrounded())
                {
                    currentDashForce *= 2f;
                    isBoostDash = false;
                    Debug.Log("2단 점프 강화 공중 대시 사용!");
                }

                StartCoroutine(DashVelocity(inputDir, currentDashForce));

                if (!IsGrounded())
                {
                    hasAirDashed = true;
                }
            }
        }
        
        // F키 대시 강화 (지상에서만 허용)
        if (Input.GetKeyDown(KeyCode.F) && !isRotating && IsGrounded())
        {
            StartCoroutine(RotatePlayer(0.3f));
            isGroundBoostReady = true; // 플래그를 true로 설정 (대시 시 사용됨)
        }
    }

    private bool IsGrounded()
    {
        float rayLength = 0.2f;
        return Physics.Raycast(groundCheck.position, Vector3.down, rayLength, groundLayer);
    }

    IEnumerator DashVelocity(Vector3 direction, float speed)
    {
        isDashing = true;

        float duration = 0.2f;
        float timer = 0f;

        Vector3 dashVelocity = direction * speed;
        dashVelocity.y = rb.velocity.y; // y속도 유지 (점프와 충돌 방지)

        while (timer < duration)
        {
            rb.velocity = dashVelocity;
            timer += Time.deltaTime;
            yield return null;
        }

        rb.velocity = new Vector3(0f, rb.velocity.y, 0f); // 대시 후 멈춤

        isDashing = false;
    }

    IEnumerator RotatePlayer(float duration)
    {
        isRotating = true;

        float elapsed = 0f;
        float startRotation = transform.eulerAngles.y;
        float endRotation = startRotation + 360f;

        while (elapsed < duration)
        {
            float yRotation = Mathf.Lerp(startRotation, endRotation, elapsed / duration);
            transform.eulerAngles = new Vector3(0f, yRotation, 0f);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.eulerAngles = new Vector3(0f, endRotation, 0f);
        isRotating = false;
    }
}