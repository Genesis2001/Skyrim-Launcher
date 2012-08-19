namespace Launcher.Commands
{
    using Launcher.ViewModels;

    using System;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Input;

    public class LaunchGameCommand : ICommand
    {
        #region Constructor(s)

        public LaunchGameCommand(MasterViewModel viewModel)
        {
            m_ViewModel = viewModel;
        }

        #endregion

        #region Fields

        protected MasterViewModel m_ViewModel;

        #endregion

        #region ICommand Members

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            return !String.IsNullOrEmpty(m_ViewModel.Character.Current);
        }

        public void Execute(object parameter)
        {
            MessageBox.Show("hello world", "Launching Game", MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }

        #endregion
    }
}
