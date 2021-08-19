using System.ComponentModel.DataAnnotations;

namespace TehGM.PoE.QualityRecipesCalculator.Network
{
    public class PoeHttpClientOptions
    {
        public static string DefaultUserAgent { get; } = "TehGM's Vendor Recipe Helper";

        [Required]
        public string SessionID { get; set; }
        [Required]
        public string AccountName { get; set; }
        public string UserAgent { get; set; } = DefaultUserAgent;
        public string Realm { get; set; } = PoeRealmNames.PC;
    }
}
