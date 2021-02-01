﻿using System.Linq;
using UnityEngine;

public class MeasureAnchorWidthStep : WizardStepBase, IWizardStep
{
    public string helpText => $"Possession activated. Now put your hands on your real {_part}, and Press next when ready.";

    private readonly string _part;
    private readonly ControllerAnchorPoint _anchor;
    private readonly FreeControllerV3 _leftHandControl;
    private readonly FreeControllerV3 _rightHandControl;

    public MeasureAnchorWidthStep(EmbodyContext context, string part, ControllerAnchorPoint anchor)
        : base(context)
    {
        _part = part;
        _anchor = anchor;
        _leftHandControl = context.containingAtom.freeControllers.First(fc => fc.name == "lHandControl");
        _rightHandControl = context.containingAtom.freeControllers.First(fc => fc.name == "rHandControl");
    }

    public override void Update()
    {
        // VisualCuesHelper.Cross(Color.green).transform.position = _anchor.GetInGameWorldPosition() + (-_anchor.RigidBody.transform.right * (_anchor.InGameSize.x / 2f + _context.handsDistance / 2f));
        // TODO: Cancel out the hand size, whatever that is. Figure out if we should compute it or just hardcode it.
        _leftHandControl.control.position = _anchor.GetInGameWorldPosition() + (-_anchor.rigidBody.transform.right * (_anchor.inGameSize.x / 2f + TrackersConstants.handsDistance / 2f));
        _leftHandControl.control.eulerAngles = _anchor.rigidBody.rotation.eulerAngles;
        _leftHandControl.control.Rotate(new Vector3(0, 0, 90));

        _rightHandControl.control.position = _anchor.GetInGameWorldPosition() + (_anchor.rigidBody.transform.right * (_anchor.inGameSize.x / 2f + TrackersConstants.handsDistance / 2f));
        _rightHandControl.control.eulerAngles = _anchor.rigidBody.rotation.eulerAngles;
        _rightHandControl.control.Rotate(new Vector3(0, 0, -90));
    }

    public void Apply()
    {
        // TODO: Highlight the ring where we want the hands to be.
        // TODO: Make the model move their hand in the right position.
        var gameHipsCenter = _anchor.GetInGameWorldPosition();
        // TODO: Check the forward size too, and the offset.
        // TODO: Don't check the _hand control_ distance, instead check the relevant distance (from inside the hands)
        var realHipsWidth = Vector3.Distance(context.leftHand.position, context.rightHand.position) - TrackersConstants.handsDistance;
        var realHipsXCenter = (context.leftHand.position + context.rightHand.position) / 2f;
        _anchor.realLifeSize = new Vector3(realHipsWidth, 0f, _anchor.inGameSize.z);
        _anchor.realLifeOffset = realHipsXCenter - gameHipsCenter;
        SuperController.LogMessage($"Real Hips height: {realHipsXCenter.y}, Game Hips height: {gameHipsCenter}");
        SuperController.LogMessage($"Real Hips width: {realHipsWidth}, Game Hips width: {_anchor.realLifeSize.x}");
        SuperController.LogMessage($"Real Hips center: {realHipsXCenter}, Game Hips center: {gameHipsCenter}");
    }
}
