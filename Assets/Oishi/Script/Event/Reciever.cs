using UnityEngine;
using System.Collections;

public class Reciever : MonoBehaviour, RecieveMessage
{

    public void OnRecieve1()
    {
        transform.position = new Vector3(0, 4, 0);
        Debug.Log("OnRecieve1");
    }

    public void OnRecieve2()
    {
        transform.position = new Vector3(4, 4, 0);
        Debug.Log("OnRecieve2");
    }

    public void OnRecieve3()
    {
        transform.position = new Vector3(0, 4, 4);
        Debug.Log("OnRecieve3");
    }

}
