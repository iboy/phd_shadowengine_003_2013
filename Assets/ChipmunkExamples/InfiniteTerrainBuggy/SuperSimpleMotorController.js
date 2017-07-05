#pragma strict

// The speed of the wheel in radians/second.
var speed = 100.0;

// The torque of the wheel when the motor is running.
var motorTorque = 10000.0;

// The torque on the wheel when the motor is not running.
// This will slow the car down realistically.
var frictionTorque = 100.0;

function Update () {
	// Grab the motor component.
	var motor = GetComponent(ChipmunkSimpleMotor);
	
	if(Input.GetButton("Jump")){
		// When the button is down set the torque high,
		motor.maxForce = motorTorque;
		// and the rate to the desired wheel speed in radians/second.
		motor.rate = speed;
	} else {
		// When the button is not down, slow the car down.
		// Set a small torque that slows the wheel down.
		motor.maxForce = frictionTorque;
		// Set the rate to zero so it slowly comes to a stop.
		motor.rate = 0;
	}
}