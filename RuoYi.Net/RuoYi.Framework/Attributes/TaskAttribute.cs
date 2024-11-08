namespace RuoYi.Framework.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class TaskAttribute : Attribute
{
  public TaskAttribute(string name)
  {
    Name = name;
  }

  /// <summary>
  ///   任务名称
  /// </summary>
  public string Name { get; set; }
}
