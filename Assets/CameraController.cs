using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject map;
    public LayerMask mapLayer;



	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        RaycastHit hit;
        Physics.Raycast(transform.position, Vector3.forward, out hit, 9999f, mapLayer);
        Quaternion rot = Quaternion.Euler(45, 45, 0);//Quaternion.AngleAxis(45, Vector3.up);

        //transform.position = rot * 10f;

    }
}
