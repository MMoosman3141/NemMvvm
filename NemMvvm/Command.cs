using System;
using System.Windows.Input;

namespace NemMvvm {
  /// <summary>
  /// The Command class is a generic implementation of ICommand, allowing the specification of and Action to be executed, and a function to check if the action can be executed.
  /// </summary>
  public class Command : ICommand {
    private Action _execute;
    private Func<bool> _canExecute;
    private Action<object> _executeParamed;
    private Func<object, bool> _canExecuteParamed;

    /// <summary>
    /// Constructor of the Command object
    /// </summary>
    /// <param name="execute">The simple method called when the command object is invoked.</param>
    public Command(Action execute) {
      _execute = execute;
      _canExecute = null;
      _executeParamed = null;
      _canExecuteParamed = null;
    }

    /// <summary>
    /// Constructor of the Command object
    /// </summary>
    /// <param name="execute">The simple method called when the command object is invoked.</param>
    /// <param name="canExecute">The method which determines if the command can be executed.</param>
    public Command(Action execute, Func<bool> canExecute) {
      _execute = execute;
      _canExecute = canExecute;
      _executeParamed = null;
      _canExecuteParamed = null;
    }

    /// <summary>
    /// Constructor of the Command object
    /// </summary>
    /// <param name="execute">The simple method called when the command object is invoked.</param>
    /// <param name="canExecute">The method which determines if the command can be executed. Taking an object parameter.</param>
    public Command(Action execute, Func<object, bool> canExecute) {
      _execute = execute;
      _canExecute = null;
      _executeParamed = null;
      _canExecuteParamed = canExecute;
    }

    /// <summary>
    /// Constructor of the Command object
    /// </summary>
    /// <param name="execute">The method, taking an object parameter, to call when the command is invoked.</param>
    public Command(Action<object> execute) {
      _execute = null;
      _canExecute = null;
      _executeParamed = execute;
      _canExecuteParamed = null;
    }

    /// <summary>
    /// Constructor of the Command object
    /// </summary>
    /// <param name="execute">The method, taking an object parameter, to call when the command is invoked.</param>
    /// <param name="canExecute">The method which determines if the command can be executed.</param>
    public Command(Action<object> execute, Func<bool> canExecute) {
      _execute = null;
      _canExecute = canExecute;
      _executeParamed = execute;
      _canExecuteParamed = null;
    }

    /// <summary>
    /// Constructor of the Command object
    /// </summary>
    /// <param name="execute">The method, taking an object parameter, to call when the command is invoked.</param>
    /// <param name="canExecute">The method which determines if the command can be executed. Taking an object parameter.</param>
    public Command(Action<object> execute, Func<object, bool> canExecute) {
      _execute = null;
      _canExecute = null;
      _executeParamed = execute;
      _canExecuteParamed = canExecute;
    }

    /// <summary>
    /// Executes the command.  If the parameter is specified, and the Command object was constructed with an action accepting a parameter, the parameter is passed to the action.
    /// </summary>
    /// <param name="parameter">The parameter to pass to the action, if the Command object was constructed with an action taking a parameter.</param>
    public void Execute(object parameter) {
      if(_execute != null) {
        _execute.Invoke();
      } else if(_executeParamed != null) {
        _executeParamed?.Invoke(parameter);
      }
    }

    /// <summary>
    /// Executes the CanExecute method, which determines if the Command object action can be invoked.  If the parameter is specified, and the Command object was constructed with a canExecute method which accepts a parameter, the parameter is passed to the canExecuteMethod.
    /// </summary>
    /// <param name="parameter">The parameter to pass to the action, if the Command object was constructed with a canExucute method wich accecpts a parameter.</param>
    /// <returns>Return true or false based on the canExecute method established at construction time.</returns>
    public bool CanExecute(object parameter) {
      if(_canExecute != null) {
        return _canExecute();
      } else if(_canExecuteParamed != null) {
        return _canExecuteParamed(parameter);
      }

      return true;
    }

    /// <summary>
    /// Method to raise the CanExecuteChanged event which refreshes the CanExecute status of the command object.
    /// </summary>
    public void RaiseCanExecuteChanged() {
      CanExecuteChanged?.Invoke(this, new EventArgs());
    }

    /// <summary>
    /// Event called if the ability to execute the command object changes.
    /// </summary>
    public event EventHandler CanExecuteChanged;
  }
}
