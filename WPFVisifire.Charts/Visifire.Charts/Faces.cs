using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Visifire.Charts
{
	public class Faces
	{
		public double Width;

		public double Height;

		public List<Shape> BorderElements;

		public List<FrameworkElement> BevelElements;

		public List<FrameworkElement> LightingElements;

		public List<FrameworkElement> ShadowElements;

		public List<FrameworkElement> VisualComponents;

		public List<DependencyObject> Parts;

		public FrameworkElement Visual;

		public Canvas LabelCanvas;

		public object DataContext;

		public Area3DDataPointFace Area3DLeftFace;

		public Area3DDataPointFace Area3DLeftTopFace;

		public Area3DDataPointFace Area3DRightTopFace;

		public Area3DDataPointFace Area3DRightFace;

		public LineSegment AreaFrontFaceLineSegment;

		public LineSegment AreaFrontFaceBaseLineSegment;

		public Line BevelLine;

		public IDataPoint PreviousDataPoint;

		public IDataPoint NextDataPoint;

		public List<Path> FrontFacePaths;

		public bool IsStartPoint;

		public bool IsEndPoint;

		public Faces()
		{
			this.VisualComponents = new List<FrameworkElement>();
			this.BorderElements = new List<Shape>();
			this.BevelElements = new List<FrameworkElement>();
			this.LightingElements = new List<FrameworkElement>();
			this.ShadowElements = new List<FrameworkElement>();
			this.Parts = new List<DependencyObject>();
		}

		internal void ClearInstanceRefs()
		{
			if (this.VisualComponents != null)
			{
				foreach (FrameworkElement current in this.VisualComponents)
				{
					current.Tag = null;
				}
			}
			if (this.BorderElements != null)
			{
				foreach (FrameworkElement current2 in this.BorderElements)
				{
					current2.Tag = null;
				}
			}
			if (this.Parts != null)
			{
				this.Parts = null;
			}
			if (this.Visual != null)
			{
				this.Visual.Tag = null;
			}
			this.ClearFrontArea3DFaces();
			this.BorderElements = null;
			this.BevelElements = null;
			this.LightingElements = null;
			this.ShadowElements = null;
			this.VisualComponents = null;
			this.Parts = null;
			this.Visual = null;
			this.LabelCanvas = null;
			this.DataContext = null;
		}

		public void ClearFrontArea3DFaces()
		{
			this.Area3DLeftFace = null;
			this.Area3DLeftTopFace = null;
			this.Area3DRightTopFace = null;
			this.Area3DRightFace = null;
		}

		internal void ClearList(ref List<DependencyObject> listReference)
		{
			using (List<DependencyObject>.Enumerator enumerator = listReference.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					FrameworkElement frameworkElement = (FrameworkElement)enumerator.Current;
					Panel panel = frameworkElement.Parent as Panel;
					if (panel != null)
					{
						panel.Children.Remove(frameworkElement);
					}
				}
			}
			listReference.Clear();
		}

		internal void ClearList(ref List<FrameworkElement> listReference)
		{
			foreach (FrameworkElement current in listReference)
			{
				Panel panel = current.Parent as Panel;
				if (panel != null)
				{
					panel.Children.Remove(current);
				}
			}
			listReference.Clear();
		}

		public void ClearFronta3DFaces()
		{
			this.Area3DLeftFace = null;
			this.Area3DLeftTopFace = null;
			this.Area3DRightTopFace = null;
			this.Area3DRightFace = null;
		}
	}
}
