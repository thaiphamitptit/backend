namespace backend.Utils;

public class CommonUtils {
  public static string AutoGenerateCode(string code) {
    var prefix = code.Substring(0, 2);
    var postfix = code.Substring(2);
    int number = int.Parse(postfix) + 1;
    return $"{prefix}{number:D5}";
  }
}
