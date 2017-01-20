using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class sendMessageTest : MonoBehaviour 
{
    public GameObject aa;
    void Update () 
	{
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            ExecuteEvents.Execute<RecieveMessage>(
            target: aa,
            eventData: null,
            functor: (sendTest, y) => sendTest.OnRecieve1());
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ExecuteEvents.Execute<RecieveMessage>(
            target: aa,
            eventData: null,
            functor: (sendTest, y) => sendTest.OnRecieve2());
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            ExecuteEvents.Execute<RecieveMessage>(
            target: aa,
            eventData: null,
            functor: (sendTest, y) => sendTest.OnRecieve3());
        }

    }

}
