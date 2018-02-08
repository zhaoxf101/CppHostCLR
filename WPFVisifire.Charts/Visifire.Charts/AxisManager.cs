using System;
using System.Globalization;

namespace Visifire.Charts
{
	internal class AxisManager
	{
		private bool _includeZero;

		private decimal _min;

		private decimal _max;

		private int _maxNoOfInterval = 10;

		private decimal _interval;

		private decimal _axisMaximumValue;

		private decimal _axisMinimumValue;

		private bool _overrideAxisMaximumValue;

		private bool _overrideAxisMinimumValue;

		private bool _overrideInterval;

		public AxisRepresentations AxisRepresentation
		{
			get;
			set;
		}

		public bool IsDateTimeAxis
		{
			get;
			set;
		}

		public bool Logarithmic
		{
			get;
			set;
		}

		public double LogarithmicBase
		{
			get;
			set;
		}

		public bool StartFromZero
		{
			get;
			set;
		}

		public double MinimumValue4LogScale
		{
			get;
			set;
		}

		public bool IsCircularAxis
		{
			get;
			set;
		}

		public int MaximumNoOfInterval
		{
			get
			{
				return this.GetNoOfIntervals();
			}
			set
			{
				if (value < 0)
				{
					throw new Exception("Invalid property value:: Expected number of intervals should be positive.");
				}
				if (value > 100)
				{
					throw new Exception("Property out of range:: Expected number of intervals should be less than or equals to 1000.");
				}
				this._maxNoOfInterval = value;
			}
		}

		public bool IncludeZero
		{
			get
			{
				return this._includeZero;
			}
			set
			{
				this._includeZero = true;
				if (value && this._min > 0m)
				{
					this._min = 0m;
				}
			}
		}

		public double AxisMaximumValue
		{
			get
			{
				return (double)this._axisMaximumValue;
			}
			set
			{
				this._axisMaximumValue = (decimal)value;
				this._overrideAxisMaximumValue = true;
			}
		}

		public double Interval
		{
			get
			{
				return (double)this._interval;
			}
			set
			{
				if (value < 0.0)
				{
					throw new Exception("Invalid property value:: Interval size should be positive always.");
				}
				this._interval = (decimal)value;
				this._overrideInterval = true;
			}
		}

		public double AxisMinimumValue
		{
			get
			{
				return (double)this._axisMinimumValue;
			}
			set
			{
				this._axisMinimumValue = (decimal)value;
				this._overrideAxisMinimumValue = true;
			}
		}

		public double MinimumValue
		{
			get
			{
				return (double)this._min;
			}
			set
			{
				this._min = (decimal)value;
			}
		}

		public double MaximumValue
		{
			get
			{
				return (double)this._max;
			}
			set
			{
				this._max = (decimal)value;
			}
		}

