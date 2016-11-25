using UnityEngine;
using System.Collections;

public class ropeController : MonoBehaviour {

    //射出弾のプレハブ
	public GameObject bulletPrefab;
    //右手射出機
	public GameObject rightGunTag;
    //左手射出機
    public GameObject leftGunTag;
    //右射出弾インスタンス
	private GameObject rightBulletInstance;
    //左射出弾インスタンス
    private GameObject leftBulletInstance;

    public GameObject player;

    private RopeSimulate leftRope;
    private GameObject leftRopeInstance;
    private bool isCreateLeftRope = false;

    public GameObject ropePrefab;

	public Camera camera;
    //
	public float pullSpeed = 1f;

	public float ropeSize = 0.01f;
	private LineRenderer lineRendererRight;
	private LineRenderer lineRendererLeft;
	
	void Start () {
		lineRendererRight = rightGunTag.GetComponent<LineRenderer> ();
		lineRendererRight.SetWidth (0, 0);
		lineRendererLeft = leftGunTag.GetComponent<LineRenderer> ();
		lineRendererLeft.SetWidth (0, 0);
	}
	
	void Update () {
		//左射出機
		if (Input.GetButtonDown ("Fire1")) {
			leftBulletInstance = Instantiate(bulletPrefab, leftGunTag.transform.position, leftGunTag.transform.rotation) as GameObject;
		}
		
		if (Input.GetButton ("Fire1")) {
			if (leftBulletInstance.GetComponent<Rigidbody> ().isKinematic) {
				transform.GetComponent<Rigidbody>().AddForce(leftBulletInstance.transform.position);

                if (isCreateLeftRope == false)
                {
                    CreateLeftRope();
                }

                player.transform.position = leftRope.transform.position;
            } 
			lineRendererLeft.SetWidth(ropeSize, ropeSize);
			lineRendererLeft.SetPosition (0, leftGunTag.transform.position);
			lineRendererLeft.SetPosition (1, leftBulletInstance.transform.position);
        }
        else if (!Input.GetButton ("Fire1") && leftBulletInstance != null) {
            EraseLeftRope();

            Destroy(leftBulletInstance);
			lineRendererLeft.SetWidth (0, 0);
		}


        //右射出機
        if (Input.GetButtonDown("Fire2"))
        {
            rightBulletInstance = Instantiate(bulletPrefab, rightGunTag.transform.position, rightGunTag.transform.rotation) as GameObject;
        }

        if (Input.GetButton("Fire2"))
        {
            if (rightBulletInstance.GetComponent<Rigidbody>().isKinematic)
            {
                transform.GetComponent<Rigidbody>().AddForce(rightBulletInstance.transform.position);
            }
            lineRendererRight.SetWidth(ropeSize, ropeSize);
            lineRendererRight.SetPosition(0, rightGunTag.transform.position);
            lineRendererRight.SetPosition(1, rightBulletInstance.transform.position);
        }
        else if (!Input.GetButton("Fire2") && rightBulletInstance != null)
        {
            Destroy(rightBulletInstance);
            lineRendererRight.SetWidth(0, 0);
        }

    }

    void CreateLeftRope()
    {
        leftRopeInstance = Instantiate(ropePrefab) as GameObject;
        //leftRopeInstance.GetComponent<RopeSimulate>().RopeInitialize(leftBulletInstance.transform.position, leftGunTag.transform.position);
        isCreateLeftRope = true;
    }

    void EraseLeftRope()
    {
        leftRope = leftRopeInstance.GetComponent<RopeSimulate>();

        leftRope.RopeEnd();
        //Destroy(ropeInstance);
        isCreateLeftRope = false;
    }
}
