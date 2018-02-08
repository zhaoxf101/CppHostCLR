using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Visifire.Charts
{
	public class Area3DDataPointFace
	{
		private PointCollection _frontFacePoints;

		private PointCollection _backFacePoints;

		private double _depth3D;

		public DependencyObject TopFace;

		public DependencyObject LeftFace;

		public DependencyObject RightFace;

		public double Depth3d
		{
			get
			{
				return this._depth3D;
			}
		}

		public PointCollection FrontFacePoints
		{
			get
			{
				return this._frontFacePoints;
			}
			set
			{
				this._frontFacePoints = value;
			}
		}

		public PointCollection BackFacePoints
		{
			get
			{
				return this._backFacePoints;
			}
		}

		public Area3DDataPointFace(double depth3D)
		{
			this._depth3D = depth3D;
			this._frontFacePoints = new PointCollection();
		}

		public void CalculateBackFacePoints()
		{
			this._backFacePoints = new PointCollection();
			foreach (Point current in this._frontFacePoints)
			{
				this._backFacePoints.Add(new Point(current.X + this._depth3D, current.Y - this._depth3D));
			}
		}

		public PointCollection GetFacePoints()
		{
			PointCollection pointCollection = new PointCollection();
			for (int i = 1; i < this._frontFacePoints.Count; i++)
			{
				pointCollection.Add(this._frontFacePoints[i]);
			}
			for (int j = this._backFacePoints.Count - 1; j >= 0; j--)
			{
				pointCollection.Add(this._backFacePoints[j]);
			}
			return pointCollection;
		}

		public static PathFigure GetPathFigure(Path path)
		{
			PathGeometry pathGeometry = path.Data as PathGeometry;
			return pathGeometry.Figures[0];
		}

		public static LineSegment GetLineSegment(Path path, int index)
		{
			PathGeometry pathGeometry = path.Data as PathGeometry;
			return pathGeometry.Figures[0].Segments[index] as LineSegment;
		}
	}
}
