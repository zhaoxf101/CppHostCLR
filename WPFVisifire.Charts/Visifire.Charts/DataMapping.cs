using System;
using System.Globalization;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace Visifire.Charts
{
	public class DataMapping
	{
		private static readonly DependencyProperty DummyProperty = DependencyProperty.RegisterAttached("Dummy", typeof(object), typeof(DependencyObject), new PropertyMetadata(null));

		public string MemberName
		{
			get;
			set;
		}

		public string Path
		{
			get;
			set;
		}

		public void Map(object dataSource, object target)
		{
			object obj = this.GetPropertyValue(dataSource);
			PropertyInfo property = target.GetType().GetProperty(this.MemberName);
			Type type;
			if (!DataMapping.IsNullable(property.PropertyType, out type))
			{
				if (!property.PropertyType.Equals(typeof(Brush)) && obj != null)
				{
					if (obj.GetType() == typeof(TimeSpan) && !this.MemberName.Equals("XValue") && !this.MemberName.Equals("YValue") && !this.MemberName.Equals("ZValue"))
					{
						obj = obj.ToString();
					}
					obj = Convert.ChangeType(obj, property.PropertyType, CultureInfo.CurrentCulture);
				}
			}
			else if (type != null && obj != null)
			{
				obj = Convert.ChangeType(obj, type, CultureInfo.CurrentCulture);
			}
			if (property.PropertyType == typeof(double) && obj == null)
			{
				obj = double.NaN;
			}
			target.GetType().GetProperty(this.MemberName).SetValue(target, obj, null);
		}

		public static bool IsNullable(Type type, out Type innerType)
		{
			innerType = null;
			bool flag;
			if (!type.IsGenericType)
			{
				flag = false;
			}
			else
			{
				Type genericTypeDefinition = type.GetGenericTypeDefinition();
				flag = genericTypeDefinition.Equals(typeof(Nullable<>));
				if (flag)
				{
					PropertyInfo[] properties = type.GetProperties();
					innerType = properties[1].PropertyType;
				}
			}
			return flag;
		}

		public object GetPropertyValue(object source)
		{
			return DataMapping.Eval(source, this.Path);
		}

		internal static object Eval(object source, string pathExpression)
		{
			Binding binding = new Binding(pathExpression)
			{
				Source = source
			};
			ContentControl contentControl = new ContentControl();
			BindingOperations.SetBinding(contentControl, DataMapping.DummyProperty, binding);
			return contentControl.GetValue(DataMapping.DummyProperty);
		}
	}
}
