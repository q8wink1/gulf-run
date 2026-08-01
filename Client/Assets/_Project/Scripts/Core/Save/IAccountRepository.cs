using GulfRun.Domain;

namespace GulfRun.Core.Save
{
    /// <summary>
    /// Abstraction over the one-time Account Creation step (Sprint 8):
    /// Display Name + Country, captured exactly once and permanently linked
    /// thereafter — "Country cannot be changed later." Mirrors
    /// <see cref="IProgressRepository"/>'s "contract now, real storage later"
    /// posture: <c>Managers.SaveManager</c> implements this with an
    /// in-memory store today; a future local-file/Cloud Save/backend-synced
    /// implementation can replace the storage with zero caller changes,
    /// since every caller only ever depends on this interface.
    /// </summary>
    public interface IAccountRepository
    {
        /// <summary>True once <see cref="CreateAccount"/> has succeeded. Every Account-gated flow (Character/Country selection, matchmaking) waits for this.</summary>
        bool HasAccount { get; }

        /// <summary>The permanently-linked account. Only meaningful once <see cref="HasAccount"/> is true.</summary>
        PlayerAccount GetAccount();

        /// <summary>
        /// Creates the account exactly once. If an account already exists,
        /// this is a no-op that returns the existing (unchanged) account —
        /// the enforcement point for "Country cannot be changed later,"
        /// even against a caller that tries to call this twice.
        /// </summary>
        PlayerAccount CreateAccount(string displayName, GulfCountry country);
    }
}
