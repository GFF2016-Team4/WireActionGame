using UnityEngine;
using System.Collections;

public class EnemyPattern : MonoBehaviour
{
    public GameObject m_player;
    public float LaserAttackTime;

    //パンチの範囲
    public float Punch;

    [Header("パンチの行動にする間隔")]
    public float punchMinTime;
    public float punchMaxTime;
    private float punchTime;

    private int punchCount;

    [Header("レートが上がるほどレーザーになる確率が減る")]
    [SerializeField, Range(1, 100)]
    private int laserRate;


    //レーザーの範囲
    public float m_Laser;

    public float Lasertimer;

    [System.NonSerialized]
    public bool Laser;

    //[System.NonSerialized]
    public bool m_changeTag;

    Animator m_Animator;
    private float m_LazerCoolDown = 10f;

    [System.NonSerialized]
    public bool counter;

    // Use this for initialization
    void Start()
    {
        m_Animator = transform.Find("Enemy3").GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        m_LazerCoolDown -= 1f * Time.deltaTime;
        punchTime -= Time.deltaTime;

        int xDeistance = Mathf.RoundToInt(this.transform.position.x - m_player.transform.position.x);
        int zDeistance = Mathf.RoundToInt(this.transform.position.z - m_player.transform.position.z);
        //Debug.Log(zDeistance);
        //Debug.Log(xDeistance);

        AnimatorStateInfo anim = this.m_Animator.GetCurrentAnimatorStateInfo(0);

        //パンチの範囲内処理
        if (xDeistance <= Punch && xDeistance >= -Punch && zDeistance <= Punch && zDeistance >= -Punch && m_LazerCoolDown < 0)
        {
            PunchAttack(anim);
        }
        //レーザーの範囲内処理
        else if ((xDeistance <= m_Laser && xDeistance >= -m_Laser && zDeistance <= m_Laser && zDeistance >= -m_Laser) || m_LazerCoolDown > 0)
        {
            LaserAttack(anim);
        }
        else
        {
            Move();
        }

        if (counter == true)
            LaserAttack();
    }
    void LaserAttack()
    {
        if (Laser == false)
            Lasertimer += Time.deltaTime;

        if (Lasertimer >= LaserAttackTime)
        {
            Lasertimer = 0;
            Laser = true;
        }
    }

    void LaserAttack(AnimatorStateInfo anim)
    {
        m_Animator.SetBool("IsAttack", false);
        if (!m_Animator.GetBool("IsLazer") && m_LazerCoolDown <= 0)
        {
            m_Animator.SetBool("IsLazer", true);
            m_LazerCoolDown = 10f;
        }
           
        if (anim.fullPathHash == Animator.StringToHash("Base Layer.Lazer"))
        {
            counter = true;  
        }
        else
        {
            Lasertimer = 0;
        }
            

        //Debug.Log("レーザー範囲");
    }

    void PunchAttack(AnimatorStateInfo anim)
    {
        //Debug.Log("パンチ範囲");

        if(punchTime > 0)
        {
            Move();
            return;
        }

        int rand = Random.Range(punchCount, laserRate+1);

        if(rand == laserRate)
        {
            LaserAttack(anim);
            punchCount = 0;
            return;
        }

        if (!m_Animator.GetBool("IsAttack") && 
            !m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Explosion") &&
            punchTime <= 0)
        {
                
            m_Animator.SetBool("IsAttack", true);
            m_Animator.SetBool("IsLazer", false);
            punchTime = Random.Range(punchMinTime, punchMaxTime);

            punchCount++;
        }

        if (anim.fullPathHash == Animator.StringToHash("Base Layer.Attack"))
        {
            m_changeTag = true;
        }
        else
        {
            m_changeTag = false;
        }

        //Lasertimer = 0;

        //テスト用
        //timer += Time.deltaTime;
        //if (timer >= 10)
        //{
        //    m_changeTag = true;
        //    Debug.Log("パンチ範囲");
        //}
        //if (timer >= 15)
        //{
        //    timer = 0;
        //    m_changeTag = false;
        //}
    }

    void Move()
    {
        //Debug.Log("移動");
        m_Animator.SetBool("IsAttack", false);
        m_Animator.SetBool("IsLazer", false);
    }
}
