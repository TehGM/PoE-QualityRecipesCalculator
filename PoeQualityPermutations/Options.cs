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
    }
}
