using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetaryRotation : MonoBehaviour {

    [SerializeField]
    Vector3 rotationAxis;

    [SerializeField]
    float dayLengthSeconds;

	void Update () {
        transform.Rotate(rotationAxis, 360 / dayLengthSeconds * Time.deltaTime);		
	}
}
