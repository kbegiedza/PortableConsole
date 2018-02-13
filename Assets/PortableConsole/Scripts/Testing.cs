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
            Debug.Log("F1");
        }

        if (Input.GetKeyDown(KeyCode.F2))
        {
            Debug.LogWarning("F2");
        }

        if (Input.GetKeyDown(KeyCode.F3))
        {
            Debug.LogError("F3");
        }

        if(Input.GetKey(KeyCode.F4))
        {
            Debug.LogError("holding F4");
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