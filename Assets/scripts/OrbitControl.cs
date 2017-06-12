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

    float angular = 0f;

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

                angular = Quaternion.FromToRotation(
                    fc.transform.forward,
                    shipHeading
                ).eulerAngles.magnitude;

                Debug.Log(angular);

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
                fc.transform.Rotate(axis, angle, Space.World);
                angular += Input.GetAxis("Horizontal");
                fc.transform.rotation = Quaternion.LookRotation(tangent, planetToShipDirection);
                fc.transform.Rotate(planetToShipDirection, angular, Space.World);
                if (orbitState == OrbitState.Entering)
                {
                    orbitState = OrbitState.Orbiting;
                } else if (orbitState == OrbitState.Exiting && InOrbitExitArea)
                {
                    fc.LeaveOrbit(entryVelocity * Vector3.Dot(tangent, entryVelocity.normalized));
                }
            }
        }
	}

    bool InOrbitExitArea {
        get
        {            
            Vector3 euler = fc.transform.rotation.eulerAngles;
            float y = Mathf.Min(Mathf.Abs(270 - euler.y), Mathf.Abs(-90f - euler.y) % 360f);
            float z = Mathf.Abs(90f - euler.z) % 360f;
            //Debug.Log(string.Format("{0}, {1}", y, z));
            float tolerance = 5f;
            return y < tolerance && z < tolerance;
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
