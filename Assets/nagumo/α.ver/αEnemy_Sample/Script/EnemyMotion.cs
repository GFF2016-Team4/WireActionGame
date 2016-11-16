using UnityEngine;
using System.Collections;

public class EnemyMotion : MonoBehaviour {

    private NavMeshAgent agent;
    Animator animator;
    private float time;
    private float time2;

    public bool Attack;
    private float NextAttackTime = 2;
    public Transform Destination;
    public Transform PlayerTarget;
    public float walkSpeed;
	// Use this for initialization
	void Start () {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        agent.speed = walkSpeed;

        time2 += 2;
    }

    // Update is called once per frame
    void Update()
    {
        agent.speed = walkSpeed;
        agent.destination = Destination.position;
        AnimatorStateInfo Info = animator.GetCurrentAnimatorStateInfo(0);
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
        if (Attack == true)
        {
            if (time >= 0.3f)
            {
                animator.SetTrigger("Attack");
                Attack = false;
                time = 0;
            }
        }
        if (Info.IsName("BaseLayer.Attack"))
        {
            agent.speed = 0;
            Debug.Log("攻撃now");
        }
        
        if(time2 >= NextAttackTime)
        {
            time2 = 0;
            Attack = true;

            Vector3 eye = PlayerTarget.position;
            eye.y = transform.position.y;
            transform.LookAt(eye);
        }
    }

    void OnTriggerStay(Collider collider)
    {
        AnimatorStateInfo Info = animator.GetCurrentAnimatorStateInfo(0);
        if (collider.gameObject.tag == "Player" && Info.IsName("BaseLayer.idle"))
        {
            //agent.speed = 0;
            //Attack = true;
            time += Time.deltaTime;
            time2 += Time.deltaTime;
        }
    }

    void OnTriggerExit(Collider collider)
    {
        Attack = false;
    }
}
