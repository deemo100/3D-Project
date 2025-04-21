using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    private static readonly int FORWARD = Animator.StringToHash("Forward");
    private static readonly int BACKWARD = Animator.StringToHash("Backward");
    private static readonly int LEFT = Animator.StringToHash("Left");
    private static readonly int RIGHT = Animator.StringToHash("Right");
    private static readonly int JUMP = Animator.StringToHash("Jump");
    private static readonly int FSPIN = Animator.StringToHash("Fspin");
    private static readonly int ROLL = Animator.StringToHash("Roll");
    private static readonly int ISGROUD = Animator.StringToHash("Isground");

    [Header("이동 설정")]
    public float dashForce = 20f;
    private bool isDashing;

    [Header("점프 설정")]
    public float jumpForce = 8f;
    public int maxJumpCount = 2;
    [SerializeField] private int jumpCount;
    private bool isBoostDash;
    private bool hasAirDashed;

    [Header("지면 체크")]
    public LayerMask groundLayer;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    private bool isGrounded;
    private bool wasGrounded;

    private bool isGroundBoostReady;

    private Rigidbody rigid;
    private TimingManager timingManager;

    [SerializeField] private Animator animator;

    private readonly KeyCode[] triggerKeys = {
        KeyCode.Space, KeyCode.F,
        KeyCode.W, KeyCode.A, KeyCode.S, KeyCode.D
    };

    private int lastJudgement = 3; // 판정 결과 저장
    private bool movementTriggered = false;
    private bool jumpTriggered = false;
    private bool boostTriggered = false;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        if (groundCheck == null)
            Debug.LogError("⚠ groundCheck가 설정되지 않았습니다!");
    }

    private void Start()
    {
        timingManager = FindObjectOfType<TimingManager>();
        if (timingManager == null)
            Debug.LogError("⚠ TimingManager를 찾을 수 없습니다!");
    }

    private void Update()
    {
        HandleGroundCheck();
        HandleNoteInput();
        HandleDashBoostInput();
        HandleJumpInput();
        HandleDashInput();
    }

    // ==================== 노트 입력 및 판정 처리 ====================
    private void HandleNoteInput()
    {
        foreach (KeyCode key in triggerKeys)
        {
            if (Input.GetKeyDown(key))
            {
                lastJudgement = timingManager.CheckDualTiming();
                movementTriggered = false;
                jumpTriggered = false;
                boostTriggered = false;

                if (lastJudgement == 3 && isGroundBoostReady)
                {
                    isGroundBoostReady = false; // Miss → 차지 해제
                    Debug.Log("❌ Miss 판정 → 차지 해제");
                }

                break;
            }
        }
    }

    // ==================== 이동 처리 ====================
    private void HandleDashInput()
    {
        if (isDashing || lastJudgement == 3 || movementTriggered)
            return;

        Vector3 inputDir = Vector3.zero;

        if (Input.GetKeyDown(KeyCode.W))
        {
            inputDir += transform.forward;
            animator.SetTrigger(FORWARD);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            inputDir -= transform.forward;
            animator.SetTrigger(BACKWARD);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            inputDir -= transform.right;
            animator.SetTrigger(LEFT);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            inputDir += transform.right;
            animator.SetTrigger(RIGHT);
        }

        inputDir.y = 0f;
        if (inputDir != Vector3.zero)
        {
            inputDir.Normalize();
            TryDash(inputDir);
            movementTriggered = true;
        }
    }

    private void TryDash(Vector3 direction)
    {
        if (!isGrounded && hasAirDashed) return;

        float force = lastJudgement switch
        {
            0 => dashForce * 1.5f,
            1 => dashForce * 1.5f,
            2 => dashForce,
            _ => 0f,
        };

        if (!isGrounded && isBoostDash)
        {
            force *= 2f;
            isBoostDash = false;
            Debug.Log(" 공중 강화 대시: 2배 거리");
        }

        if (force <= 0f) return;

        StartCoroutine(DashCoroutine(direction, force));

        if (!isGrounded)
            hasAirDashed = true;
    }

    private IEnumerator DashCoroutine(Vector3 direction, float force)
    {
        isDashing = true;

        float duration = 0.2f;
        if (isGroundBoostReady)
        {
            duration *= 2f; // 차지 상태면 이동 거리 2배
            isGroundBoostReady = false;
            Debug.Log("차지: 이동 거리 2배");
        }

        float timer = 0f;
        Vector3 velocity = direction * force;
        velocity.y = rigid.velocity.y;

        while (timer < duration)
        {
            rigid.velocity = velocity;
            timer += Time.deltaTime;
            yield return null;
        }

        rigid.velocity = new Vector3(0f, rigid.velocity.y, 0f);
        isDashing = false;
    }

    // ==================== 차지 입력 ====================
    private void HandleDashBoostInput()
    {
        if (Input.GetKeyDown(KeyCode.F) && isGrounded && !boostTriggered && lastJudgement != 3)
        {
            isGroundBoostReady = true;
            boostTriggered = true;
            animator.SetTrigger(FSPIN); // ✅ 회전 대신 애니메이션만 재생
        }
    }

    // ==================== 점프 입력 ====================
    private void HandleJumpInput()
    {
        if (Input.GetKeyDown(KeyCode.Space) && jumpCount < maxJumpCount && !jumpTriggered && lastJudgement != 3)
        {
            rigid.velocity = new Vector3(rigid.velocity.x, 0f, rigid.velocity.z);

            float force = (jumpCount == 0) ? jumpForce : jumpForce * 0.8f;
            rigid.AddForce(Vector3.up * force, ForceMode.Impulse);

            isBoostDash = (jumpCount == 1);
            jumpCount++;
            jumpTriggered = true;


            if (jumpCount == 1)
            {
                animator.SetTrigger(JUMP); // 1단 점프
            }
            else
            {
                animator.SetTrigger(ROLL);
            }
            
            
            if (isGroundBoostReady)
            {
                isGroundBoostReady = false;
                Debug.Log(" 점프 시 차지 해제");
            }

            Debug.Log($"{jumpCount}단 점프 실행");
        }
    }

    // ==================== 지면 체크 ====================
  
    private void HandleGroundCheck()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);

        // 착지했을 때
        if (isGrounded && !wasGrounded)
        {
            jumpCount = 0;
            hasAirDashed = false;
            animator.SetBool(ISGROUD, true); // 착지 애니메이션 상태
        }

        // 공중에 떠 있는 동안
        if (!isGrounded && wasGrounded)
        {
            animator.SetBool(ISGROUD, false);
        }

        wasGrounded = isGrounded;
    }

    // ==================== 디버그 시각화 ====================
    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}
