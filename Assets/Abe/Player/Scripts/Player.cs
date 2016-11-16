﻿using UnityEngine;
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

        [SerializeField, Header("ロープの縮めるスピード")]
        private float ropeTakeUpSpeed;

        [SerializeField, Header("ロープの伸ばすスピード")]
        private float ropeTakeDownSpeed;

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

        public bool isRopeExist
        {
            get { return ropeController.ropeExist; }
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
            //ステートの切り替え
            bool keyDown =  RopeInput.isLeftRopeButton|| RopeInput.isRightRopeButton;
            playerMove.enabled     = !keyDown;
            playerRopeMove.enabled =  keyDown;
        }

        //地面についているときの移動
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

        public void RopeMove()
        {
            if(RopeControl(ropeController.centerRopeInst)) return;
            RopeControl(ropeController.left .ropeInst);
            RopeControl(ropeController.right.ropeInst);
        }

        bool RopeControl(RopeSimulate rope)
        {
            if(rope == null) return false;

            bool isDown = false;
            if(RopeInput.isTakeUpButton)
            {
                rope.AddRopeLength(ropeTakeUpSpeed);
                isDown = true;
            }

            if(RopeInput.isTakeDownButton)
            {
                rope.SubRopeLength(ropeTakeDownSpeed);
                isDown = true;
            }

            return isDown;
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
            return velocity; //正規化はしない
        }

        public void LookRotation(Vector3 velocity)
        {
            //ベクトルの方向でプレイヤーの向きを変える
            Quaternion rotation = Quaternion.LookRotation(velocity);
            transform.rotation = rotation;
        }

        //アニメーションを動かす
        public void StartAnimation()
        {
            animator.SetBool("Stop", false);
        }

        //アニメーションを止める
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

        public void FreezeRope  (RopeSimulate rope)
        {
            if(rope != null) { rope.RopeLock();   }
        }

        public void UnFreezeRope(RopeSimulate rope)
        {
            if(rope != null) { rope.RopeUnLock(); }
        }

        public void SyncRope()
        {
            bool leftButton  = RopeInput.isLeftRopeButton;
            bool rightButton = RopeInput.isRightRopeButton;

            if(leftButton && rightButton)
            {
                //第三のロープと同期
                if(SyncRopeCenter()) return;
            }

            if(leftButton)
            {
                //左のロープと同期
                if(SyncRope(ropeController.left)) return;
            }

            if(rightButton)
            {
                //右のロープと同期
                if(SyncRope(ropeController.right)) return;
            }
        }

        bool SyncRopeCenter()
        {
            if(!ropeController.centerRopeExist) return false;
            UnFreezeRope(ropeController.centerRopeInst);

            Vector3 center         = Vector3.down;
            Vector3 playerPosition = center + ropeController.centerRopeInst.tailPosition;
            transform.position     = playerPosition;
            return true;
        }

        bool SyncRope(RopeController.RopeGun rope)
        {
            if(!rope.ropeExist) return false;
            UnFreezeRope(rope.ropeInst);

            Vector3 handPos    = transform.position - rope.gun.position;
            Vector3 playerPosition = handPos  + rope.ropeInst.tailPosition;
            transform.position     = playerPosition;
            return true;
        }

        void CreateCenterRope()
        {
            ropeController.CreateCenterRope();
            FreezeRope(ropeController.left.ropeInst);
            FreezeRope(ropeController.right.ropeInst);
        }

        //ロープが生成されたときのイベント
        public void OnRopeCreate(RopeSimulate rope)
        {
            //プレイヤーのVelocityをロープに反映
            //スピード感を出すために元のVelocityに数値を掛ける
            rope.tailRig.AddForce(playerVelocity * ropeAcceleration, ForceMode.VelocityChange);
            ResetGravity();

            bool ropeExist = ropeController.leftRopeExist && ropeController.rightRopeExist;
            bool isButtonDown = RopeInput.isLeftRopeButton && RopeInput.isRightRopeButton;

            if(ropeExist && isButtonDown)
            {
                CreateCenterRope();
            }

            if(isGround)
            {
                //同期するためにロックする
                rope.RopeLock();

                if(ropeController.centerRopeExist)
                {
                    ropeController.centerRopeInst.RopeLock();
                }
            }
        }

        //ロープを放した時のイベント
        public void OnRopeRelease(RopeSimulate rope)
        {
            bool isLeftGrab  = ropeController.leftRopeExist  && RopeInput.isLeftRopeButton;
            bool isRightGrab = ropeController.rightRopeExist && RopeInput.isRightRopeButton;

            if(isLeftGrab)
            {
                RopeSimulate leftRope = ropeController.left.ropeInst;
                UnFreezeRope(leftRope);
                leftRope.tailRig.AddForce(rope.tailRig.velocity, ForceMode.VelocityChange);
                ResetGravity();
                return;
            }
            if(isRightGrab)
            {
                RopeSimulate rightRope = ropeController.right.ropeInst;
                UnFreezeRope(rightRope);
                rightRope.tailRig.AddForce(rope.tailRig.velocity, ForceMode.VelocityChange);
                ResetGravity();
                return;
            }

            if(isGround) return;
            Rigidbody tailRig  = rope.tailRig;
            playerVelocity    += tailRig.velocity;
            rope.RopeLock();
        }
    }
}