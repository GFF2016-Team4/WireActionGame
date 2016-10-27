using UnityEngine;
using System.Collections;

public class EnemyBattle : MonoBehaviour
{
    public Transform Destination;
    public Transform PlayerTarget;
    private NavMeshAgent agent;

    private int Ran;
    private int Rand;

    public float time;
    public float EnemyAttack;
    public float EnemyMissileAttack;
    public int walkSpeed;

    public bool Attack;
    public bool MissileAttack;

    // Use this for initialization
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = walkSpeed;
    }

    void OnTriggerStay(Collider collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            Attack = true;
            MissileAttack = false;
            //agent.speed = walkSpeed;
            //agent.speed = 0;
        }
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            Attack = false;
            MissileAttack = true;
            //agent.speed = walkSpeed;
        }
    }

    // Update is called once per frame
    void Update()
    {
        agent.destination = Destination.position;

        if (Attack == true)
        {
            time += Time.deltaTime;

            if (time >= EnemyAttack)
            {
                time = 0.0f;
                Ran = Random.Range(0, 3);
                //Debug.Log(Ran);

                if (Ran == 1)
                {
                    Debug.Log("攻撃！");
                }
                if (Ran == 2)
                {
                    Debug.Log("マジ殴り");
                }
            }
            Vector3 eye = PlayerTarget.position;
            eye.y = transform.position.y;
            transform.LookAt(eye);
        }

        if (MissileAttack==true)
        {
            time += Time.deltaTime;
            if (time>=EnemyMissileAttack)
            {
                time = 0.0f;
                Rand = Random.Range(0, 3);
                //Debug.Log(Rand);

                if(Rand == 1)
                {
                    Debug.Log("ミサイル");
                }
                if(Rand == 2)
                {
                    Debug.Log("超電磁砲");
                }
            }
        }

    }
    
}
