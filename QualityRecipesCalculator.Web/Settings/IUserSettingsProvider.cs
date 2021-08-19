using System;

namespace TehGM.PoE.QualityRecipesCalculator.Settings
{
    public interface IUserSettingsProvider
    {
        UserSettings CurrentSettings { get; }

        void Update(Action<UserSettings> changes);
        void Reset();

        event EventHandler<UserSettings> SettingsChanged;
    }
}
