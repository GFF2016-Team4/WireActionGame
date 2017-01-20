using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public interface RecieveMessage : IEventSystemHandler
{
    void OnRecieve1();
    void OnRecieve2();
    void OnRecieve3();

}
