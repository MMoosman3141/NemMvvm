#NemMvvm

NemMvvm is a library designed to make coding of Mvvm patterns just a little easier.

There are 2 parts:

###NotifyProperyChanged

NotifyPropertyChanged is a class inheriting from INotifyPropertyChanged. You can use it to avoid the need to create your own method for raising a notification message.

There are basically 2 methods available from this class:

**SetProperty** - Use this method within a property to set a field value, and raise the notification message for the property, if the property values changes (if setting the value to the same existing value the notification message is not raised.)

**RaisePropertyChanged** - Use this method if you need to raise the notification message for a property outside of the properties Set method itself.

For example:

    public class Person : NotifyPropertyChanged {
       private string _firstName;
       private string _middleName;
       private string _lastName;
       private uint _age;
    
       public string FirstName {
         get {
           return _firstName;
         }
         set {
           SetProperty(ref _firstName, value);
         }
       }
    
       public string MiddleName {
         get {
           return _middleName;
         }
         set {
           SetProperty(ref _middleName, value);
         }
       }
    
       public string LastName {
         get {
           return _lastName;
         }
         set {
           SetProperty(ref _lastName, value);
         }
       }
    
       public uint Age {
         get {
           return _age;
         }
         set {
           SetProperty(ref _age, value);
         }
       }
    }

###Command object

The Command object is a generic implementation of ICommand allowing you to specify an action to take, and a method to check if the action can be executed. It also allows you to specify Commands with the SetProperty handling described above to allow the Command to be checked if the property changes.

For example:

    public class CommandExecution : NotifyPropertyChanged {
      private Command _browseCommand;
      private bool _allowBrowse = false;
    
      public Command BrowseCommand {
        get {
          return _browseCommand;
        }
        set {
          SetProperty(ref _browseCommand, value);
       }
      }
      public bool AllowBrowse {
        get {
          return _allowBrowse;
        }
        set {
          SetProperty(ref _allowBrowse, value, BrowseCommand);
        }
      }
    
      public CommandExecution() {
        BrowseCommand = new Command(BrowseForFile, CanBrowseForFile);
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
    }