namespace Launcher
{
    using Launcher.ViewModels;
    using Launcher.Views;

    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Linq;
    using System.Windows;

    /// <summary>
    ///     <para>Interaction logic for App.xaml</para>
    /// </summary>
    public partial class App : Application
    {
        #region Fields

        protected MasterViewModel m_MasterViewModel;

        #endregion

        #region Methods

        protected void Application_Startup(object sender, StartupEventArgs e)
        {
            m_MasterViewModel = new MasterViewModel(this);

            if (MainWindow == null)
            {
                MainWindow = new MainWindow();
            }

            m_MasterViewModel.Initialise();
            MainWindow.DataContext = m_MasterViewModel;
            MainWindow.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            m_MasterViewModel.Save();
            m_MasterViewModel.Shutdown();
        }

        #endregion
    }
}
