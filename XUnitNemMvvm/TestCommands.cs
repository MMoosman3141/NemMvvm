using XUnitNemMvvm;

namespace XUnitNemMvvm {
  public class TestCommands : NotifyPropertyChanged {
    private string _testValue1 = "oldValue";
    private string _testValue2 = "oldValue";
    private string _testValue3 = "oldValue";
    private string _testValue4 = "oldValue";
    private bool? _actionCondition = null;
    private bool _command1CanRun;
    private bool _command2CanRun;
    private Command _command1;
    private Command _command2;

    public string TestValue1 {
      get {
        return _testValue1;
      }
      set {
        SetProperty(ref _testValue1, value);
      }
    }

    public string TestValue2 {
      get {
        return _testValue2;
      }
      set {
        _testValue3 = "notCompleted";

        SetProperty(ref _testValue2, value, () => {
          _testValue3 = "actionComplete";
        }, _actionCondition);
      }
    }

    public string TestValue4 {
      get {
        return _testValue4;
      }
      set {
        SetProperty(ref _testValue4, value, new Command[] { Command1, Command2 }, () => {
          _testValue3 = "actionComplete";
        }, _actionCondition);
      }
    }

    public Command Command1 {
      get {
        return _command1;
      }
      set {
        SetProperty(ref _command1, value);
      }
    }
    public Command Command2 {
      get {
        return _command2;
      }
      set {
        SetProperty(ref _command2, value);
      }
    }

    [Fact]
    public void SetPropertyWithCommandObjectsAndActions() {
      bool propertyChanged = false;
      this.PropertyChanged += (s, e) => {
        if (e.PropertyName == nameof(TestValue4))
          propertyChanged = true;
      };

      _command1CanRun = false;
      _command2CanRun = false;

      Command1 = new Command(() => {
        Console.WriteLine("Some stuff");
      }, CanCommand1Run);
      Command2 = new Command(() => {
        Console.WriteLine("Some stuff");
      }, CanCommand2Run);

      bool command1CanExecuteChanged = false;
      Command1.CanExecuteChanged += (s, e) => {
        command1CanExecuteChanged = true;
      };

      bool command2CanExecuteChanged = false;
      Command2.CanExecuteChanged += (s, e) => {
        command2CanExecuteChanged = true;
      };

      if (Command1.CanExecute())
        Command1.Execute();
      if (Command2.CanExecute())
        Command2.Execute();

      _actionCondition = null;

      TestValue4 = "newValue";

      Assert.Equal("newValue", TestValue4);
      Assert.Equal("actionComplete", _testValue3);
      Assert.True(propertyChanged);
      Assert.True(_command1CanRun);
      Assert.True(_command2CanRun);
      Assert.True(command1CanExecuteChanged);
      Assert.True(command2CanExecuteChanged);
    }

    [Fact]
    public void TestCommandsNoParameters() {
      int var1 = 10;
      int var2 = 10;
      bool canRun2 = true;

      Command command1 = null;
      Command command2 = null;

      command1 = new Command(() => {
        var1++;
      });
      command2 = new Command(() => {
        var2++;
        canRun2 = false;
        command2?.RaiseCanExecuteChanged();
      }, () => {
        return canRun2;
      });

      if (command1.CanExecute()) {
        command1.Execute();
      }
      Assert.True(var1 == 11);

      if (command1.CanExecute()) {
        command1.Execute();
      }
      Assert.True(var1 == 12);

      if (command2.CanExecute()) {
        command2.Execute();
      }
      Assert.True(var2 == 11);

      if (command2.CanExecute()) {
        command2.Execute();
      }
      Assert.True(var2 == 11);
    }

    [Fact]
    public void TestCommandsWithExecuteParameters() {
      int var1 = 10;
      int var2 = 10;
      bool canRun2 = true;

      Command<int> command1 = null;
      Command<int> command2 = null;

      command1 = new Command<int>(val => {
        var1 = val;
      });
      command2 = new Command<int>(val => {
        var2 = val;
        canRun2 = false;
        command2?.RaiseCanExecuteChanged();
      }, val => {
        return canRun2;
      });

      if (command1.CanExecute(5)) {
        command1.Execute(5);
      }
      Assert.True(var1 == 5);

      if (command1.CanExecute(7)) {
        command1.Execute(7);
      }
      Assert.True(var1 == 7);

      if (command2.CanExecute(5)) {
        command2.Execute(5);
      }
      Assert.True(var2 == 5);

      if (command2.CanExecute(7)) {
        command2.Execute(7);
      }
      Assert.True(var2 == 5);
    }

    [Fact]
    public void TestCommandExecuteWithInvalidTypeParameter() {
      int value = 10;

      Command<int> command = new(val => value = val);
#pragma warning disable CS0618
      Assert.Throws<ArgumentException>(() => command.Execute("This is something that should not work."));
#pragma warning restore CS0618
    }

    [Fact]
    public void TestTypedActionCommand() {
      string testVal = "fail";
      Command<string> testCommand = new(val => {
        testVal = val;
      });

      CommandSourceForTest cmdSrc = new(testCommand);

      cmdSrc.Command.Execute("success");

      Assert.Equal("success", testVal);
    }

    [Fact]
    public void TestTypedActionCommandWithCanExecute() {
      string testVal = "fail";
      Command<string> testCommand = new(val => { testVal = val; }, val => { testVal = val; return true; });

      CommandSourceForTest commandSrc = new(testCommand, "success 2");
      

      commandSrc.Command.Execute("success");

      Assert.Equal("success", testVal);

      testCommand.RaiseCanExecuteChanged(); //This call triggers the commandSrc to run the canExecute method

      Assert.Equal("success 2", testVal);
    }

    private bool CanCommand1Run() {
      _command1CanRun = true;
      return true;
    }

    private bool CanCommand2Run() {
      _command2CanRun = true;
      return true;
    }
  }
}