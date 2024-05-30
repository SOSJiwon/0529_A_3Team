using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;
    public float jumpPower;
    private Vector2 curMovementinput;
    public LayerMask groundLayerMask;

    [Header("Look")]
    public Transform cameraContainer;
    public float minXLook;
    public float maxXLook;
    private float camCurXRot;
    public float lookSensitivity;
    private Vector2 mouseDelta;

    private Rigidbody _rigidbody;
    public Animator _animator;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _animator = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        //마우스 커서가 안보이게 하는 모드
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void LateUpdate()
    {
        CameraLook();
    }

    void Move()
    {
        Vector3 dir = transform.forward * curMovementinput.y + transform.right * curMovementinput.x;
        //커서 인풋으로 앞으로 입력받았다면 (0,1)을 받았을 것, 해당 식으로 계산하면 0(입력받지 않은 움직임)은 0으로 처리
        //입력받은 방향의 움직임 = 1은 움직이게 될 것.
        //여기서 forward와 right에 곱하는 이유는 0,0 에 곱해봐야 1이 되기 때문. 앞뒤는 y에서 양옆은 x에서 관리하므로 해당 방식으로 처리
        //만약 뒤로 간다면 -1이 되기 때문에 곱하기로 그만큼의 값을 받는 것 같음.

        dir *= moveSpeed;
        dir.y = _rigidbody.velocity.y; //점프를 했을 때만 위아래로 움직여야 하기 때문에 값이 변동되지 않도록 설정
        _rigidbody.velocity = dir;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed) //키가 눌렸을 때, 눌리고 나서 실행이 완료 되었을때, 등 페이즈에 따라 설정 가능
        {
            curMovementinput = context.ReadValue<Vector2>(); //키가 눌리면 값을 받아온다.

            _animator.SetBool("IsRun", true);
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            curMovementinput = Vector2.zero;
            _animator.SetBool("IsRun", false);
        }

    }

    void CameraLook()
    {//캐릭터가 좌우로 회전하려면 y 값을 변경해줘야 한다.
        camCurXRot += mouseDelta.y * lookSensitivity; //민감도
        camCurXRot = Mathf.Clamp(camCurXRot, minXLook, maxXLook); //최소값보다 작으면 최소값을, 최대값보다 크면 최대값을 반환

        cameraContainer.localEulerAngles = new Vector3(-camCurXRot, 0, 0);

        transform.eulerAngles += new Vector3(0, mouseDelta.x * lookSensitivity, 0);

        
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        mouseDelta = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed && IsGrounded())
        {
            _rigidbody.AddForce(Vector2.up * jumpPower, ForceMode.Impulse);
            _animator.SetBool("IsJump", true);
        }
        else if(context.phase == InputActionPhase.Canceled)
        {
            _animator.SetBool("IsJump", false);
        }

    }

    bool IsGrounded()
    {
        Ray[] rays = new Ray[4]
        {
            //레이 만드는 법, Ray(위치, 쏘는 방향)
            new Ray(transform.position + (transform.forward * 0.2f) + (transform.up * 0.01f), Vector3.down), //레이 위치를 앞옆으로 옮겨주는중
            new Ray(transform.position + (-transform.forward * 0.2f) + (transform.up * 0.01f), Vector3.down),
            new Ray(transform.position + (transform.right * 0.2f) + (transform.up * 0.01f), Vector3.down),
            new Ray(transform.position + (-transform.right * 0.2f) + (transform.up * 0.01f), Vector3.down)
        };

        for (int i = 0; i < rays.Length; i++)
        {
            if (Physics.Raycast(rays[i], 0.1f, groundLayerMask))
            {
                return true;
            }
        }

        return false;
    }

    //bool FrontRay()
    //{

    //}

}

