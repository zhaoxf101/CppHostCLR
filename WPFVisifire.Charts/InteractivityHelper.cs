using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Visifire.Charts;
using Visifire.Commons;

internal class InteractivityHelper
{
	public static Brush SELECTED_MARKER_BORDER_COLOR;

	public static Brush SELECTED_MARKER_FILL_COLOR;

	private static double OPACITY_FACTOR = 0.6;

	public static void ApplyBorderEffect(Shape shape, BorderStyles borderStyles, double borderThickness, Brush borderColor)
	{
		if (borderColor != null)
		{
			shape.Stroke = borderColor;
			shape.StrokeThickness = borderThickness;
		}
		shape.StrokeStartLineCap = PenLineCap.Round;
		shape.StrokeEndLineCap = PenLineCap.Round;
		shape.StrokeDashOffset = 0.4;
		shape.StrokeLineJoin = PenLineJoin.Bevel;
		shape.StrokeDashArray = Graphics.LineStyleToStrokeDashArray(borderStyles.ToString());
	}

	public static void ApplyBorderEffect(Shape shape, BorderStyles borderStyles, Brush fillColor, double scaleFactor, double borderThickness, Brush borderColor)
	{
		InteractivityHelper.ApplyBorderEffect(shape, borderStyles, borderThickness, borderColor);
		shape.Fill = fillColor;
	}

	public static void RemoveBorderEffect(Shape shape, BorderStyles borderStyle, double borderThickness, Brush lineColor)
	{
		shape.Stroke = lineColor;
		shape.StrokeThickness = borderThickness;
		shape.StrokeStartLineCap = PenLineCap.Flat;
		shape.StrokeEndLineCap = PenLineCap.Flat;
		shape.StrokeDashOffset = 0.0;
		shape.StrokeLineJoin = PenLineJoin.Bevel;
		shape.StrokeDashArray = Graphics.LineStyleToStrokeDashArray(borderStyle.ToString());
	}

	public static void RemoveBorderEffect(Shape shape, BorderStyles borderStyle, double borderThickness, Brush lineColor, Brush fillColor, double width, double height)
	{
		InteractivityHelper.RemoveBorderEffect(shape, borderStyle, borderThickness, lineColor);
		shape.Fill = fillColor;
		shape.Height = height;
		shape.Width = width;
		shape.SetValue(Canvas.TopProperty, -shape.Height / 2.0);
		shape.SetValue(Canvas.LeftProperty, -shape.Width / 2.0);
	}

	public static void ApplyOnMouseOverOpacityInteractivity(FrameworkElement element)
	{
		if (element == null)
		{
			return;
		}
		InteractivityHelper.RemoveOnMouseOverOpacityInteractivity(element, double.NaN);
		element.MouseEnter += new MouseEventHandler(InteractivityHelper.element_MouseEnter);
		element.MouseLeave += new MouseEventHandler(InteractivityHelper.element_MouseLeave);
	}

	public static void ApplyOnMouseOverOpacityInteractivity2Visuals(FrameworkElement element)
	{
		if (element == null)
		{
			return;
		}
		element.MouseEnter += new MouseEventHandler(InteractivityHelper.MultipleElements_MouseEnter);
		element.MouseLeave += new MouseEventHandler(InteractivityHelper.MultipleElements_MouseLeave);
	}

	private static void MultipleElements_MouseLeave(object sender, MouseEventArgs e)
	{
		FrameworkElement frameworkElement = sender as FrameworkElement;
		InteractivityHelper.RemoveOpacity(sender as FrameworkElement);
		if (frameworkElement.Tag == null)
		{
			return;
		}
		IDataPoint dataPoint = (frameworkElement.Tag as ElementData).Element as IDataPoint;
		foreach (FrameworkElement current in dataPoint.DpInfo.Faces.VisualComponents)
		{
			if (current != sender)
			{
				InteractivityHelper.RemoveOpacity(current);
			}
		}
	}

	private static void MultipleElements_MouseEnter(object sender, MouseEventArgs e)
	{
		FrameworkElement frameworkElement = sender as FrameworkElement;
		InteractivityHelper.ApplyOpacity(sender as FrameworkElement);
		if (frameworkElement.Tag == null)
		{
			return;
		}
		IDataPoint dataPoint = (frameworkElement.Tag as ElementData).Element as IDataPoint;
		foreach (FrameworkElement current in dataPoint.DpInfo.Faces.VisualComponents)
		{
			if (current != sender)
			{
				InteractivityHelper.ApplyOpacity(current);
			}
		}
	}

	public static void DetachOpacityPropertyFromAnimation(FrameworkElement fe, double resetTo)
	{
		fe.BeginAnimation(UIElement.OpacityProperty, null);
		fe.Opacity = resetTo;
	}

	public static void RemoveOnMouseOverOpacityInteractivity(FrameworkElement element, double resetOpacity)
	{
		if (element == null)
		{
			return;
		}
		if (!double.IsNaN(resetOpacity))
		{
			element.Opacity = resetOpacity;
		}
		element.MouseEnter -= new MouseEventHandler(InteractivityHelper.element_MouseEnter);
		element.MouseLeave -= new MouseEventHandler(InteractivityHelper.element_MouseLeave);
		element.MouseEnter -= new MouseEventHandler(InteractivityHelper.MultipleElements_MouseEnter);
		element.MouseLeave -= new MouseEventHandler(InteractivityHelper.MultipleElements_MouseLeave);
	}

	private static void element_MouseLeave(object sender, MouseEventArgs e)
	{
		InteractivityHelper.RemoveOpacity(sender as FrameworkElement);
	}

	private static void element_MouseEnter(object sender, MouseEventArgs e)
	{
		InteractivityHelper.ApplyOpacity(sender as FrameworkElement);
	}

	private static void ApplyOpacity(FrameworkElement obj)
	{
		if (obj != null)
		{
			obj.Opacity *= InteractivityHelper.OPACITY_FACTOR;
		}
	}

	private static void RemoveOpacity(FrameworkElement obj)
	{
		if (obj != null)
		{
			obj.Opacity /= InteractivityHelper.OPACITY_FACTOR;
		}
	}
}