		public AxisManager(double maxValue, double minValue, bool startFromZero, bool allowLimitOverflow, bool stackingOverride, bool isCircularAxis, AxisRepresentations axisRepresentation, bool isLogarithmic, double logarithmicBase, bool startFromMinimumValue4LogScale, bool isDateTimeAxis)
		{
			if (maxValue < minValue)
			{
				throw new ArgumentException("Invalid Argument:: Maximum Data value should be always greater than the minimum data value.");
			}
			this._max = (decimal)maxValue;
			this._min = (decimal)minValue;
			this.AxisRepresentation = axisRepresentation;
			this.IsDateTimeAxis = isDateTimeAxis;
			this.StartFromZero = startFromZero;
			if (!isCircularAxis)
			{
				this.Logarithmic = isLogarithmic;
				this.LogarithmicBase = logarithmicBase;
				if (startFromMinimumValue4LogScale)
				{
					if (this._min == 0m)
					{
						this.MinimumValue4LogScale = 1.0;
					}
					else
					{
						this.MinimumValue4LogScale = (double)this._min;
					}
				}
				else
				{
					this.MinimumValue4LogScale = double.NaN;
				}
			}
			if (axisRepresentation == AxisRepresentations.AxisX && startFromZero)
			{
				if (minValue >= 0.0 && maxValue != 0.0)
				{
					if (this.Logarithmic)
					{
						if (minValue < 1.0)
						{
							this.AxisMinimumValue = minValue;
						}
						else
						{
							this.AxisMinimumValue = Math.Pow(this.LogarithmicBase, 0.0);
						}
					}
					else
					{
						this.AxisMinimumValue = 0.0;
					}
				}
				else if (maxValue <= 0.0 && minValue != 0.0)
				{
					this.AxisMaximumValue = 0.0;
				}
			}
			if (startFromZero && minValue >= 0.0 && maxValue != 0.0 && this.Logarithmic)
			{
				if (minValue < 1.0)
				{
					this.AxisMinimumValue = minValue;
				}
				else
				{
					this.AxisMinimumValue = Math.Pow(this.LogarithmicBase, 0.0);
				}
			}
			if (this._min == this._max && startFromZero && this._min == this._max)
			{
				if (minValue == 0.0 && maxValue != 0.0)
				{
					if (this.Logarithmic)
					{
						this.AxisMinimumValue = Math.Pow(this.LogarithmicBase, 0.0);
					}
					else
					{
						this.AxisMinimumValue = 0.0;
					}
				}
				else if (maxValue == 0.0 && minValue != 0.0)
				{
					this.AxisMaximumValue = 0.0;
				}
			}
			if (!allowLimitOverflow && minValue == 0.0 && maxValue != 0.0 && this.Logarithmic)
			{
				this.AxisMinimumValue = Math.Pow(this.LogarithmicBase, 0.0);
			}
			if (axisRepresentation == AxisRepresentations.AxisX && !allowLimitOverflow)
			{
				if (minValue == 0.0 && maxValue != 0.0)
				{
					if (this.Logarithmic)
					{
						this.AxisMinimumValue = Math.Pow(this.LogarithmicBase, 0.0);
					}
					else
					{
						this.AxisMinimumValue = 0.0;
					}
				}
				if (maxValue == 0.0 && minValue != 0.0)
				{
					this.AxisMaximumValue = 0.0;
				}
			}
			if (!allowLimitOverflow & stackingOverride)
			{
				this.AxisMaximumValue = maxValue;
				if (this.Logarithmic && minValue == 0.0)
				{
					this.AxisMinimumValue = Math.Pow(this.LogarithmicBase, 0.0);
				}
				else
				{
					this.AxisMinimumValue = minValue;
				}
			}
			if (isCircularAxis && axisRepresentation == AxisRepresentations.AxisX)
			{
				this.IsCircularAxis = isCircularAxis;
				if (!this._overrideAxisMaximumValue)
				{
					this.AxisMaximumValue = 360.0;
				}
			}
		}

