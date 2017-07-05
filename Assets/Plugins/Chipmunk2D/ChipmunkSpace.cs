// Copyright 2013 Howling Moon Software. All rights reserved.
// See http://chipmunk2d.net/legal.php for more information.

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CP = ChipmunkBinding;
using System;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

/// An internal class that wraps Chipmunk's space object.
/// This isn't a concept that Unity exposes,
/// and there is only one space active at any given time.
public partial class ChipmunkSpace {
	public IntPtr _handle;
	
	public IntPtr _staticBody;
	
	protected bool locked;
	
#if UNITY_IPHONE
	[DllImport(ChipmunkBinding.IMPORT)] public static extern IntPtr  cpHastySpaceNew();
	[DllImport(ChipmunkBinding.IMPORT)] public static extern void cpHastySpaceStep(IntPtr space, float dt);
#endif
	
	public ChipmunkSpace(){
#if UNITY_EDITOR
		_handle = CP.cpSpaceNew();		
#elif UNITY_IPHONE
		_handle = cpHastySpaceNew();
#else
		_handle = CP.cpSpaceNew();
#endif

		_staticBody = CP.cpBodyNewStatic();
	}
	
	~ChipmunkSpace(){
//		Debug.Log("ChipmunkSpace deallocated.");
		CP.cpBodyFree(_staticBody);
		CP.cpSpaceFree(_handle);
		_handle = IntPtr.Zero;
		_staticBody = IntPtr.Zero;
		
		// Free the GCHandles for the collision handlers.
		foreach(var kvp in handlers){
			kvp.Value.Free();
		}
	}
	
	public List<ChipmunkBody> bodies = new List<ChipmunkBody>();
	
	public void _Add(ChipmunkBody obj){
		PostStepFunc del = delegate(){
	//		Debug.Log("Adding body.");
			
			bodies.Add(obj);
			CP.cpSpaceAddBody(_handle, obj.handle);
		};
		
		if(locked){
			_AddPostStepCallback(_handle, obj.handle, del);
		} else {
			del();
		}
	}
	
	public void _Remove(ChipmunkBody obj){
		if(obj._handle == IntPtr.Zero){
			Debug.LogError("ChipmunkBody handle is NULL");
			return;
		}
		
		if(!CP.cpSpaceContainsBody(_handle, obj._handle)){
			Debug.LogError("Space does not contain ChipmunkBody.");
			return;
		}
		
		PostStepFunc del = delegate(){
	//		Debug.Log("Removing body.");
			bodies.Remove(obj);
			CP.cpSpaceRemoveBody(_handle, obj._handle);
		};
		
		if(locked){
			_AddPostStepCallback(_handle, obj.handle, del);
		} else {
			del();
		}
	}
	
	public void _Add(ChipmunkShape obj){
		PostStepFunc del = delegate(){
	//		Debug.Log("Adding shape.");
			CP.cpSpaceAddShape(_handle, obj.handle);
		};
		
		if(locked){
			_AddPostStepCallback(_handle, obj.handle, del);
		} else {
			del();
		}
	}
	
	public void _Remove(ChipmunkShape obj){
		if(obj._handle == IntPtr.Zero){
			Debug.LogError("ChipmunkShape handle is NULL");
			return;
		}
		
		if(!CP.cpSpaceContainsShape(_handle, obj._handle)){
			Debug.LogError("Space does not contain ChipmunkShape.");
			return;
		}
		
		PostStepFunc del = delegate(){
	//		Debug.Log("Removing shape.");
			CP.cpSpaceRemoveShape(_handle, obj._handle);
		};
		
		if(locked){
			_AddPostStepCallback(_handle, obj.handle, del);
		} else {
			del();
		}
	}
	
	public void _Add(ChipmunkConstraint obj){
		PostStepFunc del = delegate(){
	//		Debug.Log("Adding shape.");
			CP.cpSpaceAddConstraint(_handle, obj.handle);
		};
		
		if(locked){
			_AddPostStepCallback(_handle, obj.handle, del);
		} else {
			del();
		}
	}
	
	public void _Remove(ChipmunkConstraint obj){
		if(obj._handle == IntPtr.Zero){
			Debug.LogError("ChipmunkConstraint handle is NULL");
			return;
		}
		
		if(!CP.cpSpaceContainsConstraint(_handle, obj._handle)){
			Debug.LogError("Space does not contain ChipmunkConstraint.");
			return;
		}
		
		PostStepFunc del = delegate(){
	//		Debug.Log("Removing shape.");
			CP.cpSpaceRemoveConstraint(_handle, obj._handle);
		};
		
		if(locked){
			_AddPostStepCallback(_handle, obj.handle, del);
		} else {
			del();
		}
	}
	
	public void _Step(float dt){
		locked = true; {
#if UNITY_EDITOR
			CP.cpSpaceStep(_handle, dt);
#elif UNITY_IPHONE
			cpHastySpaceStep(_handle, dt);
#else
			CP.cpSpaceStep(_handle, dt);
#endif
		} locked = false;
	}
	
