using UnityEngine;
using System.Collections;

public class InertialCharacterController : MonoBehaviour
{
    public float m_MaxSpeed = 4.0f;     // 最高速度（メートル/秒）
    public float m_MinSpeed = -2.0f;        // 最低速度（最大バック速度）
    public float m_AccelPower = 2.0f;       // 加速度（メートル/秒/秒）
    public float m_BrakePower = 6.0f;       // ブレーキ速度（メートル/秒/秒）
    public float m_RotateSpeed = 180.0f;    // 回転速度（度/秒）
    public float m_Gravity = 18.0f;     // 重力加速度（メートル/秒/秒）
    public float m_JumpPower = 0.0f;       // ジャンプ力（初速(メートル/秒)）

    float m_VelocityY = 0f;     // y軸方向の移動量
    float m_Speed = 0f;         // 前進速度（前進はプラス、後退はマイナス）

    int m_Flag = 0;

    public Transform child;

    public Transform rightHand;
    float x, y, speed;

    CharacterController m_Controller;

    Animator m_Animator;

    public GameObject deadReplacement;
    private bool deadReplace;

    private int ropeCounter;

    void Start()
    {
        deadReplace = false;
        m_Controller = GetComponent<CharacterController>();
        m_Animator = transform.Find("EnemyRobot").GetComponent<Animator>();

        rightHand = transform.Find("mixamorig:RightArm").GetComponent<Transform>();

        ropeCounter = 0;
        speed = 1;

    }

    void Update()
    {
        Debug.Log("rightHand:" + rightHand.transform.childCount);

        float axisVertical = Input.GetAxisRaw("Vertical");

        // 減速する（入力が無い場合 or 進行方向と逆に入力がある場合）
        if ((axisVertical == 0) || (m_Speed * axisVertical < 0))
        {
            if (m_Speed > 0)
            {
                m_Speed = Mathf.Max(m_Speed - m_BrakePower * Time.deltaTime, 0);
            }
            else {
                m_Speed = Mathf.Min(m_Speed + m_BrakePower * Time.deltaTime, 0);
            }
        }

        // 上下キーで加速
        m_Speed +=
            m_AccelPower
            * axisVertical
            * Time.deltaTime;

        // 速度制限
        m_Speed = Mathf.Clamp(m_Speed, m_MinSpeed, m_MaxSpeed);

        // 速度を、プレイヤーが向いている方向のベクトルに変換する
        Vector3 velocity = transform.forward * m_Speed;

        // 接地しているなら
        if (m_Controller.isGrounded)
        {
            // 落下を止める
            m_VelocityY = 0;
        }

        // スペースキーが押されたら、ジャンプする
        if (m_Controller.isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            m_VelocityY = m_JumpPower;
        }

        // 重力加速度を加算
        m_VelocityY -= m_Gravity * Time.deltaTime;

        // y軸方向の移動量を加味する
        velocity.y = m_VelocityY;

        // CharacterControllerに命令して移動する

        m_Animator.SetFloat("Speed", m_Speed);
        m_Animator.SetInteger("Flag", m_Flag);

        if (Input.GetMouseButton(0))
        {
            rightHand.Rotate(0, 0, 90);
        }
        if(Input.GetKeyDown(KeyCode.Q))
        {
            x = -1;
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            x = 1;
        }

        //m_Animator.GetBoneTransform(rightHand).transform.Rotate(x, y, 0);

        if (Input.GetKey(KeyCode.Z))
        {
            deadReplace = true;
        }

        if (deadReplace)
        {
            //gameObject.GetComponent<Animator>().enabled = false;
            Destroy(gameObject);

            Transform dead = Instantiate(deadReplacement, transform.position, transform.rotation) as Transform;
            dead.GetComponent<AnimationClip>().legacy = true;
            dead.GetComponent<Animation>().GetClip("walking");

            CopyTransformsRecurse(transform, dead);
        }

        if (Input.GetKey(KeyCode.Alpha1))
        {
            m_Speed -= Time.deltaTime * 0.01f;
            if (m_Speed <= 0)
            {
                m_Speed = 0;
            }
            m_Animator.SetLayerWeight(1, 1f);
            //m_Animator.SetLayerWeight(2, 1f);
            //m_Animator.SetLayerWeight(4, 1f);
            //m_Animator.SetLayerWeight(5, 1f);
            //m_Controller.Move(Vector3.zero);
            //transform.Rotate(Vector3.zero);
        }
        else
        {
            m_Animator.SetLayerWeight(1, 0);
            m_Animator.SetLayerWeight(3, 0);
            m_Animator.SetLayerWeight(4, 0);
            m_Controller.Move(velocity * Time.deltaTime);

            // 左右キーで回転
            transform.Rotate(
    Mathf.Clamp(x * m_RotateSpeed * Time.deltaTime,-10, 10),
    Input.GetAxis("Horizontal") * m_RotateSpeed * Time.deltaTime,
    0);
        }

        if (Input.GetKey(KeyCode.Alpha2)) m_Animator.SetLayerWeight(2, 1);
        else m_Animator.SetLayerWeight(2, 0);

        if (Input.GetKey(KeyCode.Alpha3)) m_Animator.SetLayerWeight(3, 1);
        else m_Animator.SetLayerWeight(3, 0);

        if (Input.GetKey(KeyCode.Alpha4)) m_Animator.SetLayerWeight(4, 1);
        else m_Animator.SetLayerWeight(4, 0);

        if (Input.GetKey(KeyCode.Alpha5)) m_Animator.SetLayerWeight(5, 1);
        else m_Animator.SetLayerWeight(5, 0);

        if (Input.GetKeyDown(KeyCode.L))
        {
            m_Flag = 1;
        }
        else if (Input.GetKeyDown(KeyCode.K))
        {
            m_Flag = 2;
        }
        else if (Input.GetKeyUp(KeyCode.L))
        {
            m_Flag = 0;
        }

    }

    static void CopyTransformsRecurse(Transform src, Transform dst)
    {
        dst.position = src.position;
        dst.rotation = src.rotation;

        foreach (Transform child in dst)
        {
            // Match the transform with the same name
            Transform curSrc = src.Find(child.name);
            if (curSrc)
                CopyTransformsRecurse(curSrc, child);
        }
    }

    public bool IsDead()
    {
        return deadReplace;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Rope")
            ropeCounter += 1;

        Debug.Log("In");

    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Rope")
            ropeCounter -= 1;

        Debug.Log("Out");

    }

    public void OnTriggerStay(Collider other)
    {
        if (ropeCounter >= 4)
            deadReplace = true;

        Debug.Log(ropeCounter);
    }
}
