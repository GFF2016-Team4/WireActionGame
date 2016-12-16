using UnityEngine;
using System.Collections;

public class Respawn : MonoBehaviour
{

    private Transform respawnPosition;
    
    void Start()
    {
        respawnPosition.position = gameObject.transform.position;
    }

    void Update()
    {

    }

}
