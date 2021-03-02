﻿using System.Linq;
using UnityEngine;

public class PossessionPose
{
    private readonly EmbodyContext _context;

    public PossessionPose(EmbodyContext context)
    {
        _context = context;
    }
    public void Apply()
    {
        var preferToes = _context.trackers.viveTrackers.Any(t => t.SyncMotionControl());

        var measurements = new PersonMeasurements(_context);
        var height = measurements.MeasureHeight();
        // TODO: Measure shoulders
        var scale = _context.scaleChangeReceiver.scale;
        var globalForwardOffset = (-0.025f) * scale;
        var footHalfDistance = (0.045f) * scale;
        const float footYaw = 4f;
        const float footPitch = 18f;
        var footFloorDistance = (0.055f) * scale;
        var headForwardOffset = (-0.020f + globalForwardOffset) * scale;
        var hipForwardOffset = (0.020f + globalForwardOffset) * scale;
        var feetForwardOffset = (0.010f + globalForwardOffset) * scale;
        var handsToHipOffset = (-0.10f) * scale;
        var handsForwardOffset = (0.05f + globalForwardOffset) * scale;
        var handsRightOffset = (0.22f) * scale;

        var head = _context.containingAtom.freeControllers.First(fc => fc.name == "headControl");
        var pelvis = _context.containingAtom.freeControllers.First(fc => fc.name == "pelvisControl");
        var hip = _context.containingAtom.freeControllers.First(fc => fc.name == "hipControl");
        var lFoot = _context.containingAtom.freeControllers.First(fc => fc.name == "lFootControl");
        var rFoot = _context.containingAtom.freeControllers.First(fc => fc.name == "rFootControl");
        var lToe = _context.containingAtom.freeControllers.First(fc => fc.name == "lToeControl");
        var rToe = _context.containingAtom.freeControllers.First(fc => fc.name == "rToeControl");
        var lHand = _context.containingAtom.freeControllers.First(fc => fc.name == "lHandControl");
        var rHand = _context.containingAtom.freeControllers.First(fc => fc.name == "rHandControl");

        var position = (pelvis.control.position + hip.control.position) / 2f;
        position.Scale(new Vector3(1f, 0f, 1f));
        var direction = (
            Vector3.ProjectOnPlane(hip.control.eulerAngles, Vector3.up).y +
            Vector3.ProjectOnPlane(pelvis.control.eulerAngles, Vector3.up).y +
            Vector3.ProjectOnPlane(head.control.eulerAngles, Vector3.up).y
        ) / 3f;

        foreach (var controller in _context.containingAtom.freeControllers.Where(fc => fc.name.EndsWith("Control")).Where(fc => fc.control != null))
        {
            SetState(controller, FreeControllerV3.PositionState.Off, FreeControllerV3.RotationState.Off);
        }

        SetState(head, FreeControllerV3.PositionState.On, FreeControllerV3.RotationState.On);
        head.control.localEulerAngles = new Vector3(0f, direction, 0f);
        var headEulerAngles = head.control.localEulerAngles;
        var headForward = head.control.forward;
        var headRight = head.control.right;
        head.control.position = position + new Vector3(0f, height, 0f) + headForward * headForwardOffset;
        head.RBHoldPositionSpring = 10000f;
        head.RBHoldRotationSpring = 1000f;

        SetState(hip, FreeControllerV3.PositionState.On, FreeControllerV3.RotationState.On);
        hip.control.localEulerAngles = new Vector3(0f, direction, 0f);
        var hipForward = hip.control.forward;
        var hipHeight = measurements.MeasureToHip("lFoot");
        hip.control.position = position + new Vector3(0f, hipHeight, 0f) + hipForward * hipForwardOffset;
        hip.RBHoldPositionSpring = 4000f;
        hip.RBHoldRotationSpring = 1000f;

        SetState(lFoot, FreeControllerV3.PositionState.On, FreeControllerV3.RotationState.On);
        lFoot.control.localEulerAngles = new Vector3(footPitch, direction - footYaw, 0f);
        lFoot.control.position = position - headRight * footHalfDistance + Vector3.up * footFloorDistance + headForward * feetForwardOffset;
        lFoot.RBHoldPositionSpring = 10000f;
        lFoot.RBHoldRotationSpring = 1000f;

        SetState(rFoot, FreeControllerV3.PositionState.On, FreeControllerV3.RotationState.On);
        rFoot.control.localEulerAngles = new Vector3(footPitch, direction + footYaw, 0f);
        rFoot.control.position = position + headRight * footHalfDistance + Vector3.up * footFloorDistance + headForward * feetForwardOffset;
        rFoot.RBHoldPositionSpring = 10000f;
        rFoot.RBHoldRotationSpring = 1000f;

        if (preferToes)
        {
            // TODO: In theory it would be better if the feet sole moved but the toes didn't, but the toes have weird movements and it's had to get the sole to stay on the ground.
            /*
            var toeBone = _context.bones.First(b => b.name == "rToe");
            var toeToFeetDistance = toeBone.transform.localPosition.magnitude;

            SetState(lToe, FreeControllerV3.PositionState.On, FreeControllerV3.RotationState.On);
            lToe.control.localEulerAngles = lFoot.control.eulerAngles;
            lToe.control.position = lFoot.control.position + lFoot.control.forward * toeToFeetDistance;
            lToe.RBHoldPositionSpring = 10000f;
            lToe.RBHoldRotationSpring = 500f;

            SetState(rToe, FreeControllerV3.PositionState.On, FreeControllerV3.RotationState.On);
            rToe.control.localEulerAngles = rFoot.control.eulerAngles;
            rToe.control.position = rFoot.control.position + rFoot.control.forward * toeToFeetDistance;
            rToe.RBHoldPositionSpring = 10000f;
            rToe.RBHoldRotationSpring = 500f;
            */
        }

        SetState(lHand, FreeControllerV3.PositionState.On, FreeControllerV3.RotationState.On);
        lHand.control.localEulerAngles = new Vector3(-25f, 0, 90f);
        lHand.control.position = position + Vector3.up * (hipHeight + handsToHipOffset) + headForward * handsForwardOffset - headRight * handsRightOffset;
        lHand.RBHoldPositionSpring = 10000f;
        lHand.RBHoldRotationSpring = 1000f;

        SetState(rHand, FreeControllerV3.PositionState.On, FreeControllerV3.RotationState.On);
        rHand.control.localEulerAngles = new Vector3(-25f, 0, -90f);
        rHand.control.position = position + Vector3.up * (hipHeight + handsToHipOffset) + headForward * handsForwardOffset + headRight * handsRightOffset;
        rHand.RBHoldPositionSpring = 10000f;
        rHand.RBHoldRotationSpring = 1000f;
    }

    private static void SetState(FreeControllerV3 controller, FreeControllerV3.PositionState positionState, FreeControllerV3.RotationState rotationState)
    {
        controller.currentPositionState = positionState;
        controller.currentRotationState = rotationState;
    }
}