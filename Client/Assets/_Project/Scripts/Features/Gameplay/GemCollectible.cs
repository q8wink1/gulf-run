using GulfRun.Domain;
using UnityEngine;

namespace GulfRun.Features.Gameplay
{
    /// <summary>Sprint 23.12 — premium on-track gem pickup.</summary>
    public sealed class GemCollectible : Collectible
    {
        public override CollectibleType Type => CollectibleType.Gem;
    }
}
