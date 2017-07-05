// Copyright 2013 Howling Moon Software. All rights reserved.
// See http://chipmunk2d.net/legal.php for more information.

using UnityEngine;
using System.Collections;

/// This class is used as a delegate to recieve collision events.
/// Chipmunk works by having a single collision callback for a pair of colliding shapes.
/// It doesn't work like Unity by sending messages like OnCollisionEnter() to all MonoBehaviors attached to the collider.
/// Unity uses private API magic to be able to send those events efficiently.
/// You can have multiple ChipmunkCollisionManager objects within a scene.
/// There are four events you can catch that are described below.
/// As an example, when you want to find out when a shape with a collisionType string of "player" begins colliding with one of collisionType "monster",
/// you would need to make a ChipmunkBegin_player_monster() method on a manager in the scene.
public abstract class ChipmunkCollisionManager : MonoBehaviour {
	private void Awake(){
		Chipmunk.manager._space.AddCollisionManager(this);
	}
}
