using System.Text.RegularExpressions;

namespace XUnitNemMvvm;

public class TestNotifyDataErrorInfo : NotifyPropertyChanged {
  private string _intValue;
  private string _noDigitsValue;

  public string IntValue {
    get => _intValue;
    set => SetProperty(ref _intValue, value, IsIntValidator);
  }
  public string NoDigitsValue {
    get => _noDigitsValue;
    set => SetProperty(ref _noDigitsValue, value, NoDigitsValidator);
  }

  private List<string> IsIntValidator(string value) {
    List<string> errors = new();

    if (!int.TryParse(value, out _)) {
      errors.Add("Not an integer");
    }

    return errors;
  }

  private List<string> NoDigitsValidator(string value) {
    List<string> errors = new();

    Regex hasDigitsRgx = new(@"\d", RegexOptions.Compiled);

    if (hasDigitsRgx.IsMatch(value)) {
      errors.Add("No digits allowed");
    }

    return errors;
  }

  [Theory]
  [InlineData("123", 0)]
  [InlineData("abc", 1)]
  public void TestIsIntValidator(string testValue, int expectedErrorCount) {
    IntValue = testValue;
    List<string> errors = (List<string>)GetErrors(nameof(IntValue));
    Assert.Equal(expectedErrorCount, errors?.Count ?? 0);
  }

  [Theory]
  [InlineData("abcdef", 0)]
  [InlineData("abc123def", 1)]
  public void TestNoDigitsValidator(string testValue, int expectedErrorCount) {
    NoDigitsValue = testValue;
    List<string> errors = (List<string>)GetErrors(nameof(NoDigitsValue));
    Assert.Equal(expectedErrorCount, errors?.Count ?? 0);
  }

}
