using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShowToggleDebug : MonoBehaviour
{
    [SerializeField, Tooltip("説明文")]
    bool showDebug;


    void Awake()
    {
        Debug.logger.logEnabled = showDebug;
    }
    
    void Start()
    {
        
    }
    
    void Update()
    {
        
    }
}