		public void Calculate()
		{
			if (this.AxisRepresentation == AxisRepresentations.AxisY && this._max > 2m && this._max < 10m && this._min >= 0m)
			{
				this._maxNoOfInterval = (int)(++this._max);
			}
			if (this.AxisRepresentation == AxisRepresentations.AxisX)
			{
				if (this._max > 2m && this._max < 10m && this._min >= 0m)
				{
					this._maxNoOfInterval = (int)(++this._max);
				}
				else if (this._max <= 2m && this._min >= 0m && this._max == this._min)
				{
					this._maxNoOfInterval = (int)(++this._max);
				}
				if (this.IsCircularAxis)
				{
					this._maxNoOfInterval = 20;
				}
			}
			if (this._max == this._min && !this.Logarithmic)
			{
				this.CalculateSingle();
				return;
			}
			decimal num5;
			decimal num6;
			decimal num7;
			if (!this.Logarithmic)
			{
				if (this.AxisRepresentation == AxisRepresentations.AxisY)
				{
					if (this._overrideAxisMinimumValue && !this._overrideAxisMaximumValue)
					{
						if (this.AxisMinimumValue >= Convert.ToDouble(this._max) && this.AxisMinimumValue >= 0.0)
						{
							this.AxisMaximumValue = this.AxisMinimumValue + 1.0;
							if (!this._overrideInterval)
							{
								this._interval = 1m;
							}
							return;
						}
						if (this.AxisMinimumValue >= Convert.ToDouble(this._max) && this.AxisMinimumValue < 0.0)
						{
							this.AxisMaximumValue = 0.0;
						}
						else if (this.AxisMinimumValue <= Convert.ToDouble(this._max) && this._max <= 0m)
						{
							this.AxisMaximumValue = 0.0;
						}
					}
					else if (!this._overrideAxisMinimumValue && !this._overrideAxisMaximumValue && this.StartFromZero)
					{
						if (this._max <= 0m)
						{
							this.AxisMaximumValue = 0.0;
						}
						else if ((this._max > 0m && this._min > 0m) || (this._max > 0m && this._min == 0m))
						{
							this.AxisMinimumValue = 0.0;
						}
					}
					else if (this._overrideAxisMaximumValue && !this._overrideAxisMinimumValue)
					{
						if (this.AxisMaximumValue <= Convert.ToDouble(this._min) && this.AxisMaximumValue <= 0.0)
						{
							this.AxisMinimumValue = this.AxisMaximumValue - 1.0;
							if (!this._overrideInterval)
							{
								this._interval = 1m;
							}
							return;
						}
						if (this.AxisMaximumValue <= Convert.ToDouble(this._min) && this.AxisMaximumValue >= 0.0 && this._min > 0m && this._max > 0m)
						{
							this.AxisMinimumValue = 0.0;
							if (!this._overrideInterval)
							{
								this._interval = 1m;
							}
						}
						else if (this.AxisMaximumValue > Convert.ToDouble(this._min) && this.AxisMaximumValue >= 0.0 && this._min > 0m && this._max > 0m)
						{
							this.AxisMinimumValue = 0.0;
							if (!this._overrideInterval)
							{
								this._interval = 1m;
							}
						}
					}
					else if (this._overrideAxisMinimumValue && this._overrideAxisMaximumValue && this.AxisMaximumValue <= this.AxisMinimumValue)
					{
						throw new Exception("AxisMaximum should always be greater than AxisMinimum");
					}
				}
				int num = 0;
				int num2;
				if (this._overrideAxisMaximumValue)
				{
					num2 = this.OrderOfMagnitude(this._axisMaximumValue);
				}
				else
				{
					num2 = this.OrderOfMagnitude(this._max);
				}
				int num3;
				if (this._overrideAxisMinimumValue)
				{
					num3 = this.OrderOfMagnitude(this._axisMinimumValue);
				}
				else
				{
					num3 = this.OrderOfMagnitude(this._min);
				}
				int num4 = (num2 > num3) ? num2 : num3;
				if (this._overrideInterval)
				{
					num5 = this._interval;
				}
				else
				{
					num5 = (decimal)Math.Pow(10.0, (double)(num4 + 1));
				}
				if (this._overrideAxisMaximumValue)
				{
					num6 = this._axisMaximumValue;
				}
				else
				{
					num6 = this.RoundAxisMaximumValue(this._max, num5);
				}
				if (this._overrideAxisMinimumValue)
				{
					num7 = this._axisMinimumValue;
				}
				else if (this._overrideAxisMaximumValue && !this._overrideAxisMinimumValue && this.AxisMaximumValue > 0.0 && this._min == 0m && this._max >= 0m)
				{
					num7 = this._axisMinimumValue;
				}
				else
				{
					num7 = this.RoundAxisMinimumValue(this._min, num5);
				}
				this._interval = num5;
				this._axisMaximumValue = num6;
				this._axisMinimumValue = num7;
				while (++num < 100)
				{
					if (!this._overrideInterval)
					{
						num5 = this.ReduceInterval(num5);
					}
					if (num5 == 0m)
					{
						return;
					}
					if (!this._overrideAxisMaximumValue)
					{
						num6 = this.RoundAxisMaximumValue(this._max, num5);
					}
					if (!this._overrideAxisMinimumValue && (!this._overrideAxisMaximumValue || this._overrideAxisMinimumValue || this.AxisMaximumValue <= 0.0 || !(this._min == 0m) || !(this._max >= 0m)))
					{
						num7 = this.RoundAxisMinimumValue(this._min, num5);
					}
					int num8 = (int)((num6 - num7) / num5);
					if (num8 > this._maxNoOfInterval)
					{
						return;
					}
					this._axisMaximumValue = num6;
					this._axisMinimumValue = num7;
					this._interval = num5;
				}
				return;
			}
			decimal value = 0m;
			if (!double.IsNaN(this.MinimumValue4LogScale))
			{
				value = (decimal)Math.Floor(Math.Log(this.MinimumValue4LogScale, this.LogarithmicBase));
			}
			num5 = (decimal)Math.Ceiling(Math.Log((double)this._max, this.LogarithmicBase));
			double num9 = 1.0;
			double num10 = 1.0;
			int num11 = 0;
			if (this._overrideInterval)
			{
				num9 = (double)this._interval;
			}
			if (this._overrideAxisMaximumValue)
			{
				if (this._axisMaximumValue <= 0m)
				{
					throw new Exception("AxisMaximum should always be positive for Logarithmic charts. Negative or zero values cannot be plotted correctly on Logarithmic charts");
				}
				num6 = this._axisMaximumValue;
			}
			else
			{
				num6 = (decimal)Math.Pow(this.LogarithmicBase, (double)num5);
			}
			if (this._overrideAxisMinimumValue)
			{
				if (this._axisMinimumValue <= 0m)
				{
					throw new Exception("AxisMinimum should always be positive for Logarithmic charts. Negative or zero values cannot be plotted correctly on Logarithmic charts");
				}
				num7 = this._axisMinimumValue;
			}
			else if ((decimal)Math.Pow(this.LogarithmicBase, num9) < this._min)
			{
				if (!double.IsNaN(this.MinimumValue4LogScale))
				{
					num7 = (decimal)Math.Pow(this.LogarithmicBase, (double)value);
				}
				else
				{
					num7 = (decimal)Math.Pow(this.LogarithmicBase, num9);
				}
			}
			else
			{
				num7 = (decimal)Math.Pow(this.LogarithmicBase, 0.0);
			}
			this._axisMaximumValue = (decimal)Math.Log((double)num6, this.LogarithmicBase);
			this._axisMinimumValue = (decimal)Math.Log((double)num7, this.LogarithmicBase);
			if (!this._overrideInterval)
			{
				this._interval = (decimal)num10;
			}
			double num12 = 0.0;
			int num13 = 1;
			while (num12 <= (double)num5)
			{
				if (!((decimal)Math.Log((double)num6, this.LogarithmicBase) <= (decimal)Math.Log((double)this._max, this.LogarithmicBase)))
				{
					this._axisMaximumValue = (decimal)Math.Log((double)num6, this.LogarithmicBase);
					this._axisMinimumValue = (decimal)Math.Log((double)num7, this.LogarithmicBase);
					break;
				}
				double num14 = (double)num13 + (double)(++num11) * num9;
				this._axisMaximumValue = (decimal)Math.Log((double)num6, this.LogarithmicBase);
				this._axisMinimumValue = (decimal)Math.Log((double)num7, this.LogarithmicBase);
				if (!this._overrideInterval)
				{
					this._interval = (decimal)num10;
				}
				if (!this._overrideAxisMinimumValue && (decimal)Math.Pow(this.LogarithmicBase, num14) < this._min)
				{
					num7 = (decimal)Math.Pow(this.LogarithmicBase, num14);
				}
				if (!this._overrideAxisMaximumValue)
				{
					num6 = (decimal)Math.Pow(this.LogarithmicBase, num14);
				}
				if (num14 > (double)this._maxNoOfInterval * num10)
				{
					num10 += 1.0;
				}
				num12 += num9;
			}
			if (!this._overrideAxisMinimumValue && !this._overrideAxisMaximumValue && this._axisMaximumValue == 0m && this._axisMinimumValue == 0m)
			{
				this._axisMaximumValue = 1m;
			}
		}

