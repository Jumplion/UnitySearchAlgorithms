using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[ExecuteInEditMode]
public class Breadcrumb : MonoBehaviour
{
    Renderer rend;

    private void Awake()
    {
        rend = GetComponent<Renderer>();
    }

    // Use this for initialization
    void Start ()
    {
        SetCrumb(false, Color.green);
        
        //transform.parent = Map.mapObj.transform;
	}

    public void SetCrumb(bool active, Color col)
    {
        gameObject.SetActive(active);
        rend.material.color = col;
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}
