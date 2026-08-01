namespace GulfRun.Domain
{
    /// <summary>
    /// Pure, deterministic fireworks burst for the finish-line banner —
    /// same "one pure function, no mutable particle state" shape as
    /// <see cref="ConfettiSimulation"/>.
    /// </summary>
    public static class FireworkSimulation
    {
        public static FireworkParticle Evaluate(int particleIndex, double elapsedSeconds, float burstSpeed)
        {
            float angle = Hash01(particleIndex, 2.1f) * (float)System.Math.PI * 2f;
            float speed = 0.35f + Hash01(particleIndex, 4.7f) * burstSpeed;
            float life = (float)elapsedSeconds * speed;
            float radius = Frac(life) * 0.55f;
            float x = 0.5f + (float)System.Math.Cos(angle) * radius;
            float y = 0.45f - (float)System.Math.Sin(angle) * radius * 0.85f - Frac(life) * 0.15f;
            float alpha = 1f - Frac(life);

            return new FireworkParticle(x, y, alpha);
        }

        private static float Hash01(int index, float salt)
        {
            float value = (float)System.Math.Sin(index * 12.9898f + salt * 78.233f) * 43758.5453f;
            return Frac(value);
        }

        private static float Frac(float value) => value - (float)System.Math.Floor(value);
    }

    /// <summary>One fireworks particle's normalized screen position + alpha.</summary>
    public readonly struct FireworkParticle
    {
        public readonly float NormalizedX;
        public readonly float NormalizedY;
        public readonly float Alpha01;

        public FireworkParticle(float normalizedX, float normalizedY, float alpha01)
        {
            NormalizedX = normalizedX;
            NormalizedY = normalizedY;
            Alpha01 = alpha01;
        }
    }
}
