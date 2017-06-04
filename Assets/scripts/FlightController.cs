using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlightController : MonoBehaviour {

    [SerializeField]
    float forwardThrust = 100f;

    [SerializeField, Range(0, 1)]
    float reverseThrustFactor = 0.2f;

    [SerializeField, Range(0, 1)]
    float angularThrustFactor = 0.1f;

    [SerializeField]
    bool flippedAngularDirection = true;

    public bool interplanitaryFlight = true;

    public Rigidbody rb;

	void Start () {
        rb = GetComponent<Rigidbody>();
	}
	
	void Update () {

        if (interplanitaryFlight)
        {
            float thrust = Input.GetAxis("Vertical");
            float angular = Input.GetAxis("Horizontal");

            thrust *= forwardThrust * (Mathf.Sign(thrust) == 1f ? 1f : reverseThrustFactor) * Time.deltaTime;
            angular *= forwardThrust * angularThrustFactor * Time.deltaTime * (flippedAngularDirection ? -1f : 1f);

            rb.AddForce(transform.up * thrust);
            rb.AddTorque(Vector3.forward * angular);
        }
	}

    public void LeaveOrbit(float orbitSpeed)
    {
        Vector3 speed = transform.forward * orbitSpeed;
        speed.z = 0;

        Vector3 pos = transform.position;
        pos.z = 0;
        transform.position = pos;
        rb.velocity = speed;
        interplanitaryFlight = true;
    }
}
