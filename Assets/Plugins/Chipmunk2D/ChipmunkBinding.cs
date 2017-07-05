// Copyright 2013 Howling Moon Software. All rights reserved.
// See http://chipmunk2d.net/legal.php for more information.

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System;
using CP = ChipmunkBinding;

public static partial class ChipmunkBinding {
#if UNITY_EDITOR
	public const string IMPORT = "Chipmunk";
#elif UNITY_IPHONE
	public const string IMPORT = "__Internal";
#else
	public const string IMPORT = "Chipmunk";
#endif
	
	public struct BB {
		public float l, b, r, t;
		
		public BB(float l, float b, float r, float t){
			this.l = l;
			this.b = b;
			this.r = r;
			this.t = t;
		}
	}
	
	public abstract class Base : MonoBehaviour {
		public IntPtr _handle;
		
		public IntPtr handle {
			get {
				if(_handle == IntPtr.Zero){
					// Didn't initialize this yet
					Awake();
				}
				return _handle;
			}
		}
		
		protected abstract void Awake();
		protected abstract void OnDestroy();
		
		// Called to manually resyncronize the Chipmunk object with it's transform.
		// Returns the (if any) body that was affected by this.
		public abstract ChipmunkBody _UpdatedTransform();
		
		protected Matrix4x4 BodyRelativeMatrix(ChipmunkBody body){
			if(body != null){
				Matrix4x4 bmatrix = body.transform.worldToLocalMatrix;
				Vector3 bodyScale = body.transform.localScale;
				
				return Matrix4x4.Scale(bodyScale)*bmatrix*this.transform.localToWorldMatrix;
			} else {
				return this.transform.localToWorldMatrix;
			}
		}
		
		public T GetComponentUpwards<T>() where T : Component {
			return GetComponentUpwardsFrom<T>(this.gameObject);
		}

    public static T GetComponentUpwardsFrom<T>(GameObject g) where T : Component {
			for(Transform t = g.transform; t != null; t = t.parent){
				T component = t.GetComponent<T>();
				if(component != null) return component;
			}
			
			return null;
    }
	}
	
	// String interning for IDs.
	private static int stringIDCounter = 0;
	private static Dictionary<string, int> internedStrings = new Dictionary<string, int>();
	
	// Get a unique integer for a particular string.
	public static int InternString(string key){
		if(internedStrings.ContainsKey(key)){
			return internedStrings[key];
		} else {
			int id = stringIDCounter;
			stringIDCounter++;
			
			internedStrings[key] = id;
			return id;
		}
	}
	
	static ChipmunkBinding(){
//		Debug.Log("Reading Chipmunk functions from: " + IMPORT);
		ChipmunkRegisterInternalCalls();
				
		// Intern the blank string first to ensure that it's 0.
		InternString("");
	}
	
	/// Register internal calls and other C initialization.
	[DllImport(IMPORT)] public static extern void ChipmunkRegisterInternalCalls();
	
	// Misc wrapper methods to avoid using the awkward parts of P/Invoke.
	[DllImport(IMPORT)] public static extern int MakeConvexHull(int count, Vector2[] verts, float tol);
	[DllImport(IMPORT)] public static extern IntPtr NewConvexPolyShapeWithVerts(int count, Vector2[] verts);
	[DllImport(IMPORT)] public static extern void UpdateConvexPolyShapeWithVerts(IntPtr shape, int count, Vector2[] verts);
	[DllImport(IMPORT)] public static extern void ConstraintSetBodies(IntPtr constraint, IntPtr a, IntPtr b);
}
