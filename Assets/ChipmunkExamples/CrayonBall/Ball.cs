// Copyright 2013 Howling Moon Software. All rights reserved.
// See http://chipmunk2d.net/legal.php for more information.

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Ball : MonoBehaviour {
	protected int color;
	
	static Color[] colors = new Color[]{Color.red, Color.green, Color.blue, Color.yellow, Color.magenta, Color.cyan};
	static string[] collisionTypes = new string[]{"red", "green", "blue", "yellow", "magenta", "cyan"};
	
	protected void Awake(){
		color = Random.Range(0, colors.Length - 3);
		this.renderer.material.color = colors[color];
		GetComponent<ChipmunkShape>().collisionType = collisionTypes[color];
	}
	
	protected void OnEnable(){
		CrayonBallLogic.balls.Add(this);
	}
	
	protected void OnDisable(){
		CrayonBallLogic.balls.Remove(this);
	}
	
	
	private int _children;
	public int count { get { return _children + 1; } }
	
	private Ball _parent;
	public Ball parent {
		set {
			_parent = value;
			value._children += count;
		}
	}
	
	public Ball root {
		get { return (_parent ? _parent.root : this); }
	}
	
	public void ResetGraphNode(){
		_children = 0;
		_parent = null;
	}
}
