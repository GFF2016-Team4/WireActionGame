using UnityEngine;
using System;

namespace Player
{
    public class Player : MonoBehaviour, RopeEventHandlar
    {
        [SerializeField]
        private Camera playerCamera;

        [SerializeField]
        private float moveSpeed;

        [SerializeField]
        private float ropeAcceleration;

        Vector3 gravity;
        Vector3 playerVelocity;

        PlayerMove     playerMove;
        PlayerRopeMove playerRopeMove;

        RopeController ropeController;

        [NonSerialized]
        public Animator animator;
        [NonSerialized]
        public CharacterController controller;
        
        public bool isGround
        {
            get
            {
                //CharacterControllerのisGroundedだけだとうまく判定しない為
                Vector3 position = transform.position;
                position.y      += 0.6f; //始点を上げないと
                Ray ray          = new Ray(position, Vector3.down);
                bool isRayHit    = Physics.SphereCast(ray, controller.radius, 0.3f, LayerMask.NameToLayer("Player"));

                return controller.isGrounded || isRayHit;
            }
        }

        public bool isLeftRopeExist
        {
            get { return ropeController.LeftRopeExist; }
        }

        public bool isRightRopeExist
        {
            get { return ropeController.RightRopeExist; }
        }

        public bool isRopeExist
        {
            get { return isLeftRopeExist || isRightRopeExist; }
        }

        void Awake()
        {
            playerMove     = gameObject.AddComponent<PlayerMove>();
            playerRopeMove = gameObject.AddComponent<PlayerRopeMove>();
            
            playerMove.player     = this;
            playerRopeMove.player = this;

            animator       = GetComponent<Animator>();
            controller     = GetComponent<CharacterController>();
            ropeController = GetComponent<RopeController>();

            //物理演算と出来る限り同じ挙動にするため
            gravity = Physics.gravity;
        }
    
        void Update()
        {
            bool keyDown =  RopeInput.isLeftRopeButton|| RopeInput.isRightRopeButton;
            playerMove.enabled     = !keyDown;
            playerRopeMove.enabled =  keyDown;
        }

        public void NormalMove()
        {
            Vector3 right;
            Vector3 forward;

            GetCameraAxis(out forward, out right);

            Vector3 velocity = GetInputVelocity(forward, right);
            float   speed    = velocity.magnitude;

            //アニメーションで移動させる
            animator.SetFloat("MoveSpeed", speed);
            animator.speed = moveSpeed;

            //velocityがゼロベクトルだとだと変な方向に向くため
            if(velocity != Vector3.zero)
            {
                LookRotation(velocity);
            }
        }

        public void GetCameraAxis(out Vector3 forward, out Vector3 right)
        {
            right   = playerCamera.transform.right;
            forward = playerCamera.transform.forward;

            //変な方向に動くため
            right.y   = 0;
            forward.y = 0;

            right.Normalize();
            forward.Normalize();
        }

        public Vector3 GetInputVelocity(Vector3 forward, Vector3 right)
        {
            Vector3 velocity;
            velocity  = forward * Input.GetAxis("Vertical");
            velocity += right   * Input.GetAxis("Horizontal");
            return velocity;
        }

        public void LookRotation(Vector3 velocity)
        {
            Quaternion rotation = Quaternion.LookRotation(velocity);
            transform.rotation = rotation;
        }

        public void StartAnimation()
        {
            animator.SetBool("Stop", false);
        }

        public void StopAnimation()
        {
            animator.SetBool("Stop", true);
        }

        public void ResetGravity()
        {
            //地面についているときなど
            playerVelocity = Vector3.zero;
        }

        public void ApplyGravity()
        {
            playerVelocity += gravity * Time.fixedDeltaTime;
            controller.Move(playerVelocity * Time.fixedDeltaTime);
        }

        public void FreezeRope(RopeSimulate rope)
        {
            if(rope != null)
            {
                rope.RopeLock();
            }
        }

        public void UnFreezeRope(RopeSimulate rope)
        {
            if(rope != null)
            {
                rope.RopeUnLock();
            }
        }

        public void SyncRope()
        {
            bool leftButton  = RopeInput.isLeftRopeButton;
            bool rightButton = RopeInput.isRightRopeButton;

            if(leftButton && rightButton)
            {
                SyncRopeLeftAndRight();
            }
            else if(leftButton)
            {
                SyncLeftRope();
            }
            else if(rightButton)
            {
                SyncRightRope();
            }
        }

        void SyncRopeLeftAndRight()
        {
            if(!isLeftRopeExist ) return;
            if(!isRightRopeExist) return;

            //ロック解除（解除しているのであればそのまま）
            UnFreezeRope(ropeController.leftRopeInst);
            UnFreezeRope(ropeController.rightRopeInst);

            Vector3 leftRopeTailPos  = ropeController.leftRopeInst .tailPosition;
            Vector3 rightRopeTailPos = ropeController.rightRopeInst.tailPosition;

            Vector3 applyVelocity = rightRopeTailPos - leftRopeTailPos;
            applyVelocity /= 2;

            transform.position = leftRopeTailPos + applyVelocity;
        }

        void SyncLeftRope()
        {
            if(!isLeftRopeExist) return;
            UnFreezeRope(ropeController.leftRopeInst);

            Vector3 leftHandPos    = transform.position - ropeController.leftGun .position;
            Vector3 playerPosition = leftHandPos  + ropeController.leftRopeInst.tailPosition;
            transform.position     = playerPosition;
        }

        void SyncRightRope()
        {
            if(!isRightRopeExist) return;
            UnFreezeRope(ropeController.rightRopeInst);

            Vector3 rightHandPos   = transform.position - ropeController.rightGun.position;
            Vector3 playerPosition = rightHandPos + ropeController.rightRopeInst.tailPosition;
            transform.position     = playerPosition;
        }

        public void OnRopeCreate(RopeSimulate rope)
        {
            if(isGround)
            {
                //同期するためにロックする
                rope.RopeLock();
                return;
            }
            rope.tailRig.AddForce(playerVelocity * ropeAcceleration, ForceMode.VelocityChange);
            ResetGravity();
        }

        public void OnRopeRelease(RopeSimulate rope)
        {
            if(isGround) return;
            Rigidbody tailRig  = rope.tailRig;
            playerVelocity    += tailRig.velocity;
            rope.RopeLock();
        }
    }
}