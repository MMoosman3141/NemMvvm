using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NemMvvm;

namespace UnitTestNemMvvm {
	[TestClass]
	public class UnitTest1 : NotifyPropertyChanged {
		private string _testValue1 = "oldValue";
		private string _testValue2 = "oldValue";
		private string _testValue3 = "oldValue";
		private bool? _actionCondition = null;

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
			TestValue1 = "newValue";

			Assert.AreEqual("newValue", TestValue1);
		}

		[TestMethod]
		public void SetPropertyActionTest() {
			_actionCondition = null;

			TestValue2 = "newValue";

			Assert.AreEqual("newValue", TestValue2);
			Assert.AreEqual("actionComplete", _testValue3);
		}

		[TestMethod]
		public void SetPropertyActionTestOnTrue() {
			_actionCondition = true;

			TestValue2 = "newValue";

			Assert.AreEqual("newValue", TestValue2);
			Assert.AreEqual("actionComplete", _testValue3);
		}

		[TestMethod]
		public void SetPropertyActionTestOnFalse() {
			_actionCondition = false;

			_testValue2 = "newValue";
			TestValue2 = "newValue";

			Assert.AreEqual("newValue", TestValue2);
			Assert.AreEqual("actionComplete", _testValue3);
		}
		[TestMethod]
		public void SetPropertyActionTestNotCompleted() {
			_actionCondition = false;

			_testValue2 = "oldValue";
			TestValue2 = "newValue";

			Assert.AreEqual("newValue", TestValue2);
			Assert.AreEqual("notCompleted", _testValue3);
		}


	}
}
