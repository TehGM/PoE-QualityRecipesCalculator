using CommandLine;

namespace TehGM.PoE.QualityRecipesCalculator
{
    public class Options
    {
        [Option('s', "sessionid", Required = true, HelpText = "PoE Session ID")]
        public string SessionID { get; set; }
        [Option('a', "account", Required = true, HelpText = "PoE Account name")]
        public string AccountName { get; set; }
        [Option('l', "league", Required = false, HelpText = "League name", Default = "Standard")]
        public string League { get; set; }
        [Option('r', "realm", Required = false, HelpText = "Platform Realm", Default = "pc")]
        public string Realm { get; set; }

        [Option("only-exact", Required = false, HelpText = "Makes app output only combinations that result in exactly 40 quality", Default = false)]
        public bool OnlyExact { get; set; }
        [Option("show-invalid", Required = false, HelpText = "Makes app output combinations that are not enough for vendor recipe", Default = false)]
        public bool ShowInvalid { get; set; }
        [Option("item-names", Required = false, HelpText = "Makes app output item names along with qualities", Default = false)]
        public bool ShowItemNames { get; set; }
        [Option("large", Required = false, HelpText = "Amount of items that will be considered a large batch and produce warning", Default = (uint)20)]
        public uint LargeBatchSize { get; set; }
        [Option("remove-used", Required = false, HelpText = "Qualities used in prior combinations are not re-used in future combinations", Default = false)]
        public bool RemoveUsed { get; set; }

        [Option("debug", Required = false, HelpText = "Enables debug output", Default = false)]
        public bool Debug { get; set; }
    }
}
