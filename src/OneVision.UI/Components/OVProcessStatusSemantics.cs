namespace OneVision.UI.Components;

public static class OVProcessStatusSemantics
{
    public static double CalculateProgressPercent(long? current, long? total)
    {
        if (current is null || total is null || total <= 0)
            return 0d;

        return Math.Clamp((double)current.Value / total.Value * 100d, 0d, 100d);
    }
}
