# NemMvvm

NemMvvm is a library designed to make coding of Mvvm patterns just a little easier.

There are 2 parts:

### NotifyProperyChanged

NotifyPropertyChanged is a class inheriting from INotifyPropertyChanged and INotifyDataErrorInfo. You can use it to avoid the need to create your own method for raising a notification message or error raising.

There are basically 2 methods available from this class:

**SetProperty** - Use this method within a property to set a field value, and raise the notification message for the property, if the property values changes (if setting the value to the same existing value the notification message is not raised.)

**RaisePropertyChanged** - Use this method if you need to raise the notification message for a property in which you can't use the SetProperty method (A common case for this, is if you are setting the field directly rather than using the property).

For example:
```csharp
public class Person : NotifyPropertyChanged {
  private int _changeCount = 0;
  private string _firstName;
  private string _middleName;
  private string _lastName;
  private uint _age;

  private bool _middleSet = false;
  private bool _lastChanged = false;
  private bool _ageNotChanged = false;

  public int ChangedCount {
    get => _changeCount;
  }

  public string FirstName {
    get => _firstName;
    set => SetProperty(ref _firstName, value);
  }

  //It is also possible to run code depending on the out come of the SetProperty
  public string MiddleName {
    get => _middleName;
    //Runs the action regardless of if the SetProperty actually makes a change or not.
    set => SetProperty(ref _middleName, value, () => {
      _middleSet = true;
    });
  }

  public string LastName {
    get => _lastName;
    //Action only runs if _lastName is actually changed.  If it is not changed the action is not run.
    set => SetProperty(ref _lastName, value, () => {
      _lastChanged = true;
    }, true);
  }

  public uint Age {
    get => _age;
    //Action only runs if _age is not changed,  If it is changed the action is not run.
    set => SetProperty(ref _age, value, () => {
      _ageNotChanged = true;
    }, false);
  }

  public Person() {
    PropertyChanged += MyPropertyChanged;
  }

  private void MyPropertyChanged(object sender, PropertyChangedEventArgs e) {
    Task.Run(() => {
      //Since properties can't be passed by reference we have to call the RaisePropertyChanged manually when incrementing via Interlocked.Increment
      Interlocked.Increment(ref _changeCount);
      RaisePropertyChanged(nameof(ChangedCount));
      //Slower, but functional alternative
      //RaisePropertyChanged(() => ChangedCount);
    });
  }
}
```

For error handling through the INotifyDataErrorInfo interface implementation we would simply add validator methods that are referenced in the SetProperty methods.  This will all for the HasErrors property and GetErrors methods to be properly handled.
```csharp
public class MainWindowVM : NotifyPropertyChanged {
    private int _item1; //Must be odd
    private string _item2; //Must be a number
    private string _item3; //Must be a SSN
    private string _item4; //Must be a phone number in the format (###) ###-####
    private Command _runCmd;

    public int Item1 {
      get => _item1;
      set => SetProperty(ref _item1, value, Item1Validator);
    }
    public string Item2 {
      get => _item2;
      set => SetProperty(ref _item2, value, Item2Validator);
    }
    public string Item3 {
      get => _item3;
      set => SetProperty(ref _item3, value, Item3Validator);
    }
    public string Item4 {
      get => _item4;
      set => SetProperty(ref _item4, value, Item4Validator);
    }
    public Command RunCmd {
      get => _runCmd;
      private set => SetProperty(ref _runCmd, value);
    }

    public MainWindowVM() {
      RunCmd = new Command(RunAction, CanRun);

      PropertyChanged += MainWindowVM_PropertyChanged;
    }

    private void MainWindowVM_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
      RunCmd.RaiseCanExecuteChanged();
    }

    private void RunAction() {
      MessageBox.Show("Hey you hit the button.");
    }

    private bool CanRun() {
      if (string.IsNullOrWhiteSpace(Item2)) {
        return false;
      }
      if (string.IsNullOrWhiteSpace(Item3)) {
        return false;
      }
      if (string.IsNullOrWhiteSpace(Item4)) {
        return false;
      }

      return !HasErrors;
    }

    private ICollection Item1Validator(int arg) {
      List<string> errors = new List<string>();

      if(arg % 2 == 0) {
        errors.Add("Item 1 must be an odd number.");
      }

      return errors;
    }

    private ICollection Item2Validator(string arg) {
      List<string> errors = new List<string>();

      if(!decimal.TryParse(arg, out decimal _)) {
        errors.Add("Item 2 must be a number.");
      }

      return errors;
    }

    private ICollection Item3Validator(string arg) {
      List<string> errors = new List<string>();

      Regex validRgx = new Regex(@"[1-9]{3}-[1-9]{2}-[1-9]{4}");

      if (!validRgx.IsMatch(arg)) {
        errors.Add("Item 2 must be a social security number in the format ###-##-####");
      }

      return errors;
    }

    private ICollection Item4Validator(string arg) {
      List<string> errors = new List<string>();

      Regex validRgx = new Regex(@"\(\d{3}\) \d{3}-\d{4}");

      if (!validRgx.IsMatch(arg)) {
        errors.Add("Item 2 must be a telephone number in the format (###) ###-####");
      }

      return errors;
    }
  }
```
This kind code can then then be used to provide extra information for controls in WPF to display errors.
```xaml
<Window x:Class="Test_NemMvvm.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
        xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
        xmlns:local="clr-namespace:Test_NemMvvm"
        mc:Ignorable="d"
        Title="MainWindow" Height="450" Width="800">
  <Window.Resources>
    <local:MainWindowVM x:Key="vm" />

    <Style TargetType="TextBox">
      <Style.Triggers>
        <Trigger Property="Validation.HasError" Value="True">
          <Setter Property="ToolTip" Value="{Binding RelativeSource={RelativeSource Self}, Path=(Validation.Errors)/ErrorContent}" />
          <Setter Property="BorderBrush" Value="Red" />
        </Trigger>
      </Style.Triggers>
    </Style>
  </Window.Resources>
  <Grid DataContext="{StaticResource vm}">
    <StackPanel>
      <DockPanel>
        <Label Content="Item 1"/>
        <TextBox Text="{Binding Item1}" />
      </DockPanel>
      <DockPanel>
        <Label Content="Item 2"/>
        <TextBox Text="{Binding Item2}" />
      </DockPanel>
      <DockPanel>
        <Label Content="Item 3"/>
        <TextBox Text="{Binding Item3}" />
      </DockPanel>
      <DockPanel>
        <Label Content="Item 4"/>
        <TextBox Text="{Binding Item4}" />
      </DockPanel>
      <Button Content="Click Me!" Command="{Binding RunCmd}" />
    </StackPanel>
  </Grid>
</Window>
```

