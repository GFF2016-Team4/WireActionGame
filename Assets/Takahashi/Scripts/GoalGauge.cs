using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GoalGauge : MonoBehaviour {
    public GameObject enemy;
    public GameObject goal;
    private Vector3 enemyPos;
    private Vector3 goalPos;
    private float maxDistance;
    private float distance;

    private Slider slider;
    // Use this for initialization
    void Start () {
        enemyPos = enemy.transform.position;
        goalPos = goal.transform.position;
        maxDistance = Vector3.Distance(enemyPos, goalPos);
        slider = GetComponent<Slider>();
        slider.maxValue = maxDistance;
    }

    // Update is called once per frame
    void Update () {
        enemyPos = enemy.transform.position;
        distance = Vector3.Distance(enemyPos, goalPos);
        slider.value = distance;
    }
}
