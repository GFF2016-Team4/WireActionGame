using UnityEngine;
using System;
using UniRx;

public class Player : MonoBehaviour, RopeEventHandlar
{
    [SerializeField]
    private float moveSpeed;

    [SerializeField]
    private float airMoveSpeed;

    [SerializeField, Tooltip("ジャンプの強さ")]
    private float jumpPower;

    [SerializeField, Range(0, 1), Header("上方向の減衰値")]
    private float upVecDampingPow;

    [SerializeField, Range(0, 1), Header("横方向の減衰値")]
    private float sideVecDampingPow;

    [SerializeField]
    private float ropeAcceleration;

    [SerializeField, Header("ロープの伸縮スピード")]
    private float ropeTakeSpeed;

    [SerializeField, Header("ロープの伸縮時に加える力")]
    private float ropeTakeForce;

    [SerializeField, Header("ロープの加える力")]
    private float ropeForcePower;

    [SerializeField, Header("移動用・捕獲用ロープの飛ぶ長さ")]
    private float normalRopeDistance = Mathf.Infinity;

    [SerializeField, Header("打ち込み用ロープの飛ぶ長さ")]
    private float lockRopeDistance = 50.0f;

    [SerializeField]
    private Transform footOrigin;

    [SerializeField]
    private Transform leftForeArm;

    [SerializeField]
    private Transform rightForeArm;

    bool    isJump = false;
    float   jumpTime = 0.0f;
    Vector3 gravity;
    Vector3 playerVelocity;

    bool isControll = true;

    RopeController ropeController;

    PlayerMove     playerMove;
    PlayerRopeMove playerRopeMove;

    PlayerCameraInfo cameraInfo;

    [NonSerialized]
    public Animator animator;
    [NonSerialized]
    public CharacterController controller;

    public bool IsJump
    {
        get { return isJump; }
    }

    public bool IsRopeExist
    {
        get { return ropeController.IsRopeExist; }
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

        cameraInfo     = GetComponent<PlayerCameraInfo>();

        //物理演算と出来る限り同じ挙動にするため
        gravity = Physics.gravity;
    }

    void Start()
    {
        ropeController.normalRopeDistance = normalRopeDistance;
    }

    void Update()
    {
        if(isControll)
        {
            bool keyDown = RopeInput.isLeftRopeButton  || 
                           RopeInput.isRightRopeButton ||
                           RopeInput.isCatchRopeButton;
            playerMove.enabled     = !keyDown;
            playerRopeMove.enabled =  keyDown;
        }
        else
        {
            playerMove.enabled     = false;
            playerRopeMove.enabled = false;
        }
    }

    public bool IsGround()
    {
        //CharacterControllerのisGroundedだけだとうまく判定しない為
        Vector3 position = footOrigin.position;
        position.y += transform.localScale.y * 0.2f; //始点を上げないと
        Vector3 size = new Vector3()
        {
            x = controller.radius * transform.localScale.x*0.8f,
            y = transform.localScale.y*0.8f,
            z = controller.radius * transform.localScale.z*0.8f
        };

        int  layerMask = PlayersLayerMask.IgnorePlayerAndRopes;

        bool isBoxHit  = Physics.CheckBox(position, size, Quaternion.identity, layerMask);

        bool isUpVelocity = playerVelocity.y > 0.25f;

        return ((controller.isGrounded || isBoxHit) && !isUpVelocity);
    }

    public void NormalMove()
    {
        if(isJump) isJump = false;
        Vector3 velocity   = cameraInfo.GetInputVelocity();
        float   speed      = velocity.magnitude;

        //velocityがゼロベクトルだとだと変な方向に向くため
        if(velocity != Vector3.zero) transform.forward = velocity;

        //回転の後に移動
        //アニメーションで移動させる
        animator.SetFloat("MoveSpeed", speed);
        animator.speed = moveSpeed;
    }

    public void AirMove()
    {
        if(IsGround()) return;
        transform.position += cameraInfo.GetInputVelocity() * airMoveSpeed * Time.deltaTime;
    }

    public void RopeControl()
    {
        Vector3 velocity = cameraInfo.GetInputVelocity();
        if(velocity != Vector3.zero && !IsGround())
        {
            ropeController.AddForce(velocity * ropeForcePower, ForceMode.Force);
        }

        RopeTake();
    }

    public void RopeTake()
    {
        ropeController.AddLength(ropeTakeSpeed * -Input.GetAxisRaw("RopeTake") * Time.deltaTime);
    }

