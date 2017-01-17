using UnityEngine;
using System.Collections;

namespace Gaken
{

    public class EnemyController : MonoBehaviour
    {
        private NavMeshAgent agent;

        private float time;
        private float time2;

        public bool Attack;
        private float NextAttackTime = 2;
        public Transform Destination;
        public Transform player;

        public float m_MaxSpeed = 4.0f;         // 最高速度（メートル/秒）
        public float m_MinSpeed = -2.0f;        // 最低速度（最大バック速度）
        public float m_AccelPower = 2.0f;       // 加速度（メートル/秒/秒）
        public float m_BrakePower = 6.0f;       // ブレーキ速度（メートル/秒/秒）
        public float m_RotateSpeed = 45.0f;    // 回転速度（度/秒）
        public float m_Gravity = 18.0f;         // 重力加速度（メートル/秒/秒）
        public float m_JumpPower = 0.0f;        // ジャンプ力（初速(メートル/秒)）
        public float m_LazerCoolDown = 20.0f;

        float m_VelocityY = 0f;                 // y軸方向の移動量
        public float m_Speed = 0f;                     // 前進速度（前進はプラス、後退はマイナス）

        bool m_IsAttack = false;                         //攻撃パターンフラグ
        bool m_IsAttackReady = false;
        bool m_IsAttackFinish = false;

        public bool ikActive = false;
        public Transform leftShoulder;
        public Transform leftHand;      //左腕Transform

        public int attackCount = 0;

        /************************************************************************
                                      仮宣言 
        ************************************************************************/
        CharacterController m_Controller;    //キャラクタコントローラ
        Animator m_Animator;                 //アニメター

        private bool isDead;            //死亡切替を行うか?
        private int ropeCounter;             //ロープとエネミーの接触点数

        private float x = 0; // 水平方向
        public float xLimit; // 水平方向の限度値
        private float initX = 30;
        private float y = 0; // 垂直方向
        public float yLimit = 30; //垂直方向の限度値
        private float initY;

        private bool setWaitshotFlag = false; //　構え終わった時に値をセットしたらOn

        bool EnemyForward = false, EnemyLeft = false, EnemyRight = false, EnemyBack = false;
        int c = 0;

        int enemyDirection;
        enum EnemyDirection
        {
            CW_Front,       //時計回り前
            CW_Right,       //時計回り右
            CW_Back,        //時計回り後
            CW_Left,        //時計回り左
            ACW_Front,      //反時計回り前
            ACW_Left,       //反時計回り左
            ACW_Back,       //反時計回り後
            ACW_Right,      //反時計回り右
        }

        void Start()
        {
            //初期化
            isDead = false;
            m_Controller = GetComponent<CharacterController>();
            agent = GetComponent<NavMeshAgent>();
            agent.speed = m_Speed;

            time2 += 1;

            ropeCounter = 0;

            //アニメターは子のアニメターを取得
            m_Animator = transform.Find("EnemyRobot").GetComponent<Animator>();             //こっちはfbx形式

            /************************************************************************
                                        仮初期化 
            ************************************************************************/
            //右腕の取得（使えるかどうか確定できていない）
            leftHand = transform.GetComponent<Transform>();
            //player = transform.Find("Player").GetComponent<Transform>();

        }

