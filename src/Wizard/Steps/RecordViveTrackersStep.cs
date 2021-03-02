﻿using System.Collections.Generic;
using System.Linq;

public class RecordViveTrackersStep : WizardStepBase, IWizardStep
{
    public string helpText => @"
Your Vive trackers will be mapped to a control on the atom. Their offset and rotation will be recorded.

- Try to <b>align</b> your pose
- Stand <b>straight</b>
- Look <b>forward</b>
- You can adjust the <b>hip</b> node

Press <b>Next</b> when you are ready.
".TrimStart();

    private float _heightAdjust;

    public RecordViveTrackersStep(EmbodyContext context)
        : base(context)
    {

    }

    public override void Enter()
    {
        base.Enter();

        context.trackers.previewTrackerOffsetJSON.val = true;
        context.embody.activeJSON.val = true;
        _heightAdjust = SuperController.singleton.playerHeightAdjust;
        /*
        This method creates less movement noise, but it's harder to precisely align eyes
        context.worldScale.enabledJSON.val = true;
        SuperController.singleton.AlignRigAndController(
            context.containingAtom.freeControllers.First(fc => fc.name == "headControl"),
            context.trackers.motionControls.First(mc => mc.name == MotionControlNames.Head),
            false);
        */
    }

    public override void Update()
    {
        SuperController.singleton.playerHeightAdjust = _heightAdjust;
    }

    public bool Apply()
    {
        context.diagnostics.TakeSnapshot($"{nameof(RecordViveTrackersStep)}.{nameof(Apply)}.Before");
        var autoSetup = new TrackerAutoSetup(context.containingAtom);
        var hashSet = new HashSet<string>();
        foreach (var mc in context.trackers.viveTrackers)
        {
            if (mc.mappedControllerName != null) continue;
            if (!mc.SyncMotionControl()) continue;
            autoSetup.AttachToClosestNode(mc);
            if (!hashSet.Add(mc.mappedControllerName))
            {
                lastError = $"The same controller was bound more than once: {mc.mappedControllerName}";
                context.diagnostics.Log(lastError);
                return false;
            }
        }
        context.Refresh();
        context.diagnostics.TakeSnapshot($"{nameof(RecordViveTrackersStep)}.{nameof(Apply)}.After");

        return true;
    }

    public override void Leave(bool final)
    {
        base.Leave(final);

        context.embody.activeJSON.val = false;
        context.trackers.previewTrackerOffsetJSON.val = false;
        /*
        context.worldScale.enabledJSON.val = false;
        _navigationRigSnapshot?.Restore();
        */
    }
}
