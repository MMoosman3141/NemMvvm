using System;
using System.Windows.Input;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NemMvvm;

namespace UnitTestNemMvvm {
  [TestClass]
  public class UnitTest1 : NotifyPropertyChanged {
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
    public void SetPropertyTest() {
      bool propertyChanged = false;
      this.PropertyChanged += (s, e) => {
        if(e.PropertyName == nameof(TestValue1))
          propertyChanged = true;
      };

      TestValue1 = "newValue";

      Assert.AreEqual("newValue", TestValue1);
      Assert.IsTrue(propertyChanged);
    }

    [TestMethod]
    public void SetPropertyActionTest() {
      bool propertyChanged = false;
      this.PropertyChanged += (s, e) => {
        if(e.PropertyName == nameof(TestValue2))
          propertyChanged = true;
      };

      _actionCondition = null;

      TestValue2 = "newValue";

      Assert.AreEqual("newValue", TestValue2);
      Assert.AreEqual("actionComplete", _testValue3);
      Assert.IsTrue(propertyChanged);
    }

    [TestMethod]
    public void SetPropertyActionTestOnTrue() {
      bool propertyChanged = false;
      this.PropertyChanged += (s, e) => {
        if(e.PropertyName == nameof(TestValue2))
          propertyChanged = true;
      };

      _actionCondition = true;

      TestValue2 = "newValue";

      Assert.AreEqual("newValue", TestValue2);
      Assert.AreEqual("actionComplete", _testValue3);
      Assert.IsTrue(propertyChanged);
    }

    [TestMethod]
    public void SetPropertyActionTestOnFalse() {
      bool propertyChanged = false;
      this.PropertyChanged += (s, e) => {
        if(e.PropertyName == nameof(TestValue2))
          propertyChanged = true;
      };

      _actionCondition = false;

      _testValue2 = "newValue";
      TestValue2 = "newValue";

      Assert.AreEqual("newValue", TestValue2);
      Assert.AreEqual("actionComplete", _testValue3);
      Assert.IsFalse(propertyChanged);
    }
    [TestMethod]
    public void SetPropertyActionTestNotCompleted() {
      bool propertyChanged = false;
      this.PropertyChanged += (s, e) => {
        if(e.PropertyName == nameof(TestValue2))
          propertyChanged = true;
      };

      _actionCondition = false;

      _testValue2 = "oldValue";
      TestValue2 = "newValue";

      Assert.AreEqual("newValue", TestValue2);
      Assert.AreEqual("notCompleted", _testValue3);
      Assert.IsTrue(propertyChanged);
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
      }, () => {
        return canRun2;
      });

      if(command1.CanExecute()) {
        command1.Execute(5);
      }
      Assert.IsTrue(var1 == 5);

      if(command1.CanExecute()) {
        command1.Execute(7);
      }
      Assert.IsTrue(var1 == 7);

      if(command2.CanExecute()) {
        command2.Execute(5);
      }
      Assert.IsTrue(var2 == 5);

      if(command2.CanExecute()) {
        command2.Execute(7);
      }
      Assert.IsTrue(var2 == 5);
    }

    [TestMethod]
    public void TestCommandsWithExecuteAndCanExecuteParameters() {
      int var1 = 10;
      int var2 = 10;
      bool canRun2 = true;

      Command<int, bool> command1 = null;
      Command<int, bool> command2 = null;

      //There's no good reason to specify a type for the canExecute and then not have a func there, but it is possible.
      command1 = new Command<int, bool>(val => {
        var1 = val;
      });
      command2 = new Command<int, bool>(val => {
        var2 = val;
        canRun2 = false;
        command2?.RaiseCanExecuteChanged();
      }, val => {
        return val == canRun2;
      });

      if(command1.CanExecute(true)) { //A bool has to be specified even though there is no method to use it.
        command1.Execute(5);
      }
      Assert.IsTrue(var1 == 5);

      if(command1.CanExecute(true)) { //A bool has to be specified even though there is no method to use it.
        command1.Execute(7);
      }
      Assert.IsTrue(var1 == 7);

      if(command2.CanExecute(true)) {
        command2.Execute(5);
      }
      Assert.IsTrue(var2 == 5);

      if(command2.CanExecute(true)) {
        command2.Execute(7);
      }
      Assert.IsTrue(var2 == 5);

      if(command2.CanExecute(false)) {
        command2.Execute(9);
      }
      Assert.IsTrue(var2 == 9);
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
    [ExpectedException(typeof(ArgumentException))]
    public void TestCommandCanExectureWithInvalidTypeParamter() {
      int value = 10;
      bool canRun = true;

      Command<int, bool> command = new Command<int, bool>(val => value = val, check => canRun);

#pragma warning disable CS0618
      if(command.CanExecute("This is wrong")) {
        command.Execute(5);
      }
#pragma warning restore CS0618

    }

    [TestMethod]
    public void TestCommandImproperConstructors() {
      try {
        Command<string> command1 = new Command<string>(null);
      } catch(ArgumentNullException) {
        Assert.IsTrue(true);
      }

      try {
        Command<string, string> command2 = new Command<string, string>(null, null);
      } catch(ArgumentNullException) {
        Assert.IsTrue(true);
      }
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
