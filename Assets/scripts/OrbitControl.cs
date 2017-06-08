using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum OrbitState { Entering, Orbiting, Exiting};

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

    OrbitState orbitState;

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

                orbitState = OrbitState.Entering;

            } else if (Input.GetButtonUp("Orbit") && !fc.interplanetaryFlight)
            {
                orbitState = OrbitState.Exiting;                
            }

            if (!fc.interplanetaryFlight)
            {
                curAltitude = Mathf.Lerp(curAltitude, flightAltitude, attackAltitude * Time.deltaTime);

                float angle = angularSpeed * Time.deltaTime;

                Quaternion rotation = Quaternion.Euler(angle * axis);

                Vector3 planetToShipDirection = (fc.transform.position - planet.position).normalized;
                Vector3 curDirection = rotation * planetToShipDirection;
                Vector3 tangent = Vector3.Cross(axis, planetToShipDirection);

                fc.transform.position = planet.transform.position + curAltitude * curDirection;
                fc.transform.rotation = Quaternion.LookRotation(tangent, planetToShipDirection);
                //fc.transform.rotation *= Quaternion.Euler(angle * tangent); // RotateAround(fc.transform.position, axis, angle);

                if (orbitState == OrbitState.Entering)
                {
                    orbitState = OrbitState.Orbiting;
                } else if (orbitState == OrbitState.Exiting && InOrbitExitArea)
                {
                    fc.LeaveOrbit(entryVelocity);
                }
            }
        }
	}

    bool InOrbitExitArea {
        get
        {
            Vector3 euler = fc.transform.rotation.eulerAngles;
            float x = Mathf.Min(euler.x, Mathf.Abs(360f - euler.x));
            float y = Mathf.Min(euler.y, Mathf.Abs(360f - euler.y));
            return x < 3f && y < 3f;
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
