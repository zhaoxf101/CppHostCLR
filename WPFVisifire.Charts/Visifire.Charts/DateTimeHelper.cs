using System;
using System.Collections.Generic;
using System.Globalization;

namespace Visifire.Charts
{
	internal class DateTimeHelper
	{
		public static IntervalTypes GetAutoIntervalType(TimeSpan minDateDifference, TimeSpan maxDateDifference, IntervalTypes currentIntervalTypes)
		{
			if (maxDateDifference.Hours != 0)
			{
				return IntervalTypes.Hours;
			}
			if (maxDateDifference.Minutes != 0)
			{
				return IntervalTypes.Minutes;
			}
			if (maxDateDifference.Seconds != 0)
			{
				return IntervalTypes.Seconds;
			}
			if (maxDateDifference.Milliseconds != 0)
			{
				return IntervalTypes.Milliseconds;
			}
			return IntervalTypes.Days;
		}

		public static DateTime XValueToDateTime(DateTime minDate, double XValue, IntervalTypes intervalTypes)
		{
			DateTime result = minDate;
			try
			{
				switch (intervalTypes)
				{
				case IntervalTypes.Years:
					result = minDate.AddMinutes(XValue / 1.9025875190258751E-06);
					break;
				case IntervalTypes.Months:
					result = minDate.AddHours(XValue / 0.00137);
					break;
				case IntervalTypes.Weeks:
					result = minDate.AddDays(XValue * 7.0);
					break;
				case IntervalTypes.Days:
					result = minDate.AddDays(XValue);
					break;
				case IntervalTypes.Hours:
					result = minDate.AddHours(XValue);
					break;
				case IntervalTypes.Minutes:
					result = minDate.AddMinutes(XValue);
					break;
				case IntervalTypes.Seconds:
					result = minDate.AddSeconds(XValue);
					break;
				case IntervalTypes.Milliseconds:
					result = minDate.AddMilliseconds(XValue);
					break;
				}
			}
			catch
			{
			}
			return result;
		}

		public static double DateDiff(DateTime dateTime1, DateTime dateTime2, TimeSpan minDateDifference, TimeSpan maxDateDifference, IntervalTypes intervalTypes, ChartValueTypes xValueType)
		{
			TimeSpan timeSpan = dateTime1.Subtract(dateTime2);
			double result = 1.0;
			while (true)
			{
				switch (intervalTypes)
				{
				case IntervalTypes.Auto:
					intervalTypes = DateTimeHelper.GetAutoIntervalType(minDateDifference, maxDateDifference, intervalTypes);
					continue;
				case IntervalTypes.Years:
					goto IL_53;
				case IntervalTypes.Months:
					goto IL_67;
				case IntervalTypes.Weeks:
					goto IL_7B;
				case IntervalTypes.Days:
					goto IL_8F;
				case IntervalTypes.Hours:
					goto IL_99;
				case IntervalTypes.Minutes:
					goto IL_A3;
				case IntervalTypes.Seconds:
					goto IL_AD;
				case IntervalTypes.Milliseconds:
					goto IL_B7;
				}
				break;
			}
			return result;
			IL_53:
			result = 1.9025875190258751E-06 * timeSpan.TotalMinutes;
			return result;
			IL_67:
			result = 0.00137 * timeSpan.TotalHours;
			return result;
			IL_7B:
			result = timeSpan.TotalDays / 7.0;
			return result;
			IL_8F:
			result = timeSpan.TotalDays;
			return result;
			IL_99:
			result = timeSpan.TotalHours;
			return result;
			IL_A3:
			result = timeSpan.TotalMinutes;
			return result;
			IL_AD:
			result = timeSpan.TotalSeconds;
			return result;
			IL_B7:
			result = timeSpan.TotalMilliseconds;
			return result;
		}

		public static void CalculateMinMaxDate(List<DateTime> dateTimeList, out DateTime minDate, out DateTime maxDate, out TimeSpan minDateRange, out TimeSpan maxDateRange)
		{
			dateTimeList.Sort();
			minDateRange = default(TimeSpan);
			maxDateRange = default(TimeSpan);
			minDate = dateTimeList[0];
			if (dateTimeList.Count > 0)
			{
				maxDate = dateTimeList[dateTimeList.Count - 1];
				if (dateTimeList.Count >= 2)
				{
					minDateRange = dateTimeList[1].Subtract(minDate);
					maxDateRange = maxDate.Subtract(minDate);
					return;
				}
			}
			else
			{
				maxDate = minDate;
			}
		}

		internal static void CalculateMinMaxDate4PolarChart(List<DateTime> dateTimeList, Axis axisX, out DateTime minDate, out DateTime maxDate, out TimeSpan minDateRange, out TimeSpan maxDateRange)
		{
			if (axisX.AxisMinimum == null)
			{
				minDate = DateTime.Parse("12/30/1899", CultureInfo.InvariantCulture);
			}
			else
			{
				minDate = Convert.ToDateTime(axisX.AxisMinimum);
			}
			if (axisX.AxisMaximum != null)
			{
				maxDate = Convert.ToDateTime(axisX.AxisMaximum);
			}
			else
			{
				maxDate = dateTimeList[dateTimeList.Count - 1];
			}
			dateTimeList.Sort();
			minDateRange = default(TimeSpan);
			maxDateRange = default(TimeSpan);
			if (dateTimeList.Count > 0)
			{
				if (dateTimeList.Count >= 2)
				{
					minDateRange = dateTimeList[1].Subtract(minDate);
					maxDateRange = maxDate.Subtract(minDate);
					return;
				}
			}
			else
			{
				maxDate = minDate;
			}
		}

