using UnityEngine;
using System.Collections;

public class shakeObj : MonoBehaviour
{

    public GameObject shakeObject;
    public GameObject Pcamera;
    public float x;
    public float y;
    public float time;

    Vector3 tempPos;
    void Start()
    {
        shakeObject.transform.localPosition = shakeObject.transform.parent.GetComponent<PlayerCamera>().offset;
    }

    void Update()
    {
        shakeObject.transform.parent.GetComponent<PlayerCamera>().offset = shakeObject.transform.localPosition;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Field")
        {
            tempPos = shakeObject.transform.parent.GetComponent<PlayerCamera>().offset;
            iTween.PunchPosition(shakeObject, iTween.Hash("x", x, "y", y, "time", time));
        }
    }
}
