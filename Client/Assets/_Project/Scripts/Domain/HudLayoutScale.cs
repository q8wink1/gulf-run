namespace GulfRun.Domain
{
    /// <summary>
    /// Responsive OnGUI scale factor from screen size — phones get a slight
    /// bump for readability, tablets stay near 1.0. Pure math so layout
    /// rules stay out of view classes.
    /// </summary>
    public static class HudLayoutScale
    {
        public static float Resolve(int screenWidth, int screenHeight)
        {
            int shortest = screenWidth < screenHeight ? screenWidth : screenHeight;
            if (shortest <= 0)
            {
                return 1f;
            }

            // Reference: 375pt-class phone short side → 1.0; clamp for tiny/huge.
            float scale = shortest / 375f;
            if (scale < 0.85f)
            {
                return 0.85f;
            }

            if (scale > 1.35f)
            {
                return 1.35f;
            }

            return scale;
        }
    }
}
