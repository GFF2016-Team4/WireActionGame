using UnityEngine;
using System.Collections;

public class WalkSmoke : MonoBehaviour {
    private ParticleSystem ps;
	// Use this for initialization
	public void Start () {
        ps = GetComponent<ParticleSystem>();
	}

    // Update is called once per frame
    void Update()
    {
        if (!ps.IsAlive())
        {
            Destroy(gameObject);
        }
    }
}
