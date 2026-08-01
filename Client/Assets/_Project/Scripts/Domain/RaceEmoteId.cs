namespace GulfRun.Domain
{
    /// <summary>
    /// Fixed in-race quick-emote vocabulary (Sprint 15 Race HUD). Closed enum
    /// on purpose — free-text chat moderation does not exist yet, so emotes
    /// stay a fixed, harmless preset list (same posture as
    /// <see cref="QuickChatMessage"/> for the Pre-Race Lobby).
    /// </summary>
    public enum RaceEmoteId
    {
        Smile,
        Laugh,
        Cool,
        Clap,
        Flex,
        Heart
    }

    /// <summary>Pure glyph mapping for OnGUI placeholder presentation.</summary>
    public static class RaceEmoteGlyphResolver
    {
        public static string ResolveGlyph(RaceEmoteId emote) => emote switch
        {
            RaceEmoteId.Smile => "😀",
            RaceEmoteId.Laugh => "😂",
            RaceEmoteId.Cool => "😎",
            RaceEmoteId.Clap => "👏",
            RaceEmoteId.Flex => "💪",
            RaceEmoteId.Heart => "❤️",
            _ => string.Empty
        };
    }
}
