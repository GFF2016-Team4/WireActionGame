using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SoundSpeedToVolume : MonoBehaviour
{
    [SerializeField, Tooltip("説明文")]
    AudioSource source;

    [SerializeField]
    Rigidbody rig;

    [SerializeField]
    float maxSpeed;

    private void OnValidate()
    {
        if(maxSpeed <= 0.0f)
        {
            maxSpeed = 0.00001f;
        }
    }
    
    void Update()
    {
        float vel = rig.velocity.magnitude;

        float volume = vel / maxSpeed;
        source.volume = Mathf.Lerp(source.volume, volume, 0.25f);
    }
}