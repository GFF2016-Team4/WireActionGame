using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PointsMove : MonoBehaviour {

	public GameObject oyaObj;
	private GameObject[] childObj;
	private Vector3[] childPos;

	public int moveTime;

	int childcount;

	// Use this for initialization
	void Start ()
	{
		childcount = oyaObj.transform.childCount;

		childPos=new Vector3[childcount];
		childObj = new GameObject[childcount];


		for(int i =0; i< childcount; i++)
		{
			childObj[i]=oyaObj.transform.GetChild(i).gameObject;
			childPos [i] = childObj [i].transform.position;
		}
	}
	void Update ()
	{	
		if (Input.GetKeyDown (KeyCode.Space)) 
		{
			Move ();
		}
	}

	void Move()
	{
		iTween.MoveTo (gameObject, iTween.Hash ("path", childPos,
												"time", moveTime));
											 
	}
}
