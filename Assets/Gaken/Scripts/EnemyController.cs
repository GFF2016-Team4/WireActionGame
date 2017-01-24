using UnityEngine;
using System.Collections;

namespace Gaken
{
    public class EnemyController : MonoBehaviour
    {
        private NavMeshAgent agent;

        //private float time;
        //private float time2;
        
        //public bool Attack;
        //private float NextAttackTime = 2;

        public Transform Destination;
        public Transform player;

        float m_VelocityY = 0f;                 // y軸方向の移動量
        public float m_Speed = 10f;                     // 前進速度（前進はプラス、後退はマイナス）

        //bool m_IsAttack = false;                         //攻撃パターンフラグ
        //bool m_IsAttackReady = false;
        //bool m_IsAttackFinish = false;

        //public bool ikActive = false;

        //public Transform leftShoulder;
        //public Transform leftHand;      //左腕Transform

        public float m_LazerCoolDown = 10f;
        public float m_CountDown = 10.0f;
        public float m_WaitTime = 2.0f;

        public float m_WaitTimeCount = 2f;
        /************************************************************************
                                      仮宣言 
        ************************************************************************/
        CharacterController m_Controller;    //キャラクタコントローラ
        Animator m_Animator;                 //アニメター
        Camera m_Camera;
        public GameObject m_Lazer;
        Rigidbody m_Rigidbody;

        private bool isDead;            //死亡切替を行うか?
        //private int ropeCounter;             //ロープとエネミーの接触点数

        //private float x = 0; // 水平方向
        //public float xLimit; // 水平方向の限度値
        //private float initX = 30;
        //private float y = 0; // 垂直方向
        //public float yLimit = 30; //垂直方向の限度値
        //private float initY;

        //private bool setWaitshotFlag = false; //　構え終わった時に値をセットしたらOn

        bool EnemyForward = false, EnemyLeft = false, EnemyRight = false, EnemyBack = false;
        int deathCount = 0;

        GameObject target;
        float rotateSpeed = 0.8f;
        Transform m_Transform;

        void Start()
        {
            //初期化
            isDead = false;
            m_Controller = GetComponent<CharacterController>();
            agent = GetComponent<NavMeshAgent>();
            m_Camera = transform.FindChild("Camera").GetComponent<Camera>();
            m_Rigidbody = transform.GetComponent<Rigidbody>();
            m_Transform = transform.GetComponent<Transform>();

            target = GameObject.FindGameObjectWithTag("Player");
            agent.speed = m_Speed = 10f;

            //m_Lazer = transform.GetComponent<GameObject>();
            //time2 += 1;
            //ropeCounter = 0;

            //アニメターは子のアニメターを取得
            m_Animator = transform.Find("EnemyRobot").GetComponent<Animator>();             //こっちはfbx形式

            /************************************************************************
                                        仮初期化 
            ************************************************************************/
            //右腕の取得（使えるかどうか確定できていない）
            //leftHand = transform.GetComponent<Transform>();
            //player = transform.Find("Player").GetComponent<Transform>();

        }

