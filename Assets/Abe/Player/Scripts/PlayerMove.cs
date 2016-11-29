using UnityEngine;

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

        void Update()
        {
            player.ApplyGravity();
            if(player.IsGround)
            {
                player.ResetGravity();
                player.NormalMove();
                player.Jump();
            }
            else
            {
                player.JumpMove();
            }
        }
    }
}