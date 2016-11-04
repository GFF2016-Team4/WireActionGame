using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace Player
{
    [RequireComponent(typeof(Animator))]
    public class PlayerMove : MonoBehaviour
    {
        [SerializeField, Tooltip("説明文")]
        private Transform playerCamera;

        private Animator            animator;
        private CharacterController controller;
        private float               gravity;

        void Awake()
        {
            animator   = GetComponent<Animator>();
            controller = GetComponent<CharacterController>();

            //物理演算と出来る限り同じ挙動にするため
            gravity = Physics.gravity.y;
        }

        void Update()
        {        
            Vector3 right;
            Vector3 forward;

            getCameraAxis(out forward, out right);

            Vector3 velocity = GetInputVelocity(forward, right);
            float speed = velocity.magnitude;

            //アニメーションで移動させる
            animator.SetFloat("MoveSpeed", speed);

            //velocityがゼロベクトルだとだと変な方向に向くため
            if(velocity != Vector3.zero)
            {
                LookRotation(velocity);
            }

            //地面についていない場合
            if(!controller.isGrounded)
            {
                ApplyGravity();
            }
        }

        void getCameraAxis(out Vector3 forward, out Vector3 right)
        {
            right   = playerCamera.right;
            forward = playerCamera.forward;

            //変な方向に動くため
            right.y   = 0;
            forward.y = 0;

            right.Normalize();
            forward.Normalize();
        }

        Vector3 GetInputVelocity(Vector3 forward, Vector3 right)
        {
            Vector3 velocity;
            velocity  = forward * Input.GetAxis("Vertical");
            velocity += right   * Input.GetAxis("Horizontal");
            return velocity;
        }

        void LookRotation(Vector3 velocity)
        {
            Quaternion rotation = Quaternion.LookRotation(velocity);
            transform.rotation = rotation;
        }

        void ApplyGravity()
        {
            float velocityY;
            velocityY = gravity * Time.deltaTime;

            controller.Move(new Vector3(0.0f, velocityY, 0.0f));
        }
    }
}