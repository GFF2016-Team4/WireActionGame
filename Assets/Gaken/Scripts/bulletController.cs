using UnityEngine;
using System.Collections;

public class bulletController : MonoBehaviour {

    //射出力
	public float force = 300f;

	void Start () {
		transform.GetComponent<Rigidbody>().AddForce(transform.forward * force);
	}
	
	void Update () {

	}

	void OnCollisionEnter(Collision collision){
		transform.GetComponent<Rigidbody> ().isKinematic = true;      
	}
}
