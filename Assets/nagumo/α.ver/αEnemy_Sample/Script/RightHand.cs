using UnityEngine;
using System.Collections;

public class RightHand : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    void OnTriggerEnter(Collider collider)
    {
        GameObject.Destroy(collider.gameObject);
    }
}