		public static double CalculateAutoInterval(double width, double height, AxisOrientation axisOrientation, DateTime minDateTime, DateTime maxDateTime, out IntervalTypes type, double maxInterval, ChartValueTypes xValueType)
		{
			TimeSpan timeSpan = maxDateTime.Subtract(minDateTime);
			double num = (axisOrientation == AxisOrientation.Horizontal) ? (maxInterval * 0.8) : maxInterval;
			double num2 = ((axisOrientation == AxisOrientation.Horizontal) ? width : height) / (2000.0 / num);
			timeSpan = new TimeSpan((long)((double)timeSpan.Ticks / num2));
			double totalMinutes = timeSpan.TotalMinutes;
			if (xValueType != ChartValueTypes.Date)
			{
				if (totalMinutes <= 1.0)
				{
					double totalMilliseconds = timeSpan.TotalMilliseconds;
					if (totalMilliseconds <= 10.0)
					{
						type = IntervalTypes.Milliseconds;
						return 1.0;
					}
					if (totalMilliseconds <= 50.0)
					{
						type = IntervalTypes.Milliseconds;
						return 4.0;
					}
					if (totalMilliseconds <= 200.0)
					{
						type = IntervalTypes.Milliseconds;
						return 20.0;
					}
					if (totalMilliseconds <= 500.0)
					{
						type = IntervalTypes.Milliseconds;
						return 50.0;
					}
					double totalSeconds = timeSpan.TotalSeconds;
					if (totalSeconds <= 7.0)
					{
						type = IntervalTypes.Seconds;
						return 1.0;
					}
					if (totalSeconds <= 15.0)
					{
						type = IntervalTypes.Seconds;
						return 2.0;
					}
					if (totalSeconds <= 30.0)
					{
						type = IntervalTypes.Seconds;
						return 5.0;
					}
					if (totalSeconds <= 60.0)
					{
						type = IntervalTypes.Seconds;
						return 10.0;
					}
				}
				else
				{
					if (totalMinutes <= 2.0)
					{
						type = IntervalTypes.Seconds;
						return 20.0;
					}
					if (totalMinutes <= 3.0)
					{
						type = IntervalTypes.Seconds;
						return 30.0;
					}
					if (totalMinutes <= 10.0)
					{
						type = IntervalTypes.Minutes;
						return 1.0;
					}
					if (totalMinutes <= 20.0)
					{
						type = IntervalTypes.Minutes;
						return 2.0;
					}
					if (totalMinutes <= 60.0)
					{
						type = IntervalTypes.Minutes;
						return 5.0;
					}
					if (totalMinutes <= 120.0)
					{
						type = IntervalTypes.Minutes;
						return 10.0;
					}
					if (totalMinutes <= 180.0)
					{
						type = IntervalTypes.Minutes;
						return 30.0;
					}
					if (totalMinutes <= 720.0)
					{
						type = IntervalTypes.Hours;
						return 1.0;
					}
					if (totalMinutes <= 1440.0)
					{
						type = IntervalTypes.Hours;
						return 4.0;
					}
					if (totalMinutes <= 2880.0)
					{
						type = IntervalTypes.Hours;
						return 6.0;
					}
					if (totalMinutes <= 4320.0)
					{
						type = IntervalTypes.Hours;
						return 12.0;
					}
				}
			}
			if (totalMinutes <= 14400.0)
			{
				type = IntervalTypes.Days;
				return 1.0;
			}
			if (totalMinutes <= 28800.0)
			{
				type = IntervalTypes.Days;
				return 2.0;
			}
			if (totalMinutes <= 43200.0)
			{
				type = IntervalTypes.Days;
				return 3.0;
			}
			if (totalMinutes <= 87840.0)
			{
				type = IntervalTypes.Weeks;
				return 1.0;
			}
			if (totalMinutes <= 219600.0)
			{
				type = IntervalTypes.Weeks;
				return 2.0;
			}
			if (totalMinutes <= 527040.0)
			{
				type = IntervalTypes.Months;
				return 1.0;
			}
			if (totalMinutes <= 1054080.0)
			{
				type = IntervalTypes.Months;
				return 3.0;
			}
			if (totalMinutes <= 2108160.0)
			{
				type = IntervalTypes.Months;
				return 6.0;
			}
			type = IntervalTypes.Years;
			double num3 = totalMinutes / 60.0 / 24.0 / 365.0;
			if (num3 < 5.0)
			{
				return 1.0;
			}
			if (num3 < 10.0)
			{
				return 2.0;
			}
			return Math.Floor(num3 / 5.0);
		}

