using System;
using System.Reflection;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public class ActionCategoryAttribute : Attribute
{
    public string Category { get; }

    public ActionCategoryAttribute(string category)
    {
        Category = category;
    }
}
