using System;
using System.ComponentModel;
using System.Globalization;

namespace Visifire.Commons
{
	public class Converters
	{
		public class ObjectConverter : TypeConverter
		{
			public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
			{
				return true;
			}

			public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
			{
				return value;
			}
		}

		public class ValueConverter : TypeConverter
		{
			public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
			{
				return true;
			}

			public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
			{
				double num;
				if (value.GetType().Equals(typeof(string)))
				{
					if ((string)value == string.Empty)
					{
						num = double.NaN;
					}
					else
					{
						num = double.Parse((string)value, CultureInfo.InvariantCulture);
					}
				}
				else
				{
					num = (double)value;
				}
				return num;
			}
		}

		public class DoubleArrayConverter : TypeConverter
		{
			public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
			{
				return !(sourceType == typeof(string)) || base.CanConvertFrom(context, sourceType);
			}

			public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
			{
				double[] array2;
				if ((string)value == string.Empty)
				{
					double[] array = new double[4];
					array2 = array;
				}
				else
				{
					string[] array3 = value.ToString().Split(new char[]
					{
						','
					});
					array2 = new double[4];
					int num = 0;
					string[] array4 = array3;
					for (int i = 0; i < array4.Length; i++)
					{
						string s = array4[i];
						double num2;
						bool flag = double.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out num2);
						if (flag)
						{
							array2[num++] = num2;
						}
					}
				}
				return array2;
			}
		}
	}
}
