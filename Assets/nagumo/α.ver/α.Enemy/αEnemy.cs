using UnityEngine;
using System.Collections;

public class αEnemy : MonoBehaviour {

    private NavMeshAgent agent;
    private float time;

    public Transform Destination;
    public float AttackTime;
    public float walkSpeed;
    public bool Attack;

    // Use this for initialization
    void Start () {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = walkSpeed;
	}
	
	// Update is called once per frame
	void Update () {
        agent.destination = Destination.position;

        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 10.0f))
        {
            if(hit.collider.gameObject.name == "kabe")
            {
                Debug.Log(hit.collider.gameObject.name + "発見");
            }
        }
        if(Attack == true)
        {
            time += Time.deltaTime;
            if (time >= AttackTime)
            {
                time = 0.0f;
                int ran = Random.Range(0, 3);
                if (ran == 1)
                {
                    Debug.Log("Attack");
                }
            }
        }
	}

    void OnTriggerStay(Collider collider)
    {
        if(collider.gameObject.tag == "Player")
        {
            Attack = true;
        }
    }
    void OnTriggerExit(Collider collider)
    {
        if(collider.gameObject.tag == "Player")
        {
            Attack = false;
        }
    }
}
