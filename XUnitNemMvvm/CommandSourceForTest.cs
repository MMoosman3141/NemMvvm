using System.Windows;
using System.Windows.Input;

namespace XUnitNemMvvm;

public class CommandSourceForTest : ICommandSource {
  private ICommand _cmd;

  public ICommand Command {
    get => _cmd;
    private set {
      _cmd = value;
      _cmd.CanExecuteChanged += Cmd_CanExecuteChanged;
    }
  }

  public IInputElement CommandTarget => null;

  private void Cmd_CanExecuteChanged(object sender, EventArgs e) {
    _cmd?.CanExecute(CommandParameter);
  }

  public object CommandParameter { get; private set; }

  public CommandSourceForTest(ICommand command, object commandParameter = null) {
    Command = command;

    CommandParameter = commandParameter;
  }

}
