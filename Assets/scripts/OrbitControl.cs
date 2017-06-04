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

	void Update () {
        if (fc)
        {
            if (fc.interplanitaryFlight && Input.GetButtonDown("Orbit"))
            {
                fc.interplanitaryFlight = false;

                Vector3 shipHeading = fc.rb.velocity.normalized;

                fc.rb.Sleep();

                Vector3 planetToShipDirection = (fc.transform.position - planet.position);
                curAltitude = planetToShipDirection.magnitude;
                planetToShipDirection = planetToShipDirection.normalized;
                
                axis = Vector3.Cross(planetToShipDirection, shipHeading);

                //TODO: What is the angle of the ship to its own speed vector?


            } else if (Input.GetButtonUp("Orbit") && !fc.interplanitaryFlight)
            {
                fc.LeaveOrbit(angularSpeed / (2 * Mathf.PI) * flightAltitude);
            }

            if (!fc.interplanitaryFlight)
            {
                curAltitude = Mathf.Lerp(curAltitude, flightAltitude, attackAltitude * Time.deltaTime);
                Vector3 planetToShipDirection = (fc.transform.position - planet.position).normalized;

                /*
                curAngle += angularSpeed * Time.deltaTime;
                curAngle %= 360;
                if (curAngle < 0)
                {
                    curAngle += 360;
                }*/

                Quaternion rotation = Quaternion.Euler(angularSpeed * Time.deltaTime * axis);
                planetToShipDirection = rotation * planetToShipDirection;

                fc.transform.position = planet.transform.position + curAltitude * planetToShipDirection;

                //Rotate only belly bit facing planet
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
        if (fc && fc.gameObject == other.gameObject && fc.interplanitaryFlight)
        {
            fc = null;
        }
    }

}
