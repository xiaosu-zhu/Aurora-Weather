using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Core;
using Windows.UI.Xaml;

namespace Com.Aurora.Shared.MVVM
{
    //public class DelegateCommand<T> : ICommand
    //{
    //    private readonly Action<T> executeAction;
    //    private readonly Func<T, bool> canExecuteAction;
    //    public event EventHandler CanExecuteChanged;

    //    public DelegateCommand(Action<T> executeAction)
    //        : this(executeAction, null)
    //    {
    //    }

    //    public DelegateCommand(Action<T> executeAction,
    //        Func<T, bool> canExecuteAction)
    //    {
    //        this.executeAction = executeAction;
    //        this.canExecuteAction = canExecuteAction;
    //    }
    //    public bool CanExecute(object parameter)
    //    {
    //        if (canExecuteAction != null)
    //        {
    //            return canExecuteAction((T)parameter);
    //        }
    //        return true;
    //    }

    //    public void Execute(object parameter)
    //    {
    //        if (CanExecute(parameter))
    //        {
    //            executeAction((T)parameter);
    //        }
    //    }
    //}

    /// <summary>
    /// An <see cref="ICommand"/> whose delegates can be attached for <see cref="Execute"/> and <see cref="CanExecute"/>.
    /// It also implements the <see cref="IActiveAware"/> interface, which is
    /// useful when registering this command in a <see cref="CompositeCommand"/>
    /// that monitors command's activity.
    /// </summary>
    /// <typeparam name="T">Parameter type.</typeparam>
    public partial class DelegateCommand<T> : ICommand
    {
        #region Fields 

        /// <summary>
        /// The method executed when CanExecute has changed
        /// </summary>
        private readonly Func<T, bool> canExecuteMethod = null;

        /// <summary>
        /// The method executed when this instance executes.
        /// </summary>
        private readonly Action<T> executeMethod = null;

        #endregion Fields 

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateCommand{T}"/> class.
        /// </summary>
        /// <param name="executeMethod">Delegate to execute when Execute is called on the command.  This can be null to just hook up a CanExecute delegate.</param>
        /// <param name="canExecuteMethod">Delegate to execute when CanExecute is called on the command.  This can be null.</param>
        /// <exception cref="ArgumentNullException">When both <paramref name="executeMethod"/> and <paramref name="canExecuteMethod"/> ar <see langword="null"/>.</exception>
        public DelegateCommand(Action<T> executeMethod, Func<T, bool> canExecuteMethod)
        {
            // TODO: Replace the message with a variable
            if (executeMethod == null && canExecuteMethod == null)
            {
                throw new ArgumentNullException("executeMethod", "Delegate Command cannot be null");
            }

            this.executeMethod = executeMethod;
            this.canExecuteMethod = canExecuteMethod;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateCommand{T}"/> class.
        /// </summary>
        /// <param name="executeMethod">Delegate to execute when Execute is called on the command.  This can be null to just hook up a CanExecute delegate.</param>
        /// <remarks><seealso cref="CanExecute"/> will always return true.</remarks>
        public DelegateCommand(Action<T> executeMethod)
            : this(executeMethod, null)
        {
        }

        #region Events 

        /// <summary>
        /// Occurs when changes occur that affect whether or not the command should execute.
        /// </summary>
        public event EventHandler CanExecuteChanged;

        #endregion Events 

        #region Methods 

        /// <summary>
        /// Defines the method that determines whether the command can execute in its current state.
        /// </summary>
        /// <param name="parameter">Data used by the command. If the command does not require data to be passed, this object can be set to <see langword="null" />.</param>
        /// <returns>
        /// <see langword="true" /> if this command can be executed; otherwise, <see langword="false" />.
        /// </returns>
        public bool CanExecute(T parameter)
        {
            if (this.canExecuteMethod == null)
            {
                return true;
            }

            return this.canExecuteMethod(parameter);
        }

        /// <summary>
        /// Defines the method to be called when the command is invoked.
        /// </summary>
        /// <param name="parameter">Data used by the command. If the command does not require data to be passed, this object can be set to <see langword="null" />.</param>
        public void Execute(T parameter)
        {
            if (this.executeMethod == null)
            {
                return;
            }

            this.executeMethod(parameter);
        }

        /// <summary>
        /// Raises <see cref="CanExecuteChanged"/> on the UI thread so every command invoker
        /// can requery to check if the command can execute.
        /// <remarks>Note that this will trigger the execution of <see cref="CanExecute"/> once for each invoker.</remarks>
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate")]
        public void RaiseCanExecuteChanged()
        {
            this.OnCanExecuteChanged();
        }

        /// <summary>
        /// Defines the method that determines whether the command can execute in its current state.
        /// </summary>
        /// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
        /// <returns>
        /// true if this command can be executed; otherwise, false.
        /// </returns>
        bool ICommand.CanExecute(object parameter)
        {
            return this.CanExecute((T)parameter);
        }

        /// <summary>
        /// Defines the method to be called when the command is invoked.
        /// </summary>
        /// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
        void ICommand.Execute(object parameter)
        {
            this.Execute((T)parameter);
        }

        /// <summary>
        /// Raises <see cref="CanExecuteChanged"/> on the UI thread so every command invoker can requery to check if the command can execute.
        /// </summary>
        protected virtual async void OnCanExecuteChanged()
        {
            CoreDispatcher dispatcher = null;
            if (Window.Current != null)
            {
                dispatcher = Window.Current.Dispatcher;
            }

            EventHandler canExecuteChangedHandler = this.CanExecuteChanged;
            if (canExecuteChangedHandler != null)
            {
                if (dispatcher != null && !dispatcher.HasThreadAccess)
                {
                    await dispatcher.RunAsync(CoreDispatcherPriority.Normal, OnCanExecuteChanged);
                }
                else
                {
                    canExecuteChangedHandler(this, EventArgs.Empty);
                }
            }
        }

        #endregion Methods 
    }
}
#if shangmiandefaille
public class DelegateCommand : ICommand
{
    Func<object, bool> canExecuteAction;
    Action<object> executeAction;
    public event EventHandler CanExecuteChanged;

    public DelegateCommand(Action<object> executeAction)
        : this(executeAction, null)
    {
    }

    public DelegateCommand(Action<object> executeAction,
        Func<object, bool> canExecuteAction)
    {
        this.executeAction = executeAction;
        this.canExecuteAction = canExecuteAction;
    }
    public bool CanExecute(object parameter)
    {
        if (canExecuteAction != null)
        {
            return canExecuteAction(parameter);
        }
        return true;
    }

    public void Execute(object parameter)
    {
        if (CanExecute(parameter))
        {
            executeAction(parameter);
        }
    }
}
#endif