namespace GulfRun.Domain
{
    /// <summary>
    /// The Pre-Race Lobby's fixed Quick Chat vocabulary (Sprint 15 "CHAT:
    /// Quick Chat, Emoji, Simple Messages — Ready, Good Luck, Wait, Hello").
    /// A closed enum rather than free text on purpose — no chat moderation/
    /// profanity-filter system exists anywhere in this project yet, so Quick
    /// Chat is the only lobby chat surface until one does (see Sprint report
    /// Remaining TODOs).
    /// </summary>
    public enum QuickChatMessage
    {
        Ready,
        GoodLuck,
        Wait,
        Hello
    }

    /// <summary>Pure display mapping — no UnityEngine dependency, same "engine-free Domain" posture as <see cref="RoomCodeGenerator"/>.</summary>
    public static class QuickChatMessageTextResolver
    {
        public static string ResolveText(QuickChatMessage message) => message switch
        {
            QuickChatMessage.Ready => "Ready! ✅",
            QuickChatMessage.GoodLuck => "Good Luck! 🍀",
            QuickChatMessage.Wait => "Wait... ⏳",
            QuickChatMessage.Hello => "Hello! 👋",
            _ => string.Empty
        };
    }
}
