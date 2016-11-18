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
            }
            else
            {
                player.StopAnimation();
                player.ApplyGravity();
            }
        }

        //public void OnControllerColliderHit(ControllerColliderHit hit)
        //{
        //    if(!player.isGround) return;

        //    //地面についたときにロープの物理挙動解除
            
        //}
    }
}