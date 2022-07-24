namespace NemMvvm; 

/// <summary>
/// A generic implentation of the ICommand interface.
/// </summary>
/// <typeparam name="T">For Commands with parameters, the type of parameter used in the Execute methods.</typeparam>
public sealed class Command<T> : BaseCommand<T> {
  /// <summary>
  /// Constructor of the Command object
  /// </summary>
  /// <param name="execute">The simple method called when the command object is invoked.</param>
  /// <param name="canExecute">Optional: The method which determines if the command can be executed.</param>
  public Command(Action<T> execute, Func<T, bool> canExecute = null) : base(null, null, execute, canExecute) { }

  /// <summary>
  /// Executes the command.  If the parameter is specified, and the Command object was constructed with an action accepting a parameter, the parameter is passed to the action.
  /// </summary>
  /// <param name="parameter">The parameter to pass to the action.</param>
  public void Execute(T parameter) {
    if (ExecuteWithParamAction == null) {
      throw new ArgumentException("This Command object does not make use of a Execute method that takes a parameter.");
    }
    if (parameter == null) {
      throw new ArgumentNullException(nameof(parameter), "null not allowed in Typed Command");
    }

#pragma warning disable CS0618
    base.Execute(parameter);
#pragma warning restore CS0618
  }

  /// <summary>
  /// Executes the CanExecute method, which determines if the Command object action can be invoked.  If the parameter is specified, and the Command object was constructed with a canExecute method which accepts a parameter, the parameter is passed to the canExecuteMethod.
  /// </summary>
  /// <returns>Return true or false based on the canExecute method established at construction time.</returns>
  public bool CanExecute(T parameter) {
    if (parameter == null) {
      throw new ArgumentNullException(nameof(parameter), "null not allowed in Typed Command");
    }
#pragma warning disable CS0618
    return base.CanExecute(parameter);
#pragma warning restore CS0618
  }
} //class
