namespace TehGM.PoE.QualityRecipesCalculator
{
    public class ProcessStatus
    {
        public string MainText { get; set; }
        public string SubText { get; set; }
        public int CurrentProgress { get; set; }
        public int MaxProgress { get; set; }

        public bool HasProgressBar => this.MaxProgress != 0;
        public double ProgressPercentage => this.HasProgressBar ? (double)this.CurrentProgress / (double)this.MaxProgress : 1;

        public ProcessStatus(string mainText, string subText, int currentProgress, int progressTotal)
        {
            this.MainText = mainText;
            this.SubText = subText;
            this.CurrentProgress = currentProgress;
            this.MaxProgress = progressTotal;
        }

        public ProcessStatus(string mainText)
            : this(mainText, null, 0, 0) { }
    }
}