        void Update()
        {
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
            agent.destination = Destination.position;

            //AnimatorStateInfo Info = m_Animator.GetCurrentAnimatorStateInfo(0);
            //Ray ray = new Ray(transform.position, transform.forward);
            //RaycastHit hit;

            //Debug.DrawRay(ray.origin, ray.direction, Color.blue, 2f);

            //if (Physics.Raycast(ray, out hit, 1.5f))
            //{
            //    if (hit.collider.gameObject.name == "kabe" && Info.IsName("BaseLayer.idle"))
            //    {
            //        agent.speed = 0;
            //        Debug.Log(hit.collider.gameObject.name + "発見");
            //        Attack = true;
            //        time += Time.deltaTime;
            //    }
            //}

            //if (time2 >= NextAttackTime)
            //{
            //    time2 = 0;
            //    Attack = true;

            //    //PlayerTargetに指定したObjectを見続ける
            //    Vector3 eye = player.position;
            //    eye.y = transform.position.y;
            //    transform.LookAt(eye);
            //}

            //if (Attack == true)
            //{
            //    if (time >= 0.3f)
            //    {
            //        m_Animator.SetTrigger("Attack");
            //        Attack = false;
            //        time = 0;
            //    }
            //}
            //モーションのAttack中の処理
            //if (Info.IsName("UpperBody.Attack"))
            //{
            //    agent.speed = 0;

            //    m_Animator.SetBool("IsAttack", true);

            //    Debug.Log("攻撃now");
            //}

            m_Animator.SetFloat("Speed", agent.speed);

            /****************************************************************
                                        仮更新
            ****************************************************************/
            //アニメーション一時停止
            //if (Input.GetKey(KeyCode.Alpha3))
            //{
            //    m_Animator.speed = 0;
            //}
            //else
            //{
            //    m_Animator.speed = 1;
            //}

            //右腕のちぎれ
            //if (Input.GetKey(KeyCode.Space))
            //{
            //    leftShoulder.GetComponent<SkinnedMeshRenderer>().enabled = false;
            //}

            m_LazerCoolDown -= 1f * Time.deltaTime;
            m_WaitTime -= 1f * Time.deltaTime;

            //攻撃
            if (m_Animator.GetBool("IsAttack"))
            {
                EnemyAttack();
            }
            else if (m_Animator.GetBool("IsLazer") && m_LazerCoolDown <= 0)
            {
                EnemyLazer();
                //agent.enabled = false;
            }
            else
            {
                EnemyNormal();
            }

            if (m_Animator.GetBool("IsKnee"))
            {
                deathCount += CircleCount();
                if (deathCount >= 4)
                {
                    isDead = true;
                }
            }

            //自爆
            if (m_Animator.GetBool("IsExplosion"))
            {
                agent.speed = 0;
                m_Camera.depth = 2;
                m_Animator.SetBool("IsAttack", false);

                m_CountDown -= 1 * Time.deltaTime;

                if (m_CountDown <= 0)
                    EnemyExplosion();
            }
        }

        //死んでいますか?
        public bool IsDead()
        {
            return isDead;
        }

        //トリガーに入ると同時に
        public void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Destination" && m_Animator.GetCurrentAnimatorStateInfo(0).IsName("MoveTree"))
            {
                m_Animator.SetBool("IsExplosion", true);
            }
        }

        //トリガーに出ると同時に
        public void OnTriggerExit(Collider other)
        {
            if (other.gameObject.tag == "Rope/Normal")
            {
                m_Animator.SetBool("IsKnee", false);
            }
        }

        //トリガに入っているときに
        public void OnTriggerStay(Collider other)
        {
            if (other.gameObject.tag == "Rope/Normal")
            {
                m_Animator.SetBool("IsKnee", true);
            }
        }

        //void FiringBeam(GameObject[] obj)
        //{
        //    var lineRenderer = GetComponent<LineRenderer>();
        //    lineRenderer.SetPosition(0, obj[0].transform.position);
        //    lineRenderer.SetPosition(1, obj[1].transform.position);
        //}

        void RotateToPlayer(Quaternion offset)
        {
            //Debug.DrawLine(target.transform.position, this.transform.position, Color.yellow);
            m_Transform.rotation = Quaternion.Slerp(
                m_Transform.rotation,
                Quaternion.LookRotation(target.transform.position - m_Transform.transform.position) * offset,
                rotateSpeed * Time.deltaTime);
        }
        void RotateToPlayer()
        {
            Debug.Log(123);
            //Debug.DrawLine(target.transform.position, this.transform.position, Color.yellow);
            m_Transform.rotation = Quaternion.Slerp(
                m_Transform.rotation,
                Quaternion.LookRotation(target.transform.position - m_Transform.transform.position),
                rotateSpeed * Time.deltaTime);
        }

        void EnemyAttack()
        {
            m_Animator.SetBool("IsAttack", false);
            //m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
            agent.speed = 0;
            RotateToPlayer(Quaternion.AngleAxis(-25, Vector3.up));
        }

        void EnemyLazer()
        {
            agent.speed = 0;
            if(m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Lazer"))
            {
                RotateToPlayer();
            }
            else
            {
                m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
            }
        }

        void EnemyNormal()
        {
            //m_Animator.SetBool("IsAttack", false);
            if (m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
            {
                agent.speed = 0;
            }
            else if (m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Lazer") || m_Animator.GetCurrentAnimatorStateInfo(0).IsName("LazerOver"))
            {
                agent.speed = 0;
            }
            else
            {
                agent.speed = m_Speed;
            }
        }

        void EnemyExplosion()
        {
            transform.Find("DYNAMITE").transform.gameObject.SetActive(true);
            transform.GetComponent<EnemyController>().enabled = false;
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

