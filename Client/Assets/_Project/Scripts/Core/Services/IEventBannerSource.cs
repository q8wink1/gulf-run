using System.Collections.Generic;

namespace GulfRun.Core.Services
{
    /// <summary>
    /// Implemented by any Feature manager that can contribute one or more
    /// live messages to the Sprint 13 Main Menu's rotating "EVENT BANNER"
    /// (brief: "Ramadan. National Days. Battle Pass. Limited Offers.
    /// Special Events."). Each implementer owns deciding what counts as
    /// "currently active" for its own domain — the banner widget itself
    /// only ever rotates through whatever <see cref="EventBannerRegistry.CollectActiveMessages"/>
    /// currently returns, with zero Feature-specific knowledge.
    /// </summary>
    public interface IEventBannerSource
    {
        /// <summary>Zero or more short, already-formatted display strings. Return an empty list (never null) when nothing is currently active.</summary>
        IReadOnlyList<string> GetActiveBannerMessages();
    }
}
