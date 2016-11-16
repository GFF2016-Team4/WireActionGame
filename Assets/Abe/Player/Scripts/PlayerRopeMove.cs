﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Player
{
    public class PlayerRopeMove : MonoBehaviour
    {
        [SerializeField, Tooltip("説明文")]
        GameObject rope;

        public Player player;

        void Awake()
        {
            enabled = false;
        }

        void Update()
        {
            if(player.isGround)
            {
                player.StartAnimation();
                player.NormalMove();
            }
            else
            {
                if(!player.isRopeExist)
                {
                    player.ApplyGravity();
                    player.StopAnimation();
                    return;
                }

                player.RopeMove();
                player.SyncRope();
                player.StopAnimation();
            }
        }
    }
}