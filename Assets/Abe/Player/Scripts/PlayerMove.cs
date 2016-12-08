using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Animator))]
public class PlayerMove : MonoBehaviour
{
    public Player player;

    void Awake()
    {
        enabled = false;
    }

    void OnEnable()
    {
        player.animator.SetBool("IsGrab", false);
        player.ResetUpTrans();
    }

    void Update()
    {
        player.ApplyGravity();
        
        if(player.IsGround())
        {
            player.animator.SetBool("IsGround", true);
            player.ResetGravity();
            player.NormalMove();
            player.Jump();
        }
        else
        {
            player.animator.SetBool("IsGround", false);
            player.AirMove();
        }
    }
}