		private int GetNoOfIntervals()
		{
			return (int)((this._axisMaximumValue - this._axisMinimumValue) / this._interval);
		}

		private decimal RemoveDecimalPoint(decimal number)
		{
			if (this.IsInterger(number))
			{
				return number;
			}
			while (!this.IsInterger(number))
			{
				number *= 10m;
			}
			return number;
		}

		public bool IsInterger(decimal number)
		{
			return decimal.Truncate(number) == number;
		}

		private int IndexOfDecimalPoint(decimal number)
		{
			int num = 0;
			while (!this.IsInterger(number))
			{
				num++;
				number *= 10m;
			}
			return num;
		}

		private decimal RemoveZeroFromInt(decimal number)
		{
			while (number % 10m == 0m)
			{
				number /= 10m;
			}
			return decimal.Truncate(number);
		}

		private int NoOfZeroAtEndInInt(decimal number)
		{
			int num = 0;
			while (number % 10m == 0m)
			{
				num++;
				number /= 10m;
			}
			return num;
		}

		private decimal GetMantissaOrExponent(MantissaOrExponent mantissaOrExponent, decimal number)
		{
			if (mantissaOrExponent == MantissaOrExponent.Exponent)
			{
				decimal d = this.NoOfZeroAtEndInInt(this.RemoveDecimalPoint(number));
				return d - this.IndexOfDecimalPoint(number);
			}
			return this.RemoveZeroFromInt(this.RemoveDecimalPoint(number));
		}

