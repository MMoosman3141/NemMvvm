namespace XUnitNemMvvm;
public class TestNotifyPropertyChanged : NotifyPropertyChanged {
  private string _testValue1 = "oldValue";
  private string _testValue2 = "oldValue";
  private string _testValue3 = "oldValue";
  private bool? _actionCondition = null;

  public string Property1 { get; set; } = "oldValue";
  public string TestValue1 {
    get => _testValue1;
    set => SetProperty(ref _testValue1, value);
  }
  public string TestValue2 {
    get => _testValue2;
    set {
      _testValue3 = "notCompleted";

      SetProperty(ref _testValue2, value, () => {
        _testValue3 = "actionComplete";
      }, _actionCondition);
    }
  }


  [Fact]
  public void SetPropertyTest() {
    bool propertyChanged = false;
    PropertyChanged += (s, e) => {
      if (e.PropertyName == nameof(TestValue1))
        propertyChanged = true;
    };

    TestValue1 = "newValue";

    Assert.Equal("newValue", TestValue1);
    Assert.True(propertyChanged);
  }

  [Fact]
  public void SetPropertyActionTest() {
    bool propertyChanged = false;
    PropertyChanged += (s, e) => {
      if (e.PropertyName == nameof(TestValue2))
        propertyChanged = true;
    };

    _actionCondition = null;

    TestValue2 = "newValue";

    Assert.Equal("newValue", TestValue2);
    Assert.Equal("actionComplete", _testValue3);
    Assert.True(propertyChanged);
  }

  [Fact]
  public void SetPropertyActionTestOnTrue() {
    bool propertyChanged = false;
    PropertyChanged += (s, e) => {
      if (e.PropertyName == nameof(TestValue2))
        propertyChanged = true;
    };

    _actionCondition = true;

    TestValue2 = "newValue";

    Assert.Equal("newValue", TestValue2);
    Assert.Equal("actionComplete", _testValue3);
    Assert.True(propertyChanged);
  }

  [Fact]
  public void SetPropertyActionTestOnFalse() {
    bool propertyChanged = false;
    PropertyChanged += (s, e) => {
      if (e.PropertyName == nameof(TestValue2))
        propertyChanged = true;
    };

    _actionCondition = false;

    _testValue2 = "newValue";
    TestValue2 = "newValue";

    Assert.Equal("newValue", TestValue2);
    Assert.Equal("actionComplete", _testValue3);
    Assert.False(propertyChanged);
  }

  [Fact]
  public void SetPropertyActionTestNotCompleted() {
    bool propertyChanged = false;
    PropertyChanged += (s, e) => {
      if (e.PropertyName == nameof(TestValue2))
        propertyChanged = true;
    };

    _actionCondition = false;

    _testValue2 = "oldValue";
    TestValue2 = "newValue";

    Assert.Equal("newValue", TestValue2);
    Assert.Equal("notCompleted", _testValue3);
    Assert.True(propertyChanged);
  }

  [Fact]
  public void TestSetPropertyRaisePropertyChangedName() {
    bool propChanged = false;
    PropertyChanged += (s, e) => {
      if (e.PropertyName == nameof(Property1)) {
        propChanged = true;
      }
    };

    Property1 = "newValue";
    RaisePropertyChanged(nameof(Property1));

    Assert.True(propChanged);
  }
}