        void Update()
        {
            ////垂直軸の値取得
            //float axisVertical = Input.GetAxisRaw("Vertical");

            //// 減速する（入力が無い場合 or 進行方向と逆に入力がある場合）
            //if ((axisVertical == 0) || (m_Speed * axisVertical < 0))
            //{
            //    if (m_Speed > 0)
            //    {
            //        m_Speed = Mathf.Max(m_Speed - m_BrakePower * Time.deltaTime, 0);
            //    }
            //    else {
            //        m_Speed = Mathf.Min(m_Speed + m_BrakePower * Time.deltaTime, 0);
            //    }
            //}

            //// 上下キーで加速
            //m_Speed += m_AccelPower * axisVertical * Time.deltaTime;

            //// 速度制限
            //m_Speed = Mathf.Clamp(m_Speed, m_MinSpeed, m_MaxSpeed);

            //// 速度を、エネミーが向いている方向のベクトルに変換する
            //Vector3 velocity = transform.forward * m_Speed;

            //// 接地しているなら
            //if (m_Controller.isGrounded)
            //{
            //    // 落下を止める
            //    m_VelocityY = 0;
            //}

            //// スペースキーが押されたら、ジャンプする
            //if (m_Controller.isGrounded && Input.GetKeyDown(KeyCode.Space))
            //{
            //    m_VelocityY = m_JumpPower;
            //}

            //// 重力加速度を加算
            //m_VelocityY -= m_Gravity * Time.deltaTime;

            //// y軸方向の移動量を加味する
            //velocity.y = m_VelocityY;

            // CharacterControllerに命令して移動する

            //アニメターに数値を知らせる
            //m_Animator.SetFloat("Speed", m_Speed);

            //エネミーを即死させる
            if (Input.GetKey(KeyCode.Z))
            {
                isDead = true;
            }
            //死亡交代
            if (isDead)
            {
                m_Animator.SetBool("IsDead", true);

                m_Controller.enabled = false;
                agent.enabled = false;
                Debug.Log("Clear");
                //transform.GetComponent<CapsuleCollider>().enabled = false;
            }

            agent.speed = m_Speed;
            //agent.destination = Destination.position;
            AnimatorStateInfo Info = m_Animator.GetCurrentAnimatorStateInfo(0);
            Ray ray = new Ray(transform.position, transform.forward);
            RaycastHit hit;

            Debug.DrawRay(ray.origin, ray.direction, Color.blue, 2f);

            if (Physics.Raycast(ray, out hit, 1.5f))
            {
                if (hit.collider.gameObject.name == "kabe" && Info.IsName("BaseLayer.idle"))
                {
                    agent.speed = 0;
                    Debug.Log(hit.collider.gameObject.name + "発見");
                    Attack = true;
                    time += Time.deltaTime;
                }
            }

            if (time2 >= NextAttackTime)
            {
                time2 = 0;
                Attack = true;

                //PlayerTargetに指定したObjectを見続ける
                Vector3 eye = player.position;
                eye.y = transform.position.y;
                transform.LookAt(eye);
            }

            if (Attack == true)
            {
                if (time >= 0.3f)
                {
                    m_Animator.SetTrigger("Attack");
                    Attack = false;
                    time = 0;
                }
            }
            //モーションのAttack中の処理
            if (Info.IsName("UpperBody.Attack"))
            {
                agent.speed = 0;

                m_Animator.SetBool("IsAttack", true);

                Debug.Log("攻撃now");
            }

            m_Animator.SetFloat("Speed", agent.speed);


            /****************************************************************
                                        仮更新
            ****************************************************************/
            //アニメーション一時停止
            if (Input.GetKey(KeyCode.Alpha3))
            {
                m_Animator.speed = 0;
            }
            else
            {
                m_Animator.speed = 1;
            }

            //右腕のちぎれ
            if (Input.GetKey(KeyCode.Space))
            {
                leftShoulder.GetComponent<SkinnedMeshRenderer>().enabled = false;
                m_Animator.speed = 0;
            }

            m_LazerCoolDown -= 1f * Time.deltaTime;

            //攻撃
            if (Input.GetKey(KeyCode.Alpha1))
            {
                EnemyAttack();
            }
            else if (Input.GetKey(KeyCode.Alpha2) && m_LazerCoolDown <= 0)
            {
                EnemyLazer();
                m_LazerCoolDown = 20.0f;
            }
            else if(m_Animator.GetBool("IsKnee"))
            {
                c += CircleCount();

                if(c >= 4)
                {
                    isDead = true;
                }
            }
            else
            {
                EnemyNormal();
            }

            //自爆
            if (m_Animator.GetBool("IsExplosion"))
            {
                EnemyExplosion();
            }

            //Debug.Log(c);
        }

        //Transformの循環コピー
        //static void CopyTransformsRecurse(Transform src, Transform dst)
        //{
        //    dst.position = src.position;
        //    dst.rotation = src.rotation;

        //    foreach (Transform child in dst)
        //    {
        //        // 同じ名前でTransformをマッチする = Match the transform with the same name
        //        Transform curSrc = src.Find(child.name);
        //        if (curSrc)
        //            CopyTransformsRecurse(curSrc, child);
        //    }
        //}

