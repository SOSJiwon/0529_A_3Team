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
        //���콺 Ŀ���� �Ⱥ��̰� �ϴ� ���
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
        //Ŀ�� ��ǲ���� ������ �Է¹޾Ҵٸ� (0,1)�� �޾��� ��, �ش� ������ ����ϸ� 0(�Է¹��� ���� ������)�� 0���� ó��
        //�Է¹��� ������ ������ = 1�� �����̰� �� ��.
        //���⼭ forward�� right�� ���ϴ� ������ 0,0 �� ���غ��� 1�� �Ǳ� ����. �յڴ� y���� �翷�� x���� �����ϹǷ� �ش� ������� ó��
        //���� �ڷ� ���ٸ� -1�� �Ǳ� ������ ���ϱ�� �׸�ŭ�� ���� �޴� �� ����.

        dir *= moveSpeed;
        dir.y = _rigidbody.velocity.y; //������ ���� ���� ���Ʒ��� �������� �ϱ� ������ ���� �������� �ʵ��� ����
        _rigidbody.velocity = dir;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed) //Ű�� ������ ��, ������ ���� ������ �Ϸ� �Ǿ�����, �� ����� ���� ���� ����
        {
            curMovementinput = context.ReadValue<Vector2>(); //Ű�� ������ ���� �޾ƿ´�.

            _animator.SetBool("IsRun", true);
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            curMovementinput = Vector2.zero;
            _animator.SetBool("IsRun", false);
        }

    }

    void CameraLook()
    {//ĳ���Ͱ� �¿�� ȸ���Ϸ��� y ���� ��������� �Ѵ�.
        camCurXRot += mouseDelta.y * lookSensitivity; //�ΰ���
        camCurXRot = Mathf.Clamp(camCurXRot, minXLook, maxXLook); //�ּҰ����� ������ �ּҰ���, �ִ밪���� ũ�� �ִ밪�� ��ȯ

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
            //���� ����� ��, Ray(��ġ, ��� ����)
            new Ray(transform.position + (transform.forward * 0.2f) + (transform.up * 0.01f), Vector3.down), //���� ��ġ�� �տ����� �Ű��ִ���
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

