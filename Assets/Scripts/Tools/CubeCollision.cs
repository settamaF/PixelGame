//******************************************************************************
// Authors: Frederic SETTAMA  
//******************************************************************************

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeCollision : MonoBehaviour
{
	#region Script Parameters

	#endregion

	#region Static

	#endregion

	#region Properties
	public BlockGenerator Generator;
#endregion

#region Fields
	// Const -------------------------------------------------------------------

	// Static ------------------------------------------------------------------

	// Private -----------------------------------------------------------------

#endregion

#region Unity Methods
	// Use this for initialization
	void Start()
	{

	}
	
	// Update is called once per frame
	void Update()
	{

	}

	private void OnCollisionEnter(Collision collision)
	{
		Debug.Log(collision.collider.name);
	}

	private void OnTriggerEnter(Collider other)
	{
		Debug.Log(other.name);
	}

	private void OnTriggerStay(Collider other)
	{
		Debug.Log("Stay " + other.name);
	}
	#endregion

	#region Methods

	#endregion

	#region Implementation

	#endregion
}
