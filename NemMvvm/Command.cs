using System;

namespace NemMvvm {
  /// <summary>
  /// The Command class is a generic implementation of ICommand, allowing the specification of and Action to be executed, and a function to check if the action can be executed.
  /// </summary>
  public sealed class Command : BaseCommand<object, object> {
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
    /// Constructor of the Command object
    /// </summary>
    /// <param name="execute">The simple method called when the command object is invoked.</param>
    /// <param name="canExecute">The method which determines if the command can be executed. Taking an object parameter.</param>
    [Obsolete("Use a typed Command rather than using a Command with object types.  This constructor will be removed in a future version.")]
    public Command(Action execute, Func<object, bool> canExecute) : base(execute, null, null, canExecute) { }

    /// <summary>
    /// Constructor of the Command object
    /// </summary>
    /// <param name="execute">The method, taking an object parameter, to call when the command is invoked.</param>
    [Obsolete("Use a typed Command rather than using a Command with object types.  This constructor will be removed in a future version.")]
    public Command(Action<object> execute) : base(null, null, execute, null) { }

    /// <summary>
    /// Constructor of the Command object
    /// </summary>
    /// <param name="execute">The method, taking an object parameter, to call when the command is invoked.</param>
    /// <param name="canExecute">The method which determines if the command can be executed.</param>
    [Obsolete("Use a typed Command rather than using a Command with object types.  This constructor will be removed in a future version.")]
    public Command(Action<object> execute, Func<bool> canExecute) : base(null, canExecute ,execute, null) { }

    /// <summary>
    /// Constructor of the Command object
    /// </summary>
    /// <param name="execute">The method, taking an object parameter, to call when the command is invoked.</param>
    /// <param name="canExecute">The method which determines if the command can be executed. Taking an object parameter.</param>
    [Obsolete("Use a typed Command rather than using a Command with object types.  This constructor will be removed in a future version.")]
    public Command(Action<object> execute, Func<object, bool> canExecute) : base(null, null, execute, canExecute) { }

    /// <summary>
    /// Executes the Action specified in the constructor for execution
    /// </summary>
    public void Execute() {
#pragma warning disable CS0618
      base.Execute(null);
#pragma warning restore CS0618
    }

    /// <summary>
    /// Executes the Func specified in the constructor to see if execution is valid.
    /// </summary>
    /// <returns></returns>
    public bool CanExecute() {
#pragma warning disable CS0618
      return base.CanExecute(null);
#pragma warning restore CS0618
    }
  } //class
} //namespace
