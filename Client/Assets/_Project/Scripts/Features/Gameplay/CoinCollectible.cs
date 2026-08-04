using GulfRun.Domain;
using UnityEngine;

namespace GulfRun.Features.Gameplay
{
    /// <summary>Sprint 23.12 — standard on-track coin pickup.</summary>
    public sealed class CoinCollectible : Collectible
    {
        public override CollectibleType Type => CollectibleType.Coin;
    }
}
