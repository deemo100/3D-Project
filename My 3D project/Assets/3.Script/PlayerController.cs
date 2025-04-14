using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // 이동
    private Rigidbody rb;
    public float dashForce = 10f;
    private bool isDashing = false;

    // 점프
    public float jumpForce = 10f;
    public LayerMask groundLayer;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;

    // 2단 점프 / maxJumpCount = 3 일 경우 3단 점프 
    private int jumpCount = 0;
    public int maxJumpCount = 2;
    
    private int dashCount = 0; // 현재 가능한 돌진 횟수
    
    private bool isFloating = false;  // 공중 정지 상태 여부
    public float floatDuration = 0.05f;  // 공중에 머무는 시간
    
    void Start() {
        // 리지드바디 초기화
        rb = GetComponent<Rigidbody>();
        if (rb == null) 
        {
            Debug.LogError("Rigidbody 컴포넌트가 없습니다!");
        }
    }

    void Update() 
    {
        // 바닥 체크 → 점프 카운트 초기화
        if (IsGrounded()) 
        {
            jumpCount = 0;
            dashCount = 0; // 점프와 함께 돌진도 초기화
        }
        
        if (!IsGrounded() && rb.velocity.y < -0.1f && dashCount < jumpCount) 
        {
            dashCount = jumpCount;
        }
        

        // 점프 입력
        if (jumpCount < maxJumpCount && Input.GetKeyDown(KeyCode.Space)) 
        {
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            float actualJumpForce = (jumpCount == 0) ? jumpForce : jumpForce * 0.5f;
            rb.AddForce(Vector3.up * actualJumpForce, ForceMode.Impulse);

            jumpCount++;
            dashCount++;  // 점프마다 돌진 기회 1회 부여
        }

        // 방향키 돌진
        if (!isDashing) 
        {
            Vector3 inputDir = Vector3.zero;

            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) 
            {
                inputDir = Vector3.forward;
            }
            else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) 
            {
                inputDir = Vector3.back;
            }
            else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) 
            {
                inputDir = Vector3.left;
            }
            else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) 
            {
                inputDir = Vector3.right;
            }

            // 공중이면 1회만, 지상이면 무제한
            if (inputDir != Vector3.zero && (IsGrounded() || dashCount > 0)) 
            {
                StartCoroutine(Dash(inputDir));
                if (!IsGrounded()) 
                {
                    dashCount--;  // 공중에서는 사용 시 차감
                }
            }
        }
        
        // 공중에서만 감지하고, isFloating이 false일 때만 실행
        if (!IsGrounded() && !isFloating && Mathf.Abs(rb.velocity.y) < 0.1f) 
        {
            StartCoroutine(FloatInAir());
        }
        
    }

    // 바닥 체크 여부
    private bool IsGrounded()
    {
        return Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);
    }

    // 대쉬 함수
    IEnumerator Dash(Vector3 direction) 
    {
        isDashing = true;

        // 돌진 거리
        float dashDistance = 10f;

        // 현재 위치 + 일정 방향으로 이동
        Vector3 targetPos = transform.position + direction.normalized * dashDistance;

        rb.velocity = new Vector3(0f, rb.velocity.y, 0f);  // 기존 이동 제거, Y속도는 유지

        float elapsed = 0f;
        float dashTime = 0.3f;

        while (elapsed < dashTime) 
        {
            rb.MovePosition(Vector3.Lerp(transform.position, targetPos, elapsed / dashTime));
            elapsed += Time.deltaTime;
            yield return null;
        }

        rb.MovePosition(targetPos);  // 마지막 위치 보정

        isDashing = false;
    }
    
    IEnumerator FloatInAir() 
    {
        isFloating = true;

        // 중력 제거
        rb.useGravity = false;
        rb.velocity = Vector3.zero;  // 상승/하강 멈추기

        yield return new WaitForSeconds(floatDuration);

        // 다시 중력 활성화
        rb.useGravity = true;

        isFloating = false;
        
        if (!IsGrounded() && !isFloating && Mathf.Abs(rb.velocity.y) < 0.1f) 
        {
            StartCoroutine(FloatInAir());
        }

        // 하강 중 대쉬 보정
        if (!IsGrounded() && rb.velocity.y < -0.1f && dashCount < jumpCount) 
        {
            dashCount = jumpCount;
        }
    }
    
}


// 2번째 점프에서는 공중에 머무르는 거 없이 바로 낙하 지피티한테 물어보기