namespace GulfRun.Domain
{
    /// <summary>
    /// Pure function that resolves a smooth render pose between two buffered
    /// <see cref="NetworkPlayerSnapshot"/>s for a given render time. Provides
    /// both the "Network Interpolation" requirement (smooth blend between
    /// two known samples) and the "prepare future prediction system"
    /// requirement (a short, clamped linear extrapolation kicks in if the
    /// render time runs ahead of the last received snapshot, e.g. during a
    /// brief network stall — beyond <paramref name="maxExtrapolationSeconds"/>
    /// it holds at the last known pose instead of overshooting, which is
    /// what avoids visible jitter/teleporting on longer stalls).
    /// </summary>
    public static class NetworkInterpolator
    {
        public static NetworkPlayerSnapshot Resolve(
            NetworkPlayerSnapshot from,
            NetworkPlayerSnapshot to,
            double renderTimeSeconds,
            double maxExtrapolationSeconds)
        {
            double span = to.TimestampSeconds - from.TimestampSeconds;

            if (span <= 0d || renderTimeSeconds <= from.TimestampSeconds)
            {
                return from;
            }

            if (renderTimeSeconds <= to.TimestampSeconds)
            {
                float t = (float)((renderTimeSeconds - from.TimestampSeconds) / span);
                return Lerp(from, to, t);
            }

            double overrun = renderTimeSeconds - to.TimestampSeconds;
            double clampedOverrun = overrun > maxExtrapolationSeconds ? maxExtrapolationSeconds : overrun;
            float extrapolatedT = (float)(1d + clampedOverrun / span);
            return Lerp(from, to, extrapolatedT);
        }

        private static NetworkPlayerSnapshot Lerp(NetworkPlayerSnapshot from, NetworkPlayerSnapshot to, float t)
        {
            NetVector2 position = NetVector2.Lerp(from.Position, to.Position, t);
            float rotation = from.RotationDegrees + (to.RotationDegrees - from.RotationDegrees) * t;
            PlayerMovementState animationState = t >= 0.5f ? to.AnimationState : from.AnimationState;
            double timestamp = from.TimestampSeconds + (to.TimestampSeconds - from.TimestampSeconds) * t;

            return new NetworkPlayerSnapshot(to.ConnectionId, position, rotation, animationState, timestamp);
        }
    }
}
