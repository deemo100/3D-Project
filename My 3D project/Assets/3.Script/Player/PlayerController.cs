using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("애니메이션")]
    [SerializeField] private Animator animator;
    private static readonly int FORWARD = Animator.StringToHash("Forward");
    private static readonly int BACKWARD = Animator.StringToHash("Backward");
    private static readonly int LEFT = Animator.StringToHash("Left");
    private static readonly int RIGHT = Animator.StringToHash("Right");
    private static readonly int JUMP = Animator.StringToHash("Jump");
    private static readonly int FSPIN = Animator.StringToHash("Fspin");
    private static readonly int ROLL = Animator.StringToHash("Roll");
    private static readonly int ISGROUD = Animator.StringToHash("Isground");
    private static readonly int BOUNCE = Animator.StringToHash("Bounce");
    private static readonly int DEAD = Animator.StringToHash("Dead");

    [Header("이동 설정")]
    public float dashForce = 20f;
    private bool isDashing;

    [Header("점프 설정")]
    public float jumpForce = 8f;
    public int maxJumpCount = 2;
    [SerializeField] private int jumpCount;
    private bool isBoostDash;

    [Header("지면 체크")]
    public LayerMask groundLayer;
    public Transform groundCheck;
    public Vector3 groundCheckBoxSize = new Vector3(0.4f, 0.1f, 0.4f);

    private bool isGrounded;
    private bool wasGrounded;
    private bool isGroundBoostReady;

    private Rigidbody rigid;
    private TimingManager timingManager;

    private readonly KeyCode[] triggerKeys = {
        KeyCode.Space, KeyCode.F,
        KeyCode.W, KeyCode.A, KeyCode.S, KeyCode.D
    };

    private int lastJudgement = 3;
    private bool movementTriggered = false;
    private bool jumpTriggered = false;
    private bool boostTriggered = false;

    public static bool noMovePlayer = true;

    private int airDashCount = 0;
    private const int maxAirDashCount = 1;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        if (groundCheck == null)
            Debug.LogError(" groundCheck가 설정되지 않았습니다!");
    }

    private void Start()
    {
        timingManager = FindObjectOfType<TimingManager>();
        if (timingManager == null)
            Debug.LogError(" TimingManager를 찾을 수 없습니다!");

        noMovePlayer = false; //  게임 시작 시 입력 활성화
    }

    private void Update()
    {
        //  게임 오버나 클리어 시 입력 차단
        if (GameManager.Instance != null && (GameManager.Instance.IsGameClear || GameManager.Instance.IsGameOver))
            return;

        if (noMovePlayer) return;

        HandleGroundCheck();
        HandleNoteInput();
        HandleDashBoostInput();
        HandleJumpInput();
        HandleDashInput();
    }

    // ==================== 노트 입력 ====================
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
                    isGroundBoostReady = false;
                }

                if (key == KeyCode.F && !isGrounded && lastJudgement != 3)
                {
                    PerformAirFall(lastJudgement);
                }

                break;
            }
        }
    }

    // ==================== 대시 이동 ====================
    private void HandleDashInput()
    {
        if (isDashing || lastJudgement == 3 || movementTriggered)
            return;

        Vector3 inputDir = Vector3.zero;

        if (Input.GetKeyDown(KeyCode.W)) { inputDir += transform.forward; animator.SetTrigger(FORWARD); }
        if (Input.GetKeyDown(KeyCode.S)) { inputDir -= transform.forward; animator.SetTrigger(BACKWARD); }
        if (Input.GetKeyDown(KeyCode.A)) { inputDir -= transform.right; animator.SetTrigger(LEFT); }
        if (Input.GetKeyDown(KeyCode.D)) { inputDir += transform.right; animator.SetTrigger(RIGHT); }

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
        if (!isGrounded && airDashCount >= maxAirDashCount)
            return;

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
        }

        if (force <= 0f) return;

        StartCoroutine(DashCoroutine(direction, force));
    }

    private IEnumerator DashCoroutine(Vector3 direction, float force)
    {
        isDashing = true;

        float duration = 0.2f;
        if (isGroundBoostReady)
        {
            duration *= 2f;
            isGroundBoostReady = false;
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

        if (!isGrounded)
        {
            airDashCount++;
        }
    }

    // ==================== 차지 입력 (F키) ====================
    private void HandleDashBoostInput()
    {
        if (Input.GetKeyDown(KeyCode.F) && isGrounded && !boostTriggered && lastJudgement != 3)
        {
            isGroundBoostReady = true;
            boostTriggered = true;
            animator.SetTrigger(FSPIN);
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
                animator.SetTrigger(JUMP);
            }
            if (jumpCount == 2)
            {
                animator.SetTrigger(ROLL);
                airDashCount = 0;
            }

            if (isGroundBoostReady)
            {
                isGroundBoostReady = false;
            }
        }
    }

    private void PerformAirFall(int judgement)
    {
        float fallPower = jumpForce * 4f;

        rigid.velocity = new Vector3(0f, -fallPower, 0f);

        animator.SetTrigger(BOUNCE);

        EffectManager effectManager = FindObjectOfType<EffectManager>();
        if (effectManager != null)
        {
            effectManager.JudgementHitEffect(judgement);
            effectManager.NoteHitEffect();
        }
        
    }

    // ==================== 지면 체크 ====================
    private void HandleGroundCheck()
    {
        isGrounded = Physics.CheckBox
        (
            groundCheck.position,
            groundCheckBoxSize / 2f,
            Quaternion.identity,
            groundLayer
        );

        if (isGrounded && !wasGrounded)
        {
            jumpCount = 0;
            airDashCount = 0;
            animator.SetBool(ISGROUD, true);
        }

        if (!isGrounded && wasGrounded)
        {
            animator.SetBool(ISGROUD, false);
        }

        wasGrounded = isGrounded;
    }

    public void TriggerDeathAnimation()
    {
        animator.SetTrigger(DEAD);
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(groundCheck.position, groundCheckBoxSize);
    }
}