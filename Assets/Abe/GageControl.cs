using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GageControl : MonoBehaviour
{
    //[SerializeField, Tooltip("説明文")]

    [SerializeField]
    Transform destination;

    [SerializeField]
    Transform enemy;

    Slider slider;

    float startDistance;
      
    void Awake()
    {
        slider = GetComponent<Slider>();
        startDistance = Vector3.Distance(destination.position, enemy.position);
    }
    
    void Start()
    {
        
    }
    
    void Update()
    {
        slider.value = Vector3.Distance(destination.position, enemy.position) / startDistance;
    }
}