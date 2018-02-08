using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Visifire.Charts;

namespace Visifire.Commons
{
	internal class AnimationHelper
	{
		public static void ApplyScaleAnimation(ScaleDirection direction, Storyboard storyBoard, FrameworkElement element, double from, double to, TimeSpan duration, double beginTime, bool applyFromValueInitially)
		{
			if (storyBoard == null)
			{
				storyBoard = new Storyboard();
			}
			if (element.RenderTransform == null || !element.RenderTransform.GetType().Equals(typeof(ScaleTransform)))
			{
				element.RenderTransform = new ScaleTransform();
			}
			if (applyFromValueInitially)
			{
				if (direction == ScaleDirection.ScaleX)
				{
					(element.RenderTransform as ScaleTransform).ScaleX = from;
				}
				else
				{
					(element.RenderTransform as ScaleTransform).ScaleY = from;
				}
			}
			DoubleAnimation doubleAnimation = new DoubleAnimation
			{
				From = new double?(from),
				To = new double?(to),
				Duration = new Duration(duration),
				BeginTime = new TimeSpan?(TimeSpan.FromSeconds(beginTime)),
				SpeedRatio = 2.0
			};
			Transform renderTransform = element.RenderTransform;
			string path = (direction == ScaleDirection.ScaleX) ? "(ScaleTransform.ScaleX)" : "(ScaleTransform.ScaleY)";
			Storyboard.SetTarget(doubleAnimation, renderTransform);
			Storyboard.SetTargetProperty(doubleAnimation, new PropertyPath(path, new object[0]));
			Storyboard.SetTargetName(doubleAnimation, (string)element.GetValue(FrameworkElement.NameProperty));
			if (direction == ScaleDirection.ScaleX)
			{
				renderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, doubleAnimation);
			}
			else
			{
				renderTransform.BeginAnimation(ScaleTransform.ScaleYProperty, doubleAnimation);
			}
			storyBoard.Children.Add(doubleAnimation);
		}

		public static void ApplyPointAnimation(Storyboard existingStoryBoard, DependencyObject target, string targetName, string propertyName, Point oldPosition, Point newPosition, double animationTime, double beginTime)
		{
			PointAnimation pointAnimation = new PointAnimation();
			pointAnimation.From = new Point?(oldPosition);
			pointAnimation.To = new Point?(newPosition);
			pointAnimation.SpeedRatio = 2.0;
			pointAnimation.Duration = new Duration(new TimeSpan(0, 0, 0, (int)animationTime * 1000));
			target.SetValue(FrameworkElement.NameProperty, targetName);
			Storyboard.SetTarget(pointAnimation, target);
			Storyboard.SetTargetProperty(pointAnimation, new PropertyPath(propertyName, new object[0]));
			Storyboard.SetTargetName(pointAnimation, (string)target.GetValue(FrameworkElement.NameProperty));
			existingStoryBoard.Children.Add(pointAnimation);
		}

		public static List<KeySpline> GenerateKeySplineList(int count)
		{
			List<KeySpline> list = new List<KeySpline>();
			for (int i = 0; i < count; i++)
			{
				list.Add(AnimationHelper.GetKeySpline(new Point(0.0, 0.0), new Point(1.0, 1.0)));
			}
			return list;
		}

		internal static KeySpline GetKeySpline(Point controlPoint1, Point controlPoint2)
		{
			return new KeySpline
			{
				ControlPoint1 = controlPoint1,
				ControlPoint2 = controlPoint2
			};
		}

