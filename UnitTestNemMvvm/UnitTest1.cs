using System;
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
				if (e.PropertyName == nameof(TestValue1))
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
				if (e.PropertyName == nameof(TestValue2))
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
				if (e.PropertyName == nameof(TestValue2))
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
				if (e.PropertyName == nameof(TestValue2))
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
				if (e.PropertyName == nameof(TestValue2))
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
				if (e.PropertyName == nameof(TestValue4))
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

			if (Command1.CanExecute(null))
				Command1.Execute(null);
			if (Command2.CanExecute(null))
				Command2.Execute(null);

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
