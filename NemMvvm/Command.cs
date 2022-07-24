namespace NemMvvm; 

/// <summary>
/// The Command class is a generic implementation of ICommand, allowing the specification of and Action to be executed, and a function to check if the action can be executed.
/// </summary>
public sealed class Command : BaseCommand<object> {
  /// <summary>
  /// Constructor of the Command object
  /// </summary>
  /// <param name="execute">The simple method called when the command object is invoked.</param>
  public Command(Action execute) : base(execute, null, null, null) { }

  /// <summary>
  /// Constructor of the Command object
  /// </summary>
  /// <param name="execute">The simple method called when the command object is invoked.</param>
  /// <param name="canExecute">The method which determines if the command can be executed.</param>
  public Command(Action execute, Func<bool> canExecute) : base(execute, canExecute, null, null) { }

  /// <summary>
  /// Executes the Action specified in the constructor for execution
  /// </summary>
  public void Execute() {
#pragma warning disable CS0618
    Execute(null);
#pragma warning restore CS0618
  }

  /// <summary>
  /// Executes the Func specified in the constructor to see if execution is valid.
  /// </summary>
  /// <returns></returns>
  public bool CanExecute() {
#pragma warning disable CS0618
    return CanExecute(null);
#pragma warning restore CS0618
  }
} //class
