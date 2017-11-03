using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCapsuleBehaviour : MonoBehaviour {

	// Use this for initialization
	void Start () {
        var rigidBody = GetComponent<Rigidbody>();
        //rigidBody.AddForce(new Vector3(-20,20,100), ForceMode.VelocityChange);
        //rigidBody.AddRelativeForce(rigidBody.transform.up * 80, ForceMode.VelocityChange);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnMouseDown()
    {
        var cameraRayToMouse = Camera.main.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(cameraRayToMouse.origin, cameraRayToMouse.direction * 100, Color.yellow, 1);

        this.MainCameraLookAtTarget();
    }

    private void MainCameraLookAtTarget()
    {
        var target = GameObject.Find("ClaySphere");
        if(target != null)
        {
            Camera.main.transform.LookAt(target.transform);
        }
    }
}
