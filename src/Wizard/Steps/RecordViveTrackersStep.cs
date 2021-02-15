﻿public class RecordViveTrackersStep : WizardStepBase, IWizardStep
{
    public string helpText => "Align all remaining vive trackers as closely as possible to your model's position, and press Next.";

    public RecordViveTrackersStep(EmbodyContext context)
        : base(context)
    {

    }

    public void Apply()
    {
        var autoSetup = new TrackerAutoSetup(context.containingAtom);
        foreach (var mc in context.trackers.viveTrackers)
        {
            if(mc.mappedControllerName != null) continue;
            if (!mc.SyncMotionControl()) continue;
            autoSetup.AttachToClosestNode(mc);
        }
        context.Refresh();
    }

    public override void Update()
    {
        // TODO: Enable and disable the preview
        // TODO: Draw lines connecting trackers with their closest target
    }
}
