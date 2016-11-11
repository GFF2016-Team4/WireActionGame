using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace Player
{
    [RequireComponent(typeof(Animator))]
    public class PlayerMove : MonoBehaviour
    {
        public Player player;

        void Awake()
        {
            enabled = false;
        }

        public void OnEnable()
        {
            player.RopeReleaseCheck();
            player.StartAnimation();
        }

        public void FixedUpdate()
        {
            //地面についている
            if(player.isGround)
            {
                player.ResetGravity();
            }
            else
            {
                player.ApplyGravity();
            }
        }

        void Update()
        {
            player.NormalMove();
        }
    }
}