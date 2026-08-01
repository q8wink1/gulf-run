namespace GulfRun.Domain
{
    /// <summary>
    /// Minimal, engine-independent 2D vector used for network snapshots and
    /// spawn-layout math, so this Domain code (unlike UnityEngine.Vector2)
    /// can run unmodified on a future dedicated server process.
    /// </summary>
    public readonly struct NetVector2
    {
        public readonly float X;
        public readonly float Y;

        public NetVector2(float x, float y)
        {
            X = x;
            Y = y;
        }

        public static NetVector2 Zero => new NetVector2(0f, 0f);

        /// <summary>Linear interpolation; passing t outside [0,1] extrapolates linearly.</summary>
        public static NetVector2 Lerp(NetVector2 a, NetVector2 b, float t) =>
            new NetVector2(a.X + (b.X - a.X) * t, a.Y + (b.Y - a.Y) * t);
    }
}
