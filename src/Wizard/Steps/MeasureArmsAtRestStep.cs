﻿using System.Linq;
using UnityEngine;

public class MeasureArmsAtRestStep : WizardStepBase, IWizardStep
{
    public string helpText => $"Stand straight and relax your hands like the model is doing right now, and press Next when ready.\n\nTry not to overstretch your arms and match as closely as you can the hands position of the model.";

    private readonly ControllerAnchorPoint _anchor;
    private readonly FreeControllerV3 _leftHandControl;
    private readonly FreeControllerV3 _rightHandControl;
    private readonly MotionControllerWithCustomPossessPoint _leftHandMotion;
    private readonly MotionControllerWithCustomPossessPoint _rightHandMotion;
    private FreeControllerV3Snapshot _leftHandSnapshot;
    private FreeControllerV3Snapshot _rightHandSnapshot;

    public MeasureArmsAtRestStep(EmbodyContext context, ControllerAnchorPoint anchor)
        : base(context)
    {
        _anchor = anchor;
        _leftHandControl = context.containingAtom.freeControllers.First(fc => fc.name == "lHandControl");
        _rightHandControl = context.containingAtom.freeControllers.First(fc => fc.name == "rHandControl");
        _leftHandMotion = context.trackers.motionControls.First(mc => mc.name == MotionControlNames.LeftHand);
        _rightHandMotion = context.trackers.motionControls.First(mc => mc.name == MotionControlNames.RightHand);
    }

    public override void Enter()
    {
        _leftHandSnapshot = FreeControllerV3Snapshot.Snap(_leftHandControl);
        _rightHandSnapshot = FreeControllerV3Snapshot.Snap(_rightHandControl);
        _leftHandControl.currentPositionState = FreeControllerV3.PositionState.Off;
        _leftHandControl.currentRotationState = FreeControllerV3.RotationState.Off;
        _rightHandControl.currentPositionState = FreeControllerV3.PositionState.Off;
        _rightHandControl.currentRotationState = FreeControllerV3.RotationState.Off;
    }

    public void Apply()
    {
        var realY = (_leftHandMotion.controllerPointTransform.position.y + _rightHandMotion.controllerPointTransform.position.y) / 2f;
        var inGameY = (_leftHandControl.control.position.y + _rightHandControl.control.position.y) / 2f;
        var difference = realY - inGameY;
        _anchor.realLifeOffset = new Vector3(0f, difference, 0f);
    }

    public override void Leave()
    {
        _leftHandSnapshot.Restore(true);
        _rightHandSnapshot.Restore(true);
    }
}
