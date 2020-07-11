using Microsoft.VisualStudio.TestTools.UnitTesting;
using NemMvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestNemMvvm {
  [TestClass]
  public class TestNotifyPropertyChanged : NotifyPropertyChanged {
    private string _testValue1 = "oldValue";
    private string _testValue2 = "oldValue";
    private string _testValue3 = "oldValue";
    private bool? _actionCondition = null;

    public string Property1 { get; set; } = "oldValue";
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
    public void TestSetFieldRaisePropertyChangedLambda() {
      bool propChanged = false;
      this.PropertyChanged += (s, e) => {
        if(e.PropertyName == nameof(Property1)) {
          propChanged = true;
        }
      };

      Property1 = "newValue";
#pragma warning disable CS0618
      RaisePropertyChanged(() => Property1);
#pragma warning restore

      Assert.IsTrue(propChanged);
    }

    [TestMethod]
    public void TestSetPropertyRaisePropertyChangedName() {
      bool propChanged = false;
      this.PropertyChanged += (s, e) => {
        if(e.PropertyName == nameof(Property1)) {
          propChanged = true;
        }
      };

      Property1 = "newValue";
      RaisePropertyChanged(nameof(Property1));

      Assert.IsTrue(propChanged);
    }
  }
}
