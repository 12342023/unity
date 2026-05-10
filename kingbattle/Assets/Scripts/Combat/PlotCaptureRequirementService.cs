using Core;
using Map;

namespace Combat
{
    /// <summary>
    /// Pure data layer: returns the minimum number of soldiers required
    /// to capture a plot, based on its PlotSize.
    ///
    /// Rules:
    ///   Small  = 1
    ///   Medium = 2
    ///   Large  = 3
    ///   null / unknown = 0
    ///
    /// This does not change current capture behaviour (arrival = capture).
    /// The requirement data is available for future systems.
    /// </summary>
    public static class PlotCaptureRequirementService
    {
        /// <summary>Required soldier count for a given plot. Returns 0 for null.</summary>
        public static int GetRequiredSoldierCount(PlotData plot)
        {
            if (plot == null) return 0;
            return GetRequiredSoldierCount(plot.size);
        }

        /// <summary>Required soldier count for a given plot size.</summary>
        public static int GetRequiredSoldierCount(PlotSize size)
        {
            return size switch
            {
                PlotSize.Small  => 1,
                PlotSize.Medium => 2,
                PlotSize.Large  => 3,
                _ => 0
            };
        }
    }
}