		private int OrderOfMagnitude(decimal number)
		{
			if (number == 0m)
			{
				return 0;
			}
			decimal mantissaOrExponent = this.GetMantissaOrExponent(MantissaOrExponent.Mantissa, number);
			decimal mantissaOrExponent2 = this.GetMantissaOrExponent(MantissaOrExponent.Exponent, number);
			if (number > 0m)
			{
				return mantissaOrExponent.ToString(CultureInfo.InvariantCulture).Length + (int)(--mantissaOrExponent2);
			}
			return mantissaOrExponent.ToString(CultureInfo.InvariantCulture).Length + (int)(--mantissaOrExponent2) - 1;
		}

		private decimal RoundAxisMaximumValue(decimal axisMaxValue, decimal intervalValue)
		{
			if (this.AxisRepresentation == AxisRepresentations.AxisX && this.IsDateTimeAxis && this._max == this._min)
			{
				axisMaxValue = ++axisMaxValue;
				axisMaxValue = decimal.Floor(axisMaxValue);
			}
			else
			{
				axisMaxValue /= intervalValue;
				axisMaxValue = decimal.Floor(axisMaxValue);
				axisMaxValue = ++axisMaxValue * intervalValue;
			}
			return axisMaxValue;
		}

		private decimal RoundAxisMinimumValue(decimal axisMinValue, decimal intervalValue)
		{
			if (this.AxisRepresentation == AxisRepresentations.AxisX && this.IsDateTimeAxis && this._max == this._min)
			{
				axisMinValue = --axisMinValue;
				axisMinValue = (decimal)Math.Ceiling((double)axisMinValue);
			}
			else
			{
				axisMinValue /= intervalValue;
				axisMinValue = (decimal)Math.Ceiling((double)axisMinValue);
				axisMinValue = --axisMinValue * intervalValue;
			}
			return axisMinValue;
		}

		private decimal ReduceInterval(decimal intervalValue)
		{
			decimal mantissaOrExponent = this.GetMantissaOrExponent(MantissaOrExponent.Mantissa, intervalValue);
			if (mantissaOrExponent == 5m)
			{
				return intervalValue * 2m / 5m;
			}
			if (mantissaOrExponent == 2m)
			{
				return intervalValue * 1m / 2m;
			}
			if (mantissaOrExponent == 1m)
			{
				return intervalValue * 5m / 10m;
			}
			return 0m;
		}

