using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Visifire.Commons
{
	public class LabelPlacementHelper
	{
		public static double LABEL_LINE_GAP = 3.0;

		internal static Rect[] VerticalLabelPlacement(Rect area, ref Rect[] labelsInfo)
		{
			int num = 0;
			while (num < labelsInfo.Length && num != labelsInfo.Length - 1)
			{
				if (LabelPlacementHelper.CheckOverlap(labelsInfo[num], labelsInfo[num + 1]))
				{
					double num2 = LabelPlacementHelper.CalculateShiftValue(labelsInfo[num], labelsInfo[num + 1]);
					int num3 = num;
					double num4 = LabelPlacementHelper.FindSpaceTowardsTopAtIndex(ref num3, num2 * 2.0, area, labelsInfo);
					if (num4 != 0.0 || num3 != -1)
					{
						LabelPlacementHelper.ShiftTowardsTop(num2 * 2.0, num, num3, ref labelsInfo);
					}
					else
					{
						num3 = num;
						num4 = LabelPlacementHelper.FindSpaceTowardsBottomAtIndex(ref num3, num2 * 2.0, area, labelsInfo);
						if (num4 != 0.0 || num3 != -1)
						{
							if (num == num3)
							{
								LabelPlacementHelper.ShiftTowardsBottom(num2 * 2.0, num, num3, ref labelsInfo);
							}
							else
							{
								LabelPlacementHelper.ShiftTowardsBottom(num2 * 2.0, num + 1, num3, ref labelsInfo);
							}
						}
					}
				}
				num++;
			}
			num = 0;
			while (num < labelsInfo.Length && num != labelsInfo.Length - 1)
			{
				if (LabelPlacementHelper.CheckOverlap(labelsInfo[num], labelsInfo[num + 1]))
				{
					double num5 = LabelPlacementHelper.CalculateShiftValue(labelsInfo[num], labelsInfo[num + 1]) * 2.0;
					int num6 = num;
					while (num6 > 0 && num5 > 0.0)
					{
						double num7 = LabelPlacementHelper.CheckSpaceAtTop(num6, area, labelsInfo);
						if (num7 > 0.0 && num5 > num7)
						{
							LabelPlacementHelper.ShiftTowardsTop(num7, num, num6, ref labelsInfo);
							num5 -= num7;
						}
						num6--;
					}
				}
				num++;
			}
			num = 0;
			while (num < labelsInfo.Length && num != labelsInfo.Length - 1)
			{
				if (LabelPlacementHelper.CheckOverlap(labelsInfo[num], labelsInfo[num + 1]))
				{
					double num8 = LabelPlacementHelper.CalculateShiftValue(labelsInfo[num], labelsInfo[num + 1]) * 2.0;
					int num9 = num + 1;
					while (num9 < labelsInfo.Length && num8 > 0.0)
					{
						double num10;
						if (num9 == labelsInfo.Length - 1)
						{
							num10 = num8;
						}
						else
						{
							num10 = LabelPlacementHelper.CheckSpaceAtBottom(num9, area, labelsInfo);
						}
						if (num10 > 0.0 && num8 >= num10)
						{
							LabelPlacementHelper.ShiftTowardsBottom(num10, num + 1, num9, ref labelsInfo);
							num8 -= num10;
						}
						num9++;
					}
				}
				num++;
			}
			return labelsInfo;
		}

		private static double FindSpaceTowardsTopAtIndex(ref int index, double spaceRequired, Rect area, Rect[] labelsInfo)
		{
			double num = 0.0;
			while (index >= 0)
			{
				double num2 = LabelPlacementHelper.CheckSpaceAtTop(index, area, labelsInfo);
				if (spaceRequired <= num2)
				{
					num = num2;
					break;
				}
				index--;
			}
			if (num == 0.0)
			{
				index = -1;
			}
			return num;
		}

		private static double FindSpaceTowardsBottomAtIndex(ref int index, double spaceRequired, Rect area, Rect[] labelsInfo)
		{
			double num = 0.0;
			while (index < labelsInfo.Length - 1)
			{
				double num2 = LabelPlacementHelper.CheckSpaceAtBottom(index, area, labelsInfo);
				if (spaceRequired <= num2)
				{
					num = num2;
					break;
				}
				index++;
			}
			if (num == 0.0)
			{
				index = -1;
			}
			return num;
		}

		private static double CheckSpaceAtTop(int index, Rect baseArea, Rect[] labelsInfo)
		{
			if (index == 0)
			{
				if (labelsInfo[index].Top < 0.0)
				{
					return 0.0;
				}
				return labelsInfo[index].Top;
			}
			else
			{
				if (labelsInfo.Length <= 1)
				{
					return 0.0;
				}
				if (labelsInfo[index].Top > labelsInfo[index - 1].Top + labelsInfo[index - 1].Height)
				{
					return labelsInfo[index].Top - (labelsInfo[index - 1].Top + labelsInfo[index - 1].Height);
				}
				return 0.0;
			}
		}

		private static void ShiftTowardsTop(double value, int startIndex, ref Rect[] labelsInfos)
		{
			for (int i = startIndex; i >= 0; i--)
			{
				Rect[] expr_0C_cp_0 = labelsInfos;
				int expr_0C_cp_1 = i;
				expr_0C_cp_0[expr_0C_cp_1].Y = expr_0C_cp_0[expr_0C_cp_1].Y - value;
			}
		}

		private static void ShiftTowardsTop(double value, int fromIndex, int toIndex, ref Rect[] labelsInfos)
		{
			for (int i = fromIndex; i >= toIndex; i--)
			{
				Rect[] expr_0C_cp_0 = labelsInfos;
				int expr_0C_cp_1 = i;
				expr_0C_cp_0[expr_0C_cp_1].Y = expr_0C_cp_0[expr_0C_cp_1].Y - value;
			}
		}

		private static void ShiftTowardsTop(double value, ref Rect labelsInfo)
		{
			labelsInfo.Y -= value;
		}

		private static void ShiftTowardsBottom(double value, int fromIndex, int toIndex, ref Rect[] labelsInfos)
		{
			int num = fromIndex;
			while (num <= toIndex && toIndex < labelsInfos.Length)
			{
				Rect[] expr_0C_cp_0 = labelsInfos;
				int expr_0C_cp_1 = num;
				expr_0C_cp_0[expr_0C_cp_1].Y = expr_0C_cp_0[expr_0C_cp_1].Y + value;
				num++;
			}
		}

		private static void ShiftTowardsBottom(double value, int startIndex, ref Rect[] labelsInfos)
		{
			for (int i = startIndex; i < labelsInfos.Length; i++)
			{
				Rect[] expr_0C_cp_0 = labelsInfos;
				int expr_0C_cp_1 = i;
				expr_0C_cp_0[expr_0C_cp_1].Y = expr_0C_cp_0[expr_0C_cp_1].Y + value;
			}
		}

		private static void ShiftTowardsBottom(double value, ref Rect labelsInfo)
		{
			labelsInfo.Y += value;
		}

		private static double CheckSpaceAtBottom(int index, Rect baseArea, Rect[] labelsInfo)
		{
			if (index == labelsInfo.Length - 1)
			{
				double num = baseArea.Height - (labelsInfo[index].Top + labelsInfo[index].Height);
				if (num <= 0.0)
				{
					return 0.0;
				}
				return num;
			}
			else
			{
				if (labelsInfo.Length <= 1)
				{
					return 0.0;
				}
				if (labelsInfo[index + 1].Top > labelsInfo[index].Top + labelsInfo[index].Height)
				{
					return labelsInfo[index + 1].Top - (labelsInfo[index].Top + labelsInfo[index].Height);
				}
				return 0.0;
			}
		}

		private static bool CheckOverlap(Rect areaInfo1, Rect areaInfo2)
		{
			return areaInfo1.Top + areaInfo1.Height > areaInfo2.Top;
		}

		private static double CalculateShiftValue(Rect areaInfo1, Rect areaInfo2)
		{
			if (areaInfo1.Top + areaInfo1.Height > areaInfo2.Top)
			{
				return Math.Abs((areaInfo1.Top + areaInfo1.Height - areaInfo2.Top) / 2.0);
			}
			return 0.0;
		}

		private static void GetLeftAndRightLabels(List<CircularLabel> labels, out List<CircularLabel> labelsAtLeft, out List<CircularLabel> labelsAtRight)
		{
			List<CircularLabel> collection = (from cl in labels
			where cl.CurrentMeanAngle >= 4.71238898038469
			orderby cl.CurrentMeanAngle
			select cl).ToList<CircularLabel>();
			List<CircularLabel> collection2 = (from cl in labels
			where cl.CurrentMeanAngle >= 0.0 && cl.CurrentMeanAngle <= 1.5707963267948966
			orderby cl.CurrentMeanAngle
			select cl).ToList<CircularLabel>();
			labelsAtRight = new List<CircularLabel>();
			labelsAtRight.AddRange(collection);
			labelsAtRight.AddRange(collection2);
			labelsAtLeft = (from cl in labels
			where cl.CurrentMeanAngle > 1.5707963267948966 && cl.CurrentMeanAngle < 4.71238898038469
			orderby cl.CurrentMeanAngle
			select cl).ToList<CircularLabel>();
		}

		private static void RearrangeLabelsVertically(List<CircularLabel> labels, double leftOfArea, double topOfArea, double areaHeight, double areaWidth)
		{
			Rect[] array = new Rect[labels.Count];
			int num = 0;
			foreach (CircularLabel current in labels)
			{
				double x = (double)current.LabelVisual.GetValue(Canvas.LeftProperty);
				double y = (double)current.LabelVisual.GetValue(Canvas.TopProperty);
				array[num++] = new Rect(x, y, current.LabelVisual.Width, current.LabelVisual.Height);
			}
			LabelPlacementHelper.VerticalLabelPlacement(new Rect(leftOfArea, topOfArea, areaWidth, areaHeight), ref array);
			num = 0;
			foreach (CircularLabel current2 in labels)
			{
				double num2 = array[num].Top + topOfArea;
				double num3 = array[num].Left;
				double num4 = current2.CalculateAngleByYCoordinate(num2);
				double num5 = 0.0;
				if (!double.IsNaN(num4))
				{
					num4 = CircularLabel.ResetMeanAngle(num4);
					current2.Position = current2.GetCartesianCoordinates4Labels(num4);
					if (num3 < current2.Center.X)
					{
						num3 = current2.Center.X - (current2.Position.X - current2.Center.X);
					}
					else
					{
						num3 = current2.Position.X;
					}
					current2.CurrentMeanAngle = num4;
				}
				if (num2 > current2.YRadiusLabel * 2.0 / 6.0 && num2 < 5.0 * (current2.YRadiusLabel * 2.0) / 6.0)
				{
					if (num3 < current2.Center.X)
					{
						double num6 = num3 - current2.LabelVisual.Width;
						if (num6 > 0.0)
						{
							num5 = -num6 / 3.0;
						}
					}
					else
					{
						double num7 = areaWidth - (num3 + current2.LabelVisual.Width);
						if (num7 > 0.0)
						{
							num5 = num7 / 3.0;
						}
					}
				}
				current2.Position = new Point(num3 + num5, num2);
				current2.UpdateLabelVisualPosition();
				num++;
			}
		}

		public static void CircularLabelPlacment(Rect boundingArea, List<CircularLabel> labels, bool skippingEnabled)
		{
			List<CircularLabel> list = new List<CircularLabel>();
			List<CircularLabel> list2;
			List<CircularLabel> list3;
			LabelPlacementHelper.GetLeftAndRightLabels(labels, out list2, out list3);
			list.AddRange(list3);
			list.AddRange(list2);
			list[0].IsFirst = true;
			list[list.Count - 1].IsLast = true;
			foreach (CircularLabel current in labels)
			{
				current.PlaceLabel(false);
			}
			if (skippingEnabled)
			{
				foreach (CircularLabel current2 in labels)
				{
					current2.PlaceLabel(true);
				}
			}
			foreach (CircularLabel current3 in labels)
			{
				current3.UpdateLabelVisualPosition();
			}
			List<CircularLabel> list4 = (from lb in labels
			where !lb.IsSkiped
			select lb).ToList<CircularLabel>();
			LabelPlacementHelper.GetLeftAndRightLabels(list4, out list2, out list3);
			list2.Reverse();
			LabelPlacementHelper.RearrangeLabelsVertically(list3, double.NaN, 0.0, boundingArea.Height, boundingArea.Width);
			LabelPlacementHelper.RearrangeLabelsVertically(list2, double.NaN, 0.0, boundingArea.Height, boundingArea.Width);
			foreach (CircularLabel current4 in list4)
			{
				double arg_18C_0 = (double)current4.LabelVisual.GetValue(Canvas.LeftProperty);
				double num = (double)current4.LabelVisual.GetValue(Canvas.TopProperty);
				if (num + current4.Height > boundingArea.Height || current4.CheckOutOfBounds())
				{
					if (skippingEnabled)
					{
						current4.SkipLabel();
					}
					else
					{
						current4.Position = current4.GetCartesianCoordinates4Labels(current4.BaseMeanAngle);
						current4.UpdateLabelVisualPosition();
					}
				}
			}
		}
	}
}
