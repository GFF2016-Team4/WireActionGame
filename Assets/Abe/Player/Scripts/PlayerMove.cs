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
            if(player.isGround)
            {
                player.ResetGravity();
                player.StartAnimation();
                player.NormalMove();
                player.Jump();
            }
            else
            {
                player.StopAnimation();
                player.ApplyGravity();
            }
        }
    }
}