Joints 2D Gizmos
================
Copyright © 2013  Illogika

Contact us at support@illogika.com

This package provides Gizmos for 2D physics joint components introduced in Unity 4.3.
The visual artifacts help understanding the relationships between joints and connected rigid body. They also speed up design of 2D physic objects.

You can find shortcuts to add the gizmos to your objects under the Component/Physics 2D menu.

Demo
----

The ExampleJoints2DGizmos.unity scene showcases the Gizmos in both Runtime and Editor mode.

Colors
------
Colors can be configured easily in the base class Joint2DGizmo. This may need changing if the default color scheme conflicts with your project's color theme.

Common concepts
---------------
The yellow link connects the object center to the object's anchor point.
Arrows indicate motor direction, if any.

Note on 2D joints rotation
--------------------------

2D Joints sometimes behave strangely when rotated. You should never rotate a 2D joint along its x or y axis as this can lead to unpredicted behaviours.
Also note that, though Distance and Spring joints behave as one would expect when rotated both in edit and runtime mode, Hinge and Slider joints have strange
behaviours when rotating them or their connected body with huge differences between what affects them at runtime compared to edit mode.

Distance
--------
The cyan circle indicates the maximum distance. 
The red link connects the object to the joint anchor.
If you want to make an object have a fixed distance from it's connected joint, set the distance parameter to the minimum (0.005) and move the joint's anchor
to the fixed relative position you want your attached object to be.
Note that a bug seems to make the distance joint behave like a spring if the connected anchor is not 0.

Hinge
-----
The radius of the hinge is drawn. Arcs are displayed if angle limits are in effect.

Special rotation effects: When in edit mode, the hinge's arc is affected by the connected body's global rotation, the hinge's rotation having no incidence on the arc.
When in play mode however, rotating the hinge or one of its parent changes the rotation of the arc realtively to its original position when play mode was activated.
Rotating the connected body manually at runtime can lead to strange behaviours.

Slider
------
A line shows the line constraint.

Special rotation effects: Rotating the slider or its parent in edit mode does not affect the slider's rotation in play mode.
Rotating the slider in play mode rotates the connected body around its anchor. Rotating the slider's parent changes the slider's direction.
Rotating first the slider's parent and then the slider leads to strange and unexpected results. You should avoid doing it.

Spring
------
The cyan circle shows the distance parameter of the joint. This is the resting length of the spring.
The magenta link represents the springs.

Change Log
----------

v1.1
- Updated the Gizmos to display correctly when their z rotation is not 0.
- Updated the readme to better explain the effects of rotation on 2D joints.
