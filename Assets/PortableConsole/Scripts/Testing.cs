using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testing : MonoBehaviour 
{
    //------------------------------
    // Unity methods
    //------------------------------
    private void Awake() 
	{
	}

	private void Start() 
	{
	}
	
	private void Update() 
	{
        if(Input.GetKeyDown(KeyCode.F1))
        {
            Debug.Log("1");
        }

        if (Input.GetKeyDown(KeyCode.F2))
        {
            Debug.LogWarning("xD");
        }

        if (Input.GetKeyDown(KeyCode.F3))
        {
            Debug.LogError(":D");
        }

        if(Input.GetKey(KeyCode.F4))
        {
            Debug.LogError("hold...");
        }
    }

    //------------------------------
    // public methods
    //------------------------------

    //------------------------------
    // private methods
    //------------------------------

    //------------------------------
    // coroutines
    //------------------------------
}