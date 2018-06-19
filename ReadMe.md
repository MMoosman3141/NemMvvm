# NemMvvm

NemMvvm is a library designed to make coding of Mvvm patterns just a little easier.

There are 2 parts:

### NotifyProperyChanged

NotifyPropertyChanged is a class inheriting from INotifyPropertyChanged. You can use it to avoid the need to create your own method for raising a notification message.

There are basically 2 methods available from this class:

**SetProperty** - Use this method within a property to set a field value, and raise the notification message for the property, if the property values changes (if setting the value to the same existing value the notification message is not raised.)

**RaisePropertyChanged** - Use this method if you need to raise the notification message for a property outside of the properties Set method itself.

For example:
```csharp
public class Person : NotifyPropertyChanged {
  private string _firstName;
  private string _middleName;
  private string _lastName;
  private uint _age;

  private bool _middleSet = false;
  private bool _lastChanged = false;
  private bool _ageNotChanged = false;

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
      _lastChanged = true;_
    }, true);
  }

  public uint Age {
    get => _age;
    //Action only runs if _age is not changed,  If it is changed the action is not run.
    set => SetProperty(ref _age, value, () => {
      _ageNotChanged = true;
    }, false);
  }
}
```
### Command object

The Command object is a generic implementation of ICommand allowing you to specify an action to take, and a method to check if the action can be executed. It also allows you to specify Commands with the SetProperty handling described above to allow the Command to be checked if the property changes.

For example:
```csharp
public class CommandExecution : NotifyPropertyChanged {
  private Command _browseCmd;
  private Command<string> _setTextCmd;
  private Command<string, int> _setTextIfValidCmd;
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

  public Command<string, int> SetTextIfValidCmd {
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
    SetTextIfValidCmd = new Command<string, int>(SetText, CanSetText);
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

  public bool CanSetText(int num) {
    return num >= ValidNum;
  }

}
```