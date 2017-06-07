using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elipse : MonoBehaviour {

    [SerializeField]
    float a = 10;

    [SerializeField]
    float b = 7;

    [SerializeField, Range(0, 1)]
    float eccentricity = 0.8f;

    float h
    {
        get
        {
            return (a - b) * (a - b) / ((a + b) * (a + b));
        }
    }

    /// <summary>
    /// Ramachandran aproximation
    /// </summary>
    float Perimeter
    {
        get
        {
            float h = this.h;
            return Mathf.PI * (a + b) * (1 + 3 * h / (10 + Mathf.Sqrt(4 - 3 * h)));
        }
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}


}
