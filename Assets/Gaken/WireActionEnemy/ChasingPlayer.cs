using UnityEngine;
using System.Collections;

public class ChasingPlayer : MonoBehaviour {
	
	private Transform playerTransform;
	private Vector3 distanceFromPlayer;
	
	void Start () {
		playerTransform = GameObject.Find("Player").transform;
		distanceFromPlayer = transform.position - playerTransform.position;
	}
	
	// カメラ位置を更新するのは、
	// ターゲットの移動が終わった後である必要があるため、
	// LateUpdate内で行う
	void LateUpdate () {
		transform.position = playerTransform.position + distanceFromPlayer;
	}
}
