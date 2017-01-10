using UnityEngine;
using System.Collections;

public class ColliderWithHand : MonoBehaviour
{
    public Transform parent;

    // Use this for initialization
    void Start()
    {
        parent = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnCollisionExit(Collision collision)
    {
        if(collision.transform.tag == "Floor")
        {
            Debug.Log("ok");
            parent.GetComponent<Animator>().speed = 0;

            //parent.SendMessage();
        }
    }
}
