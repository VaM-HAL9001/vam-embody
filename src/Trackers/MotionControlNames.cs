﻿public static class MotionControlNames
{
    public const string Head = "Head";
    public const string LeftHand = "LeftHand";
    public const string RightHand = "RightHand";
    public const string ViveTrackerPrefix = "ViveTracker";

    public static bool IsHeadOrHands(string name)
    {
        return name == Head || name == LeftHand || name == RightHand;
    }

    public static bool IsViveTracker(string name)
    {
        return name.StartsWith(ViveTrackerPrefix);
    }
}
