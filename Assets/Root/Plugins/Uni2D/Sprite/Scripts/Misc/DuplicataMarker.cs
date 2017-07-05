using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Scriptable object used to test if a component have been duplicated by the user
public class DuplicataMarker : ScriptableObject
{	
	[HideInInspector]
	[SerializeField]
	// Owner
	private Object m_rOriginalOwner; 
	
	// Is a duplicate
	public bool IsADuplicate(Object a_rCurrentOwner)
	{
		return a_rCurrentOwner != m_rOriginalOwner;
	}
	
	// Create
	public void Create(Object a_rOriginalOwner)
	{
		m_rOriginalOwner = a_rOriginalOwner;
	}
}
