namespace TehGM.PoE.QualityRecipesCalculator
{
    public class CombinationRequirements
    {
        public int TargetQuality { get; init; }
        public int MaxItems { get; init; }

        public CombinationRequirements(int maxItems, int targetQuality)
        {
            this.TargetQuality = targetQuality;
            this.MaxItems = maxItems;
        }

        public CombinationRequirements() : this(60, 40) { }
    }
}
