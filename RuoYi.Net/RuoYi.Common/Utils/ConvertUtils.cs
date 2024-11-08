namespace RuoYi.Common.Utils;

public static class ConvertUtils
{
  public static long[] ToLongArray(string str, string split = ",")
  {
    if (string.IsNullOrEmpty(str)) return Array.Empty<long>();
    var arr = str.Split(split);
    var longs = new long[arr.Length];
    for (var i = 0; i < arr.Length; i++) longs[i] = Convert.ToInt64(arr[i]);
    return longs;
  }
}
