using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Clear : MonoBehaviour
{
    //[SerializeField, Tooltip("説明文")]    
    void Start()
    {
        SoundManager.Instance.PlayBGM(AUDIO.SE_Clear);
    }
}