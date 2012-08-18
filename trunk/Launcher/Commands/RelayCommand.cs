namespace Launcher.Commands
{
    using System;
    using System.Windows.Input;

    public class RelayCommand : ICommand
    {
        #region Constructor(s)

        public RelayCommand()
        {
        }

        public RelayCommand(Action<object> execute, Predicate<object> canExecute)
        {
            m_Execute = execute;
            m_CanExecute = canExecute;
        }

        #endregion

        #region Fields

        protected Predicate<object> m_CanExecute;
        protected Action<object> m_Execute;

        #endregion

        #region ICommand Members

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            if (m_CanExecute == null)
            {
                return true;
            }

            return m_CanExecute(parameter);
        }

        public void Execute(object parameter)
        {
            if (m_Execute != null)
            {
                m_Execute(parameter);
            }
        }

        #endregion
    }
}
