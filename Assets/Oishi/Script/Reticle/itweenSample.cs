using UnityEngine;
using System.Collections;

public class itweenSample : MonoBehaviour
{

    public void Move(GameObject obj, Vector3 obj2)
    {
        iTween.MoveTo(obj, obj2, 2.0f);
    }

    public void Return(GameObject obj, GameObject obj2)
    {
        iTween.MoveTo(obj, obj2.transform.position, 2.0f);
    }

}