    public void Jump()
    {
        if(!IsGround())                  return;
        if(!Input.GetButtonDown("Jump")) return;

        SoundManager.Instance.PlaySE(AUDIO.SE_playerLanding);
        playerVelocity += Vector3.up * jumpPower;
        playerVelocity += cameraInfo.GetInputVelocity()*2.0f;
        controller.Move(playerVelocity * Time.deltaTime);
        isJump = true;
        jumpTime = Time.time;
    }

    public void ApplyGravity()
    {
        DampingVerticalVelocity();
        DampingHorizontalVelocity();

        playerVelocity += gravity * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);

        CheckJumpState();
    }

    void CheckJumpState()
    {
        if(!isJump) return;
        //重力加速度をみて初速度より速くなればjump状態解除 
        if(jumpPower + gravity.y * (Time.time - jumpTime) < -jumpPower * Time.deltaTime)
        {
            isJump = false;
        }
    }

    void DampingVerticalVelocity()
    {
        if(playerVelocity.y > 0.1f)
        {
            playerVelocity.y *= upVecDampingPow;
        }
    }

    void DampingHorizontalVelocity()
    {
        Vector2 hrVel = new Vector2(playerVelocity.x, playerVelocity.z);
        if(hrVel.magnitude == 0) return;

        hrVel *= sideVecDampingPow;
        playerVelocity.x = hrVel.x;
        playerVelocity.z = hrVel.y;
    }

    public void ResetGravity()
    {
        //地面についているときなど
        playerVelocity = Vector3.zero;
    }

    public void ResetUpTrans()
    {
        Vector3 rot = transform.rotation.eulerAngles;
        rot.x = 0;
        rot.z = 0;

        transform.rotation = Quaternion.Euler(rot);
    }

    public void StartRopeSimulate()
    {
        ropeController.SimulateStart();
    }

    public void StopRopeSimulate()
    {
        ropeController.SimulateStop();
    }

    /// <summary>
    /// ロープの動きにプレイヤーを同期させる
    /// </summary>
    public void SyncPlayerToRope()
    {
        isJump = false;
        playerVelocity = Vector3.zero;
        if(ropeController.IsRopeExist)
        {
            //角度と軸の算出
            float   dir     = Vector3.Angle(ropeController.Direction, transform.forward);
            Vector3 axisRot = Vector3.Cross(ropeController.Direction, transform.forward);

            Quaternion rot  = Quaternion.AngleAxis((90-dir), axisRot);

            Vector3 forward = rot * transform.forward;
            Vector3 up      = ropeController.Direction;

            transform.rotation = Quaternion.LookRotation(forward, up);

            animator.SetBool("CenterRopeExist",ropeController.IsCenterRopeExist);
            if(ropeController.IsCenterRopeExist)
            {
                CalcArm(ropeController.LeftOrigin , leftForeArm , "Left" );
                CalcArm(ropeController.RightOrigin, rightForeArm, "Right");
            }
        }
        ropeController.SimulateStart();
        ropeController.SyncTransformToRope(transform);
    }

    public void CalcArm(Vector3 origin, Transform arm, string dir)
    {
        Vector3 fixedDir = origin - arm.position;
        fixedDir.Normalize();
        //内積で向きを計算
        float vertical = Vector3.Dot(transform.up, fixedDir);
        animator.SetFloat(dir + "Vertical", vertical);
        float horizontal = Vector3.Dot(transform.forward, fixedDir);
        animator.SetFloat(dir + "Horizontal", horizontal);
    }

    /// <summary>
    /// プレイヤーの動きにロープを同期させる
    /// </summary>
    public void SyncRopeToPlayer()
    {
        ropeController.SimulateStop();
        ropeController.SyncRopeToTransform();
    }

    public void OnNormalRopeCreate(NormalRopeSimulate rope)
    {
        rope.AddForce(playerVelocity * ropeAcceleration, ForceMode.VelocityChange);
        ResetGravity();
    }

    public void OnNormalRopeRelease(NormalRopeSimulate rope)
    {
        ResetGravity();
        if(IsGround()) return;
        playerVelocity = rope.velocity;
    }

    public void ShootLockRope()
    {
        if(!Input.GetButtonDown("LockRope")) return;
        
        Vector3 rayOffset = Vector3.up * 0.01f;
        if(ropeController.CreateLockRope(footOrigin.position + rayOffset, lockRopeDistance))
        {
            animator.SetTrigger("ShootLockRope");
        }
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if(!isControll) return;
        if(hit.gameObject.tag == "Enemy")
        {
            animator.SetTrigger("Damage");
            isControll = false;
            ropeController.isControl = false;

            //GetComponent<FadeRespawn>().Respawn();
        }
    }

    //ダメージアニメーション終了時の処理
    void DamageEnd()
    {
        isControll = true;
        ropeController.isControl = true;
    }
}