using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class EnemyGameover : MonoBehaviour
{
    public Transform Destination;
    public Transform PlayerTarget;
    private NavMeshAgent agent;

    public int walkSpeed;

    // Use this for initialization
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = walkSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        agent.destination = Destination.position;

        Vector3 eye = PlayerTarget.position;
        eye.y = transform.position.y;
        transform.LookAt(eye);
    }
}