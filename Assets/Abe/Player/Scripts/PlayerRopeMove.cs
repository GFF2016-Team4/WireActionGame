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
                player.StartAnimation();
                player.NormalMove();
                player.Jump();
                return;
            }

            bool buttonDown = Input.GetButton("Jump");

            if(!player.isRopeExist)
            {
                player.ApplyGravity();
            }
            else
            {
                if(buttonDown)
                {
                    player.FreezeRope();
                    player.ApplyGravity();
                }
                else
                {
                    player.RopeMove();
                    player.SyncRope();
                }
            }
            player.StopAnimation();
        }
    }
}