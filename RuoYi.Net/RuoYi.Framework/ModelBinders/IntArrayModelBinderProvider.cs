using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

namespace RuoYi.Framework.ModelBinders;

/// <summary>
///   整数数字数组模型绑定提供器
/// </summary>
public class IntArrayModelBinderProvider : IModelBinderProvider
{
  public IModelBinder GetBinder(ModelBinderProviderContext context)
  {
    ArgumentNullException.ThrowIfNull(context);
    if (context.Metadata.ModelType == typeof(long[]) || context.Metadata.ModelType == typeof(int[]))
      return new BinderTypeModelBinder(typeof(IntArrayModelBinder));
    return null;
  }
}
