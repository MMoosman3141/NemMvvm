using System;

namespace NemMvvm {
  /// <summary>
  /// Abstract class used as a base for typed and untyped Command objects.
  /// </summary>
  /// <typeparam name="T">The type of parameter used in the Action for Execute</typeparam>
  /// <typeparam name="R">The type of parameter used in the Func for CanExecute</typeparam>
  public abstract class BaseCommand<T, R> : IFoundationCommand {
#pragma warning disable CS1591
    protected readonly Action _execute;
    protected readonly Func<bool> _canExecute;
    protected readonly Action<T> _executeWithParam;
    protected readonly Func<R, bool> _canExecuteWithParam;
#pragma warning restore CS1591

    /// <summary>
    /// Constructor for BaseCommand objects
    /// </summary>
    /// <param name="execute">The action to run for Commands without parameters.  This should never be set when executeWithParam is set.</param>
    /// <param name="canExecute">The func to run for Commands without parameters.  This should never be set when canExecuteWithParam is set</param>
    /// <param name="executeWithParam">The action to run for Commands with parameters.  This should never be set when execute is set.</param>
    /// <param name="canExecuteWithParam">The func to run for Commands with parameters.  This should never be set when canExecute is set.</param>
    public BaseCommand(Action execute = null, Func<bool> canExecute = null, Action<T> executeWithParam = null, Func<R, bool> canExecuteWithParam = null) {
      if(execute == null && executeWithParam == null) {
        throw new ArgumentNullException(nameof(execute), "No Execute action specified.");
      }
      if(execute != null && executeWithParam != null) { //This should be impossible, but checking just to be safe.
        throw new ArgumentException("Cannot specify both an execute action and an executeWithParam action.", nameof(execute));
      }
      if(canExecute != null && canExecuteWithParam != null) {  //This should be impossible, but checking just to be safe.
        throw new ArgumentException("Cannot specify both an canExecute action and a canExecuteWithParam action.", nameof(canExecute));
      }

      _execute = execute;
      _canExecute = canExecute;
      _executeWithParam = executeWithParam;
      _canExecuteWithParam = canExecuteWithParam;
    }

    /// <summary>
    /// Executes the Action for the Execute method as specified in the constructor.
    /// This method should not be run directly.  Rather, use a Command object with no types set to indicate that the Execute and CanExecute methods should not take parameters.
    /// </summary>
    /// <param name="parameter">A parameter to pass to the Execute method.</param>
    [Obsolete("This method should not be run directly.  If running with no parameters, call Execute(), if running with parameters call Execute with the properly typed parameter.")]
    public void Execute(object parameter) {
      if(parameter == null) {
        _execute?.Invoke();
      } else {
        if(parameter.GetType() != typeof(T)) {
          throw new ArgumentException($"parameter must be of type {typeof(T).Name}");
        }

        T param;
        try {
          param = (T)parameter;
        } catch(InvalidCastException) {
          throw;
        }

        _executeWithParam?.Invoke(param);
      }
    }

    /// <summary>
    /// Executes the Func for the CanExecute method as specified in the constructor.
    /// This method should not be called directly.  Rather, use a Command object without a type set for the CanExecute method or use the explicitly typed Command object if a parameter is desired.
    /// </summary>
    /// <param name="parameter">A parameter to pass to the CanExecute method.</param>
    /// <returns>True or false depending on the func specified in the constructor.</returns>
    [Obsolete("This method should not be run directly.  If running with no parameters, call CanExecute(), if running with parameters call CanExecute with the properly typed parameter.")]
    public bool CanExecute(object parameter) {
      if(parameter == null) {
        return _canExecute?.Invoke() ?? true;
      } else {
        if(parameter.GetType() != typeof(R)) {
          throw new ArgumentException($"parameter must be of type {typeof(T).Name}");
        }

        R param;
        try {
          param = (R)parameter;
        } catch(InvalidCastException) {
          throw;
        }

        return _canExecuteWithParam?.Invoke(param) ?? true;
      }
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
