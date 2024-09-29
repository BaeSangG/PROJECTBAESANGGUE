using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BSG
{
    public class SimpleInputSystem : MonoBehaviour
    {
        public static SimpleInputSystem Instance { get; private set; } = null;
        #region Awake/Destroy
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }
        private void OnDestroy()
        {
            if(Instance != null)
            {
                Instance = null;
            }
        }
        #endregion
        public Vector2 moveInput;
        public float rotation;

        public delegate void OnJumpCallBack();
        public OnJumpCallBack onJumpCallBack;

        private void Update()
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            moveInput = new Vector2(horizontal, vertical);

            rotation = 0f;
            if (Input.GetKey(KeyCode.Q))
            {
                rotation -= 1;
            }
            if (Input.GetKey(KeyCode.E)) 
            {
                rotation += 1;
            }

            if (Input.GetKey(KeyCode.Space)) 
            {
                onJumpCallBack();
            }
        }

    }
}
