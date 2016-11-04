using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Player
{
    public class Player : MonoBehaviour
    {
        [SerializeField]
        private Camera playerCamera;

        float gravity;

        PlayerMove     playerMove;
        PlayerRopeMove playerRopeMove;

        private Animator animator;
        private CharacterController controller;
        

        void Awake()
        {
            playerMove     = GetComponent<PlayerMove>();
            playerRopeMove = GetComponent<PlayerRopeMove>();

            animator       = GetComponent<Animator>();
            controller     = GetComponent<CharacterController>();

            //物理演算と出来る限り同じ挙動にするため
            gravity = Physics.gravity.y;
        }
    
        void Start()
        {
            
        }
    
        void Update()
        {
            bool keyDown = Input.GetButton("Fire1") || Input.GetButton("Fire2");
            playerMove.enabled     = !keyDown;
            playerRopeMove.enabled = keyDown;
        }

        void NormalMove()
        {

        }

        void ApplyGravity()
        {
            float velocityY;
            velocityY = gravity * Time.deltaTime;

            controller.Move(new Vector3(0.0f, velocityY, 0.0f));
        }
    }
}