using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClaySphereBehaviour : MonoBehaviour {

    private Rigidbody myrigidbody;
    private bool firstFrame = true;

	// Use this for initialization
	void Start () {
        //this.myrigidbody = GetComponent<Rigidbody>();
        //this.myrigidbody.AddForce(new Vector3(-20, 20, 20), ForceMode.VelocityChange);
	}
	
	// Update is called once per frame
	void Update () {
        //if(this.currentClay == null)
        //{
        //    this.ShootNewClay();
        //}
	}

    private void OnCollisionEnter(Collision collision)
    {
        if(this.gameObject.name != "OriginalClaySphere" && collision.collider.CompareTag("FieldFloor"))
        {
            //Debug.Log("着地");
            this.DestoryCurrentClay();
            GameObject.Find("ScoreText").GetComponent<ScoreTextBehaviour>().UpdateScore(this.gameObject.tag, CompleteAction.Away);

        }
    }

    private GameObject currentClay;
    private void ShootNewClay()
    {
        if(this.currentClay != null)
        {
            return;
        }

        this.currentClay = Instantiate(this.gameObject, this.gameObject.transform);
        this.currentClay.transform.SetParent(this.transform.parent);
        this.currentClay.GetComponent<Rigidbody>().AddForce(new Vector3(-20, 20, 20), ForceMode.VelocityChange);

        //this.currentClay = newClay;
    }

    private void DestoryCurrentClay()
    {
        Destroy(this.gameObject);
        //this.currentClay = null;
    }
}
