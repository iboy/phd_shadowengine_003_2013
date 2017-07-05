

using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System;

public partial class ChipmunkBinding {
	[DllImport(IMPORT)] public static extern BB _cpBBNew(float l, float b, float r, float t);
	[DllImport(IMPORT)] public static extern BB _cpBBNewForCircle(Vector2 p, float r);
	[DllImport(IMPORT)] public static extern bool _cpBBIntersects(BB a, BB b);
	[DllImport(IMPORT)] public static extern bool _cpBBContainsBB(BB bb, BB other);
	[DllImport(IMPORT)] public static extern bool _cpBBContainsVect(BB bb, Vector2 v);
	[DllImport(IMPORT)] public static extern BB _cpBBMerge(BB a, BB b);
	[DllImport(IMPORT)] public static extern BB _cpBBExpand(BB bb, Vector2 v);
	[DllImport(IMPORT)] public static extern float _cpBBArea(BB bb);
	[DllImport(IMPORT)] public static extern float _cpBBMergedArea(BB a, BB b);
	[DllImport(IMPORT)] public static extern bool _cpBBIntersectsSegment(BB bb, Vector2 a, Vector2 b);
	[DllImport(IMPORT)] public static extern Vector2 cpBBWrapVect(BB bb, Vector2 v);
	[DllImport(IMPORT)] public static extern IntPtr  cpBodyNew(float m, float i);
	[DllImport(IMPORT)] public static extern IntPtr  cpBodyNewStatic();
	[DllImport(IMPORT)] public static extern void cpBodyFree(IntPtr body);
	[DllImport(IMPORT)] public static extern void cpBodyAddMassForShape(IntPtr body, IntPtr shape, float density);
	[DllImport(IMPORT)] public static extern void cpBodyActivate(IntPtr body);
	[DllImport(IMPORT)] public static extern void cpBodyActivateStatic(IntPtr body, IntPtr filter);
	[DllImport(IMPORT)] public static extern void cpBodySleep(IntPtr body);
	[DllImport(IMPORT)] public static extern void cpBodySleepWithGroup(IntPtr body, IntPtr group);
	[DllImport(IMPORT)] public static extern bool _cpBodyIsSleeping(IntPtr body);
	[DllImport(IMPORT)] public static extern bool _cpBodyIsStatic(IntPtr body);
	[DllImport(IMPORT)] public static extern bool _cpBodyIsRogue(IntPtr body);
	[DllImport(IMPORT)] public static extern Vector2 _cpBodyLocal2World(IntPtr body, Vector2 v);
	[DllImport(IMPORT)] public static extern Vector2 _cpBodyWorld2Local(IntPtr body, Vector2 v);
	[DllImport(IMPORT)] public static extern IntPtr  _cpBodyGetSpace(IntPtr body);
	[DllImport(IMPORT)] public static extern float _cpBodyGetMass(IntPtr body);
	[DllImport(IMPORT)] public static extern void cpBodySetMass(IntPtr body, float m);
	[DllImport(IMPORT)] public static extern float _cpBodyGetMoment(IntPtr body);
	[DllImport(IMPORT)] public static extern void cpBodySetMoment(IntPtr body, float i);
	[DllImport(IMPORT)] public static extern Vector2 _cpBodyGetPos(IntPtr body);
	[DllImport(IMPORT)] public static extern void cpBodySetPos(IntPtr body, Vector2 pos);
	[DllImport(IMPORT)] public static extern Vector2 _cpBodyGetVel(IntPtr body);
	[DllImport(IMPORT)] public static extern void _cpBodySetVel(IntPtr body, Vector2 value);
	[DllImport(IMPORT)] public static extern Vector2 _cpBodyGetForce(IntPtr body);
	[DllImport(IMPORT)] public static extern void _cpBodySetForce(IntPtr body, Vector2 value);
	[DllImport(IMPORT)] public static extern float _cpBodyGetAngle(IntPtr body);
	[DllImport(IMPORT)] public static extern void cpBodySetAngle(IntPtr body, float a);
	[DllImport(IMPORT)] public static extern float _cpBodyGetAngVel(IntPtr body);
	[DllImport(IMPORT)] public static extern void _cpBodySetAngVel(IntPtr body, float value);
	[DllImport(IMPORT)] public static extern float _cpBodyGetTorque(IntPtr body);
	[DllImport(IMPORT)] public static extern void _cpBodySetTorque(IntPtr body, float value);
	[DllImport(IMPORT)] public static extern Vector2 _cpBodyGetRot(IntPtr body);
	[DllImport(IMPORT)] public static extern Vector2 _cpBodyGetCOGOffset(IntPtr body);
	[DllImport(IMPORT)] public static extern void _cpBodySetCOGOffset(IntPtr body, Vector2 value);
	[DllImport(IMPORT)] public static extern float _cpBodyGetVelLimit(IntPtr body);
	[DllImport(IMPORT)] public static extern void _cpBodySetVelLimit(IntPtr body, float value);
	[DllImport(IMPORT)] public static extern float _cpBodyGetAngVelLimit(IntPtr body);
	[DllImport(IMPORT)] public static extern void _cpBodySetAngVelLimit(IntPtr body, float value);
	[DllImport(IMPORT)] public static extern IntPtr _cpBodyGetUserData(IntPtr body);
	[DllImport(IMPORT)] public static extern void _cpBodySetUserData(IntPtr body, IntPtr value);
	[DllImport(IMPORT)] public static extern void cpBodyUpdateVelocity(IntPtr body, Vector2 gravity, float damping, float dt);
	[DllImport(IMPORT)] public static extern void cpBodyUpdatePosition(IntPtr body, float dt);
	[DllImport(IMPORT)] public static extern void cpBodyResetForces(IntPtr body);
	[DllImport(IMPORT)] public static extern void cpBodyApplyForce(IntPtr body, Vector2 f, Vector2 r);
	[DllImport(IMPORT)] public static extern void cpBodyApplyImpulse(IntPtr body, Vector2 j, Vector2 r);
	[DllImport(IMPORT)] public static extern Vector2 cpBodyGetVelAtWorldPoint(IntPtr body, Vector2 point);
	[DllImport(IMPORT)] public static extern Vector2 cpBodyGetVelAtLocalPoint(IntPtr body, Vector2 point);
	[DllImport(IMPORT)] public static extern float _cpBodyKineticEnergy(IntPtr body);
	[DllImport(IMPORT)] public static extern void cpShapeFree(IntPtr shape);
	[DllImport(IMPORT)] public static extern BB cpShapeCacheBB(IntPtr shape);
	[DllImport(IMPORT)] public static extern BB cpShapeUpdate(IntPtr shape, Vector2 pos, Vector2 rot);
	[DllImport(IMPORT)] public static extern IntPtr  _cpShapeGetSpace(IntPtr shape);
	[DllImport(IMPORT)] public static extern IntPtr  _cpShapeGetBody(IntPtr shape);
	[DllImport(IMPORT)] public static extern void cpShapeSetBody(IntPtr shape, IntPtr body);
	[DllImport(IMPORT)] public static extern BB _cpShapeGetBB(IntPtr shape);
	[DllImport(IMPORT)] public static extern bool _cpShapeGetSensor(IntPtr shape);
	[DllImport(IMPORT)] public static extern void _cpShapeSetSensor(IntPtr shape, bool value);
	[DllImport(IMPORT)] public static extern float _cpShapeGetElasticity(IntPtr shape);
	[DllImport(IMPORT)] public static extern void _cpShapeSetElasticity(IntPtr shape, float value);
	[DllImport(IMPORT)] public static extern float _cpShapeGetFriction(IntPtr shape);
	[DllImport(IMPORT)] public static extern void _cpShapeSetFriction(IntPtr shape, float value);
	[DllImport(IMPORT)] public static extern Vector2 _cpShapeGetSurfaceVelocity(IntPtr shape);
	[DllImport(IMPORT)] public static extern void _cpShapeSetSurfaceVelocity(IntPtr shape, Vector2 value);
	[DllImport(IMPORT)] public static extern IntPtr _cpShapeGetUserData(IntPtr shape);
	[DllImport(IMPORT)] public static extern void _cpShapeSetUserData(IntPtr shape, IntPtr value);
	[DllImport(IMPORT)] public static extern int _cpShapeGetCollisionType(IntPtr shape);
	[DllImport(IMPORT)] public static extern void _cpShapeSetCollisionType(IntPtr shape, int value);
	[DllImport(IMPORT)] public static extern int _cpShapeGetGroup(IntPtr shape);
	[DllImport(IMPORT)] public static extern void _cpShapeSetGroup(IntPtr shape, int value);
	[DllImport(IMPORT)] public static extern uint _cpShapeGetLayers(IntPtr shape);
	[DllImport(IMPORT)] public static extern void _cpShapeSetLayers(IntPtr shape, uint value);
	[DllImport(IMPORT)] public static extern void cpResetShapeIdCounter();
	[DllImport(IMPORT)] public static extern IntPtr  cpCircleShapeNew(IntPtr body, float radius, Vector2 offset);
	[DllImport(IMPORT)] public static extern Vector2 cpCircleShapeGetOffset(IntPtr shape);
	[DllImport(IMPORT)] public static extern float cpCircleShapeGetRadius(IntPtr shape);
	[DllImport(IMPORT)] public static extern IntPtr  cpSegmentShapeNew(IntPtr body, Vector2 a, Vector2 b, float radius);
	[DllImport(IMPORT)] public static extern void cpSegmentShapeSetNeighbors(IntPtr shape, Vector2 prev, Vector2 next);
	[DllImport(IMPORT)] public static extern Vector2 cpSegmentShapeGetA(IntPtr shape);
	[DllImport(IMPORT)] public static extern Vector2 cpSegmentShapeGetB(IntPtr shape);
	[DllImport(IMPORT)] public static extern Vector2 cpSegmentShapeGetNormal(IntPtr shape);
	[DllImport(IMPORT)] public static extern float cpSegmentShapeGetRadius(IntPtr shape);
	[DllImport(IMPORT)] public static extern IntPtr  cpBoxShapeNew(IntPtr body, float width, float height);
	[DllImport(IMPORT)] public static extern IntPtr  cpBoxShapeNew2(IntPtr body, BB box);
	[DllImport(IMPORT)] public static extern IntPtr  cpBoxShapeNew3(IntPtr body, BB box, float radius);
	[DllImport(IMPORT)] public static extern int cpPolyShapeGetNumVerts(IntPtr shape);
	[DllImport(IMPORT)] public static extern Vector2 cpPolyShapeGetVert(IntPtr shape, int idx);
	[DllImport(IMPORT)] public static extern float cpPolyShapeGetRadius(IntPtr shape);
	[DllImport(IMPORT)] public static extern float _cpArbiterGetElasticity(IntPtr arb);
	[DllImport(IMPORT)] public static extern void _cpArbiterSetElasticity(IntPtr arb, float value);
	[DllImport(IMPORT)] public static extern float _cpArbiterGetFriction(IntPtr arb);
	[DllImport(IMPORT)] public static extern void _cpArbiterSetFriction(IntPtr arb, float value);
	[DllImport(IMPORT)] public static extern Vector2 cpArbiterGetSurfaceVelocity(IntPtr arb);
	[DllImport(IMPORT)] public static extern void cpArbiterSetSurfaceVelocity(IntPtr arb, Vector2 vr);
	[DllImport(IMPORT)] public static extern IntPtr _cpArbiterGetUserData(IntPtr arb);
	[DllImport(IMPORT)] public static extern void _cpArbiterSetUserData(IntPtr arb, IntPtr value);
	[DllImport(IMPORT)] public static extern Vector2 cpArbiterTotalImpulse(IntPtr arb);
	[DllImport(IMPORT)] public static extern Vector2 cpArbiterTotalImpulseWithFriction(IntPtr arb);
	[DllImport(IMPORT)] public static extern float cpArbiterTotalKE(IntPtr arb);
	[DllImport(IMPORT)] public static extern void cpArbiterIgnore(IntPtr arb);
	[DllImport(IMPORT)] public static extern bool cpArbiterIsFirstContact(IntPtr arb);
	[DllImport(IMPORT)] public static extern int cpArbiterGetCount(IntPtr arb);
	[DllImport(IMPORT)] public static extern Vector2 cpArbiterGetNormal(IntPtr arb, int i);
	[DllImport(IMPORT)] public static extern Vector2 cpArbiterGetPoint(IntPtr arb, int i);
	[DllImport(IMPORT)] public static extern float cpArbiterGetDepth(IntPtr arb, int i);
	[DllImport(IMPORT)] public static extern void cpConstraintFree(IntPtr constraint);
	[DllImport(IMPORT)] public static extern void _cpConstraintActivateBodies(IntPtr constraint);
	[DllImport(IMPORT)] public static extern IntPtr  _cpConstraintGetSpace(IntPtr constraint);
	[DllImport(IMPORT)] public static extern IntPtr  _cpConstraintGetA(IntPtr constraint);
	[DllImport(IMPORT)] public static extern IntPtr  _cpConstraintGetB(IntPtr constraint);
	[DllImport(IMPORT)] public static extern float _cpConstraintGetMaxForce(IntPtr constraint);
	[DllImport(IMPORT)] public static extern void _cpConstraintSetMaxForce(IntPtr constraint, float value);
	[DllImport(IMPORT)] public static extern float _cpConstraintGetErrorBias(IntPtr constraint);
	[DllImport(IMPORT)] public static extern void _cpConstraintSetErrorBias(IntPtr constraint, float value);
	[DllImport(IMPORT)] public static extern float _cpConstraintGetMaxBias(IntPtr constraint);
	[DllImport(IMPORT)] public static extern void _cpConstraintSetMaxBias(IntPtr constraint, float value);
	[DllImport(IMPORT)] public static extern IntPtr _cpConstraintGetUserData(IntPtr constraint);
	[DllImport(IMPORT)] public static extern void _cpConstraintSetUserData(IntPtr constraint, IntPtr value);
	[DllImport(IMPORT)] public static extern float _cpConstraintGetImpulse(IntPtr constraint);
	[DllImport(IMPORT)] public static extern IntPtr  cpPinJointNew(IntPtr a, IntPtr b, Vector2 anchr1, Vector2 anchr2);
	[DllImport(IMPORT)] public static extern Vector2 _cpPinJointGetAnchr1(IntPtr constraint);
	[DllImport(IMPORT)] public static extern void _cpPinJointSetAnchr1(IntPtr constraint, Vector2 value);
	[DllImport(IMPORT)] public static extern Vector2 _cpPinJointGetAnchr2(IntPtr constraint);
	[DllImport(IMPORT)] public static extern void _cpPinJointSetAnchr2(IntPtr constraint, Vector2 value);
	[DllImport(IMPORT)] public static extern float _cpPinJointGetDist(IntPtr constraint);
	[DllImport(IMPORT)] public static extern void _cpPinJointSetDist(IntPtr constraint, float value);
	[DllImport(IMPORT)] public static extern IntPtr  cpSlideJointNew(IntPtr a, IntPtr b, Vector2 anchr1, Vector2 anchr2, float min, float max);
	[DllImport(IMPORT)] public static extern Vector2 _cpSlideJointGetAnchr1(IntPtr constraint);
	[DllImport(IMPORT)] public static extern void _cpSlideJointSetAnchr1(IntPtr constraint, Vector2 value);
	[DllImport(IMPORT)] public static extern Vector2 _cpSlideJointGetAnchr2(IntPtr constraint);
	[DllImport(IMPORT)] public static extern void _cpSlideJointSetAnchr2(IntPtr constraint, Vector2 value);
	[DllImport(IMPORT)] public static extern float _cpSlideJointGetMin(IntPtr constraint);
	[DllImport(IMPORT)] public static extern void _cpSlideJointSetMin(IntPtr constraint, float value);
	[DllImport(IMPORT)] public static extern float _cpSlideJointGetMax(IntPtr constraint);
	[DllImport(IMPORT)] public static extern void _cpSlideJointSetMax(IntPtr constraint, float value);
	[DllImport(IMPORT)] public static extern IntPtr  cpPivotJointNew(IntPtr a, IntPtr b, Vector2 pivot);
	[DllImport(IMPORT)] public static extern IntPtr  cpPivotJointNew2(IntPtr a, IntPtr b, Vector2 anchr1, Vector2 anchr2);
	[DllImport(IMPORT)] public static extern Vector2 _cpPivotJointGetAnchr1(IntPtr constraint);
	[DllImport(IMPORT)] public static extern void _cpPivotJointSetAnchr1(IntPtr constraint, Vector2 value);
	[DllImport(IMPORT)] public static extern Vector2 _cpPivotJointGetAnchr2(IntPtr constraint);
	[DllImport(IMPORT)] public static extern void _cpPivotJointSetAnchr2(IntPtr constraint, Vector2 value);
	[DllImport(IMPORT)] public static extern IntPtr  cpGrooveJointNew(IntPtr a, IntPtr b, Vector2 groove_a, Vector2 groove_b, Vector2 anchr2);
	[DllImport(IMPORT)] public static extern Vector2 _cpGrooveJointGetGrooveA(IntPtr constraint);
	[DllImport(IMPORT)] public static extern void cpGrooveJointSetGrooveA(IntPtr constraint, Vector2 value);
	[DllImport(IMPORT)] public static extern Vector2 _cpGrooveJointGetGrooveB(IntPtr constraint);
	[DllImport(IMPORT)] public static extern void cpGrooveJointSetGrooveB(IntPtr constraint, Vector2 value);
	[DllImport(IMPORT)] public static extern Vector2 _cpGrooveJointGetAnchr2(IntPtr constraint);
	[DllImport(IMPORT)] public static extern void _cpGrooveJointSetAnchr2(IntPtr constraint, Vector2 value);
	[DllImport(IMPORT)] public static extern IntPtr  cpDampedSpringNew(IntPtr a, IntPtr b, Vector2 anchr1, Vector2 anchr2, float restLength, float stiffness, float damping);
	[DllImport(IMPORT)] public static extern Vector2 _cpDampedSpringGetAnchr1(IntPtr constraint);
	[DllImport(IMPORT)] public static extern void _cpDampedSpringSetAnchr1(IntPtr constraint, Vector2 value);
	[DllImport(IMPORT)] public static extern Vector2 _cpDampedSpringGetAnchr2(IntPtr constraint);
	[DllImport(IMPORT)] public static extern void _cpDampedSpringSetAnchr2(IntPtr constraint, Vector2 value);
	[DllImport(IMPORT)] public static extern float _cpDampedSpringGetRestLength(IntPtr constraint);
	[DllImport(IMPORT)] public static extern void _cpDampedSpringSetRestLength(IntPtr constraint, float value);
	[DllImport(IMPORT)] public static extern float _cpDampedSpringGetStiffness(IntPtr constraint);
	[DllImport(IMPORT)] public static extern void _cpDampedSpringSetStiffness(IntPtr constraint, float value);
	[DllImport(IMPORT)] public static extern float _cpDampedSpringGetDamping(IntPtr constraint);
	[DllImport(IMPORT)] public static extern void _cpDampedSpringSetDamping(IntPtr constraint, float value);
	[DllImport(IMPORT)] public static extern IntPtr  cpDampedRotarySpringNew(IntPtr a, IntPtr b, float restAngle, float stiffness, float damping);
	[DllImport(IMPORT)] public static extern float _cpDampedRotarySpringGetRestAngle(IntPtr constraint);
	[DllImport(IMPORT)] public static extern void _cpDampedRotarySpringSetRestAngle(IntPtr constraint, float value);
	[DllImport(IMPORT)] public static extern float _cpDampedRotarySpringGetStiffness(IntPtr constraint);
	[DllImport(IMPORT)] public static extern void _cpDampedRotarySpringSetStiffness(IntPtr constraint, float value);
	[DllImport(IMPORT)] public static extern float _cpDampedRotarySpringGetDamping(IntPtr constraint);
	[DllImport(IMPORT)] public static extern void _cpDampedRotarySpringSetDamping(IntPtr constraint, float value);
	[DllImport(IMPORT)] public static extern IntPtr  cpRotaryLimitJointNew(IntPtr a, IntPtr b, float min, float max);
	[DllImport(IMPORT)] public static extern float _cpRotaryLimitJointGetMin(IntPtr constraint);
	[DllImport(IMPORT)] public static extern void _cpRotaryLimitJointSetMin(IntPtr constraint, float value);
	[DllImport(IMPORT)] public static extern float _cpRotaryLimitJointGetMax(IntPtr constraint);
	[DllImport(IMPORT)] public static extern void _cpRotaryLimitJointSetMax(IntPtr constraint, float value);
	[DllImport(IMPORT)] public static extern IntPtr  cpRatchetJointNew(IntPtr a, IntPtr b, float phase, float ratchet);
	[DllImport(IMPORT)] public static extern float _cpRatchetJointGetAngle(IntPtr constraint);
	[DllImport(IMPORT)] public static extern void _cpRatchetJointSetAngle(IntPtr constraint, float value);
	[DllImport(IMPORT)] public static extern float _cpRatchetJointGetPhase(IntPtr constraint);
	[DllImport(IMPORT)] public static extern void _cpRatchetJointSetPhase(IntPtr constraint, float value);
	[DllImport(IMPORT)] public static extern float _cpRatchetJointGetRatchet(IntPtr constraint);
	[DllImport(IMPORT)] public static extern void _cpRatchetJointSetRatchet(IntPtr constraint, float value);
	[DllImport(IMPORT)] public static extern IntPtr  cpGearJointNew(IntPtr a, IntPtr b, float phase, float ratio);
	[DllImport(IMPORT)] public static extern float _cpGearJointGetPhase(IntPtr constraint);
	[DllImport(IMPORT)] public static extern void _cpGearJointSetPhase(IntPtr constraint, float value);
	[DllImport(IMPORT)] public static extern float _cpGearJointGetRatio(IntPtr constraint);
	[DllImport(IMPORT)] public static extern void cpGearJointSetRatio(IntPtr constraint, float value);
	[DllImport(IMPORT)] public static extern IntPtr  cpSimpleMotorNew(IntPtr a, IntPtr b, float rate);
	[DllImport(IMPORT)] public static extern float _cpSimpleMotorGetRate(IntPtr constraint);
	[DllImport(IMPORT)] public static extern void _cpSimpleMotorSetRate(IntPtr constraint, float value);
	[DllImport(IMPORT)] public static extern IntPtr  cpSpaceNew();
	[DllImport(IMPORT)] public static extern void cpSpaceFree(IntPtr space);
	[DllImport(IMPORT)] public static extern int _cpSpaceGetIterations(IntPtr space);
	[DllImport(IMPORT)] public static extern void _cpSpaceSetIterations(IntPtr space, int value);
	[DllImport(IMPORT)] public static extern Vector2 _cpSpaceGetGravity(IntPtr space);
	[DllImport(IMPORT)] public static extern void _cpSpaceSetGravity(IntPtr space, Vector2 value);
	[DllImport(IMPORT)] public static extern float _cpSpaceGetDamping(IntPtr space);
	[DllImport(IMPORT)] public static extern void _cpSpaceSetDamping(IntPtr space, float value);
	[DllImport(IMPORT)] public static extern float _cpSpaceGetIdleSpeedThreshold(IntPtr space);
	[DllImport(IMPORT)] public static extern void _cpSpaceSetIdleSpeedThreshold(IntPtr space, float value);
	[DllImport(IMPORT)] public static extern float _cpSpaceGetSleepTimeThreshold(IntPtr space);
	[DllImport(IMPORT)] public static extern void _cpSpaceSetSleepTimeThreshold(IntPtr space, float value);
	[DllImport(IMPORT)] public static extern float _cpSpaceGetCollisionSlop(IntPtr space);
	[DllImport(IMPORT)] public static extern void _cpSpaceSetCollisionSlop(IntPtr space, float value);
	[DllImport(IMPORT)] public static extern float _cpSpaceGetCollisionBias(IntPtr space);
	[DllImport(IMPORT)] public static extern void _cpSpaceSetCollisionBias(IntPtr space, float value);
	[DllImport(IMPORT)] public static extern uint _cpSpaceGetCollisionPersistence(IntPtr space);
	[DllImport(IMPORT)] public static extern void _cpSpaceSetCollisionPersistence(IntPtr space, uint value);
	[DllImport(IMPORT)] public static extern bool _cpSpaceGetEnableContactGraph(IntPtr space);
	[DllImport(IMPORT)] public static extern void _cpSpaceSetEnableContactGraph(IntPtr space, bool value);
	[DllImport(IMPORT)] public static extern IntPtr _cpSpaceGetUserData(IntPtr space);
	[DllImport(IMPORT)] public static extern void _cpSpaceSetUserData(IntPtr space, IntPtr value);
	[DllImport(IMPORT)] public static extern IntPtr  _cpSpaceGetStaticBody(IntPtr space);
	[DllImport(IMPORT)] public static extern float _cpSpaceGetCurrentTimeStep(IntPtr space);
	[DllImport(IMPORT)] public static extern void cpSpaceRemoveCollisionHandler(IntPtr space, int a, int b);
	[DllImport(IMPORT)] public static extern IntPtr  cpSpaceAddShape(IntPtr space, IntPtr shape);
	[DllImport(IMPORT)] public static extern IntPtr  cpSpaceAddStaticShape(IntPtr space, IntPtr shape);
	[DllImport(IMPORT)] public static extern IntPtr  cpSpaceAddBody(IntPtr space, IntPtr body);
	[DllImport(IMPORT)] public static extern IntPtr  cpSpaceAddConstraint(IntPtr space, IntPtr constraint);
	[DllImport(IMPORT)] public static extern void cpSpaceRemoveShape(IntPtr space, IntPtr shape);
	[DllImport(IMPORT)] public static extern void cpSpaceRemoveStaticShape(IntPtr space, IntPtr shape);
	[DllImport(IMPORT)] public static extern void cpSpaceRemoveBody(IntPtr space, IntPtr body);
	[DllImport(IMPORT)] public static extern void cpSpaceRemoveConstraint(IntPtr space, IntPtr constraint);
	[DllImport(IMPORT)] public static extern bool cpSpaceContainsShape(IntPtr space, IntPtr shape);
	[DllImport(IMPORT)] public static extern bool cpSpaceContainsBody(IntPtr space, IntPtr body);
	[DllImport(IMPORT)] public static extern bool cpSpaceContainsConstraint(IntPtr space, IntPtr constraint);
	[DllImport(IMPORT)] public static extern void cpSpaceConvertBodyToStatic(IntPtr space, IntPtr body);
	[DllImport(IMPORT)] public static extern void cpSpaceConvertBodyToDynamic(IntPtr space, IntPtr body, float mass, float moment);
	[DllImport(IMPORT)] public static extern void cpSpaceActivateShapesTouchingShape(IntPtr space, IntPtr shape);
	[DllImport(IMPORT)] public static extern void cpSpaceReindexStatic(IntPtr space);
	[DllImport(IMPORT)] public static extern void cpSpaceReindexShape(IntPtr space, IntPtr shape);
	[DllImport(IMPORT)] public static extern void cpSpaceReindexShapesForBody(IntPtr space, IntPtr body);
	[DllImport(IMPORT)] public static extern void cpSpaceUseSpatialHash(IntPtr space, float dim, int count);
	[DllImport(IMPORT)] public static extern void cpSpaceStep(IntPtr space, float dt);
	[DllImport(IMPORT)] public static extern void cpEnableSegmentToSegmentCollisions();
	[DllImport(IMPORT)] public static extern float cpMomentForCircle(float m, float r1, float r2, Vector2 offset);
	[DllImport(IMPORT)] public static extern float cpAreaForCircle(float r1, float r2);
	[DllImport(IMPORT)] public static extern float cpMomentForSegment(float m, Vector2 a, Vector2 b);
	[DllImport(IMPORT)] public static extern float cpAreaForSegment(Vector2 a, Vector2 b, float r);
	[DllImport(IMPORT)] public static extern float cpMomentForBox(float m, float width, float height);
	[DllImport(IMPORT)] public static extern float cpMomentForBox2(float m, BB box);
	[DllImport(IMPORT)] public static extern void cpCircleShapeSetRadius(IntPtr shape, float radius);
	[DllImport(IMPORT)] public static extern void cpCircleShapeSetOffset(IntPtr shape, Vector2 offset);
	[DllImport(IMPORT)] public static extern void cpSegmentShapeSetEndpoints(IntPtr shape, Vector2 a, Vector2 b);
	[DllImport(IMPORT)] public static extern void cpSegmentShapeSetRadius(IntPtr shape, float radius);
	[DllImport(IMPORT)] public static extern void cpPolyShapeSetRadius(IntPtr shape, float radius);
}







