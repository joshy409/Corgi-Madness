using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnPointTransform : MonoBehaviour {
    public GameObject playerCamera;


	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 temp = new Vector3(playerCamera.transform.position.x, .3f, playerCamera.transform.position.z + 1.75f);
        transform.position = temp;
    }
}
