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
            if(player.isGround)
            {
                player.StartAnimation();
                player.NormalMove();
            }
            else
            {
                player.StopAnimation();
            }
        }
    }
}