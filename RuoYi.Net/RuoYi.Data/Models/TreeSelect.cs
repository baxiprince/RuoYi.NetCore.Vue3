﻿using RuoYi.Data.Dtos;
using RuoYi.Data.Entities;

namespace RuoYi.Data.Models;

public class TreeSelect
{
  public TreeSelect()
  {
  }

  public TreeSelect(SysDeptDto dept)
  {
    Id = dept.DeptId ?? 0;
    Label = dept.DeptName!;
    Children = dept.Children?.Select(c => new TreeSelect(c)).ToList();
  }

  public TreeSelect(SysMenu menu)
  {
    Id = menu.MenuId;
    Label = menu.MenuName!;
    Children = menu.Children?.Select(m => new TreeSelect(m)).ToList();
  }

  /**
   * 节点ID
   */
  public long Id { get; set; }

  /**
   * 节点名称
   */
  public string Label { get; set; }

  /**
   * 子节点
   */
  public List<TreeSelect>? Children { get; set; }

  // 按条件忽略字段: https://www.newtonsoft.com/json/help/html/conditionalproperties.htm
  public bool ShouldSerializeChildren()
  {
    return Children != null && Children.Any();
  }
}
