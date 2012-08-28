namespace Launcher.Common.Modules
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;

    public interface IGameModule : INotifyPropertyChanged
    {
        #region Properties

        ICollection<String> Characters { get; }

        string DataPath { get; }

        string Game { get; }

        string GamePath { get; }

        string InstallPath { get; }

        string SavesLocation { get; }

        #endregion

        #region Methods

        bool LoadCharacters();

        #endregion
    }
}
