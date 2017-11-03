using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleSphereBehaviourScript : MonoBehaviour {

    private Rigidbody rigidbody;

	// Use this for initialization
	void Start () {
        this.rigidbody = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    
    private void OnMouseDown()
    {
        this.rigidbody.AddForce(Vector3.up * 10f, ForceMode.VelocityChange);
    }

    
}
