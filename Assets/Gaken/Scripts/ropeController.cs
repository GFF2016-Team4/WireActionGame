using UnityEngine;
using System.Collections;

public class ropeController : MonoBehaviour {

	public GameObject bulletPrefab;
	public GameObject rightGunTag;
	public GameObject leftGunTag;
	private GameObject rightBulletInstance;
	private GameObject leftBulletInstance;
	public Camera camera;
	public float pullSpeed = 1f;
	public float ropeSize = 0.01f;
	private Vector3 pos;
	private LineRenderer lineRendererRight;
	private LineRenderer lineRendererLeft;
	
	void Start () {
		lineRendererRight = rightGunTag.GetComponent<LineRenderer> ();
		lineRendererRight.SetWidth (0, 0);
		lineRendererLeft = leftGunTag.GetComponent<LineRenderer> ();
		lineRendererLeft.SetWidth (0, 0);
	}
	
	void Update () {
		if (Input.GetButtonDown ("Fire1")) {
			rightBulletInstance = Instantiate(bulletPrefab, rightGunTag.transform.position, rightGunTag.transform.rotation) as GameObject;
		}

		if (Input.GetButton ("Fire1")) {
			if (rightBulletInstance.GetComponent<Rigidbody> ().isKinematic) {
				transform.GetComponent<Rigidbody>().AddForce(rightBulletInstance.transform.position);
			} 
			lineRendererRight.SetWidth(ropeSize, ropeSize);
			lineRendererRight.SetPosition (0, rightGunTag.transform.position);
			lineRendererRight.SetPosition (1, rightBulletInstance.transform.position);
		}else if (!Input.GetButton ("Fire1") && rightBulletInstance != null) {
			Destroy (rightBulletInstance);
			lineRendererRight.SetWidth (0, 0);
		}

		if (Input.GetButtonDown ("Fire2")) {
			leftBulletInstance = Instantiate(bulletPrefab, leftGunTag.transform.position, leftGunTag.transform.rotation) as GameObject;
		}
		
		if (Input.GetButton ("Fire2")) {
			if (leftBulletInstance.GetComponent<Rigidbody> ().isKinematic) {
				transform.GetComponent<Rigidbody>().AddForce(leftBulletInstance.transform.position);
			} 
			lineRendererLeft.SetWidth(ropeSize, ropeSize);
			lineRendererLeft.SetPosition (0, leftGunTag.transform.position);
			lineRendererLeft.SetPosition (1, leftBulletInstance.transform.position);
		}else if (!Input.GetButton ("Fire2") && leftBulletInstance != null) {
			Destroy (leftBulletInstance);
			lineRendererLeft.SetWidth (0, 0);
		}
	}
}