### Command object

The Command object is a generic implementation of ICommand allowing you to specify an action to take, and a method to check if the action can be executed. It also allows you to specify Commands with the SetProperty handling described above to allow the Command to be checked if the property changes.

For example:
```csharp
public class CommandExecution : NotifyPropertyChanged {
  private Command _browseCmd;
  private Command<string> _setTextCmd;
  private Command<string> _setTextIfValidCmd;
  private bool _allowBrowse = false;
  private string _text;
  private string _validNum;

  public Command BrowseCmd {
    get => _browseCmd;
    set => SetProperty(ref _browseCmd, value);
  }

  public Command<string> SetTextCmd {
    get => _setTextCmd;
    set => SetProperty(ref _setTextCmd, value);
  }

  public Command<string> SetTextIfValidCmd {
    get => _setTextIfValidCmd;
    set => SetProperty(ref _setTextIfValidCmd, value);
  }

  public bool AllowBrowse {
    get => _allowBrowse;
    set => SetProperty(ref _allowBrowse, value, BrowseCmd);
  }

  public string Text {
    get => _text;
    set => SetProperty(ref _text, value);
  }

  public string ValidNum {
    get => _validNum;
    set => SetProperty(ref _validNum, value, SetTextIfValidCmd);
  }

  public CommandExecution() {
    BrowseCmd = new Command(BrowseForFile, CanBrowseForFile);
    SetTextCmd = new Command<string>(SetText);
    SetTextIfValidCmd = new Command<string>(SetText, CanSetText);
  }

  public void BrowseForFile() {
    OpenFileDialog dlgOpen = new OpenFileDialog();

    if(dlgOpen.ShowDialog() == true) {
      //Do stuff
    }
  }

  public bool CanBrowseForFile() {
    return AllowBrowse;
  }

  public void SetText(string text) {
    Text = text;
  }

  public bool CanSetText(string num) {
    if(!int.TryParse(num, out result)) {
      return false;
    }

    return int.Parse(num) >= ValidNum;
  }

}
```
#### Version History
1.2.1
* Introduced typed Command objects.
* Note:  Doubly typed Command objects are possible in this version but are likely to be problematic at best, and the singly typed Command object has a bug.  The work around for the bug is to use the doubly typed object specifying the same type twice.

1.3.0
* Fixed error introduced in earlier which prevents the use of lambda expressions in RaisePropertyChanged not allowing lambdas in RaisePropertyChanged.
* Improved handling in SetProperty methods which performs equality check between old and new values.
* Added a new version of the SetPropertyMethod which takes a string value of the property name along with a params array of Command objects.
* Removed NemAnkh.ico from installing as part of the NuGet package.
* Removed doubly typed Command object.  This just was causing confusion since the vast majority of ICommandSource objects cannot take more than 1 parameter as a result there was a bug in the singly typed Command.  This is a breaking change for anyone who was making use of the double typed Command.

1.3.1
* Converted to .NET Standard 2.0 library.

1.3.2
* Added INotifyDataErrorInfo handling, allowing SetProperty methods to specify a validator function
