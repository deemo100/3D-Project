using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
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
    private bool isRotating;

    private Rigidbody rigid;
    private TimingManager timingManager;

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
        if (isDashing || isRotating || lastJudgement == 3 || movementTriggered)
            return;

        Vector3 inputDir = Vector3.zero;

        if (Input.GetKeyDown(KeyCode.W)) inputDir += transform.forward;
        if (Input.GetKeyDown(KeyCode.S)) inputDir -= transform.forward;
        if (Input.GetKeyDown(KeyCode.A)) inputDir -= transform.right;
        if (Input.GetKeyDown(KeyCode.D)) inputDir += transform.right;

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

        // ✅ 2단 점프 후 강화 대시
        if (!isGrounded && isBoostDash)
        {
            force *= 2f;
            isBoostDash = false;
            Debug.Log("🕊️ 공중 강화 대시: 2배 거리");
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
            Debug.Log("🌀 차지: 이동 거리 2배");
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
        if (Input.GetKeyDown(KeyCode.F) && isGrounded && !isRotating && !boostTriggered && lastJudgement != 3)
        {
            isGroundBoostReady = true;
            StartCoroutine(RotatePlayer(0.3f));
            boostTriggered = true;
        }
    }

    private IEnumerator RotatePlayer(float duration)
    {
        isRotating = true;

        float elapsed = 0f;
        float startY = transform.eulerAngles.y;
        float endY = startY + 360f;

        while (elapsed < duration)
        {
            float yRotation = Mathf.Lerp(startY, endY, elapsed / duration);
            transform.eulerAngles = new Vector3(0f, yRotation, 0f);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.eulerAngles = new Vector3(0f, endY, 0f);
        isRotating = false;
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

            if (isGroundBoostReady)
            {
                isGroundBoostReady = false; // 점프 시 차지 해제
                Debug.Log("🧯 점프 시 차지 해제");
            }

            Debug.Log($"{jumpCount}단 점프 실행");
        }
    }

    // ==================== 지면 체크 ====================
    private void HandleGroundCheck()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);

        if (isGrounded && !wasGrounded)
        {
            jumpCount = 0;
            hasAirDashed = false;
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