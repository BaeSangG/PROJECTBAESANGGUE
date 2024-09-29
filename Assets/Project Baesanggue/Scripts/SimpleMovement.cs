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

        private float verticalVelocity; // 수직 속도
        private bool isGrounded; // 땅에 붙어있는지 여부
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

            // transform.right : X축 방향 벡터, input.x : X축 방향 입력값,
            // transform.forward : Z축 방향 벡터, input.y : Z축 방향 입력값
            Vector3 finalDirection = (transform.right * input.x) + (transform.forward * input.y);
            Vector3 movement =
                (finalDirection * Time.deltaTime * moveSpeed) +     // finalDirection 방향으로 X/Z 축 이동하는 수식
                (Vector3.up * verticalVelocity * Time.deltaTime);   // 방향으로 Y 축 기준으로 verticalVelocity 이동하는 수식

            unityCharacterController.Move(movement);
        }

        public void GroundCheck()
        {
            // 캐릭터의 발바닥 위치 + Offset 값 만큼 아래쪽 위치를 구해본다.
            Vector3 spherePosition = transform.position + (Vector3.down * groundOffset);

            // 위에서 구한 위치에 Sphere를 하나 생성해보고, Sphere가 groundLayer 와 충돌하는지 검사한다.
            isGrounded = Physics.CheckSphere(spherePosition, groundRadius, groundLayer, QueryTriggerInteraction.Ignore);
        }

        public void FreeFall()
        {
            if (isGrounded == false) // 캐릭터가 땅에 있지 않다면 => 수직 속도를 중력 값을 가중시킨다.
            {
                verticalVelocity += Physics.gravity.y * Time.deltaTime;
            }
            else // 땅에 닿아있으니까 수직 속도를 0으로 만든다.
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

