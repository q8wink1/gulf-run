using System.Collections.Generic;

namespace GulfRun.Core.Services
{
    /// <summary>
    /// A small "many independent contributors, one shared consumer"
    /// registry for <see cref="IEventBannerSource"/> — the additive-list
    /// generalization of the single-instance locator every other Sprint
    /// uses (see <see cref="MapContextService"/>), needed here because the
    /// Sprint 13 Event Banner must rotate through several Features' data
    /// at once (Championships/Country Events, Battle Pass, Special
    /// Offers, Special Login Events) with none of them referencing each
    /// other.
    /// </summary>
    public static class EventBannerRegistry
    {
        private static readonly List<IEventBannerSource> Sources = new List<IEventBannerSource>();

        public static void Register(IEventBannerSource source)
        {
            if (source != null && !Sources.Contains(source))
            {
                Sources.Add(source);
            }
        }

        public static void Unregister(IEventBannerSource source)
        {
            Sources.Remove(source);
        }

        /// <summary>Gathers every currently-active message across every registered source, in registration order. Never null.</summary>
        public static List<string> CollectActiveMessages()
        {
            var messages = new List<string>();
            for (int i = 0; i < Sources.Count; i++)
            {
                IReadOnlyList<string> fromSource = Sources[i].GetActiveBannerMessages();
                if (fromSource == null)
                {
                    continue;
                }

                for (int j = 0; j < fromSource.Count; j++)
                {
                    if (!string.IsNullOrEmpty(fromSource[j]))
                    {
                        messages.Add(fromSource[j]);
                    }
                }
            }

            return messages;
        }
    }
}
