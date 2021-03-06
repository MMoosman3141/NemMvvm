﻿using System;
using System.Windows.Input;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NemMvvm;

namespace UnitTestNemMvvm {
  [TestClass]
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

    [TestMethod]
    public void SetPropertyWithCommandObjectsAndActions() {
      bool propertyChanged = false;
      this.PropertyChanged += (s, e) => {
        if(e.PropertyName == nameof(TestValue4))
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

      if(Command1.CanExecute())
        Command1.Execute();
      if(Command2.CanExecute())
        Command2.Execute();

      _actionCondition = null;

      TestValue4 = "newValue";

      Assert.AreEqual("newValue", TestValue4);
      Assert.AreEqual("actionComplete", _testValue3);
      Assert.IsTrue(propertyChanged);
      Assert.IsTrue(_command1CanRun);
      Assert.IsTrue(_command2CanRun);
      Assert.IsTrue(command1CanExecuteChanged);
      Assert.IsTrue(command2CanExecuteChanged);
    }

    [TestMethod]
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

      if(command1.CanExecute()) {
        command1.Execute();
      }
      Assert.IsTrue(var1 == 11);

      if(command1.CanExecute()) {
        command1.Execute();
      }
      Assert.IsTrue(var1 == 12);

      if(command2.CanExecute()) {
        command2.Execute();
      }
      Assert.IsTrue(var2 == 11);

      if(command2.CanExecute()) {
        command2.Execute();
      }
      Assert.IsTrue(var2 == 11);
    }

    [TestMethod]
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

      if(command1.CanExecute(5)) {
        command1.Execute(5);
      }
      Assert.IsTrue(var1 == 5);

      if(command1.CanExecute(7)) {
        command1.Execute(7);
      }
      Assert.IsTrue(var1 == 7);

      if(command2.CanExecute(5)) {
        command2.Execute(5);
      }
      Assert.IsTrue(var2 == 5);

      if(command2.CanExecute(7)) {
        command2.Execute(7);
      }
      Assert.IsTrue(var2 == 5);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void TestCommandExecuteWithInvalidTypeParameter() {
      int value = 10;

      Command<int> command = new Command<int>(val => value = val);
#pragma warning disable CS0618
      command.Execute("This is something that should not work.");
#pragma warning restore CS0618
    }

    [TestMethod]
    public void TestTypedActionCommand() {
      string testVal = "fail";
      Command<string> testCommand = new Command<string>(val => {
        testVal = val;
      });

      CommandSourceForTest cmdSrc = new CommandSourceForTest(testCommand);

      cmdSrc.Command.Execute("success");

      Assert.AreEqual("success", testVal);
    }

    [TestMethod]
    public void TestTypedActionCommandWithCanExecute() {
      string testVal = "fail";
      Command<string> testCommand = new Command<string>(val => { testVal = val; }, val => { testVal = val; return true; });

      CommandSourceForTest commandSrc = new CommandSourceForTest(testCommand, "success 2");

      commandSrc.Command.Execute("success");

      Assert.AreEqual("success", testVal);

      testCommand.RaiseCanExecuteChanged(); //This call triggers the commandSrc to run the canExecute method

      Assert.AreEqual("success 2", testVal);
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
