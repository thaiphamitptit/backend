using FluentValidation.Results;

namespace backend.Utils;

public class ValidationUtils {
  public static Dictionary<string, List<string>> ExtractErrors(ValidationResult validationResult) {
    var errors = new Dictionary<string, List<string>>();
    foreach (var error in validationResult.Errors) {
      if (!errors.ContainsKey(error.PropertyName)) {
        errors[error.PropertyName] = new List<string> { error.ErrorMessage };
      } else {
        errors[error.PropertyName].Add(error.ErrorMessage);
      }
    }
    return errors;
  }
}
