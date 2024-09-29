using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BSG
{
    public class SimpleMovement : MonoBehaviour
    {
        public float moveSpeed = 5f;
        public float rotationSpeed;
        public float jumpForce;


        public float groundRadius = 0.1f;
        public float groundOffset = 0.1f;
        public LayerMask groundLayer;

        public float groundDistance = 0.1f;

        private float verticalVelocity; // ���� �ӵ�
        private bool isGrounded; // ���� �پ��ִ��� ����
        private float jumpTimeout = 0.1f;
        private float jumpTimeoutDelta = 0f;


        private UnityEngine.CharacterController unityCharacterController;

        private void Awake()
        {
            unityCharacterController = GetComponent<CharacterController>();
        }

        private void Start()
        {
            SimpleInputSystem.Instance.onJumpCallBack += Jump;
        }

        private void Jump()
        {
            if (isGrounded == false)
                return;

            verticalVelocity = jumpForce;
            jumpTimeoutDelta = jumpTimeout;
        }

        private void Update()
        {
            GroundCheck();
            FreeFall();

            Vector2 input = SimpleInputSystem.Instance.moveInput;

            float inputRotation = SimpleInputSystem.Instance.rotation;
            float currentRot = transform.rotation.eulerAngles.y;
            currentRot += (inputRotation * Time.deltaTime * rotationSpeed);
            transform.rotation = Quaternion.Euler(0, currentRot, 0);

            // transform.right : X�� ���� ����, input.x : X�� ���� �Է°�,
            // transform.forward : Z�� ���� ����, input.y : Z�� ���� �Է°�
            Vector3 finalDirection = (transform.right * input.x) + (transform.forward * input.y);
            Vector3 movement =
                (finalDirection * Time.deltaTime * moveSpeed) +     // finalDirection �������� X/Z �� �̵��ϴ� ����
                (Vector3.up * verticalVelocity * Time.deltaTime);   // �������� Y �� �������� verticalVelocity �̵��ϴ� ����

            unityCharacterController.Move(movement);
        }

        public void GroundCheck()
        {
            // ĳ������ �߹ٴ� ��ġ + Offset �� ��ŭ �Ʒ��� ��ġ�� ���غ���.
            Vector3 spherePosition = transform.position + (Vector3.down * groundOffset);

            // ������ ���� ��ġ�� Sphere�� �ϳ� �����غ���, Sphere�� groundLayer �� �浹�ϴ��� �˻��Ѵ�.
            isGrounded = Physics.CheckSphere(spherePosition, groundRadius, groundLayer, QueryTriggerInteraction.Ignore);
        }

        public void FreeFall()
        {
            if (isGrounded == false) // ĳ���Ͱ� ���� ���� �ʴٸ� => ���� �ӵ��� �߷� ���� ���߽�Ų��.
            {
                verticalVelocity += Physics.gravity.y * Time.deltaTime;
            }
            else // ���� ��������ϱ� ���� �ӵ��� 0���� �����.
            {
                if (jumpTimeoutDelta > 0)
                {
                    jumpTimeoutDelta -= Time.deltaTime;
                }
                else
                {
                    verticalVelocity = 0f;
                }
            }
        }
    }
}

