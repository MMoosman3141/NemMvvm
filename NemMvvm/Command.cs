using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NemMvvm {
	public class Command : ICommand {
		private Action _execute;
		private Func<bool> _canExecute;
		private Action<object> _executeParamed;
		private Func<object, bool> _canExecuteParamed;

		public Command(Action execute) {
			_execute = execute;
			_canExecute = null;
			_executeParamed = null;
			_canExecuteParamed = null;
		}
		public Command(Action execute, Func<bool> canExecute) {
			_execute = execute;
			_canExecute = canExecute;
			_executeParamed = null;
			_canExecuteParamed = null;
		}
		public Command(Action execute, Func<object, bool> canExecute) {
			_execute = execute;
			_canExecute = null;
			_executeParamed = null;
			_canExecuteParamed = canExecute;
		}
		public Command(Action<object> execute) {
			_execute = null;
			_canExecute = null;
			_executeParamed = execute;
			_canExecuteParamed = null;
		}
		public Command(Action<object> execute, Func<bool> canExecute) {
			_execute = null;
			_canExecute = canExecute;
			_executeParamed = execute;
			_canExecuteParamed = null;
		}		
		public Command(Action<object> execute, Func<object, bool> canExecute) {
			_execute = null;
			_canExecute = null;
			_executeParamed = execute;
			_canExecuteParamed = canExecute;
		}

		public void Execute(object parameter) {
			if (_execute != null)
				_execute();
			else if(_executeParamed != null)
				_executeParamed(parameter);
		}

		public bool CanExecute(object parameter) {
			if (_canExecute != null)
				return _canExecute();
			else if (_canExecuteParamed != null)
				return _canExecuteParamed(parameter);

			return true;
		}

		public void RaiseCanExecuteChanged() {
			if (CanExecuteChanged != null)
				CanExecuteChanged(this, new EventArgs());
		}
		public event EventHandler CanExecuteChanged;
	}
}