        //死んでいますか?
        public bool IsDead()
        {
            return isDead;
        }

        //トリガーに入ると同時に
        public void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Destination")
            {
                m_Animator.SetBool("IsExplosion", true);
            }
        }

        //トリガーに出ると同時に
        public void OnTriggerExit(Collider other)
        {
            Attack = false;
            m_Animator.SetBool("IsKnee", false);

        }

        //トリガに入っているときに
        public void OnTriggerStay(Collider other)
        {
            AnimatorStateInfo Info = m_Animator.GetCurrentAnimatorStateInfo(0);
            if (other.gameObject.tag == "Player" && Info.IsName("UpperBody.MoveTree"))
            {
                //agent.speed = 0;
                //Attack = true;
                time += Time.deltaTime;
                time2 += Time.deltaTime;
            }

            if (other.gameObject.tag == "Rope/Normal")
            {
                m_Animator.SetBool("IsKnee", true);
            }

            //同時に4点がロープとぶつかっているなら死ぬ
            //if (ropeCounter >= 4)
            //deadReplace = true;

            //Debug.Log(ropeCounter);
        }

        void FiringBeam(GameObject[] obj)
        {
            var lineRenderer = GetComponent<LineRenderer>();
            lineRenderer.SetPosition(0, obj[0].transform.position);
            lineRenderer.SetPosition(1, obj[1].transform.position);
        }

        void EnemyAttack()
        {

            agent.enabled = false;
            m_Animator.SetBool("IsAttack", true);

            GameObject target = GameObject.FindGameObjectWithTag("Player");
            int rotateSpeed = 1;
            Transform myTransform = this.transform;

            Debug.DrawLine(target.transform.position, this.transform.position, Color.yellow);

            myTransform.rotation = Quaternion.Slerp(
                myTransform.rotation,
                Quaternion.LookRotation(target.transform.position - myTransform.transform.position),
                rotateSpeed * Time.deltaTime);

        }

        void EnemyLazer()
        {

        }

        void EnemyNormal()
        {
            //agent.enabled = true;
            m_Animator.SetBool("IsAttack", false);
            agent.speed = m_Speed;

        }

        void EnemyExplosion()
        {
            transform.Find("DYNAMITE").transform.gameObject.SetActive(true);
        }

