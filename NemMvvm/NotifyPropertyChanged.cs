using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace NemMvvm {
	public class NotifyPropertyChanged : INotifyPropertyChanged	{
		private object _propertyLock = new object();

		protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = "")	{
			if (string.IsNullOrWhiteSpace(propertyName))
				throw new ArgumentNullException("propertyName cannot be empty or null");

			bool propertyChanged = false;

			lock (_propertyLock) {
				/*
				                field == null       newValue == null           ^        field != null && field != newValue     ||
				 execute             true                  false             true                   false                      true
				 execute             false                 true              true                   false                      true
				 execute             false                 false             false                  true                       true
				 don't execute       true                  true              false                  false                      false
				 don't execute       false                 false             false                  false                      false
				*/
				if ((field == null ^ value == null) || (field != null && !field.Equals(value))) {
					field = value;
					RaisePropertyChanged(propertyName);
					propertyChanged = true;					
				}
			}
			return propertyChanged;
		}

		protected bool SetProperty<T>(ref T field, T value, IEnumerable<Command> commands, [CallerMemberName] string propertyName = "") {
			bool retVal = SetProperty(ref field, value, propertyName);

			foreach (Command cmd in commands)
				cmd?.RaiseCanExecuteChanged();

			return retVal;
		}

		protected bool SetProperty<T>(ref T field, T value, Expression<Func<object>> property, params Command[] commands) {
			string propertyName;
			if (property.Body is MemberExpression)
				propertyName = ((MemberExpression)property.Body).Member.Name;
			else
				propertyName = ((MemberExpression)((UnaryExpression)property.Body).Operand).Member.Name;
			
			return SetProperty(ref field, value, commands, propertyName);
		}		

		protected void RaisePropertyChanged([CallerMemberName] string propertyName = "") {
			if (string.IsNullOrWhiteSpace(propertyName))
				throw new ArgumentNullException("propertyName cannot be empty or null");

			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));

		}
		protected void RaisePropertyChanged(Expression<Func<object>> property) {
			string propertyName;
			if (property.Body is MemberExpression)
				propertyName = ((MemberExpression)property.Body).Member.Name;
			else
				propertyName = ((MemberExpression)((UnaryExpression)property.Body).Operand).Member.Name;

			RaisePropertyChanged(propertyName);
		}

		public event PropertyChangedEventHandler PropertyChanged;
	}
}
