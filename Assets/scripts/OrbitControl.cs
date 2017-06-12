using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum OrbitState { Entering, Orbiting, Exiting};

public class OrbitControl : MonoBehaviour {

    FlightController fc;

    [SerializeField]
    float targetAngularSpeed = 10;

    float angularSpeed = 0;

    [SerializeField]
    Transform planet;

    Vector3 axis;

    float flightAltitude;

    [SerializeField]
    float attackSpeed = 0.9f;

    float angular = 0f;

    Vector3 entryVelocity;
    Vector3 shipEntryHeading;
    Vector3 tangent;
    OrbitState orbitState;

	void Update () {
        if (fc)
        {
            if (fc.interplanetaryFlight && Input.GetButtonDown("Orbit"))
            {
                fc.interplanetaryFlight = false;
                entryVelocity = fc.rb.velocity;
                shipEntryHeading = entryVelocity.normalized;
                
                angular = Quaternion.FromToRotation(
                    fc.transform.forward,
                    shipEntryHeading
                ).eulerAngles.magnitude;

                //Debug.Log(angular);

                fc.rb.Sleep();

                Vector3 planetToShipDirection = (fc.transform.position - planet.position);

                flightAltitude = Vector3.Dot(planetToShipDirection, -1f * Vector3.forward);

                planetToShipDirection = planetToShipDirection.normalized;
                angularSpeed =  360f * entryVelocity.magnitude / (2 * Mathf.PI * flightAltitude) ;            
                axis = Vector3.Cross(planetToShipDirection, shipEntryHeading);

                orbitState = OrbitState.Entering;

            } else if (Input.GetButtonUp("Orbit") && !fc.interplanetaryFlight)
            {
                orbitState = OrbitState.Exiting;                
            }

            if (!fc.interplanetaryFlight)
            {
                angularSpeed = Mathf.Lerp(angularSpeed, targetAngularSpeed, attackSpeed * Time.deltaTime);
                float angle = angularSpeed * Time.deltaTime;


                Quaternion rotation = Quaternion.Euler(angle * axis);

                Vector3 planetToShipDirection = (fc.transform.position - planet.position).normalized;
                Vector3 curDirection = rotation * planetToShipDirection;
                tangent = Vector3.Cross(axis, planetToShipDirection);

                fc.transform.position = planet.transform.position + flightAltitude * curDirection;
                fc.transform.Rotate(axis, angle, Space.World);

                //Angular rotation
                angular += Input.GetAxis("Horizontal");
                fc.transform.rotation = Quaternion.LookRotation(tangent, planetToShipDirection);
                fc.transform.Rotate(planetToShipDirection, angular, Space.World);

                //Orbit axis rotation
                float axisMomentum = Input.GetAxis("Vertical") *
                    Vector3.Dot(fc.transform.forward, tangent);


                if (orbitState == OrbitState.Entering)
                {
                    orbitState = OrbitState.Orbiting;
                } else if (orbitState == OrbitState.Exiting && InOrbitExitArea)
                {
                    fc.LeaveOrbit(angularSpeed * 2 * Mathf.PI * flightAltitude / 360f * shipEntryHeading);
                }
            }
        }
	}

    [SerializeField, Range(0, 10)]
    float exitAngleTolerance = 2f;

    bool InOrbitExitArea {
        get
        {
            float a = Quaternion.FromToRotation(tangent, shipEntryHeading).eulerAngles.magnitude;
            a = Mathf.Min(a, Mathf.Abs(360 - a));
            return a < exitAngleTolerance;
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
