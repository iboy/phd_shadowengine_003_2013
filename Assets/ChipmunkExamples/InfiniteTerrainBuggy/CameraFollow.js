#pragma strict

var follow : Transform;

function Update () {
	transform.position = follow.position;
}