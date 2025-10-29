using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace RuoYi.Framework.ModelBinders;

/// <summary>
///   整数数字数组模型绑定
/// </summary>
public class IntArrayModelBinder : IModelBinder
{
  public Task BindModelAsync(ModelBindingContext bindingContext)
  {
    ArgumentNullException.ThrowIfNull(bindingContext);
    // 仅处理 int[] 或 long[] 类型（双重保险，与提供器配合）
    if (bindingContext.ModelType != typeof(int[]) && bindingContext.ModelType != typeof(long[]))
    {
      return Task.CompletedTask;
    }
    //-------------
    var modelName = bindingContext.ModelName;
    // Try to fetch the value of the argument by name
    var valueProviderResult = bindingContext.ValueProvider.GetValue(modelName);
    if (valueProviderResult == ValueProviderResult.None) return Task.CompletedTask;
    bindingContext.ModelState.SetModelValue(modelName, valueProviderResult);
    var value = valueProviderResult.FirstValue;
    // Check if the argument value is null or empty
    if (string.IsNullOrEmpty(value)) return Task.CompletedTask;
    try
    {
      if (bindingContext.ModelType == typeof(long[]))
      {
        var ids = value?.Split(',')?.Where(x=> x.All(char.IsDigit))?.Select(long.Parse)?.ToArray();
        if (ids == null) return Task.CompletedTask;
        bindingContext.Result = ModelBindingResult.Success(ids);
      }
      else
      {
        var ids = value?.Split(',')?.Where(x=> x.All(char.IsDigit))?.Select(int.Parse)?.ToArray();
        if (ids == null) return Task.CompletedTask;
        bindingContext.Result = ModelBindingResult.Success(ids);
      }
    }
    catch (Exception e)
    {
      throw new ArgumentException($"传入的参数异常：【{value}】," + e.Message);
    }
    return Task.CompletedTask;
  }
}
