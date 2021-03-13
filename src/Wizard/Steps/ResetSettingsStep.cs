﻿using System.Linq;

public class ResetSettingsStep : WizardStepBase, IWizardStep
{
    public string helpText => (@"
All Embody settings will now be <b>reset</b> to their default value, except for hand adjustments.

Only skip this if you know what you are doing!").TrimStart();

    public ResetSettingsStep(EmbodyContext context)
        : base(context)
    {
    }

    public bool Apply()
    {
        var key = context.automation.toggleKey;
        var leftOffsetControllerCustom = context.trackers.leftHandMotionControl.offsetControllerCustom;
        var leftRotateControllerCustom = context.trackers.leftHandMotionControl.rotateControllerCustom;
        var leftRotateAroundTrackerCustom = context.trackers.leftHandMotionControl.rotateAroundTrackerCustom;
        var rightOffsetControllerCustom = context.trackers.rightHandMotionControl.offsetControllerCustom;
        var rightRotateControllerCustom = context.trackers.rightHandMotionControl.rotateControllerCustom;
        var rightRotateAroundTrackerCustom = context.trackers.rightHandMotionControl.rotateAroundTrackerCustom;
        Utilities.ResetToDefaults(context);
        context.automation.toggleKey = key;
        context.trackers.leftHandMotionControl.offsetControllerCustom = leftOffsetControllerCustom;
        context.trackers.leftHandMotionControl.rotateControllerCustom = leftRotateControllerCustom;
        context.trackers.leftHandMotionControl.rotateAroundTrackerCustom = leftRotateAroundTrackerCustom;
        context.trackers.rightHandMotionControl.offsetControllerCustom = rightOffsetControllerCustom;
        context.trackers.rightHandMotionControl.rotateControllerCustom = rightRotateControllerCustom;
        context.trackers.rightHandMotionControl.rotateAroundTrackerCustom = rightRotateAroundTrackerCustom;
        return true;
    }
}