public partial class ChipmunkSpace {
	public int iterations {
		get { return (_handle != IntPtr.Zero ? ChipmunkBinding._cpSpaceGetIterations(_handle) : 0); }
		set { if(_handle != IntPtr.Zero) ChipmunkBinding._cpSpaceSetIterations(_handle, value); }
	}

	public Vector2 gravity {
		get { return (_handle != IntPtr.Zero ? ChipmunkBinding._cpSpaceGetGravity(_handle) : Vector2.zero); }
		set { if(_handle != IntPtr.Zero) ChipmunkBinding._cpSpaceSetGravity(_handle, value); }
	}

	public float damping {
		get { return (_handle != IntPtr.Zero ? ChipmunkBinding._cpSpaceGetDamping(_handle) : 0f); }
		set { if(_handle != IntPtr.Zero) ChipmunkBinding._cpSpaceSetDamping(_handle, value); }
	}

	public float idleSpeedThreshold {
		get { return (_handle != IntPtr.Zero ? ChipmunkBinding._cpSpaceGetIdleSpeedThreshold(_handle) : 0f); }
		set { if(_handle != IntPtr.Zero) ChipmunkBinding._cpSpaceSetIdleSpeedThreshold(_handle, value); }
	}

	public float sleepTimeThreshold {
		get { return (_handle != IntPtr.Zero ? ChipmunkBinding._cpSpaceGetSleepTimeThreshold(_handle) : 0f); }
		set { if(_handle != IntPtr.Zero) ChipmunkBinding._cpSpaceSetSleepTimeThreshold(_handle, value); }
	}

	public float collisionSlop {
		get { return (_handle != IntPtr.Zero ? ChipmunkBinding._cpSpaceGetCollisionSlop(_handle) : 0f); }
		set { if(_handle != IntPtr.Zero) ChipmunkBinding._cpSpaceSetCollisionSlop(_handle, value); }
	}

	public float collisionBias {
		get { return (_handle != IntPtr.Zero ? ChipmunkBinding._cpSpaceGetCollisionBias(_handle) : 0f); }
		set { if(_handle != IntPtr.Zero) ChipmunkBinding._cpSpaceSetCollisionBias(_handle, value); }
	}

	public uint collisionPersistence {
		get { return (_handle != IntPtr.Zero ? ChipmunkBinding._cpSpaceGetCollisionPersistence(_handle) : 0); }
		set { if(_handle != IntPtr.Zero) ChipmunkBinding._cpSpaceSetCollisionPersistence(_handle, value); }
	}

	public bool enableContactGraph {
		get { return (_handle != IntPtr.Zero ? ChipmunkBinding._cpSpaceGetEnableContactGraph(_handle) : false); }
		set { if(_handle != IntPtr.Zero) ChipmunkBinding._cpSpaceSetEnableContactGraph(_handle, value); }
	}
}
