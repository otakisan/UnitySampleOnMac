using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class ShooterViewCameraBehaviour : MonoBehaviour
{

    public float speed = 200.0F;
    public float rotationSpeed = 100.0F;

    public float horizontalSpeed = 2.0F;
    public float verticalSpeed = 2.0F;

    public bool trackingNearestTarget;

    // Use this for initialization
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if (this.trackingNearestTarget)
        {
            this.LookAtTargetGradually2();
        }
#if MOBILE_INPUT
        float translation = CrossPlatformInputManager.GetAxisRaw("Vertical") * speed * 1;
        float rotation = CrossPlatformInputManager.GetAxisRaw("Horizontal") * rotationSpeed;
#else
        float translation = Input.GetAxis("Vertical") * speed;
        float rotation = Input.GetAxis("Horizontal") * rotationSpeed;
#endif
        translation *= Time.deltaTime;
        rotation *= Time.deltaTime;
        //transform.Translate(0, 0, translation);
        transform.Rotate(translation, rotation, 0);

        // Mouse
        //float h = horizontalSpeed * Input.GetAxis("Mouse X");
        //float v = verticalSpeed * Input.GetAxis("Mouse Y");
        //transform.Rotate(v, h, 0);


	}

    private void LookAtTargetGradually()
    {
        var target = ClayManagerBehaviour.FindTarget();
        if(target != null)
        {
            //Quaternion targetRot = Quaternion.FromToRotation(transform.forward, target.transform.position - transform.position);
            //var steps = 30 * Time.deltaTime;
            //transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, steps);

            // http://devblog.aliasinggames.com/smooth-lookat-unity/
            var targetRotation = Quaternion.LookRotation(target.transform.position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10 * Time.deltaTime);
        }
    }

    private Vector3 angularVelocity;
    private float dampSpeed = 0.015625f;
    private void LookAtTargetGradually2()
    {
        var target = ClayManagerBehaviour.FindClosestTarget();
        if (target != null)
        {
            var targetRotation = Quaternion.LookRotation(target.transform.position - transform.position).eulerAngles;

            Vector3 currentRotation = transform.eulerAngles;

            currentRotation.x = Mathf.SmoothDampAngle(currentRotation.x, targetRotation.x, ref angularVelocity.x, dampSpeed);
            currentRotation.y = Mathf.SmoothDampAngle(currentRotation.y, targetRotation.y, ref angularVelocity.y, dampSpeed);
            currentRotation.z = Mathf.SmoothDampAngle(currentRotation.z, targetRotation.z, ref angularVelocity.z, dampSpeed);

            transform.rotation = Quaternion.Euler(currentRotation);
        }
    }

    private void LookAtTargetImmediatelly()
    {
        var target = ClayManagerBehaviour.FindTarget();
        if (target != null)
        {
            Camera.main.transform.LookAt(target.transform);
        }
    }
}
