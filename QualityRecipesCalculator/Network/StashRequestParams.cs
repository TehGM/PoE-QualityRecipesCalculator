using System;

namespace TehGM.PoE.QualityRecipesCalculator
{
    public class StashRequestParams
    {
        public string League { get; }
        public string Realm { get; }
        public string AccountName { get; }
        public bool TabDetails { get; }
        public int? TabIndex { get; }
        public string SessionID { get; }

        public StashRequestParams(string accountName, string league, string realm, string sessionID, int? tabIndex = null, bool requestTabDetails = true)
        {
            this.AccountName = accountName ?? throw new ArgumentNullException(nameof(accountName));
            this.League = league ?? throw new ArgumentNullException(nameof(league));
            this.SessionID = sessionID ?? throw new ArgumentNullException(nameof(sessionID));
            this.Realm = realm ?? "pc";
            this.TabIndex = tabIndex;
            this.TabDetails = requestTabDetails;
        }
    }
}