        int CircleCount()
        {
            Vector3 relative = transform.InverseTransformPoint(player.position);
            float angle = Mathf.Atan2(relative.x, relative.z) * Mathf.Rad2Deg;

            if (angle < 45 && angle > -45)
            {
                EnemyForward = true;
                //if (angle < 45)
                //    enemyDirection = (int)EnemyDirection.ACW_Front;
                //if (angle > -45)
                //    enemyDirection = (int)EnemyDirection.CW_Front;

                //EnemyForward = false;
            }
            if (angle > 45 && angle < 135)
            {
                EnemyRight = true;
                //if (angle < 45)
                //    enemyDirection = (int)EnemyDirection.ACW_Right;
                //if (angle > 135)
                //    enemyDirection = (int)EnemyDirection.CW_Right;

                //EnemyRight = false;
            }
            if ((angle > 135 && angle < 180) || (angle > -180 && angle < -135))
            {
                EnemyBack = true;
                //if (angle < 135)
                //    enemyDirection = (int)EnemyDirection.ACW_Back;
                //if (angle > -135)
                //    enemyDirection = (int)EnemyDirection.CW_Back;

                //EnemyBack = false;
            }
            if (angle > -45 && angle < -135)
            {
                EnemyLeft = true;
                //if (angle < 45)
                //    enemyDirection = (int)EnemyDirection.ACW_Left;
                //if (angle > 135)
                //    enemyDirection = (int)EnemyDirection.CW_Left;

                //EnemyLeft = false;
            }
            int cnt = 0;

            //Debug.Log(angle);


            //switch (enemyDirection)
            //{
            //    //時計回り
            //    //前から
            //    case (int)EnemyDirection.CW_Front:
            //        {
            if (angle < 45 && angle > -45) EnemyForward = true;
            if (EnemyForward && angle > 45 && angle < 135) EnemyRight = true;
            if (EnemyForward && EnemyRight && angle > 135 && angle < -135) EnemyBack = true;
            if (EnemyForward && EnemyRight && EnemyBack && angle > -135 && angle < -45) EnemyLeft = true;
            //        }
            //        break;

            //    //右から
            //    case (int)EnemyDirection.CW_Right:
            //        {
            if (angle > 45 && angle < 135) EnemyRight = true;
            if (EnemyRight && angle > 135 && angle < -135) EnemyBack = true;
            if (EnemyRight && EnemyBack && angle > -135 && angle < -45) EnemyLeft = true;
            if (EnemyRight && EnemyBack && EnemyLeft && angle > -45 && angle < 45) EnemyForward = true;
            //        }
            //        break;

            //    //後ろから
            //    case (int)EnemyDirection.CW_Back:
            //        {
            if (angle > 135 && angle < -135) EnemyBack = true;
            if (EnemyBack && angle > -135 && angle < -45) EnemyLeft = true;
            if (EnemyBack && EnemyLeft && angle > -45 && angle < 45) EnemyForward = true;
            if (EnemyBack && EnemyLeft && EnemyForward && angle > 45 && angle < 135) EnemyRight = true;
            //        }
            //        break;

            //    //左から
            //    case (int)EnemyDirection.CW_Left:
            //        {
            if (angle > 45 && angle < 135) EnemyLeft = true;
            if (EnemyLeft && angle > 135 && angle < -135) EnemyForward = true;
            if (EnemyLeft && EnemyForward && angle > -135 && angle < -45) EnemyRight = true;
            if (EnemyLeft && EnemyForward && EnemyRight && angle > -45 && angle < 45) EnemyBack = true;
            //        }
            //        break;


            //    //反時計回り
            //    //前から
            //    case (int)EnemyDirection.ACW_Front:
            //        {
            if (angle > -45 && angle < 45) EnemyForward = true;
            if (EnemyForward && angle > -135 && angle < -45) EnemyLeft = true;
            if (EnemyForward && EnemyLeft && angle > 135 && angle < -135) EnemyBack = true;
            if (EnemyForward && EnemyLeft && EnemyBack && angle > 45 && angle < 135) EnemyRight = true;
            //        }
            //        break;

            //    //右から
            //    case (int)EnemyDirection.ACW_Right:
            //        {
            if (angle > 45 && angle < 135) EnemyRight = true;
            if (EnemyRight && angle > 135 && angle < -135) EnemyBack = true;
            if (EnemyRight && EnemyBack && angle > -135 && angle < -45) EnemyLeft = true;
            if (EnemyRight && EnemyBack && EnemyLeft && angle > -45 && angle < 45) EnemyForward = true;
            //        }
            //        break;

            //    //後ろから
            //    case (int)EnemyDirection.ACW_Back:
            //        {
            if (angle > 135 && angle < -135) EnemyBack = true;
            if (EnemyBack && angle > 45 && angle < 135) EnemyRight = true;
            if (EnemyBack && EnemyRight && angle > -45 && angle < 45) EnemyForward = true;
            if (EnemyBack && EnemyRight && EnemyForward && angle > -135 && angle < -45) EnemyLeft = true;
            //        }
            //        break;

            //    //左から
            //    case (int)EnemyDirection.ACW_Left:
            //        {
            if (angle > -135 && angle < -45) EnemyLeft = true;
            if (EnemyLeft && angle > 135 && angle < -135) EnemyBack = true;
            if (EnemyLeft && EnemyBack && angle > 45 && angle < 135) EnemyRight = true;
            if (EnemyLeft && EnemyBack && EnemyRight && angle > -45 && angle < 45) EnemyForward = true;
            //        }
            //        break;
            //}

            //Debug.Log(EnemyForward);
            //Debug.Log(EnemyBack);
            //Debug.Log(EnemyLeft);
            //Debug.Log(EnemyRight);

            if (EnemyForward && EnemyLeft && EnemyRight && EnemyBack)
            {
                cnt++;

                EnemyForward = false;
                EnemyLeft = false;
                EnemyRight = false;
                EnemyBack = false;
            }

            return cnt;
        }
    }
}

