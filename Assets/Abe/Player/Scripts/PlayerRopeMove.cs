using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerRopeMove : MonoBehaviour
{
    //[SerializeField, Tooltip("説明文")]
    
    public Player player;

    void Awake()
    {
        enabled = false;
    }
    
    void Start()
    {
        
    }
    
    void Update()
    {
        if(player.IsGround())
        {
            player.ResetUpTrans();
            player.animator.SetBool("IsGrab",   false);
            player.animator.SetBool("IsGround", true);
            player.ResetGravity();
            player.NormalMove();
            player.Jump();
            player.SyncRopeToPlayer();
            player.RopeTakeUp();
        }
        else
        {
            player.animator.SetBool("IsGround", false);
            if((Input.GetButton("Jump") && player.IsJump) || (!player.IsRopeExist))
            {
                player.ResetUpTrans();
                player.animator.SetBool("IsGrab", false);
                player.ApplyGravity();
                player.AirMove();
                player.SyncRopeToPlayer();
            }
            else
            {
                player.animator.SetBool("IsGrab", true);
                player.SyncPlayerToRope();
                player.RopeControl();
            }
        }
    }
}