		private void CalculateSingle()
		{
			int num = 0;
			if (this._max == 0m && !this._overrideAxisMaximumValue && !this._overrideAxisMinimumValue)
			{
				this._axisMaximumValue = 1m;
				this._axisMinimumValue = 0m;
				if (!this._overrideInterval)
				{
					this._interval = 1m;
				}
				return;
			}
			long num2;
			if (this._min == 0m && this._overrideAxisMaximumValue && !this._overrideAxisMinimumValue)
			{
				if (this._axisMaximumValue == 1m)
				{
					this._axisMinimumValue = 0m;
					if (!this._overrideInterval)
					{
						this._interval = 1m;
					}
					return;
				}
				if (this._axisMaximumValue > 1m)
				{
					if (!(this._min == 0m) || !(this._max == 0m))
					{
						this.AxisMinimumValue = this.AxisMaximumValue - 1.0;
						if (!this._overrideInterval)
						{
							this.Interval = 1.0;
						}
						return;
					}
					this.AxisMinimumValue = 0.0;
				}
				else if (this._axisMaximumValue < 1m)
				{
					this.AxisMinimumValue = this.AxisMaximumValue - 1.0;
					if (!this._overrideInterval)
					{
						this.Interval = 1.0;
					}
					return;
				}
				num2 = (long)this.OrderOfMagnitude(this._axisMaximumValue);
			}
			else if (this._max == 0m && !this._overrideAxisMaximumValue && this._overrideAxisMinimumValue)
			{
				if (this._axisMinimumValue == 0m)
				{
					this._axisMaximumValue = 1m;
					if (!this._overrideInterval)
					{
						this.Interval = 1.0;
					}
					return;
				}
				if (this._axisMinimumValue >= 1m || this._axisMinimumValue < 0m)
				{
					this.AxisMaximumValue = this.AxisMinimumValue + 1.0;
					if (!this._overrideInterval)
					{
						this.Interval = 1.0;
					}
					return;
				}
				num2 = (long)this.OrderOfMagnitude(this._axisMaximumValue);
			}
			else if (this._min == this._max && this._min != 0m && this._max != 0m)
			{
				if (!this._overrideAxisMaximumValue && this._overrideAxisMinimumValue)
				{
					if (this.AxisMinimumValue >= Convert.ToDouble(this._max) && this.AxisMinimumValue >= 0.0)
					{
						this.AxisMaximumValue = this.AxisMinimumValue + 1.0;
						if (!this._overrideInterval)
						{
							this._interval = 1m;
						}
						return;
					}
					if (this.AxisMinimumValue >= Convert.ToDouble(this._max) && this.AxisMinimumValue < 0.0)
					{
						this.AxisMaximumValue = 0.0;
						if (!this._overrideInterval)
						{
							this._interval = 1m;
						}
					}
					else if (this.AxisMinimumValue <= Convert.ToDouble(this._max) && this._max <= 0m)
					{
						this.AxisMaximumValue = 0.0;
					}
					else if (this.AxisMinimumValue <= Convert.ToDouble(this._max) && this._max > 0m)
					{
						this._axisMaximumValue = this._max;
					}
				}
				else if (this._overrideAxisMaximumValue && !this._overrideAxisMinimumValue)
				{
					if (this.AxisMaximumValue <= Convert.ToDouble(this._min) && this.AxisMaximumValue <= 0.0)
					{
						this.AxisMinimumValue = this.AxisMaximumValue - 1.0;
						if (!this._overrideInterval)
						{
							this._interval = 1m;
						}
						return;
					}
					if (this.AxisMaximumValue <= Convert.ToDouble(this._min) && this.AxisMaximumValue >= 0.0 && this._min > 0m && this._max > 0m)
					{
						this.AxisMinimumValue = 0.0;
						if (!this._overrideInterval)
						{
							this._interval = 1m;
						}
					}
					else if (this.AxisMaximumValue > Convert.ToDouble(this._min) && this.AxisMaximumValue >= 0.0 && this._min > 0m && this._max > 0m)
					{
						this.AxisMinimumValue = 0.0;
						if (!this._overrideInterval)
						{
							this._interval = 1m;
						}
					}
					else if (this.AxisMaximumValue > Convert.ToDouble(this._min) && this.AxisMaximumValue >= 0.0 && this._min <= 0m && this._max <= 0m)
					{
						this._axisMinimumValue = this._min;
					}
				}
				else if (!this._overrideAxisMinimumValue && !this._overrideAxisMaximumValue)
				{
					if (this._max <= 0m)
					{
						this.AxisMaximumValue = 0.0;
						if (this._min <= 0m)
						{
							this._axisMinimumValue = this._min;
						}
					}
					else if (this._max > 0m && this._min > 0m && this.StartFromZero)
					{
						this.AxisMinimumValue = 0.0;
						this._axisMaximumValue = this._max;
					}
				}
				else if (this._overrideAxisMaximumValue && this._overrideAxisMinimumValue && this.AxisMinimumValue >= this.AxisMaximumValue)
				{
					throw new Exception("AxisMaximum should always be greater than AxisMinimum");
				}
				if (this._max < 0m && this._min < 0m)
				{
					num2 = (long)this.OrderOfMagnitude(this._axisMinimumValue);
				}
				else
				{
					num2 = (long)this.OrderOfMagnitude(this._axisMaximumValue);
				}
			}
			else if (this._max == 0m && this._min == 0m)
			{
				if (this._axisMinimumValue >= this._axisMaximumValue && this._overrideAxisMaximumValue && this._overrideAxisMinimumValue)
				{
					throw new Exception("AxisMaximum should always be greater than AxisMinimum");
				}
				if (!this._overrideAxisMaximumValue)
				{
					this._axisMaximumValue = 1m;
				}
				if (!this._overrideAxisMinimumValue)
				{
					this._axisMinimumValue = 0m;
				}
				if (this._overrideAxisMinimumValue && !this._overrideAxisMaximumValue && this._axisMinimumValue > this._axisMaximumValue)
				{
					this._axisMinimumValue = 0m;
					if (!this._overrideInterval)
					{
						this._interval = 1m;
						return;
					}
				}
				if (!this._overrideInterval && !this._overrideAxisMaximumValue && !this._overrideAxisMinimumValue)
				{
					this._interval = 1m;
					return;
				}
				num2 = (long)this.OrderOfMagnitude(this._axisMaximumValue);
			}
			else
			{
				num2 = (long)this.OrderOfMagnitude(this._max);
			}
			decimal num3;
			if (this._overrideInterval)
			{
				num3 = this._interval;
			}
			else
			{
				num3 = (decimal)Math.Pow(10.0, (double)(num2 + 1L));
			}
			decimal num4;
			if (this._overrideAxisMaximumValue)
			{
				num4 = this._axisMaximumValue;
			}
			else
			{
				num4 = this.RoundAxisMaximumValue(this._max, num3);
			}
			decimal num5;
			if (this._overrideAxisMinimumValue)
			{
				num5 = this._axisMinimumValue;
			}
			else
			{
				num5 = this.RoundAxisMinimumValue(this._min, num3);
			}
			this._interval = num3;
			this._axisMaximumValue = num4;
			this._axisMinimumValue = num5;
			while (num++ < 100)
			{
				if (!this._overrideInterval)
				{
					num3 = this.ReduceInterval(num3);
				}
				if (num3 == 0m)
				{
					return;
				}
				if (!this._overrideAxisMaximumValue)
				{
					num4 = this.RoundAxisMaximumValue(this._max, num3);
				}
				if (this._min < 0m)
				{
					if (!this._overrideAxisMinimumValue)
					{
						num5 = this.RoundAxisMinimumValue(this._min, num3);
					}
					num4 = this._axisMaximumValue;
				}
				int num6 = (int)((num4 - num5) / num3);
				if (num6 > this._maxNoOfInterval)
				{
					return;
				}
				this._axisMaximumValue = num4;
				this._axisMinimumValue = num5;
				this._interval = num3;
			}
		}
	}
}
