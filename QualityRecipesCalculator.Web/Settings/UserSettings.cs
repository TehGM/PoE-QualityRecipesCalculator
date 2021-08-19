using System;

namespace TehGM.PoE.QualityRecipesCalculator.Settings
{
    public class UserSettings
    {
        public string AccountName { get; set; }
        public string Realm { get; set; }
        public string SessionID { get; set; }
        public string League { get; set; }

        public bool IsValid =>
            !string.IsNullOrWhiteSpace(this.AccountName) &&
            !string.IsNullOrWhiteSpace(this.Realm) &&
            !string.IsNullOrWhiteSpace(this.SessionID) &&
            !string.IsNullOrWhiteSpace(this.League);
    }
}