		public static DateTime UpdateDate(DateTime dateToUpdate, double increment, IntervalTypes intervalType)
		{
			switch (intervalType)
			{
			case IntervalTypes.Years:
			{
				dateToUpdate = dateToUpdate.AddYears((int)Math.Floor(increment));
				TimeSpan value = TimeSpan.FromDays(365.0 * (increment - Math.Floor(increment)));
				dateToUpdate = dateToUpdate.Add(value);
				return dateToUpdate;
			}
			case IntervalTypes.Months:
			{
				bool flag = false;
				if (dateToUpdate.Day == DateTime.DaysInMonth(dateToUpdate.Year, dateToUpdate.Month))
				{
					flag = true;
				}
				dateToUpdate = dateToUpdate.AddMonths((int)Math.Floor(increment));
				TimeSpan value = TimeSpan.FromDays(30.0 * (increment - Math.Floor(increment)));
				if (flag && value.Ticks == 0L)
				{
					int num = DateTime.DaysInMonth(dateToUpdate.Year, dateToUpdate.Month);
					dateToUpdate = dateToUpdate.AddDays((double)(num - dateToUpdate.Day));
				}
				dateToUpdate = dateToUpdate.Add(value);
				return dateToUpdate;
			}
			case IntervalTypes.Weeks:
				return dateToUpdate.AddDays(7.0 * increment);
			case IntervalTypes.Hours:
				return dateToUpdate.AddHours(increment);
			case IntervalTypes.Minutes:
				return dateToUpdate.AddMinutes(increment);
			case IntervalTypes.Seconds:
				return dateToUpdate.AddSeconds(increment);
			case IntervalTypes.Milliseconds:
				return dateToUpdate.AddMilliseconds(increment);
			}
			return dateToUpdate.AddDays(increment);
		}

		internal static DateTime AlignDateTime(DateTime dateTime, double intervalSize, IntervalTypes type)
		{
			DateTime dateTime2 = dateTime;
			if (intervalSize == 0.0)
			{
				return dateTime2;
			}
			if (intervalSize > 0.0 && intervalSize != 1.0 && type == IntervalTypes.Months && intervalSize <= 12.0 && intervalSize > 1.0)
			{
				DateTime dateTime3 = dateTime2;
				DateTime dateTime4 = new DateTime(dateTime2.Year, 1, 1, 0, 0, 0);
				while (dateTime4 < dateTime2)
				{
					dateTime3 = dateTime4;
					dateTime4 = dateTime4.AddMonths((int)intervalSize);
				}
				dateTime2 = dateTime3;
				return dateTime2;
			}
			switch (type)
			{
			case IntervalTypes.Years:
			{
				int num = (int)((double)dateTime2.Year / intervalSize * intervalSize);
				if (num <= 0)
				{
					num = 1;
				}
				dateTime2 = new DateTime(num, 1, 1, 0, 0, 0, 0);
				break;
			}
			case IntervalTypes.Months:
			{
				int num2 = (int)((double)dateTime2.Month / intervalSize * intervalSize);
				if (num2 <= 0)
				{
					num2 = 1;
				}
				dateTime2 = new DateTime(dateTime2.Year, num2, 1, 0, 0, 0);
				break;
			}
			case IntervalTypes.Weeks:
				dateTime2 = dateTime2.AddDays((double)(-(double)dateTime2.DayOfWeek));
				dateTime2 = new DateTime(dateTime2.Year, dateTime2.Month, dateTime2.Day, 0, 0, 0);
				break;
			case IntervalTypes.Days:
			{
				int num3 = (int)((double)dateTime2.Day / intervalSize * intervalSize);
				if (num3 <= 0)
				{
					num3 = 1;
				}
				dateTime2 = new DateTime(dateTime2.Year, dateTime2.Month, num3, 0, 0, 0);
				break;
			}
			case IntervalTypes.Hours:
			{
				int hour = (int)((double)dateTime2.Hour / intervalSize * intervalSize);
				dateTime2 = new DateTime(dateTime2.Year, dateTime2.Month, dateTime2.Day, hour, 0, 0);
				break;
			}
			case IntervalTypes.Minutes:
			{
				int minute = (int)((double)dateTime2.Minute / intervalSize * intervalSize);
				dateTime2 = new DateTime(dateTime2.Year, dateTime2.Month, dateTime2.Day, dateTime2.Hour, minute, 0);
				break;
			}
			case IntervalTypes.Seconds:
			{
				int second = (int)((double)dateTime2.Second / intervalSize * intervalSize);
				dateTime2 = new DateTime(dateTime2.Year, dateTime2.Month, dateTime2.Day, dateTime2.Hour, dateTime2.Minute, second, 0);
				break;
			}
			case IntervalTypes.Milliseconds:
			{
				int millisecond = (int)((double)dateTime2.Millisecond / intervalSize * intervalSize);
				dateTime2 = new DateTime(dateTime2.Year, dateTime2.Month, dateTime2.Day, dateTime2.Hour, dateTime2.Minute, dateTime2.Second, millisecond);
				break;
			}
			}
			return dateTime2;
		}
	}
}
