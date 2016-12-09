using UnityEngine;
using System.Collections;

[AddComponentMenu("Camera-Control")]
public class MouseOrbitImproved : MonoBehaviour {
	
	public Transform target;
	public float distance = 1.0f;
    //マウス移動速度
	public float xSpeed = 120.0f;
	public float ySpeed = 120.0f;
	
	public float yMinLimit = -20f;
	public float yMaxLimit = 80f;
	
	public float distanceMin = .5f;
	public float distanceMax = 15f;

	public GameObject rightGunTag;
	public GameObject leftGunTag;
	
	//private Rigidbody rigidbody;

    //カメラの上下制限
    public float cameraLimitUp = 30f;
    public float cameraLimitDown = -30f;
	
    //カメラの視点
	float x = 0.0f;
	float y = 0.0f;
	
	void Start (){
		Vector3 angles = transform.eulerAngles;
		x = angles.y;
		y = angles.x;
		
		//GetComponent<Rigidbody>() = GetComponent<Rigidbody>();
		
		if (GetComponent<Rigidbody>() != null){
			GetComponent<Rigidbody>().freezeRotation = true;
		}

		Cursor.visible = false;
		//Screen.lockCursor = true;
	}
	
	void LateUpdate (){
		if (target){
            //カメラのリセット
            if(Input.GetKeyDown(KeyCode.R))
            {
                y = 0;
            }
            //カメラ移動速度
			x += Input.GetAxis("Mouse X") * xSpeed * 0.02f;
			y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;

            //カメラの上下移動制限
            if(y<= cameraLimitDown)
            {
                y = cameraLimitDown;
            }
            else if(y>= cameraLimitUp)
            {
                y = cameraLimitUp;
            }
			
			y = ClampAngle(y, yMinLimit, yMaxLimit);
			
			Quaternion rotation = Quaternion.Euler(y, x, 0);
			
			distance = Mathf.Clamp(distance, distanceMin, distanceMax);

            //三人称カメラ視点の調整
			Vector3 negDistance = new Vector3(0.0f, 0.2f, -distance);
			Vector3 position = rotation * negDistance + target.position;

			transform.rotation = rotation;
			rightGunTag.transform.rotation = rotation;
			leftGunTag.transform.rotation = rotation;

			transform.position = position;
		}
        //ESCを押したらカーソルを出せる
		if (Input.GetButtonDown ("Cancel")) {
			//Screen.lockCursor = false;
			Cursor.visible = true;
		}
	}
	
	public static float ClampAngle(float angle, float min, float max){
		if (angle < -360F)
			angle += 360F;
		if (angle > 360F)
			angle -= 360F;
		return Mathf.Clamp(angle, min, max);
	}
}