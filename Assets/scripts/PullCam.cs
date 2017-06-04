using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PullCam : MonoBehaviour {
    [SerializeField]
    Transform puller;

    [SerializeField, Range(0, 10)]
    float attack = 0.7f;

    Vector3 camOffset;

    [SerializeField]
    float noPullSqDist = 2f;

	void Start () {
        camOffset = transform.position - puller.position;	
	}
	
	void Update () {
        Vector3 target = puller.position + camOffset;

        if (Vector3.SqrMagnitude(transform.position - target) > noPullSqDist)
        {
            transform.position = Vector3.Lerp(transform.position, target, attack * Time.deltaTime);
        }
	}
}
