using UnityEngine;

namespace GulfRun.Core.Managers
{
    /// <summary>
    /// Central collection point for player activity, gameplay, economy and
    /// technical analytics events. Must never collect sensitive personal
    /// information and must not impact gameplay performance.
    /// References: P044 (Analytics System).
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class AnalyticsManager : Singleton<AnalyticsManager>
    {
        protected override void OnInitialize()
        {
            // TODO(Sprint 2+): Integrate an analytics provider once selected
            // (see P044 "Not Defined: Analytics Provider").
        }
    }
}
