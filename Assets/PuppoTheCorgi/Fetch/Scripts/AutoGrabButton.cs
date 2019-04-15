using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoGrabButton : MonoBehaviour {

    [Header("Controller Grab Button")]
    public VRTK.VRTK_InteractGrab rightController;

    [Header("Object Auto Grab Script")]
    public VRTK.VRTK_ObjectAutoGrab autoGrabScript;

    // Update is called once per frame
    void Update () {
		if (rightController.IsGrabButtonPressed())
        {
            autoGrabScript.GetComponent<VRTK.VRTK_ObjectAutoGrab>().enabled = true;
        }
	}
}
