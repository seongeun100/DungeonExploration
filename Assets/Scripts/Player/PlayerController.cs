using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private LayerMask groundLayerMask;
    [SerializeField] private float staminaCostOnJump;
    public float moveSpeed;
    public float jumpPower;
    private Vector2 curMovementInput;

    [Header("Look")]
    [SerializeField] private Transform cameraContainer;
    [SerializeField] private float minXLook;
    [SerializeField] private float maxXLook;
    [SerializeField] private float lookSensitivity;
    private bool canLook = true;
    private float camCurXRot;
    private Vector2 mouseDelta;

    [Header("Wall Cling")]
    [SerializeField] private float wallCheckDistance = 1f;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private float staminaCostOnClimb = 10f;
    [SerializeField] private float wallClimbForce = 5f;
    private bool isCheckWall;
    private bool isWallCling;

    private Interaction interaction;
    private PlayerCondition condition;
    private CameraSwitch cameraSwitch;

    private Rigidbody _rigidbody;
    public Action inventory;

    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // 마우스 커서 고정
        interaction = GetComponent<Interaction>();
        condition = GetComponent<PlayerCondition>();
        cameraSwitch = GetComponent<CameraSwitch>();
    }

    void Update()
    {
        if (isWallCling)
        {
            // 벽 감지 안되거나 스태미나 부족하면 벽타기 중단
            if (!IsWall() || !condition.UseStamina(staminaCostOnClimb * Time.deltaTime))
            {
                StopWallCling();
            }
            else
            {
                WallMove(); // 벽타기 이동
            }
            return;
        }

        Move(); // 일반 이동
    }


    void LateUpdate()
    {
        if (canLook)
        {
            CameraLook();
        }
    }

    void Move()
    {
        Vector3 dir = transform.forward * curMovementInput.y + transform.right * curMovementInput.x;
        dir *= moveSpeed;
        dir.y = _rigidbody.velocity.y; // y값 유지

        _rigidbody.velocity = dir;
    }

    void WallMove()
    {
        if (curMovementInput == Vector2.zero)
        {
            _rigidbody.velocity = Vector3.zero;
            return;
        }
        Vector3 dir = transform.up * curMovementInput.y + transform.right * curMovementInput.x;

        _rigidbody.AddForce(dir * wallClimbForce, ForceMode.Acceleration);
    }

    // 벽타기 상태 확인용 (PlayerCondition에서 사용)
    public bool IsWallClinging()
    {
        return isWallCling;
    }

    // 카메라 회전
    void CameraLook()
    {
        camCurXRot += mouseDelta.y * lookSensitivity;
        camCurXRot = Mathf.Clamp(camCurXRot, minXLook, maxXLook);
        cameraContainer.localEulerAngles = new Vector3(-camCurXRot, 0, 0);

        transform.eulerAngles += new Vector3(0, mouseDelta.x * lookSensitivity, 0);
    }

    // 이동 입력
    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            curMovementInput = context.ReadValue<Vector2>();
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            curMovementInput = Vector2.zero;
        }
    }

    // 마우스 키 입력
    public void OnLook(InputAction.CallbackContext context)
    {
        mouseDelta = context.ReadValue<Vector2>();
    }

    // 점프 키 입력
    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.phase != InputActionPhase.Started) return;

        if (isWallCling)
        {
            StopWallCling();
        }

        if (IsGrounded() && condition.UseStamina(staminaCostOnJump))
        {
            _rigidbody.AddForce(Vector2.up * jumpPower, ForceMode.Impulse);
        }
    }

    // 상호작용 키 입력(F키)
    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            interaction?.Interact();
        }
    }

    // 바닥에 닿았는지 확인
    bool IsGrounded()
    {
        Ray[] rays = new Ray[4]
        {
            new Ray(transform.position + (transform.forward * 0.2f) + (transform.up * 0.01f), Vector3.down),
            new Ray(transform.position + (-transform.forward * 0.2f) + (transform.up * 0.01f), Vector3.down),
            new Ray(transform.position + (transform.right * 0.2f) + (transform.up * 0.01f), Vector3.down),
            new Ray(transform.position + (-transform.right * 0.2f) + (transform.up * 0.01f), Vector3.down),
        };

        for (int i = 0; i < rays.Length; i++)
        {
            if (Physics.Raycast(rays[i], 1.3f, groundLayerMask))
            {
                return true;
            }
        }
        return false;
    }

    // 인벤토리 키 입력(TAB키)
    public void OnInventory(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            inventory?.Invoke();
            ToggleCusor();
        }
    }

    // 마우스 커서 락
    void ToggleCusor()
    {
        bool toggle = Cursor.lockState == CursorLockMode.Locked;
        Cursor.lockState = toggle ? CursorLockMode.None : CursorLockMode.Locked;
        canLook = !toggle;
    }

    // 카메라 시점 전환 키 입력(V키)
    public void OnSwitchView(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            cameraSwitch?.SwitchView();
        }
    }

    // 벽타기 키 입력(E키)
    public void OnWallCling(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started && !isWallCling)
        {
            if (IsWall() && condition.UseStamina(staminaCostOnClimb))
            {
                WallCling();
            }
        }
    }

    // 벽 감지 확인
    bool IsWall()
    {
        // 현재 오브젝트에 붙은 Collider 가져옴
        CapsuleCollider col = GetComponent<CapsuleCollider>();

        // 캡슐 중심점
        Vector3 center = transform.position + col.center;
        // 캡슐 중싱에서 반구 중심까지 계산
        Vector3 offset = Vector3.up * ((col.height / 2f) - col.radius);

        Vector3 point1 = center + offset; // 위쪽 중심점
        Vector3 point2 = center - offset; // 아래쪽 중심점

        // point1,point2 사이를 기준으로 radius만큼 커진 캡슐을 forward 방향으로 쏴서 감지
        isCheckWall = Physics.CapsuleCast(point1, point2, col.radius, transform.forward, wallCheckDistance, wallLayer);

        return isCheckWall;
    }

    // 벽타기 시작
    void WallCling()
    {
        isWallCling = true; // 벽타기 상태
        _rigidbody.useGravity = false; // 중력 비활성화
        _rigidbody.velocity = Vector3.zero; // 이동속도 초기화
    }

    // 벽타기 종료
    void StopWallCling()
    {
        isWallCling = false;
        _rigidbody.useGravity = true; // 중력 활성화
        Vector3 velocity = _rigidbody.velocity;
        velocity.y = 0f; // 벽타기 끝날시 위로 튀는 현상 방지
        _rigidbody.velocity = velocity;
    }

}
