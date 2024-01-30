using System.ComponentModel;

namespace iSystem.Core;

public static class DescriptionAttributeExtensions
{
	public static string GetClassDescription<T>(this T t) where T : class
	{
		var objects = t.GetType().GetCustomAttributes(typeof(DescriptionAttribute), false);
		if (objects.Length == 0) return string.Empty;
		var attribute = (DescriptionAttribute)objects[0];

		return attribute.Description;
	}

	public static string GetDescription<T>(this T? instance, string propertyName) where T : class
	{
		if (instance is null) return string.Empty;

		var propertyInfo = typeof(T).GetProperty(propertyName);

		var objects = propertyInfo?.GetCustomAttributes(typeof(DescriptionAttribute), true);
		if (objects is null) return string.Empty;
		if (objects.Length == 0) return string.Empty;
		var attribute = (DescriptionAttribute)objects[0];

		return attribute.Description;
	}

	public static string GetMemberDescription<T>(this T? instance, string memberName) where T : class
	{
		var memberInfos = instance?.GetType().GetMember(memberName);
		if (memberInfos is null) return string.Empty;
		if (memberInfos.Length == 0) return string.Empty;

		var objects = memberInfos[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
		if (objects.Length == 0) return string.Empty;
		var attribute = (DescriptionAttribute)objects[0];

		return attribute.Description;
	}
}