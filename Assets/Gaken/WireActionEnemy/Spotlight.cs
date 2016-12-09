using UnityEngine;
using System.Collections;

public class Spotlight : MonoBehaviour {

    public float lightIntensity = 0.2f;
    public GameObject MyLight;
	// Use this for initialization
	void Start () {
        //MyLight = transform.Find("Spotlight").gameObject;
	}
	
	// Update is called once per frame
	void Update () {
	    if(MyLight)
        {
            MyLight.GetComponent<Light>().intensity = lightIntensity;
        }
    }
}
