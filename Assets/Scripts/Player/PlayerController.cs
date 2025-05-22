using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;
    public float jumpPower;
    private Vector2 curMovementInput;
    public LayerMask groundLayerMask;

    [Header("Look")]
    public Transform cameraContainer;
    public float minXLook;
    public float maxXLook;
    private float camCurXRot;
    public float lookSensitivity;
    private Vector2 mouseDelta;
    public bool canLook = true;

    [Header("Wall Cling")]
    public float wallCheckDistance = 1f;
    public LayerMask wallLayer;
    public float staminaCostOnClimb = 10f;
    private bool isCheckWall;
    public float wallClimbForce = 5f;

    private bool isWallCling = false;

    private Interaction interaction;
    private PlayerCondition condition;
    private float staminaCostOnJump = 10f;
    private CameraSwitch cameraSwitch;

    private Rigidbody _rigidbody;
    internal Action inventory;

    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        interaction = GetComponent<Interaction>();
        condition = GetComponent<PlayerCondition>();
        cameraSwitch = GetComponent<CameraSwitch>();
    }

    void Update()
    {
        if (isWallCling)
        {
            // 매 프레임 스태미나 소모
            if (!IsWall() || !condition.UseStamina(staminaCostOnClimb * Time.deltaTime))
            {
                StopWallCling();
            }
            else
            {
                WallMove();
            }
            return;
        }

        Move();
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
        dir.y = _rigidbody.velocity.y;

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
    public bool IsWallClinging()
    {
        return isWallCling;
    }

    void CameraLook()
    {
        camCurXRot += mouseDelta.y * lookSensitivity;
        camCurXRot = Mathf.Clamp(camCurXRot, minXLook, maxXLook);
        cameraContainer.localEulerAngles = new Vector3(-camCurXRot, 0, 0);

        transform.eulerAngles += new Vector3(0, mouseDelta.x * lookSensitivity, 0);
    }

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

    public void OnLook(InputAction.CallbackContext context)
    {
        mouseDelta = context.ReadValue<Vector2>();
    }

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

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            interaction?.Interact();
        }
    }

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

    public void OnInventory(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            inventory?.Invoke();
            ToggleCusor();
        }
    }

    void ToggleCusor()
    {
        bool toggle = Cursor.lockState == CursorLockMode.Locked;
        Cursor.lockState = toggle ? CursorLockMode.None : CursorLockMode.Locked;
        canLook = !toggle;
    }

    public void OnSwitchView(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            cameraSwitch?.SwitchView();
        }
    }

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

    void WallCling()
    {
        isWallCling = true;
        _rigidbody.useGravity = false;
        _rigidbody.velocity = Vector3.zero;
    }
    void StopWallCling()
    {
        isWallCling = false;
        _rigidbody.useGravity = true;
        Vector3 velocity = _rigidbody.velocity;
        velocity.y = 0f;
        _rigidbody.velocity = velocity;
    }

}
