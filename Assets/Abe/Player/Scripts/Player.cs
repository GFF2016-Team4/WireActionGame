using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Player
{
    public class Player : MonoBehaviour, RopeCreateHandlar
    {
        [SerializeField]
        private Camera playerCamera;

        [SerializeField]
        private float moveSpeed;

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
                Vector3 position = transform.position;
                position.y      += 0.6f;
                Ray ray          = new Ray(position, Vector3.down);
                bool isRayHit    = Physics.SphereCast(ray, controller.radius, 0.15f, LayerMask.NameToLayer("Player"));

                return controller.isGrounded || isRayHit;
            }
        }

        public bool isLeftRopeExist
        {
            get { return ropeController.leftRopeInst  != null; }
        }

        public bool isRightRopeExist
        {
            get { return ropeController.rightRopeInst != null; }
        }

        public bool isRopeExist
        {
            get { return isLeftRopeExist || isRightRopeExist; }
        }

        //クラスに昇格できそう？
        public bool isLeftRopeButton
        {
            get { return Input.GetButton("Fire1");     }
        }

        public bool isRightRopeButton
        {
            get { return Input.GetButton("Fire2");     }
        }

        public bool isLeftRopeButtonUp
        {
            get { return Input.GetButtonUp("Fire1");   }
        }

        public bool isRightRopeButtonUp
        {
            get { return Input.GetButtonUp("Fire2");   }
        }

        public bool isLeftRopeButtonDown
        {
            get { return Input.GetButtonDown("Fire1"); }
        }

        public bool isRightRopeButtonDown
        {
            get { return Input.GetButtonDown("Fire2"); }
        }
        //

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
    
        void Start()
        {
            
        }
    
        void Update()
        {
            bool keyDown =  isLeftRopeButton|| isRightRopeButton;
            playerMove.enabled     = !keyDown;
            playerRopeMove.enabled =  keyDown;
        }

        public void NormalMove()
        {
            Vector3 right;
            Vector3 forward;

            GetCameraAxis(out forward, out right);

            Vector3 velocity = GetInputVelocity(forward, right);
            float speed = velocity.magnitude;

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
            playerVelocity = Vector3.zero;
        }

        public void ApplyGravity()
        {
            playerVelocity += gravity * Time.fixedDeltaTime;

            controller.Move(playerVelocity * Time.fixedDeltaTime);
        }

        public void FreezeLeftRope()
        {
            if(isLeftRopeExist)
            {
                ropeController.leftRopeInst.RopeLock();
            }
        }

        public void FreezeRightRope()
        {
            if(isRightRopeExist)
            {
                ropeController.rightRopeInst.RopeLock();
            }
        }

        public void UnFreezeLeftRope()
        {
            if(isLeftRopeExist)
            {
                ropeController.leftRopeInst.RopeUnLock();
            }
        }

        public void UnFreezeRightRope()
        {
            if(isRightRopeExist)
            {
                ropeController.rightRopeInst.RopeUnLock();
            }
        }

        public void RopeReleaseCheck()
        {
            if(isGround) return;
            
            //
            if(isLeftRopeButtonUp  && isLeftRopeExist)
            {
                AddRopeVelocity(ropeController.leftRopeInst);
            }

            if(isRightRopeButtonUp && isRightRopeExist)
            {
                AddRopeVelocity(ropeController.rightRopeInst);
            }
        }

        void AddRopeVelocity(RopeSimulate rope)
        {
            Rigidbody tailRig = rope.tailRig;
            playerVelocity += tailRig.velocity;
        }

        public void SyncRope()
        {
            if(isLeftRopeButton && isRightRopeButton)
            {
                if(SyncRopeLeftAndRight()) return;
            }

            if(isLeftRopeButton)
            {
                if(SyncLeftRope())  return;
            }

            if(isRightRopeButton)
            {
                if(SyncRightRope()) return;
            }
        }

        bool SyncRopeLeftAndRight()
        {
            if(!isLeftRopeExist ) return false;
            if(!isRightRopeExist) return false;

            UnFreezeLeftRope();
            UnFreezeRightRope();

            Vector3 leftRopeTailPos  = ropeController.leftRopeInst .tailPosition;
            //Vector3 player2leftRope  = leftRopeTailPos  - transform.position;

            Vector3 rightRopeTailPos = ropeController.rightRopeInst.tailPosition;
            //Vector3 player2rightRope = rightRopeTailPos - transform.position;

            Vector3 applyVelocity = rightRopeTailPos - leftRopeTailPos;
            applyVelocity /= 2;

            transform.position = leftRopeTailPos + applyVelocity;

            return true;
        }

        bool SyncLeftRope()
        {
            if(!isLeftRopeExist) return false;
            UnFreezeLeftRope();

            Vector3 leftHandPos    = transform.position - ropeController.leftGun .position;
            Vector3 playerPosition = leftHandPos  + ropeController.leftRopeInst.tailPosition;
            transform.position     = playerPosition;

            return true;
        }

        bool SyncRightRope()
        {
            if(!isRightRopeExist) return false;

            UnFreezeRightRope();

            Vector3 rightHandPos   = transform.position - ropeController.rightGun.position;
            Vector3 playerPosition = rightHandPos + ropeController.rightRopeInst.tailPosition;
            transform.position     = playerPosition;

            return true;
        }

        public void OnRopeCreate(RopeSimulate rope)
        {
            if(isGround)
            {
                //同期するためにロックする
                rope.RopeLock();
            }

            rope.tailRig.AddForce(playerVelocity * 1.2f, ForceMode.VelocityChange);
            ResetGravity();
        }
    }
}