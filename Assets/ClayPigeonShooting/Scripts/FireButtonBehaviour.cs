using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class FireButtonBehaviour : MonoBehaviour/*, IPointerClickHandler*/
{
    public void OnPointerClick(PointerEventData eventData)
    {
        this.ShootRay();
    }

    private List<string> notRaycastTargets = new List<string>() { "FieldFloor", "ShootingFloor", "OriginalClaySphere" };
    public void ShootRay()
    {
        //Debug.Log("Fire!");

        var gunsight = GameObject.Find("GunSightPlane");

        //var cameraRay = Camera.main.ScreenPointToRay(gunsight.transform.position);
        //var cameraViewPortRay = Camera.main.ViewportPointToRay(gunsight.transform.position);

        var gunsightWorld = gunsight.transform.TransformPoint(gunsight.transform.position);
        //gunsightWorld.z -= -1;
        var gunsightScreenPoint = Camera.main.WorldToScreenPoint(gunsightWorld);
        var gunsightViewpoint = Camera.main.WorldToViewportPoint(gunsightWorld);

        var cameraRay = Camera.main.ScreenPointToRay(gunsightScreenPoint);
        var cameraViewPortRay = Camera.main.ViewportPointToRay(gunsightViewpoint);

        var ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward * 100);


        //var cameraRayToMouse = Camera.main.ScreenPointToRay(Input.mousePosition);


        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.rotation * gunsight.transform.localPosition * 100, Color.black, 1);
        Debug.DrawRay(cameraRay.origin, cameraRay.direction * 100, Color.red, 1);
        Debug.DrawRay(ray.origin, ray.direction * 1000, Color.blue, 1);
        //Debug.DrawRay(cameraRayToMouse.origin, cameraRayToMouse.direction * 100, Color.yellow, 1);

        RaycastHit hit;
        //if(Physics.Raycast(cameraRay, out hit))
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, float.MaxValue, int.MaxValue) && !this.notRaycastTargets.Any(target => hit.collider.name == target ))
        {
            hit.collider.GetComponent<MeshRenderer>().material.color = Color.blue;

            var particle = Instantiate(GameObject.Find("ExplosionParticle"));
            particle.transform.position = hit.collider.gameObject.transform.position;
            var particlesystem = particle.GetComponent<ParticleSystem>();
            particlesystem.Play(true);
            Destroy(particle.gameObject, particlesystem.main.duration + particlesystem.main.startLifetime.constant);

            Destroy(hit.collider.gameObject);

            ClayManagerBehaviour.GetClayManager().HitTargetClay(hit.collider.tag);
        }

    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
