using CommandLine;

namespace TehGM.PoeQualityPermutations
{
    class Options
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
        [Option("only-valid", Required = false, HelpText = "Makes app output only combinations that result in at least 40 quality", Default = false)]
        public bool OnlyValid { get; set; }
        [Option("item-names", Required = false, HelpText = "Makes app output item names along with qualities", Default = false)]
        public bool ShowItemNames { get; set; }
    }
}
