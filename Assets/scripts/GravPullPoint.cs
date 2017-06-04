using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravPullPoint : MonoBehaviour {

    [SerializeField]
    float planetPull = 100f;

    [SerializeField, Range(0, 1)]
    float noPullRadius = 0.3f;

    FlightController fc;
    SphereCollider sCol;

    private void Start()
    {
        sCol = GetComponent<SphereCollider>();    
    }

    void Update () {
        if (fc && fc.interplanitaryFlight)
        {
            Vector3 v = transform.position - fc.transform.position;
            float distSq = Vector3.SqrMagnitude(v);

            //TODO: Only calc on noPullRadius change
            float linPull = Mathf.Pow(noPullRadius * sCol.radius, 2);

            if (distSq > linPull)
            {
                float pull = planetPull / distSq;
                fc.rb.AddForce(v.normalized * pull);
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
        FlightController fc = other.GetComponent<FlightController>();
        if (fc)
        {
            this.fc = null;
        }
    }
}
