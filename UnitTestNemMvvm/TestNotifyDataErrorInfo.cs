using Microsoft.VisualStudio.TestTools.UnitTesting;
using NemMvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace UnitTestNemMvvm {
  [TestClass]
  public class TestNotifyDataErrorInfo : NotifyPropertyChanged {
    private string _intValue;
    private string _noDigitsValue;

    public string IntValue {
      get => _intValue;
      set =>  SetProperty(ref _intValue, value, IsIntValidator);
    }
    public string NoDigitsValue {
      get => _noDigitsValue;
      set => SetProperty(ref _noDigitsValue, value, NoDigitsValidator);
    }

    private List<string> IsIntValidator(string value) {
      List<string> errors = new List<string>();

      if(!int.TryParse(value, out int _)) {
        errors.Add("Not an integer");
      }

      return errors;
    }

    private List<string> NoDigitsValidator(string value) {
      List<string> errors = new List<string>();

      Regex hasDigitsRgx = new Regex(@"\d", RegexOptions.Compiled);

      if(hasDigitsRgx.IsMatch(value)) {
        errors.Add("No digits allowed");
      }

      return errors;
    }

    [TestMethod]
    public void TestIsIntValidator() {
      IntValue = "1234";
      List<string> errors = (List<string>)GetErrors(nameof(IntValue));
      Assert.IsNull(errors);

      IntValue = "abcd";
      errors = (List<string>)GetErrors(nameof(IntValue));
      Assert.IsTrue(errors.Count > 0);
    }

    [TestMethod]
    public void TestNoDigitsValidator() {
      NoDigitsValue = "abcdef";
      List<string> errors = (List<string>)GetErrors(nameof(NoDigitsValue));
      Assert.IsNull(errors);

      NoDigitsValue = "abc123def";
      errors = (List<string>)GetErrors(nameof(NoDigitsValue));
      Assert.IsTrue(errors.Count > 0);
    }

  }
}
