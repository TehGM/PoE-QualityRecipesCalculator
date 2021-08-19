using System;

namespace TehGM.PoE.QualityRecipesCalculator.Settings.Services
{
    public class UserSettingsProvider : IUserSettingsProvider
    {
        public event EventHandler<UserSettings> SettingsChanged;

        private UserSettings _currentSettings;
        public UserSettings CurrentSettings
        {
            get
            {
                if (this._currentSettings == null)
                    this._currentSettings = CreateDefault();
                return this._currentSettings;
            }
        }

        public void Reset()
        {
            this._currentSettings = CreateDefault();
            this.RaiseSettingsChanged();
        }

        public void Update(Action<UserSettings> changes)
        {
            if (changes == null)
                throw new ArgumentNullException(nameof(changes));

            changes(this._currentSettings);
            this.RaiseSettingsChanged();
        }

        private static UserSettings CreateDefault()
        {
            UserSettings result = new UserSettings();
            result.League = "Standard";
            result.Realm = PoeRealmNames.PC;
            return result;
        }

        private void RaiseSettingsChanged()
            => this.SettingsChanged?.Invoke(this, this._currentSettings);
    }
}
