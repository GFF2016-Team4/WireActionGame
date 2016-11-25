using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Player
{
    public class PlayerRopeMove : MonoBehaviour
    {
        public Player player;

        void Awake()
        {
            enabled = false;
        }

        void Update()
        {
            if(player.isGround && !RopeInput.isTakeUpButton)
            {
                player.FreezeRope();
                player.ResetGravity();
                player.NormalMove();
                player.Jump();
                return;
            }

            if(player.isGround) return;

            bool buttonDown = Input.GetButton("Jump");

            if(!player.isRopeExist)
            {
                player.ApplyGravity();
                player.JumpMove();
            }
            else
            {
                if(buttonDown && player.isJump)
                {
                    player.FreezeRope();
                    player.ApplyGravity();
                    player.JumpMove();
                }
                else
                {
                    player.ResetGravity();
                    player.RopeMove();
                    player.SyncRope();
                }
            }
        }
    }
}