	//MARK: Collision Handler Stuff.
	
	private struct TypePair {
		public int a;
		public int b;
		
		public TypePair(string a, string b){
			this.a = CP.InternString(a);
			this.b = CP.InternString(b);
		}
		
		public override bool Equals(System.Object obj){
			TypePair other = (TypePair)obj;
			return (
				(this.a == other.a && this.b == other.b) ||
				(this.a == other.b && this.b == other.a)
			);
		}
		
		public override Int32 GetHashCode(){
			return (a.GetHashCode() | b.GetHashCode());
		}
		
		public override string ToString(){
			return "(" + a + ", " + b + ")";
		}
	}
	
	private struct Handler {
		public delegate bool Begin(ChipmunkArbiter arbiter);
		public delegate bool PreSolve(ChipmunkArbiter arbiter);
		public delegate void PostSolve(ChipmunkArbiter arbiter);
		public delegate void Separate(ChipmunkArbiter arbiter);
		
		public TypePair typePair;
		public Begin begin;
		public PreSolve preSolve;
		public PostSolve postSolve;
		public Separate separate;
		
		// Written on the C-side to cache the System.Delegate.Invoke() method.
		private IntPtr invokeMethod;
	}
	
	private Dictionary<TypePair, GCHandle> handlers = new Dictionary<TypePair, GCHandle>();
	
	[MethodImplAttribute(MethodImplOptions.InternalCall)]
	private static extern void _AddCollisionHandler(IntPtr space, IntPtr handle);
	
	public void AddCollisionManager(ChipmunkCollisionManager manager){
		Regex pattern = new Regex("Chipmunk(Begin|PreSolve|PostSolve|Separate)_(\\w*)_(\\w*)");
		
		Type type = manager.GetType();
		BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
		MethodInfo[] methods = type.GetMethods(flags);
		
		foreach(MethodInfo info in methods){
			Match match = pattern.Match(info.Name);
			if(match.Success){
				string eventType = match.Groups[1].Value;
				string typeA = match.Groups[2].Value;
				string typeB = match.Groups[3].Value;
			
				var typePair = new TypePair(typeA, typeB);
				Handler handler = new Handler();
				handler.typePair = typePair;
				GCHandle gch;
				
				if(handlers.TryGetValue(typePair, out gch)){
//					Debug.Log("Existing handler found " + typePair);
					handler = (Handler)gch.Target;
				} else {
//					Debug.Log("Creating new handler " + typePair);
					// Use pinned handles so we can store the address directly in Chipmunk.
					// Shouldn't really be an issue if there are only a few at any given time. 
					gch = GCHandle.Alloc(handler, GCHandleType.Pinned);
					handlers[typePair] = gch;
				}
				
				if(handler.typePair.a != typePair.a || handler.typePair.b != typePair.b){
					Debug.LogError(
						"All collision handler methods (Begin, PreSolve, PostSolve or Separate) for the collision types \"" +
						typeB + "\" and \"" + typeA + "\" must list the collision types in the same order. " +
						"The collision types in your method \"" + info.Name + "\" do not match the order of a previous method."
					);
					
					continue;
				}
				
				try {
					if(eventType == "Begin"){
						handler.begin = (Handler.Begin)Delegate.CreateDelegate(typeof(Handler.Begin), manager, info);
					}
					
					if(eventType == "PreSolve"){
						handler.preSolve = (Handler.PreSolve)Delegate.CreateDelegate(typeof(Handler.PreSolve), manager, info);
					}
				} catch {
					Debug.LogError(
						"Your collision handler method \"" + info.Name + "\" has the wrong method signature signature. " +
						"It should look like \"bool Chipmunk(Begin|PreSolve)_typeA_typeB(ChipmunkArbiter arbiter)\""
					);
					continue;
				}
				
				try {
					if(eventType == "PostSolve"){
						handler.postSolve = (Handler.PostSolve)Delegate.CreateDelegate(typeof(Handler.PostSolve), manager, info);
					}
					
					if(eventType == "Separate"){
						handler.separate = (Handler.Separate)Delegate.CreateDelegate(typeof(Handler.Separate), manager, info);
					}
				} catch {
					Debug.LogError(
						"Your collision handler method \"" + info.Name + "\" has the wrong method signature signature. " +
						"It should look like \"void Chipmunk(PostSolve|Separate)_typeA_typeB(ChipmunkArbiter arbiter)\""
					);
					continue;
				}
				
				// Push the value back to the CGHandle.
				gch.Target = handler;
				_AddCollisionHandler(_handle, GCHandle.ToIntPtr(gch));
			}
		}
	}
	
	//MARK: Post-step callbacks
	private delegate void PostStepFunc();
	[MethodImplAttribute(MethodImplOptions.InternalCall)] private static extern void _AddPostStepCallback(IntPtr handle, IntPtr key, PostStepFunc callback);
}
