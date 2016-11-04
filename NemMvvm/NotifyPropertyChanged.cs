using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace NemMvvm {
	/// <summary>
	/// This class provides functionality for INotifyPropertyChanged in order to allow easy binding of UI control properties to backend code.
	/// 
	/// Objects which inherit from this class should set properties using a call to SetProperty, when within the Set functionality of a Property definition.
	/// Doing so will update the underlying field, and appropriately raise the PropertyChanged event.
	/// 
	/// A call to RaisePropertyCahnged should be made to provide change notification for binding, when a property is set outside of it's Property definition.
	/// </summary>
	public class NotifyPropertyChanged : INotifyPropertyChanged	{
		private object _propertyLock = new object();

		protected object NotifyPropertyChangedLock {
			get {
				return _propertyLock;
			}
			set {
				_propertyLock = value;
			}
		}

		private bool SetValue<T>(ref T field, T value) {
			/*
											field == null       newValue == null           ^        field != null && field != newValue     ||
			 execute             true                  false             true                   false                      true
			 execute             false                 true              true                   false                      true
			 execute             false                 false             false                  true                       true
			 don't execute       true                  true              false                  false                      false
			 don't execute       false                 false             false                  false                      false
			*/

			bool propertyChanged = false;

			if ((field == null ^ value == null) || (field != null && !field.Equals(value))) {
				field = value;
				propertyChanged = true;
			}

			return propertyChanged;
		}

		/// <summary>
		/// Sets the value of a property, and raises the PropertyChanged event for the property.
		/// The PropertyChanged event is only called in the case that the value of the referenced field changed as determined by the Equals method of the object.
		/// </summary>
		/// <typeparam name="T">The Type of the property being set.</typeparam>
		/// <param name="field">A reference to the field value which is returned by the property.</param>
		/// <param name="value">The value to which the field is set.</param>
		/// <param name="propertyName">The name of the property being set.  If not specified, the parameter defaults to the name of the calling property.</param>
		/// <returns>Returns true if the property changed values and was set.  Returns false otherwise.</returns>
		protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = "") {
			if (string.IsNullOrWhiteSpace(propertyName))
				throw new ArgumentNullException("propertyName cannot be empty or null");

			bool propertyChanged = false;

			lock (_propertyLock) {
				propertyChanged = SetValue(ref field, value);

				if (propertyChanged) {
					RaisePropertyChanged(propertyName);
				}
			}
			return propertyChanged;
		}

		/// <summary>
		/// Sets the value of a property, and raises the PropertyChanged event for the property when the value changes.
		/// This method also runs an action if the result of the changing of the property matches the runActionCheck.  If runActionCheck is null (the default) the action is always run.
		/// The PropertyChanged event is only called in the case that the value of the referenced field changed as determined by the Equals method of the object.
		/// </summary>
		/// <typeparam name="T">The Type of the property being set.</typeparam>
		/// <param name="field">A reference to the field which is to be affected.</param>
		/// <param name="value">The value to which the field is to be set.</param>
		/// <param name="action">The action to run if the result of setting the field matches the runActionCheck parameter.</param>
		/// <param name="runActionCheck">A bool? value.  If true, and the field is successfully set the action will run.  If false and the field is not set the action will run.  If null (the default) the action will always run. </param>
		/// <param name="propertyName">The name of the property being set.  If not specified, the parameter defaults to the name of the calling property.</param>
		/// <returns>Returns true if the property changed values and was set.  Returns false otherwise.</returns>
		protected bool SetProperty<T>(ref T field, T value, Action action, bool? runActionCheck = null, [CallerMemberName] string propertyName = "") {
			if (string.IsNullOrWhiteSpace(propertyName))
				throw new ArgumentNullException("propertyName cannot be empty or null");

			bool propertyChanged = false;

			lock (_propertyLock) {
				propertyChanged = SetValue(ref field, value);

				if (runActionCheck == null || propertyChanged == runActionCheck) {
					action();
				}

				if (propertyChanged) {
					RaisePropertyChanged(propertyName);
				}
			}
			return propertyChanged;
		}

		/// <summary>
		/// Sets the value of a property, and raises the PropertyChanged event for the property.
		/// Also, an IEnumerable of Command objects have a RiaseCanExecuteChanged event raised, allowing the SetPropertyCommand to be used in conjunction with any number of Command objects.
		/// The PropertyChanged event is only called in the case that the value of the referenced field changed as determined by the Equals method of the object.
		/// </summary>
		/// <typeparam name="T">The Type of the property being set.</typeparam>
		/// <param name="field">A reference to the field value which is returned by the property.</param>
		/// <param name="value">The value to which the field is set.</param>
		/// <param name="commands">An IEnumerable of Command objects for which to raise a RaiseCanExecuteChanged event.</param>
		/// <param name="propertyName">The name of the property being set.  If not specified, the parameter defaults to the name of the calling property.</param>
		/// <returns>Returns true if the property changed values and was set.  Returns false otherwise.</returns>
		protected bool SetProperty<T>(ref T field, T value, IEnumerable<Command> commands, [CallerMemberName] string propertyName = "") {
			bool retVal = SetProperty(ref field, value, propertyName);

			foreach (Command cmd in commands)
				cmd?.RaiseCanExecuteChanged();

			return retVal;
		}

		/// <summary>
		/// Sets the value of a property, and raises the PropertyChanged event for the property.
		/// Also, an array of Command objects have a RiaseCanExecuteChanged event raised, allowing the SetPropertyCommand to be used in conjunction with any number of Command objects.
		/// The PropertyChanged event is only called in the case that the value of the referenced field changed as determined by the Equals method of the object.
		/// </summary>
		/// <typeparam name="T">The Type of the property being set.</typeparam>
		/// <param name="field">A reference to the field value which is returned by the property.</param>
		/// <param name="value">The value to which the field is set.</param>
		/// <param name="property">A lambda expression representing the Property to be set.</param>
		/// <param name="commands">An array, or comma delimited list of Command objects for which to raise a RaiseCanExecuteChanged event.</param>
		/// <returns>Returns true if the property changed values and was set.  Returns false otherwise.</returns>
		protected bool SetProperty<T>(ref T field, T value, Expression<Func<object>> property, params Command[] commands) {
			string propertyName;
			if (property.Body is MemberExpression)
				propertyName = ((MemberExpression)property.Body).Member.Name;
			else
				propertyName = ((MemberExpression)((UnaryExpression)property.Body).Operand).Member.Name;
			
			return SetProperty(ref field, value, commands, propertyName);
		}		

		/// <summary>
		/// Raises the PropertyChanged event for the named property.  If no parameter is passed, the name of the calling property is used.
		/// </summary>
		/// <param name="propertyName">The name of the property for which the PropertyChanged event should be called.  If not property name is passed, the name of the calling property is used.</param>
		protected void RaisePropertyChanged([CallerMemberName] string propertyName = "") {
			if (string.IsNullOrWhiteSpace(propertyName))
				throw new ArgumentNullException("propertyName cannot be empty or null");

			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));

		}

		/// <summary>
		/// Raises the PropertyChanged event for the property passed as a lambda expression.
		/// </summary>
		/// <param name="property">A property function passes as a lambda expression.</param>
		protected void RaisePropertyChanged(Expression<Func<object>> property) {
			string propertyName;
			if (property.Body is MemberExpression)
				propertyName = ((MemberExpression)property.Body).Member.Name;
			else
				propertyName = ((MemberExpression)((UnaryExpression)property.Body).Operand).Member.Name;

			RaisePropertyChanged(propertyName);
		}

		/// <summary>
		/// This event can be used to trigger bound controls to update when a property is set.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;
	}
}
