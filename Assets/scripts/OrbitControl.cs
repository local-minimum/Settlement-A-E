using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitControl : MonoBehaviour {

    FlightController fc;

    [SerializeField]
    float angularSpeed = 10;

    [SerializeField]
    float flightAltitude = 7;

    [SerializeField]
    Transform planet;

    Vector3 axis;

    float curAltitude;

    [SerializeField]
    float attackAltitude = 0.9f;

    Vector3 entryVelocity;

	void Update () {
        if (fc)
        {
            if (fc.interplanetaryFlight && Input.GetButtonDown("Orbit"))
            {
                fc.interplanetaryFlight = false;
                entryVelocity = fc.rb.velocity;
                Vector3 shipHeading = entryVelocity.normalized;

                fc.rb.Sleep();

                Vector3 planetToShipDirection = (fc.transform.position - planet.position);
                curAltitude = planetToShipDirection.magnitude;
                planetToShipDirection = planetToShipDirection.normalized;
                
                axis = Vector3.Cross(planetToShipDirection, shipHeading);
                //TODO: What is the angle of the ship to its own speed vector?

            } else if (Input.GetButtonUp("Orbit") && !fc.interplanetaryFlight)
            {
                fc.LeaveOrbit(entryVelocity);
            }

            if (!fc.interplanetaryFlight)
            {
                curAltitude = Mathf.Lerp(curAltitude, flightAltitude, attackAltitude * Time.deltaTime);

                float angle = angularSpeed * Time.deltaTime;

                Quaternion rotation = Quaternion.Euler(angle * axis);

                Vector3 planetToShipDirection = (fc.transform.position - planet.position).normalized;
                Vector3 curDirection = rotation * planetToShipDirection;
                Vector3 tangent = Vector3.Cross(planetToShipDirection, axis);

                fc.transform.position = planet.transform.position + curAltitude * curDirection;
                fc.transform.rotation *= Quaternion.Euler(angle * tangent); // RotateAround(fc.transform.position, axis, angle);

            }
        }
	}

    private void OnTriggerEnter(Collider other)
    {
        FlightController fc = other.GetComponent<FlightController>();
        if (fc)
        {
            this.fc = fc;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (fc && fc.gameObject == other.gameObject && fc.interplanetaryFlight)
        {
            fc = null;
        }
    }

}