		internal static PointAnimationUsingKeyFrames CreatePointAnimation(IElement parentObj, DependencyObject target, string property, double beginTime, DoubleCollection frameTime, PointCollection values, List<KeySpline> splines)
		{
			PointAnimationUsingKeyFrames da = new PointAnimationUsingKeyFrames();
			target.SetValue(FrameworkElement.NameProperty, target.GetType().Name + Guid.NewGuid().ToString().Replace('-', '_'));
			Storyboard.SetTargetName(da, target.GetValue(FrameworkElement.NameProperty).ToString());
			(parentObj as ObservableObject).Chart._rootElement.RegisterName((string)target.GetValue(FrameworkElement.NameProperty), target);
			Storyboard.SetTargetProperty(da, new PropertyPath(property, new object[0]));
			da.BeginTime = new TimeSpan?(TimeSpan.FromSeconds(beginTime));
			for (int i = 0; i < values.Count; i++)
			{
				SplinePointKeyFrame splinePointKeyFrame = new SplinePointKeyFrame();
				if (splines != null)
				{
					splinePointKeyFrame.KeySpline = splines[i];
				}
				if (!double.IsNaN(frameTime[i]))
				{
					splinePointKeyFrame.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(frameTime[i]));
					splinePointKeyFrame.Value = values[i];
					da.KeyFrames.Add(splinePointKeyFrame);
				}
			}
			da.Completed += delegate(object sender, EventArgs e)
			{
				ObservableObject observableObject = parentObj as ObservableObject;
				if (observableObject == null || target == null)
				{
					da.KeyFrames.Clear();
					return;
				}
				if (!(observableObject.Chart is Chart))
				{
					da.KeyFrames.Clear();
					return;
				}
				object obj = (parentObj as ObservableObject).Chart._rootElement.FindName((string)target.GetValue(FrameworkElement.NameProperty));
				if (obj != null)
				{
					(parentObj as ObservableObject).Chart._rootElement.UnregisterName((string)target.GetValue(FrameworkElement.NameProperty));
				}
				da.KeyFrames.Clear();
			};
			return da;
		}

		internal static DoubleAnimationUsingKeyFrames CreateDoubleAnimation(IElement parentObj, DependencyObject target, string property, double beginTime, DoubleCollection frameTime, DoubleCollection values, List<KeySpline> splines)
		{
			DoubleAnimationUsingKeyFrames da = new DoubleAnimationUsingKeyFrames();
			string name = target.GetType().Name + Guid.NewGuid().ToString().Replace('-', '_');
			target.SetValue(FrameworkElement.NameProperty, name);
			Storyboard.SetTargetName(da, target.GetValue(FrameworkElement.NameProperty).ToString());
			if (parentObj.GetType().Equals(typeof(DataPoint)))
			{
				parentObj = (parentObj as DataPoint).Parent;
			}
			(parentObj as ObservableObject).Chart._rootElement.RegisterName(name, target);
			Storyboard.SetTargetProperty(da, new PropertyPath(property, new object[0]));
			da.BeginTime = new TimeSpan?(TimeSpan.FromSeconds(beginTime));
			for (int i = 0; i < values.Count; i++)
			{
				SplineDoubleKeyFrame splineDoubleKeyFrame = new SplineDoubleKeyFrame();
				if (splines != null)
				{
					splineDoubleKeyFrame.KeySpline = splines[i];
				}
				splineDoubleKeyFrame.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(frameTime[i]));
				splineDoubleKeyFrame.Value = values[i];
				da.KeyFrames.Add(splineDoubleKeyFrame);
			}
			da.Completed += delegate(object sender, EventArgs e)
			{
				ObservableObject observableObject = parentObj as ObservableObject;
				if (observableObject == null || target == null)
				{
					da.KeyFrames.Clear();
					return;
				}
				if (!(observableObject.Chart is Chart))
				{
					da.KeyFrames.Clear();
					return;
				}
				object obj = (parentObj as ObservableObject).Chart._rootElement.FindName(name);
				if (obj != null)
				{
					(parentObj as ObservableObject).Chart._rootElement.UnregisterName(name);
				}
				da.KeyFrames.Clear();
			};
			return da;
		}

		internal static Storyboard ApplyOpacityAnimation(Marker marker, IElement parentObj, Storyboard storyboard, double beginTime, double targetValue)
		{
			if (marker != null && parentObj != null)
			{
				return AnimationHelper.ApplyOpacityAnimation(marker.Visual, parentObj, storyboard, beginTime, 0.75, 0.0, targetValue);
			}
			return storyboard;
		}

		internal static Storyboard ApplyOpacityAnimation(FrameworkElement objectToAnimate, IElement parentObj, Storyboard storyboard, double beginTime, double duration, double fromValue, double targetValue)
		{
			if (objectToAnimate != null && parentObj != null)
			{
				DoubleCollection values = Graphics.GenerateDoubleCollection(new double[]
				{
					fromValue,
					fromValue,
					targetValue
				});
				DoubleCollection frameTime = Graphics.GenerateDoubleCollection(new double[]
				{
					0.0,
					beginTime + 0.5,
					duration + beginTime + 0.5
				});
				List<KeySpline> splines = AnimationHelper.GenerateKeySplineList(new Point[]
				{
					new Point(0.0, 0.0),
					new Point(1.0, 1.0),
					new Point(0.0, 0.0),
					new Point(1.0, 1.0),
					new Point(0.0, 0.0),
					new Point(0.5, 1.0)
				});
				DoubleAnimationUsingKeyFrames value = AnimationHelper.CreateDoubleAnimation(parentObj, objectToAnimate, "(UIElement.Opacity)", 0.0, frameTime, values, splines);
				storyboard.Children.Add(value);
			}
			return storyboard;
		}

		internal static Storyboard ApplyPropertyAnimation(DependencyObject objectToAnimate, string property, IElement parentObj, Storyboard storyboard, double beginTime, double[] timeCollection, double[] valueCollection, List<KeySpline> splain)
		{
			if (objectToAnimate != null && parentObj != null)
			{
				DoubleCollection values = Graphics.GenerateDoubleCollection(valueCollection);
				DoubleCollection frameTime = Graphics.GenerateDoubleCollection(timeCollection);
				DoubleAnimationUsingKeyFrames value = AnimationHelper.CreateDoubleAnimation(parentObj, objectToAnimate, property, beginTime, frameTime, values, splain);
				storyboard.Children.Add(value);
			}
			return storyboard;
		}

		internal static Storyboard ApplyPointAnimation(DependencyObject objectToAnimate, string property, IElement parentObj, Storyboard storyboard, double beginTime, double[] timeCollection, Point[] valueCollection, List<KeySpline> splain, double speedRatio)
		{
			if (objectToAnimate != null && parentObj != null)
			{
				PointCollection values = Graphics.GeneratePointCollection(valueCollection);
				DoubleCollection frameTime = Graphics.GenerateDoubleCollection(timeCollection);
				PointAnimationUsingKeyFrames pointAnimationUsingKeyFrames = AnimationHelper.CreatePointAnimation(parentObj, objectToAnimate, property, beginTime, frameTime, values, splain);
				if (!double.IsNaN(speedRatio))
				{
					pointAnimationUsingKeyFrames.SpeedRatio = speedRatio;
				}
				storyboard.Children.Add(pointAnimationUsingKeyFrames);
			}
			return storyboard;
		}

		internal static List<KeySpline> GenerateKeySplineList(params Point[] values)
		{
			List<KeySpline> list = new List<KeySpline>();
			for (int i = 0; i < values.Length; i += 2)
			{
				list.Add(AnimationHelper.GetKeySpline(values[i], values[i + 1]));
			}
			return list;
		}
	}
}
