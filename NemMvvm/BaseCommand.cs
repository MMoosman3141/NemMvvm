using System;

namespace NemMvvm {
  /// <summary>
  /// Abstract class used as a base for typed and untyped Command objects.
  /// </summary>
  /// <typeparam name="T1">The type of parameter used in the Action for Execute</typeparam>
  /// <typeparam name="T2">The type of parameter used in the Func for CanExecute</typeparam>
  public abstract class BaseCommand<T1, T2> : IFoundationCommand {
#pragma warning disable CS1591
    protected Action ExecuteAction { get; private set; }
    protected Func<bool> CanExecuteFunc { get; private set; }
    protected Action<T1> ExecuteWithParamAction { get; private set; }
    protected Func<T2, bool> CanExecuteWithParamFunc { get; private set; }
#pragma warning restore CS1591

    /// <summary>
    /// Constructor for BaseCommand objects
    /// </summary>
    /// <param name="execute">The action to run for Commands without parameters.  This should never be set when executeWithParam is set.</param>
    /// <param name="canExecute">The func to run for Commands without parameters.  This should never be set when canExecuteWithParam is set</param>
    /// <param name="executeWithParam">The action to run for Commands with parameters.  This should never be set when execute is set.</param>
    /// <param name="canExecuteWithParam">The func to run for Commands with parameters.  This should never be set when canExecute is set.</param>
    protected BaseCommand(Action execute, Func<bool> canExecute, Action<T1> executeWithParam, Func<T2, bool> canExecuteWithParam) {
      if(execute == null && executeWithParam == null) {
        throw new ArgumentNullException(nameof(execute), "No Execute action specified.");
      }
      if(execute != null && executeWithParam != null) { //This should be impossible, but checking just to be safe.
        throw new ArgumentException("Cannot specify both an execute action and an executeWithParam action.", nameof(execute));
      }
      if(canExecute != null && canExecuteWithParam != null) {  //This should be impossible, but checking just to be safe.
        throw new ArgumentException("Cannot specify both an canExecute action and a canExecuteWithParam action.", nameof(canExecute));
      }

      ExecuteAction = execute;
      CanExecuteFunc = canExecute;
      ExecuteWithParamAction = executeWithParam;
      CanExecuteWithParamFunc = canExecuteWithParam;
    }

    /// <summary>
    /// Executes the Action for the Execute method as specified in the constructor.
    /// This method should not be run directly.  Rather, use a Command object with no types set to indicate that the Execute and CanExecute methods should not take parameters.
    /// </summary>
    /// <param name="parameter">A parameter to pass to the Execute method.</param>
    [Obsolete("This method should not be run directly.  If running with no parameters, call Execute(), if running with parameters call Execute with the properly typed parameter.")]
    public void Execute(object parameter) {
      if(parameter == null) {
        ExecuteAction?.Invoke();
      } else {
        if(parameter.GetType() != typeof(T1)) {
          throw new ArgumentException($"parameter must be of type {typeof(T1).Name}");
        }

        T1 param;
        try {
          param = (T1)parameter;
        } catch(InvalidCastException) {
          throw;
        }

        ExecuteWithParamAction?.Invoke(param);
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
        return CanExecuteFunc?.Invoke() ?? true;
      } else {
        if(parameter.GetType() != typeof(T2)) {
          throw new ArgumentException($"parameter must be of type {typeof(T1).Name}");
        }

        T2 param;
        try {
          param = (T2)parameter;
        } catch(InvalidCastException) {
          throw;
        }

        return CanExecuteWithParamFunc?.Invoke(param) ?? true;
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
