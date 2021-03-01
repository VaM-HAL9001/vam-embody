﻿using System.Linq;

public class RecordPlayerHeightStep : WizardStepBase, IWizardStep
{
    private readonly PlayerMeasurements _playerMeasurements;

    public string helpText => context.trackers.selectedJSON.val && context.trackers.viveTrackers.Any(mc => mc.enabled && mc.SyncMotionControl())
        ? @"
We will now <b>measure your height</b>.

Please <b>place one controller on the ground</b>, <b>stand straight</b>, and press <b>Next</b> when ready.".TrimStart()
        : @"
We will now <b>measure your height</b>.

This will improve automatic <b>world scale</b>, making your body height feel right.

You can optionally place a controller on the ground for a more precise height estimation.

<b>Stand straight</b>, and press <b>Next</b> when ready.".TrimStart();

    public RecordPlayerHeightStep(EmbodyContext context)
        : base(context)
    {
        _playerMeasurements = new PlayerMeasurements(context);
    }

    public bool Apply()
    {
        context.worldScale.playerHeightJSON.val = _playerMeasurements.MeasureHeight();
        context.worldScale.worldScaleMethodJSON.val = WorldScaleModule.PlayerHeightMethod;

        context.diagnostics.TakeSnapshot($"{nameof(RecordPlayerHeightStep)}.{nameof(Apply)}");

        return true;
    }
}
