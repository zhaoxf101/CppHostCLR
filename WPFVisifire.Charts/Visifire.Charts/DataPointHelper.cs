using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Shapes;
using Visifire.Commons;
using Visifire.Commons.Controls;

namespace Visifire.Charts
{
    internal static class DataPointHelper
    {
        internal static void InvokeUpdateVisual(IDataPoint dp, VcProperties property, object newValue)
        {
            if (dp.DpInfo.IsNotificationEnable)
            {
                if (dp.Parent == null || !ObservableObject.ValidatePartialUpdate(dp.Parent, property))
                {
                    return;
                }
                DataPointHelper.UpdateVisual(dp, property, newValue, false);
            }
        }

        internal static void SetHref2DataPointVisualFaces(IDataPoint dp)
        {
            if (dp.DpInfo.Faces != null)
            {
                if (dp.DpInfo.Faces.VisualComponents.Count != 0)
                {
                    using (List<FrameworkElement>.Enumerator enumerator = dp.DpInfo.Faces.VisualComponents.GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                        {
                            FrameworkElement current = enumerator.Current;
                            if (!dp.IsLightDataPoint)
                            {
                                (dp as DataPoint).AttachHref(dp.Chart, current, (dp as DataPoint).ParsedHref, (dp as DataPoint).HrefTarget.Value);
                            }
                        }
                        goto IL_DF;
                    }
                }
                if (!dp.IsLightDataPoint)
                {
                    (dp as DataPoint).AttachHref(dp.Chart, dp.DpInfo.Faces.Visual, (dp as DataPoint).ParsedHref, (dp as DataPoint).HrefTarget.Value);
                }
            }
            IL_DF:
            if (dp.DpInfo.Marker != null && !dp.IsLightDataPoint)
            {
                (dp as DataPoint).AttachHref(dp.Chart, dp.DpInfo.Marker.Visual, (dp as DataPoint).ParsedHref, (dp as DataPoint).HrefTarget.Value);
            }
        }

        internal static void AttachEventForSelection(IDataPoint dp, bool selectionEnabled)
        {
            if (dp.Parent != null && selectionEnabled && !dp.IsLightDataPoint)
            {
                dp.DpInfo.IsNotificationEnable = false;
                (dp as DataPoint).MouseLeftButtonUp -= new MouseButtonEventHandler(DataPointHelper.dataPoint_selectOrDeselect);
                (dp as DataPoint).AttachEventWithHighPriority("MouseLeftButtonUp", new MouseButtonEventHandler(DataPointHelper.dataPoint_selectOrDeselect));
                dp.DpInfo.IsNotificationEnable = true;
            }
        }

        internal static void DetachEventForSelection(IDataPoint dp)
        {
            if (!dp.IsLightDataPoint)
            {
                dp.DpInfo.IsNotificationEnable = false;
                (dp as DataPoint).MouseLeftButtonUp -= new MouseButtonEventHandler(DataPointHelper.dataPoint_selectOrDeselect);
                dp.DpInfo.IsNotificationEnable = true;
            }
        }

        private static void dataPoint_selectOrDeselect(object sender, MouseButtonEventArgs e)
        {
            IDataPoint dataPoint = sender as IDataPoint;
            if (!dataPoint.IsLightDataPoint)
            {
                (dataPoint as DataPoint).Selected = (dataPoint.Parent != null && dataPoint.Parent.SelectionEnabled && !(dataPoint as DataPoint).Selected);
            }
        }

        internal static void BindData(IDataPoint dp, object dataSource, DataMappingCollection dataMappings)
        {
            foreach (DataMapping current in dataMappings)
            {
                DataPointHelper.UpdateBindProperty(dp, current, dataSource);
            }
            INotifyPropertyChanged iNotifyPropertyChanged = dataSource as INotifyPropertyChanged;
            if (iNotifyPropertyChanged != null)
            {
                dp.DpFunc._weakEventListener = new WeakEventListener<IDataPoint, object, PropertyChangedEventArgs>(dp);
                dp.DpFunc._weakEventListener.OnEventAction = delegate (IDataPoint instance, object source, PropertyChangedEventArgs eventArgs)
                {
                    instance.DpFunc.iNotifyPropertyChanged_PropertyChanged(source, eventArgs);
                };
                dp.DpFunc._weakEventListener.OnDetachAction = delegate (WeakEventListener<IDataPoint, object, PropertyChangedEventArgs> weakEventListener)
                {
                    iNotifyPropertyChanged.PropertyChanged -= new PropertyChangedEventHandler(weakEventListener.OnEvent);
                };
                iNotifyPropertyChanged.PropertyChanged += new PropertyChangedEventHandler(dp.DpFunc._weakEventListener.OnEvent);
            }
        }

        internal static void UpdateBindProperty(IDataPoint dp, DataMapping dm, object dataSource)
        {
            string memberName;
            if ((memberName = dm.MemberName) != null && (memberName == "Open" || memberName == "Close" || memberName == "High" || memberName == "Low"))
            {
                double[] array = new double[4];
                if (dp.YValues != null)
                {
                    dp.YValues.CopyTo(array, 0);
                }
                int num = (int)Enum.Parse(typeof(OCHL), dm.MemberName, true);
                array[num] = (double)dm.GetPropertyValue(dataSource);
                dp.YValues = array;
                return;
            }
            dm.Map(dataSource, dp);
        }

        internal static void ResetBindProperty(IDataPoint dp, DataMapping dm, object dataSource)
        {
            double[] array = new double[4];
            if (dp.YValues != null)
            {
                dp.YValues.CopyTo(array, 0);
            }
            string memberName;
            if ((memberName = dm.MemberName) != null && (memberName == "Open" || memberName == "Close" || memberName == "High" || memberName == "Low"))
            {
                if (dp.YValues == null)
                {
                    int num = (int)Enum.Parse(typeof(OCHL), dm.MemberName, true);
                    dp.YValues[num] = 0.0;
                    return;
                }
            }
            else
            {
                dp.GetType().GetProperty(dm.MemberName).SetValue(dp, null, null);
            }
        }

        internal static void ClearInstanceRefs(IDataPoint dp)
        {
            if (dp.DpInfo.Storyboard != null)
            {
                dp.DpInfo.Storyboard.FillBehavior = FillBehavior.Stop;
                dp.DpInfo.Storyboard.Stop();
                dp.DpInfo.Storyboard = null;
            }
        }

        internal static void SetCursor2DataPointVisualFaces(IDataPoint dp)
        {
            if (DataPointHelper.GetDataPointValueFromProperty(dp, VcProperties.Cursor) == null)
            {
                return;
            }
            if (dp.DpInfo.Faces != null)
            {
                if (dp.DpInfo.Faces.VisualComponents.Count != 0)
                {
                    using (List<FrameworkElement>.Enumerator enumerator = dp.DpInfo.Faces.VisualComponents.GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                        {
                            FrameworkElement current = enumerator.Current;
                            current.Cursor = DataPointHelper.GetCursor(dp);
                        }
                        goto IL_A4;
                    }
                }
                if (dp.DpInfo.Faces.Visual != null)
                {
                    dp.DpInfo.Faces.Visual.Cursor = DataPointHelper.GetCursor(dp);
                }
            }
            IL_A4:
            if (dp.DpInfo.Marker != null && dp.DpInfo.Marker.Visual != null)
            {
                dp.DpInfo.Marker.Visual.Cursor = DataPointHelper.GetCursor(dp);
            }
        }

        private static Cursor GetCursor(IDataPoint dp)
        {
            if (DataPointHelper.GetDataPointValueFromProperty(dp, VcProperties.Cursor) != null)
            {
                return (Cursor)DataPointHelper.GetDataPointValueFromProperty(dp, VcProperties.Cursor);
            }
            if (dp.Parent != null && dp.Parent.Cursor == null)
            {
                return Cursors.Arrow;
            }
            return dp.Parent.Cursor;
        }

        internal static void AttachEventChanged(IDataPoint dp)
        {
            if (!dp.DpInfo.LightWeight && !dp.IsLightDataPoint)
            {
                (dp as DataPoint).EventChanged += delegate (object A_1, EventArgs A_2)
                {
                    dp.FirePropertyChanged(VcProperties.MouseEvent);
                };
            }
        }

        internal static void BindStyleAttribute(IDataPoint dp)
        {
        }

        internal static void OnSelectedChanged(IDataPoint d, DataPointPropertyChangedEventArgs e)
        {
            DataPointHelper.ApplySelectionChanged(d, (bool)e.NewValue);
        }

        private static void ApplySelectionChanged(IDataPoint dataPoint, bool selectedValue)
        {
            if (dataPoint.Parent == null)
            {
                return;
            }
            bool flag = selectedValue & dataPoint.Parent.SelectionEnabled;
            if (dataPoint.DpInfo.IsNotificationEnable && dataPoint.Parent.ListOfSelectedDataPoints != null)
            {
                dataPoint.Parent.ListOfSelectedDataPoints.Add(dataPoint);
            }
            if (flag)
            {
                DataPointHelper.Select(dataPoint, true);
                if (dataPoint.Parent.SelectionMode == SelectionModes.Single || dataPoint.Parent.RenderAs == RenderAs.SectionFunnel || dataPoint.Parent.RenderAs == RenderAs.StreamLineFunnel || dataPoint.Parent.RenderAs == RenderAs.Pyramid)
                {
                    DataPointHelper.DeSelectOthers(dataPoint);
                    return;
                }
            }
            else
            {
                DataPointHelper.DeSelect(dataPoint, true, true);
            }
        }

        internal static void OnHrefTargetChanged(IDataPoint d, DataPointPropertyChangedEventArgs e)
        {
            DataPointHelper.InvokeUpdateVisual(d, VcProperties.HrefTarget, e.NewValue);
        }

        internal static void OnHrefChanged(IDataPoint d, DataPointPropertyChangedEventArgs e)
        {
            d.DpFunc.OnHrefPropertyChanged((d as DataPoint).Href);
            DataPointHelper.InvokeUpdateVisual(d, VcProperties.Href, e.NewValue);
        }

        internal static void OnOpacityPropertyChanged(IDataPoint d, DataPointPropertyChangedEventArgs e)
        {
            DataPointHelper.InvokeUpdateVisual(d, VcProperties.Opacity, e.NewValue);
        }

        internal static void OnYValuePropertyChanged(IDataPoint d, DataPointPropertyChangedEventArgs e)
        {
            double num = (double)e.NewValue;
            double num2 = (double)e.OldValue;
            if (d.Chart == null || (d.Chart as Chart).ChartArea == null)
            {
                d.DpInfo.OldYValue = num;
                return;
            }
            double num3 = double.IsNaN(num2) ? d.DpInfo.OldYValue : num2;
            d.DpInfo.OldYValue = (double.IsNaN(num3) ? 0.0 : num3);
            if ((d.Chart as Chart).Series.Count == 1 && d.Parent.ShowInLegend.Value && (d.Parent.IncludeYValueInLegend || d.Parent.IncludePercentageInLegend))
            {
                d.FirePropertyChanged(VcProperties.YValue);
                return;
            }
            if (!d.Parent.RenderAs.Equals(RenderAs.CandleStick) && !d.Parent.RenderAs.Equals(RenderAs.Stock))
            {
                DataPointHelper.InvokeUpdateVisual(d, VcProperties.YValue, num);
            }
        }

        internal static void OnYValuesPropertyChanged(IDataPoint d, DataPointPropertyChangedEventArgs e)
        {
            d.DpInfo.OldYValues = (double[])e.OldValue;
            if (d.Parent != null)
            {
                if (d.Parent.RenderAs.Equals(RenderAs.CandleStick) || d.Parent.RenderAs.Equals(RenderAs.Stock))
                {
                    DataPointHelper.InvokeUpdateVisual(d, VcProperties.YValues, e.NewValue);
                    return;
                }
            }
            else
            {
                DataPointHelper.InvokeUpdateVisual(d, VcProperties.YValues, e.NewValue);
            }
        }

        internal static void OnZValuePropertyChanged(IDataPoint d, DataPointPropertyChangedEventArgs e)
        {
            DataPointHelper.InvokeUpdateVisual(d, VcProperties.ZValue, e.NewValue);
        }

        internal static void OnAxisXLabelPropertyChanged(IDataPoint d, DataPointPropertyChangedEventArgs e)
        {
            d.FirePropertyChanged(VcProperties.AxisXLabel);
        }

        internal static void OnColorPropertyChanged(IDataPoint d, DataPointPropertyChangedEventArgs e)
        {
            DataPointHelper.InvokeUpdateVisual(d, VcProperties.Color, e.NewValue);
        }

        internal static void OnStickColorPropertyChanged(IDataPoint d, DataPointPropertyChangedEventArgs e)
        {
            DataPointHelper.InvokeUpdateVisual(d, VcProperties.StickColor, e.NewValue);
        }

        internal static void OnEnabledPropertyChanged(IDataPoint d, DataPointPropertyChangedEventArgs e)
        {
            DataPointHelper.InvokeUpdateVisual(d, VcProperties.Enabled, e.NewValue);
        }

        internal static void OnExplodedPropertyChanged(IDataPoint d, DataPointPropertyChangedEventArgs e)
        {
            DataSeries parent = d.Parent;
            if (parent == null)
            {
                return;
            }
            Chart chart = parent.Chart as Chart;
            if (d.DpInfo.IsNotificationEnable && chart != null && chart.ChartArea != null && chart.ChartArea.PlotDetails != null && chart.ChartArea.PlotDetails.ChartOrientation == ChartOrientationType.NoAxis)
            {
                if (chart.ChartArea._isAnimationFired && (d.Parent.RenderAs == RenderAs.SectionFunnel || d.Parent.RenderAs == RenderAs.StreamLineFunnel || d.Parent.RenderAs == RenderAs.Pyramid))
                {
                    if (!d.Parent.Exploded)
                    {
                        DataPointHelper.ExplodeOrUnexplodeAnimation(d);
                        return;
                    }
                }
                else
                {
                    if (chart.InternalAnimationEnabled && chart.ChartArea._isFirstTimeRender && !chart.ChartArea._isAnimationFired)
                    {
                        return;
                    }
                    DataPointHelper.ExplodeOrUnexplodeAnimation(d);
                }
            }
        }

        internal static void OnLightingEnabledPropertyChanged(IDataPoint d, DataPointPropertyChangedEventArgs e)
        {
            DataPointHelper.InvokeUpdateVisual(d, VcProperties.LightingEnabled, e.NewValue);
        }

        internal static void OnEffectPropertyChanged(IDataPoint d, DataPointPropertyChangedEventArgs e)
        {
            DataPointHelper.InvokeUpdateVisual(d, VcProperties.Effect, e.NewValue);
        }

        internal static void OnShadowEnabledPropertyChanged(IDataPoint d, DataPointPropertyChangedEventArgs e)
        {
            DataPointHelper.InvokeUpdateVisual(d, VcProperties.ShadowEnabled, e.NewValue);
        }

        internal static void OnLabelEnabledPropertyChanged(IDataPoint d, DataPointPropertyChangedEventArgs e)
        {
            DataPointHelper.InvokeUpdateVisual(d, VcProperties.LabelEnabled, e.NewValue);
        }

        internal static void OnLabelTextPropertyChanged(IDataPoint d, DataPointPropertyChangedEventArgs e)
        {
            DataPointHelper.InvokeUpdateVisual(d, VcProperties.LabelText, e.NewValue);
        }

        internal static void OnLabelFontFamilyPropertyChanged(IDataPoint d, DataPointPropertyChangedEventArgs e)
        {
            DataPointHelper.InvokeUpdateVisual(d, VcProperties.LabelFontFamily, e.NewValue);
        }

        internal static void OnLabelFontSizePropertyChanged(IDataPoint d, DataPointPropertyChangedEventArgs e)
        {
            DataPointHelper.InvokeUpdateVisual(d, VcProperties.LabelFontSize, e.NewValue);
        }

        internal static void OnLabelFontColorPropertyChanged(IDataPoint d, DataPointPropertyChangedEventArgs e)
        {
            DataPointHelper.InvokeUpdateVisual(d, VcProperties.LabelFontColor, e.NewValue);
        }

        internal static void OnLabelFontWeightPropertyChanged(IDataPoint d, DataPointPropertyChangedEventArgs e)
        {
            DataPointHelper.InvokeUpdateVisual(d, VcProperties.LabelFontWeight, e.NewValue);
        }

        internal static void OnLabelFontStylePropertyChanged(IDataPoint d, DataPointPropertyChangedEventArgs e)
        {
            DataPointHelper.InvokeUpdateVisual(d, VcProperties.LabelFontStyle, e.NewValue);
        }

        internal static void OnLabelBackgroundPropertyChanged(IDataPoint d, DataPointPropertyChangedEventArgs e)
        {
            DataPointHelper.InvokeUpdateVisual(d, VcProperties.LabelBackground, e.NewValue);
        }

        internal static void OnLabelStylePropertyChanged(IDataPoint d, DataPointPropertyChangedEventArgs e)
        {
            DataPointHelper.InvokeUpdateVisual(d, VcProperties.LabelStyle, e.NewValue);
        }

        internal static void OnLabelAnglePropertyChanged(IDataPoint d, DataPointPropertyChangedEventArgs e)
        {
            d.FirePropertyChanged(VcProperties.LabelAngle);
        }

        internal static void OnLabelLineEnabledPropertyChanged(IDataPoint d, DataPointPropertyChangedEventArgs e)
        {
            DataPointHelper.InvokeUpdateVisual(d, VcProperties.LabelLineEnabled, e.NewValue);
        }

        internal static void OnLabelLineColorPropertyChanged(IDataPoint d, DataPointPropertyChangedEventArgs e)
        {
            DataPointHelper.InvokeUpdateVisual(d, VcProperties.LabelLineColor, e.NewValue);
        }

        internal static void OnLabelLineThicknessPropertyChanged(IDataPoint d, DataPointPropertyChangedEventArgs e)
        {
            DataPointHelper.InvokeUpdateVisual(d, VcProperties.LabelLineThickness, e.NewValue);
        }

        internal static void OnLabelLineStylePropertyChanged(IDataPoint d, DataPointPropertyChangedEventArgs e)
        {
            DataPointHelper.InvokeUpdateVisual(d, VcProperties.LabelLineStyle, e.NewValue);
        }

        internal static void OnMarkerEnabledPropertyChanged(IDataPoint d, DataPointPropertyChangedEventArgs e)
        {
            DataPointHelper.InvokeUpdateVisual(d, VcProperties.MarkerEnabled, e.NewValue);
        }

        internal static void OnMarkerTypePropertyChanged(IDataPoint d, DataPointPropertyChangedEventArgs e)
        {
            DataPointHelper.InvokeUpdateVisual(d, VcProperties.MarkerType, e.NewValue);
        }

        internal static void OnMarkerBorderThicknessPropertyChanged(IDataPoint d, DataPointPropertyChangedEventArgs e)
        {
            DataPointHelper.InvokeUpdateVisual(d, VcProperties.MarkerBorderThickness, e.NewValue);
        }

        internal static void OnMarkerBorderColorPropertyChanged(IDataPoint d, DataPointPropertyChangedEventArgs e)
        {
            DataPointHelper.InvokeUpdateVisual(d, VcProperties.MarkerBorderColor, e.NewValue);
        }

        internal static void OnMarkerSizePropertyChanged(IDataPoint d, DataPointPropertyChangedEventArgs e)
        {
            DataPointHelper.InvokeUpdateVisual(d, VcProperties.MarkerSize, e.NewValue);
        }

        internal static void OnMarkerColorPropertyChanged(IDataPoint d, DataPointPropertyChangedEventArgs e)
        {
            DataPointHelper.InvokeUpdateVisual(d, VcProperties.MarkerColor, e.NewValue);
        }

        internal static void OnMarkerScalePropertyChanged(IDataPoint d, DataPointPropertyChangedEventArgs e)
        {
            DataPointHelper.InvokeUpdateVisual(d, VcProperties.MarkerScale, e.NewValue);
        }

        internal static void OnShowInLegendPropertyChanged(IDataPoint d, DataPointPropertyChangedEventArgs e)
        {
            d.FirePropertyChanged(VcProperties.ShowInLegend);
        }

        internal static void OnLegendTextPropertyChanged(IDataPoint d, DataPointPropertyChangedEventArgs e)
        {
            DataPointHelper.InvokeUpdateVisual(d, VcProperties.LegendText, e.NewValue);
        }

        internal static void OnBorderThicknessPropertyChanged(IDataPoint d, DataPointPropertyChangedEventArgs e)
        {
            DataPointHelper.InvokeUpdateVisual(d, VcProperties.BorderThickness, e.NewValue);
            d.FirePropertyChanged(VcProperties.BorderThickness);
        }

        internal static void OnBorderColorPropertyChanged(IDataPoint d, DataPointPropertyChangedEventArgs e)
        {
            DataPointHelper.InvokeUpdateVisual(d, VcProperties.BorderColor, e.NewValue);
        }

        internal static void OnLegendMarkerColorPropertyChanged(IDataPoint d, DataPointPropertyChangedEventArgs e)
        {
            DataPointHelper.InvokeUpdateVisual(d, VcProperties.LegendMarkerColor, e.NewValue);
        }

        internal static void OnLegendMarkerTypePropertyChanged(IDataPoint d, DataPointPropertyChangedEventArgs e)
        {
            DataPointHelper.InvokeUpdateVisual(d, VcProperties.LegendMarkerType, e.NewValue);
        }

        internal static void OnBorderStylePropertyChanged(IDataPoint d, DataPointPropertyChangedEventArgs e)
        {
            DataPointHelper.InvokeUpdateVisual(d, VcProperties.BorderStyle, e.NewValue);
        }

        internal static void OnRadiusXPropertyChanged(IDataPoint d, DataPointPropertyChangedEventArgs e)
        {
            DataPointHelper.InvokeUpdateVisual(d, VcProperties.RadiusX, e.NewValue);
        }

        internal static void OnRadiusYPropertyChanged(IDataPoint d, DataPointPropertyChangedEventArgs e)
        {
            DataPointHelper.InvokeUpdateVisual(d, VcProperties.RadiusY, e.NewValue);
        }

        internal static void OnXValuePropertyChanged(IDataPoint d, DataPointPropertyChangedEventArgs e)
        {
            if (e.NewValue == null)
            {
                return;
            }
            if (e.NewValue.GetType().Equals(typeof(DateTime)))
            {
                d.DpInfo.InternalXValueAsDateTime = (DateTime)e.NewValue;
                d.DpInfo.ActualXValueAsDateTime = (DateTime)e.NewValue;
                d.DpInfo.XValueType = ChartValueTypes.DateTime;
            }
            else if (e.NewValue.GetType().Equals(typeof(string)))
            {
                double actualNumericXValue;
                if (string.IsNullOrEmpty(e.NewValue.ToString()))
                {
                    d.DpInfo.ActualNumericXValue = double.NaN;
                    d.DpInfo.XValueType = ChartValueTypes.Numeric;
                }
                else if (double.TryParse((string)e.NewValue, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingSign | NumberStyles.AllowTrailingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands | NumberStyles.AllowExponent, CultureInfo.InvariantCulture, out actualNumericXValue))
                {
                    d.DpInfo.ActualNumericXValue = actualNumericXValue;
                    d.DpInfo.XValueType = ChartValueTypes.Numeric;
                }
                else
                {
                    DateTime dateTime;
                    if (!DateTime.TryParse((string)e.NewValue, CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime))
                    {
                        throw new Exception("Invalid Input for XValue");
                    }
                    d.DpInfo.InternalXValueAsDateTime = dateTime;
                    d.DpInfo.ActualXValueAsDateTime = dateTime;
                    d.DpInfo.XValueType = ChartValueTypes.DateTime;
                }
            }
            else
            {
                try
                {
                    object value = Convert.ChangeType(e.NewValue, typeof(double), CultureInfo.InvariantCulture);
                    d.DpInfo.ActualNumericXValue = Convert.ToDouble(value);
                    d.DpInfo.XValueType = ChartValueTypes.Numeric;
                }
                catch
                {
                    throw new Exception("Invalid Input for XValue");
                }
            }
            DataPointHelper.InvokeUpdateVisual(d, VcProperties.XValue, e.NewValue);
        }

        internal static double ConvertXValue2LogarithmicValue(IDataPoint dataPoint, bool isLog, double logBase)
        {
            if (!isLog)
            {
                return Convert.ToDouble(dataPoint.DpInfo.ActualNumericXValue);
            }
            Axis axis = null;
            if (dataPoint.Parent != null && dataPoint.Parent.PlotGroup != null)
            {
                axis = dataPoint.Parent.PlotGroup.AxisX;
            }
            if (axis.IsDateTimeAxis)
            {
                throw new Exception("DateTime values cannot be plotted correctly on logarithmic charts");
            }
            return Math.Log(dataPoint.DpInfo.ActualNumericXValue, logBase);
        }

        internal static double[] ConvertYValues2LogarithmicValue(IDataPoint dataPoint, bool isLog, double logBase)
        {
            double[] yValues = dataPoint.YValues;
            if (yValues != null)
            {
                List<double> list = new List<double>();
                if (isLog)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        if (i >= yValues.Count<double>())
                        {
                            break;
                        }
                        if (!double.IsNaN(yValues[i]))
                        {
                            list.Add(Math.Log(yValues[i], logBase));
                        }
                        else
                        {
                            list.Add(0.0);
                        }
                    }
                }
                else
                {
                    int num = 0;
                    while (num < 4 && num < yValues.Count<double>())
                    {
                        if (!double.IsNaN(yValues[num]))
                        {
                            list.Add(yValues[num]);
                        }
                        else
                        {
                            list.Add(0.0);
                        }
                        num++;
                    }
                }
                return list.ToArray();
            }
            return null;
        }

        internal static double ConvertYValue2LogarithmicValue(IDataPoint dataPoint, bool isLog, double logBase)
        {
            if (isLog)
            {
                return Math.Log(dataPoint.YValue, logBase);
            }
            return dataPoint.YValue;
        }

        internal static double ConvertYValue2LogarithmicValue(Chart chart, double yValue, AxisTypes axisType)
        {
            if (chart != null && chart.ChartArea != null)
            {
                if (chart.ChartArea.AxisY != null && chart.ChartArea.AxisY.AxisType == axisType)
                {
                    if (chart.ChartArea.AxisY.Logarithmic)
                    {
                        yValue = Math.Log(yValue, chart.ChartArea.AxisY.LogarithmBase);
                    }
                }
                else if (chart.ChartArea.AxisY2 != null && chart.ChartArea.AxisY2.AxisType == axisType && chart.ChartArea.AxisY2.Logarithmic)
                {
                    yValue = Math.Log(yValue, chart.ChartArea.AxisY2.LogarithmBase);
                }
            }
            return yValue;
        }

        internal static double ConvertYValue2LogarithmicValue(DataSeries dataSeries, double yValue)
        {
            Axis axis = null;
            if (dataSeries != null && dataSeries.PlotGroup != null)
            {
                axis = dataSeries.PlotGroup.AxisY;
            }
            Chart chart = dataSeries.Chart as Chart;
            double result;
            if (axis != null && axis.Logarithmic && chart != null && chart.PlotDetails != null && chart.PlotDetails.ChartOrientation != ChartOrientationType.NoAxis)
            {
                result = Math.Log(yValue, axis.LogarithmBase);
            }
            else
            {
                result = yValue;
            }
            return result;
        }

        internal static double ConvertXValue2LogarithmicValue(Chart chart, double xValue, AxisTypes axisType)
        {
            if (chart != null && chart.ChartArea != null && chart.ChartArea.AxisX != null && chart.ChartArea.AxisX.AxisType == axisType && chart.ChartArea.AxisX.Logarithmic)
            {
                xValue = Math.Log(xValue, chart.ChartArea.AxisX.LogarithmBase);
            }
            return xValue;
        }

        internal static double ConvertLogarithmicValue2ActualValue(Chart chart, double logValue, AxisTypes axisType)
        {
            if (chart != null && chart.ChartArea != null)
            {
                if (chart.ChartArea.AxisY != null && chart.ChartArea.AxisY.AxisType == axisType)
                {
                    if (chart.ChartArea.AxisY.Logarithmic)
                    {
                        logValue = Math.Pow(chart.ChartArea.AxisY.LogarithmBase, logValue);
                    }
                }
                else if (chart.ChartArea.AxisY2 != null && chart.ChartArea.AxisY2.AxisType == axisType && chart.ChartArea.AxisY2.Logarithmic)
                {
                    logValue = Math.Pow(chart.ChartArea.AxisY2.LogarithmBase, logValue);
                }
            }
            return logValue;
        }

        internal static void DeSelectOthers(IDataPoint dataPoint)
        {
            if (dataPoint.Parent != null)
            {
                foreach (IDataPoint current in dataPoint.Parent.DataPoints)
                {
                    if (current != dataPoint)
                    {
                        DataPointHelper.DeSelect(current, false, true);
                    }
                }
            }
        }

        internal static void Select(IDataPoint dataPoint, bool allowPropertyChange)
        {
            if (dataPoint.DpInfo.Faces != null)
            {
                if (allowPropertyChange)
                {
                    DataPointHelper.UpdateExplodedPropertyForSelection(dataPoint, true, true);
                }
                foreach (Shape current in dataPoint.DpInfo.Faces.BorderElements)
                {
                    BorderStyles borderStyles = BorderStyles.Dashed;
                    double borderThickness = 1.5;
                    if (dataPoint.Parent != null)
                    {
                        if ((dataPoint.Parent.RenderAs == RenderAs.Pie || dataPoint.Parent.RenderAs == RenderAs.Doughnut) && (dataPoint.Chart as Chart).View3D)
                        {
                            borderStyles = BorderStyles.Dotted;
                        }
                        if ((dataPoint.Parent.RenderAs == RenderAs.Column || dataPoint.Parent.RenderAs == RenderAs.Bar) && !(dataPoint.Chart as Chart).View3D)
                        {
                            borderThickness = 1.5;
                        }
                    }
                    Brush borderColor;
                    if (dataPoint.Parent.RenderAs == RenderAs.Stock)
                    {
                        borderColor = StockChart.ReCalculateAndApplyTheNewBrush(current, dataPoint.Color, (bool)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LightingEnabled));
                    }
                    else
                    {
                        borderColor = Chart.CalculateDataPointLabelFontColor(dataPoint.Chart as Chart, dataPoint, null, LabelStyles.OutSide);
                    }
                    InteractivityHelper.ApplyBorderEffect(current, borderStyles, borderThickness, borderColor);
                }
            }
            if (dataPoint.Parent != null && dataPoint.DpInfo.Marker != null && (RenderHelper.IsLineCType(dataPoint.Parent) || RenderHelper.IsAreaCType(dataPoint.Parent)))
            {
                if (allowPropertyChange)
                {
                    DataPointHelper.UpdateExplodedPropertyForSelection(dataPoint, true, true);
                }
                if (InteractivityHelper.SELECTED_MARKER_BORDER_COLOR == null)
                {
                    InteractivityHelper.SELECTED_MARKER_BORDER_COLOR = Graphics.RED_BRUSH;
                }
                if (InteractivityHelper.SELECTED_MARKER_FILL_COLOR == null)
                {
                    InteractivityHelper.SELECTED_MARKER_FILL_COLOR = null;
                }
                InteractivityHelper.ApplyBorderEffect(dataPoint.DpInfo.Marker.MarkerShape, BorderStyles.Solid, InteractivityHelper.SELECTED_MARKER_BORDER_COLOR, 1.2, 2.4, InteractivityHelper.SELECTED_MARKER_FILL_COLOR);
                if (RenderHelper.IsLineCType(dataPoint.Parent))
                {
                    LineChart.SelectMovingMarker(dataPoint);
                }
            }
        }

        internal static void DeSelect(IDataPoint dataPoint, bool selfDeSelect, bool allowPropertyChange)
        {
            if (dataPoint.IsLightDataPoint)
            {
                return;
            }
            if (dataPoint.DpInfo.Faces != null)
            {
                if (allowPropertyChange)
                {
                    DataPointHelper.UpdateExplodedPropertyForSelection(dataPoint, false, selfDeSelect);
                }
                foreach (Shape current in dataPoint.DpInfo.Faces.BorderElements)
                {
                    if (dataPoint.Parent != null && (dataPoint.Parent.RenderAs == RenderAs.Pie || dataPoint.Parent.RenderAs == RenderAs.Doughnut))
                    {
                        InteractivityHelper.RemoveBorderEffect(current, (BorderStyles)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.BorderStyle), (dataPoint as DataPoint).InternalBorderThickness.Left, (Brush)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.BorderColor));
                    }
                    else
                    {
                        Brush lineColor;
                        if (dataPoint.Parent.RenderAs == RenderAs.Stock)
                        {
                            lineColor = StockChart.ReCalculateAndApplyTheNewBrush(current, dataPoint.Color, (bool)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LightingEnabled));
                        }
                        else
                        {
                            lineColor = (Brush)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.BorderColor);
                        }
                        InteractivityHelper.RemoveBorderEffect(current, (BorderStyles)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.BorderStyle), (dataPoint as DataPoint).InternalBorderThickness.Left, lineColor);
                    }
                    if (allowPropertyChange)
                    {
                        dataPoint.DpInfo.IsNotificationEnable = false;
                        (dataPoint as DataPoint).Selected = false;
                        dataPoint.DpInfo.IsNotificationEnable = true;
                    }
                }
            }
            if (dataPoint.Parent != null && dataPoint.DpInfo.Marker != null && (RenderHelper.IsLineCType(dataPoint.Parent) || RenderHelper.IsAreaCType(dataPoint.Parent)))
            {
                if (allowPropertyChange)
                {
                    DataPointHelper.UpdateExplodedPropertyForSelection(dataPoint, false, selfDeSelect);
                }
                if (dataPoint.DpInfo.Marker != null)
                {
                    if ((bool)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.MarkerEnabled))
                    {
                        InteractivityHelper.RemoveBorderEffect(dataPoint.DpInfo.Marker.MarkerShape, (BorderStyles)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.BorderStyle), dataPoint.DpInfo.Marker.BorderThickness, dataPoint.DpInfo.Marker.BorderColor, dataPoint.DpInfo.Marker.MarkerFillColor, dataPoint.DpInfo.Marker.MarkerSize.Width * dataPoint.DpInfo.Marker.ScaleFactor, dataPoint.DpInfo.Marker.MarkerSize.Height * dataPoint.DpInfo.Marker.ScaleFactor);
                        dataPoint.DpInfo.Marker.MarkerShape.Margin = new Thickness(0.0, 0.0, 0.0, 0.0);
                    }
                    else
                    {
                        InteractivityHelper.RemoveBorderEffect(dataPoint.DpInfo.Marker.MarkerShape, (BorderStyles)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.BorderStyle), dataPoint.DpInfo.Marker.BorderThickness, dataPoint.DpInfo.Marker.BorderColor, dataPoint.DpInfo.Marker.MarkerFillColor, dataPoint.DpInfo.Marker.MarkerSize.Width * dataPoint.DpInfo.Marker.ScaleFactor, dataPoint.DpInfo.Marker.MarkerSize.Height * dataPoint.DpInfo.Marker.ScaleFactor);
                        dataPoint.DpInfo.Marker.MarkerShape.Margin = new Thickness(0.0, 0.0, 0.0, 0.0);
                        dataPoint.DpInfo.Marker.MarkerShape.Fill = new SolidColorBrush(Colors.Transparent);
                    }
                }
                if (allowPropertyChange)
                {
                    dataPoint.DpInfo.IsNotificationEnable = false;
                    (dataPoint as DataPoint).Selected = false;
                    dataPoint.DpInfo.IsNotificationEnable = true;
                }
            }
        }

        public static string GetHrefFromDataPoint(IDataPoint dp)
        {
            if (!dp.IsLightDataPoint)
            {
                return (dp as DataPoint).Href;
            }
            return (dp as LightDataPoint).Parent.Href;
        }

        public static HrefTargets? GetHrefTargetFromDataPoint(IDataPoint dp)
        {
            if (!dp.IsLightDataPoint)
            {
                return (dp as DataPoint).HrefTarget;
            }
            return new HrefTargets?((dp as LightDataPoint).Parent.HrefTarget);
        }

        public static double GetOpacityFromDataPoint(IDataPoint dp)
        {
            if (!dp.IsLightDataPoint)
            {
                return (dp as DataPoint).Opacity;
            }
            return (dp as LightDataPoint).Parent.Opacity;
        }

        public static Cursor GetCursorFromDataPoint(IDataPoint dp)
        {
            if (!dp.IsLightDataPoint)
            {
                return (dp as DataPoint).Cursor;
            }
            return (dp as LightDataPoint).Parent.Cursor;
        }

        public static FontFamily GetLabelFontFamilyFromDataPoint(IDataPoint dp)
        {
            if (!dp.IsLightDataPoint)
            {
                return (dp as DataPoint).LabelFontFamily;
            }
            return (dp as LightDataPoint).Parent.LabelFontFamily;
        }

        public static double? GetLabelFontSizeFromDataPoint(IDataPoint dp)
        {
            if (!dp.IsLightDataPoint)
            {
                return (dp as DataPoint).LabelFontSize;
            }
            return (dp as LightDataPoint).Parent.LabelFontSize;
        }

        public static FontWeight? GetLabelFontWeightFromDataPoint(IDataPoint dp)
        {
            if (!dp.IsLightDataPoint)
            {
                return (dp as DataPoint).LabelFontWeight;
            }
            return new FontWeight?((dp as LightDataPoint).Parent.LabelFontWeight);
        }

        public static FontStyle? GetLabelFontStyleFromDataPoint(IDataPoint dp)
        {
            if (!dp.IsLightDataPoint)
            {
                return (dp as DataPoint).LabelFontStyle;
            }
            return new FontStyle?((dp as LightDataPoint).Parent.LabelFontStyle);
        }

        public static Brush GetLabelFontColorFromDataPoint(IDataPoint dp)
        {
            if (!dp.IsLightDataPoint)
            {
                return (dp as DataPoint).LabelFontColor;
            }
            return (dp as LightDataPoint).Parent.LabelFontColor;
        }

        public static Brush GetLabelBackgroundFromDataPoint(IDataPoint dp)
        {
            if (!dp.IsLightDataPoint)
            {
                return (dp as DataPoint).LabelBackground;
            }
            return (dp as LightDataPoint).Parent.LabelBackground;
        }

        public static double GetLabelAngleFromDataPoint(IDataPoint dp)
        {
            if (!dp.IsLightDataPoint)
            {
                return (dp as DataPoint).LabelAngle;
            }
            return (dp as LightDataPoint).Parent.LabelAngle;
        }

        public static LabelStyles? GetLabelStyleFromDataPoint(IDataPoint dp)
        {
            if (!dp.IsLightDataPoint)
            {
                return (dp as DataPoint).LabelStyle;
            }
            return (dp as LightDataPoint).Parent.LabelStyle;
        }

        public static Brush GetLabelLineColorFromDataPoint(IDataPoint dp)
        {
            if (!dp.IsLightDataPoint)
            {
                return (dp as DataPoint).LabelLineColor;
            }
            return (dp as LightDataPoint).Parent.LabelLineColor;
        }

        public static double? GetLabelLineThicknessFromDataPoint(IDataPoint dp)
        {
            if (!dp.IsLightDataPoint)
            {
                return (dp as DataPoint).LabelLineThickness;
            }
            double labelLineThickness = (dp as LightDataPoint).Parent.LabelLineThickness;
            if (labelLineThickness == 0.0)
            {
                return new double?(0.5);
            }
            return new double?(labelLineThickness);
        }

        public static LineStyles? GetLabelLineStyleFromDataPoint(IDataPoint dp)
        {
            if (!dp.IsLightDataPoint)
            {
                return (dp as DataPoint).LabelLineStyle;
            }
            return new LineStyles?((dp as LightDataPoint).Parent.LabelLineStyle);
        }

        public static bool? GetMarkerEnabledFromDataPoint(IDataPoint dp)
        {
            if (!dp.IsLightDataPoint)
            {
                return (dp as DataPoint).MarkerEnabled;
            }
            return (dp as LightDataPoint).Parent.MarkerEnabled;
        }

        public static double? GetMarkerScaleFromDataPoint(IDataPoint dp)
        {
            if (!dp.IsLightDataPoint)
            {
                return (dp as DataPoint).MarkerScale;
            }
            return (dp as LightDataPoint).Parent.MarkerScale;
        }

        public static Brush GetMarkerBorderColorFromDataPoint(IDataPoint dp)
        {
            if (!dp.IsLightDataPoint)
            {
                return (dp as DataPoint).MarkerBorderColor;
            }
            Brush brush = (dp as LightDataPoint).Parent.MarkerBorderColor;
            if (brush == null)
            {
                Brush brush2 = (dp.DpInfo.InternalColor == null) ? dp.Parent._internalColor : dp.DpInfo.InternalColor;
                brush = brush2;
            }
            return brush;
        }

        public static Thickness? GetMarkerBorderThicknessFromDataPoint(IDataPoint dp)
        {
            if (!dp.IsLightDataPoint)
            {
                return (dp as DataPoint).MarkerBorderThickness;
            }
            Thickness? markerBorderThickness = (dp as LightDataPoint).Parent.MarkerBorderThickness;
            if (markerBorderThickness.HasValue)
            {
                return markerBorderThickness;
            }
            RenderAs renderAs = (dp as LightDataPoint).Parent.RenderAs;
            MarkerTypes markerType = (dp as LightDataPoint).Parent.MarkerType;
            if (renderAs == RenderAs.Point && markerType == MarkerTypes.Cross)
            {
                return new Thickness?(new Thickness(1.0));
            }
            if (renderAs == RenderAs.Bubble || (renderAs == RenderAs.Point && markerType != MarkerTypes.Cross))
            {
                return new Thickness?(new Thickness(0.0));
            }
            return new Thickness?(new Thickness((dp as LightDataPoint).Parent.MarkerSize.Value / 6.0));
        }

        public static Brush GetMarkerColorFromDataPoint(IDataPoint dp)
        {
            if (!dp.IsLightDataPoint)
            {
                return (dp as DataPoint).MarkerColor;
            }
            Brush brush = (dp as LightDataPoint).Parent.MarkerColor;
            if (brush == null)
            {
                brush = Graphics.WHITE_BRUSH;
            }
            return brush;
        }

        public static double? GetMarkerSizeFromDataPoint(IDataPoint dp)
        {
            if (!dp.IsLightDataPoint)
            {
                return (dp as DataPoint).MarkerSize;
            }
            return (dp as LightDataPoint).Parent.MarkerSize;
        }

        public static MarkerTypes? GetMarkerTypeFromDataPoint(IDataPoint dp)
        {
            if (!dp.IsLightDataPoint)
            {
                return (dp as DataPoint).MarkerType;
            }
            return new MarkerTypes?((dp as LightDataPoint).Parent.MarkerType);
        }

        public static Effect GetEffectFromDataPoint(IDataPoint dp)
        {
            if (!dp.IsLightDataPoint)
            {
                return (dp as DataPoint).Effect;
            }
            return (dp as LightDataPoint).Parent.Effect;
        }

        public static Brush GetBorderColorFromDataPoint(IDataPoint dp)
        {
            if (!dp.IsLightDataPoint)
            {
                return (dp as DataPoint).BorderColor;
            }
            return (dp as LightDataPoint).Parent.BorderColor;
        }

        public static BorderStyles? GetBorderStyleFromDataPoint(IDataPoint dp)
        {
            if (!dp.IsLightDataPoint)
            {
                return (dp as DataPoint).BorderStyle;
            }
            return new BorderStyles?((dp as LightDataPoint).Parent.BorderStyle);
        }

        public static Thickness GetBorderThicknessFromDataPoint(IDataPoint dp)
        {
            if (!dp.IsLightDataPoint)
            {
                return (dp as DataPoint).BorderThickness;
            }
            return (dp as LightDataPoint).Parent.InternalBorderThickness;
        }

        public static CornerRadius? GetRadiusXFromDataPoint(IDataPoint dp)
        {
            if (!dp.IsLightDataPoint)
            {
                return (dp as DataPoint).RadiusX;
            }
            return new CornerRadius?((dp as LightDataPoint).Parent.RadiusX);
        }

        public static CornerRadius? GetRadiusYFromDataPoint(IDataPoint dp)
        {
            if (!dp.IsLightDataPoint)
            {
                return (dp as DataPoint).RadiusY;
            }
            return new CornerRadius?((dp as LightDataPoint).Parent.RadiusY);
        }

        public static bool? GetShadowEnabledFromDataPoint(IDataPoint dp)
        {
            if (!dp.IsLightDataPoint)
            {
                return (dp as DataPoint).ShadowEnabled;
            }
            return (dp as LightDataPoint).Parent.ShadowEnabled;
        }

        public static bool? GetLightingEnabledFromDataPoint(IDataPoint dp)
        {
            if (!dp.IsLightDataPoint)
            {
                return (dp as DataPoint).LightingEnabled;
            }
            return (dp as LightDataPoint).Parent.LightingEnabled;
        }

        public static bool? GetShowInLegendFromDataPoint(IDataPoint dp)
        {
            if (!dp.IsLightDataPoint)
            {
                return (dp as DataPoint).ShowInLegend;
            }
            return (dp as LightDataPoint).Parent.ShowInLegend;
        }

        public static string GetLegendTextFromDataPoint(IDataPoint dp)
        {
            if (!dp.IsLightDataPoint)
            {
                return (dp as DataPoint).LegendText;
            }
            return (dp as LightDataPoint).Parent.LegendText;
        }

        public static Brush GetLegendMarkerColorFromDataPoint(IDataPoint dp)
        {
            if (!dp.IsLightDataPoint)
            {
                return (dp as DataPoint).LegendMarkerColor;
            }
            return (dp as LightDataPoint).Parent.LegendMarkerColor;
        }

        public static MarkerTypes? GetLegendMarkerTypeFromDataPoint(IDataPoint dp)
        {
            if (!dp.IsLightDataPoint)
            {
                return (dp as DataPoint).LegendMarkerType;
            }
            return (dp as LightDataPoint).Parent.LegendMarkerType;
        }

        public static bool? GetLabelLineEnabledFromDataPoint(IDataPoint dp)
        {
            if (!dp.IsLightDataPoint)
            {
                return (dp as DataPoint).LabelLineEnabled;
            }
            bool value = false;
            if (dp.Parent != null)
            {
                RenderAs renderAs = dp.Parent.RenderAs;
                value = ((renderAs == RenderAs.Pie || renderAs == RenderAs.Doughnut || renderAs == RenderAs.StreamLineFunnel || renderAs == RenderAs.SectionFunnel || renderAs == RenderAs.Pyramid) && (LabelStyles)DataPointHelper.GetDataPointValueFromProperty(dp, VcProperties.LabelStyle) == LabelStyles.OutSide && (!(dp as LightDataPoint).Parent.LabelLineEnabled.HasValue || dp.Parent.LabelLineEnabled.Value));
            }
            return new bool?(value);
        }

        public static bool? GetLabelEnabledFromDataPoint(IDataPoint dp)
        {
            if (!dp.IsLightDataPoint)
            {
                return (dp as DataPoint).LabelEnabled;
            }
            return (dp as LightDataPoint).Parent.LabelEnabled;
        }

        public static string GetLabelTextFromDataPoint(IDataPoint dp)
        {
            if (!dp.IsLightDataPoint)
            {
                return (dp as DataPoint).LabelText;
            }
            return (dp as LightDataPoint).Parent.LabelText;
        }

        public static Brush GetStickColorFromDataPoint(IDataPoint dp)
        {
            if (!dp.IsLightDataPoint)
            {
                return (dp as DataPoint).StickColor;
            }
            return (dp as LightDataPoint).Parent.StickColor;
        }

        internal static object GetDataPointValueFromProperty(IDataPoint dp, VcProperties property)
        {
            object result = null;
            if (property <= VcProperties.Effect)
            {
                switch (property)
                {
                    case VcProperties.BorderColor:
                        result = DataPointHelper.GetBorderColorFromDataPoint(dp);
                        break;
                    case VcProperties.BorderStyle:
                        result = DataPointHelper.GetBorderStyleFromDataPoint(dp);
                        break;
                    case VcProperties.BorderThickness:
                        result = DataPointHelper.GetBorderThicknessFromDataPoint(dp);
                        break;
                    default:
                        if (property != VcProperties.Cursor)
                        {
                            if (property == VcProperties.Effect)
                            {
                                result = DataPointHelper.GetEffectFromDataPoint(dp);
                            }
                        }
                        else
                        {
                            result = DataPointHelper.GetCursorFromDataPoint(dp);
                        }
                        break;
                }
            }
            else if (property <= VcProperties.Opacity)
            {
                switch (property)
                {
                    case VcProperties.Href:
                        result = DataPointHelper.GetHrefFromDataPoint(dp);
                        break;
                    case VcProperties.HrefTarget:
                        result = DataPointHelper.GetHrefTargetFromDataPoint(dp);
                        break;
                    case VcProperties.Inlines:
                    case VcProperties.IncludeZero:
                    case VcProperties.InterlacedColor:
                    case VcProperties.Interval:
                    case VcProperties.IntervalType:
                    case VcProperties.IncludeYValueInLegend:
                    case VcProperties.IncludePercentageInLegend:
                    case VcProperties.IncludeDataPointsInLegend:
                    case VcProperties.LabelMargin:
                    case VcProperties.Legend:
                    case VcProperties.LightWeight:
                    case VcProperties.LineCap:
                    case VcProperties.LineFill:
                    case VcProperties.LineColor:
                    case VcProperties.LineStyle:
                    case VcProperties.LineTension:
                    case VcProperties.LineThickness:
                    case VcProperties.LineEnabled:
                    case VcProperties.Logarithmic:
                    case VcProperties.LogarithmBase:
                    case VcProperties.Margin:
                        break;
                    case VcProperties.LabelBackground:
                        result = DataPointHelper.GetLabelBackgroundFromDataPoint(dp);
                        break;
                    case VcProperties.LabelEnabled:
                        result = DataPointHelper.GetLabelEnabledFromDataPoint(dp);
                        break;
                    case VcProperties.LabelFontColor:
                        result = DataPointHelper.GetLabelFontColorFromDataPoint(dp);
                        break;
                    case VcProperties.LabelFontFamily:
                        result = DataPointHelper.GetLabelFontFamilyFromDataPoint(dp);
                        break;
                    case VcProperties.LabelFontSize:
                        result = DataPointHelper.GetLabelFontSizeFromDataPoint(dp);
                        break;
                    case VcProperties.LabelFontStyle:
                        result = DataPointHelper.GetLabelFontStyleFromDataPoint(dp);
                        break;
                    case VcProperties.LabelFontWeight:
                        result = DataPointHelper.GetLabelFontWeightFromDataPoint(dp);
                        break;
                    case VcProperties.LabelLineColor:
                        result = DataPointHelper.GetLabelLineColorFromDataPoint(dp);
                        break;
                    case VcProperties.LabelLineEnabled:
                        result = DataPointHelper.GetLabelLineEnabledFromDataPoint(dp);
                        break;
                    case VcProperties.LabelLineStyle:
                        result = DataPointHelper.GetLabelLineStyleFromDataPoint(dp);
                        break;
                    case VcProperties.LabelLineThickness:
                        result = DataPointHelper.GetLabelLineThicknessFromDataPoint(dp);
                        break;
                    case VcProperties.LabelStyle:
                        result = DataPointHelper.GetLabelStyleFromDataPoint(dp);
                        break;
                    case VcProperties.LabelText:
                        result = DataPointHelper.GetLabelTextFromDataPoint(dp);
                        break;
                    case VcProperties.LabelAngle:
                        result = DataPointHelper.GetLabelAngleFromDataPoint(dp);
                        break;
                    case VcProperties.LegendText:
                        result = DataPointHelper.GetLegendTextFromDataPoint(dp);
                        break;
                    case VcProperties.LegendMarkerColor:
                        result = DataPointHelper.GetLegendMarkerColorFromDataPoint(dp);
                        break;
                    case VcProperties.LegendMarkerType:
                        result = DataPointHelper.GetLegendMarkerTypeFromDataPoint(dp);
                        break;
                    case VcProperties.LightingEnabled:
                        result = DataPointHelper.GetLightingEnabledFromDataPoint(dp);
                        break;
                    case VcProperties.MarkerBorderColor:
                        result = DataPointHelper.GetMarkerBorderColorFromDataPoint(dp);
                        break;
                    case VcProperties.MarkerBorderThickness:
                        result = DataPointHelper.GetMarkerBorderThicknessFromDataPoint(dp);
                        break;
                    case VcProperties.MarkerColor:
                        result = DataPointHelper.GetMarkerColorFromDataPoint(dp);
                        break;
                    case VcProperties.MarkerEnabled:
                        result = DataPointHelper.GetMarkerEnabledFromDataPoint(dp);
                        break;
                    case VcProperties.MarkerScale:
                        result = DataPointHelper.GetMarkerScaleFromDataPoint(dp);
                        break;
                    case VcProperties.MarkerSize:
                        result = DataPointHelper.GetMarkerSizeFromDataPoint(dp);
                        break;
                    case VcProperties.MarkerType:
                        result = DataPointHelper.GetMarkerTypeFromDataPoint(dp);
                        break;
                    default:
                        if (property == VcProperties.Opacity)
                        {
                            result = DataPointHelper.GetOpacityFromDataPoint(dp);
                        }
                        break;
                }
            }
            else
            {
                switch (property)
                {
                    case VcProperties.RadiusX:
                        result = DataPointHelper.GetRadiusXFromDataPoint(dp);
                        break;
                    case VcProperties.RadiusY:
                        result = DataPointHelper.GetRadiusYFromDataPoint(dp);
                        break;
                    default:
                        switch (property)
                        {
                            case VcProperties.ShadowEnabled:
                                result = DataPointHelper.GetShadowEnabledFromDataPoint(dp);
                                break;
                            case VcProperties.ShowInLegend:
                                result = DataPointHelper.GetShowInLegendFromDataPoint(dp);
                                break;
                            case VcProperties.StickColor:
                                result = DataPointHelper.GetStickColorFromDataPoint(dp);
                                break;
                        }
                        break;
                }
            }
            return result;
        }

        public static Brush GetMarkerBorderColor_DataPoint(IDataPoint dp)
        {
            if (!dp.IsLightDataPoint)
            {
                return (Brush)(dp as DataPoint).GetValue(DataPoint.MarkerBorderColorProperty);
            }
            return null;
        }

        public static double? GetLabelFontSize_DataPoint(IDataPoint dp)
        {
            if (!dp.IsLightDataPoint)
            {
                return (double?)(dp as DataPoint).GetValue(DataPoint.LabelFontSizeProperty);
            }
            return null;
        }

        public static Thickness GetBorderThickness_DataPoint(IDataPoint dp)
        {
            if (!dp.IsLightDataPoint)
            {
                return (Thickness)(dp as DataPoint).GetValue(DataPoint.BorderThicknessProperty);
            }
            return new Thickness(0.0);
        }

        public static Brush GetColor_DataPoint(IDataPoint dp)
        {
            if (!dp.IsLightDataPoint)
            {
                return (Brush)(dp as DataPoint).GetValue(DataPoint.ColorProperty);
            }
            return (dp as LightDataPoint)._color;
        }

        public static bool? GetExploded_DataPoint(IDataPoint dp)
        {
            if (!dp.IsLightDataPoint)
            {
                return (bool?)(dp as DataPoint).GetValue(DataPoint.ExplodedProperty);
            }
            return (dp as LightDataPoint)._exploded;
        }

        public static bool? GetMarkerEnabled_DataPoint(IDataPoint dp)
        {
            if (!dp.IsLightDataPoint)
            {
                return (bool?)(dp as DataPoint).GetValue(DataPoint.MarkerEnabledProperty);
            }
            return null;
        }

        public static bool? GetLabelEnabled_DataPoint(IDataPoint dp)
        {
            if (!dp.IsLightDataPoint)
            {
                return (bool?)(dp as DataPoint).GetValue(DataPoint.LabelEnabledProperty);
            }
            return null;
        }

        public static Thickness? GetMarkerBorderThickness_DataPoint(IDataPoint dp)
        {
            if (!dp.IsLightDataPoint)
            {
                return (Thickness?)(dp as DataPoint).GetValue(DataPoint.MarkerBorderThicknessProperty);
            }
            return null;
        }

        public static string GetName_DataPoint(IDataPoint dp)
        {
            if (!dp.IsLightDataPoint)
            {
                return (string)(dp as DataPoint).GetValue(DataPoint.NameProperty);
            }
            return (dp as LightDataPoint).Name;
        }

        public static double GetOpacity_DataPoint(IDataPoint dp)
        {
            if (!dp.IsLightDataPoint)
            {
                return (double)(dp as DataPoint).GetValue(DataPoint.OpacityProperty);
            }
            return 1.0;
        }

        internal static void ExplodeOrUnexplodeAnimation(IDataPoint dp)
        {
            try
            {
                if (dp.Exploded.Value)
                {
                    if (dp.DpInfo.UnExplodeAnimation != null)
                    {
                        dp.DpInfo.UnExplodeAnimation.Stop();
                    }
                    if (dp.DpInfo.ExplodeAnimation != null)
                    {
                        try
                        {
                            dp.DpInfo.IsAlreadyExploded = true;
                            dp.DpInfo.ExplodeAnimation.Begin(dp.Chart._rootElement, true);
                            if (!(dp.Chart as Chart).ChartArea._isFirstTimeRender && dp.Parent != null && (dp.Parent.RenderAs == RenderAs.Pie || dp.Parent.RenderAs == RenderAs.Doughnut) && !(dp.Chart as Chart).AnimatedUpdate.Value)
                            {
                                dp.DpInfo.ExplodeAnimation.SkipToFill(dp.Chart._rootElement);
                            }
                            goto IL_17C;
                        }
                        catch
                        {
                            dp.DpInfo.IsAlreadyExploded = false;
                            goto IL_17C;
                        }
                    }
                    DataPointHelper.PieDouExplodeWithoutAnimation(dp);
                    dp.DpInfo.IsAlreadyExploded = true;
                }
                else if (dp.DpInfo.IsAlreadyExploded)
                {
                    DataPointHelper.UnExplodeFunnelSlices(dp);
                    if (dp.DpInfo.ExplodeAnimation != null)
                    {
                        dp.DpInfo.ExplodeAnimation.Stop();
                    }
                    if (dp.DpInfo.UnExplodeAnimation != null)
                    {
                        dp.DpInfo.IsAlreadyExploded = false;
                        dp.DpInfo.UnExplodeAnimation.Begin(dp.Chart._rootElement, true);
                    }
                    else
                    {
                        DataPointHelper.PieDouUnExplodeWithoutAnimation(dp);
                    }
                    dp.DpInfo.IsExplodeRunning = false;
                }
                IL_17C:
                if (!dp.DpInfo.IsExplodeRunning && dp.Exploded.Value && !(dp.Chart as Chart).ChartArea._isFirstTimeRender && dp.Parent != null && !(dp.Chart as Chart).ChartArea._isDefaultInteractivityAllowed && (dp.Parent.RenderAs == RenderAs.Pie || dp.Parent.RenderAs == RenderAs.Doughnut))
                {
                    if (dp.DpInfo.ExplodeAnimation != null)
                    {
                        dp.DpInfo.ExplodeAnimation.Begin(dp.Chart._rootElement, true);
                        dp.DpInfo.IsExplodeRunning = true;
                    }
                    else
                    {
                        DataPointHelper.PieDouExplodeWithoutAnimation(dp);
                        dp.DpInfo.IsExplodeRunning = false;
                    }
                }
            }
            catch
            {
            }
        }

        internal static void UnExplodeFunnelSlices(IDataPoint dataPoint)
        {
            if (dataPoint.Parent.RenderAs == RenderAs.SectionFunnel || dataPoint.Parent.RenderAs == RenderAs.StreamLineFunnel || dataPoint.Parent.RenderAs == RenderAs.Pyramid)
            {
                if (dataPoint.DpInfo.UnExplodeAnimation != null)
                {
                    dataPoint.DpInfo.UnExplodeAnimation.Completed -= new EventHandler(dataPoint.DpFunc.ExplodeAnimation_Completed);
                    dataPoint.DpInfo.UnExplodeAnimation = null;
                }
                dataPoint.DpInfo.UnExplodeAnimation = new Storyboard();
                TriangularChartSliceParms[] array = dataPoint.Parent.VisualParams as TriangularChartSliceParms[];
                for (int i = 0; i < array.Length; i++)
                {
                    TriangularChartSliceParms triangularChartSliceParms = array[i];
                    dataPoint.DpInfo.UnExplodeAnimation = FunnelChart.CreateUnExplodingAnimation(dataPoint.Parent, dataPoint, dataPoint.DpInfo.UnExplodeAnimation, triangularChartSliceParms.DataPoint.DpInfo.Faces.Visual as Panel, triangularChartSliceParms.Top);
                }
                dataPoint.DpInfo.UnExplodeAnimation.Completed += new EventHandler(dataPoint.DpFunc.ExplodeAnimation_Completed);
            }
        }

        internal static void InteractiveAnimation(IDataPoint dataPoint1, bool isFirstTimeAnimation, bool allowToChangePropertyValue)
        {
            if (dataPoint1.Parent.RenderAs == RenderAs.SectionFunnel || dataPoint1.Parent.RenderAs == RenderAs.StreamLineFunnel || dataPoint1.Parent.RenderAs == RenderAs.Pyramid)
            {
                if (!dataPoint1.Parent.Exploded)
                {
                    if (isFirstTimeAnimation)
                    {
                        if (dataPoint1.Exploded == true)
                        {
                            DataPointHelper.ExplodeOrUnexplodeAnimation(dataPoint1);
                            return;
                        }
                    }
                    else if (!dataPoint1.Parent.SelectionEnabled)
                    {
                        foreach (IDataPoint current in dataPoint1.Parent.InternalDataPoints)
                        {
                            if (current != dataPoint1)
                            {
                                current.DpInfo.IsNotificationEnable = false;
                                current.Exploded = new bool?(false);
                                current.DpInfo.IsAlreadyExploded = false;
                                current.DpInfo.IsNotificationEnable = true;
                            }
                        }
                        dataPoint1.Exploded = !dataPoint1.Exploded;
                        return;
                    }
                }
            }
            else if ((dataPoint1.Chart as Chart).AnimatedUpdate.Value)
            {
                if ((dataPoint1.Parent.RenderAs == RenderAs.Pie || dataPoint1.Parent.RenderAs == RenderAs.Doughnut) && !dataPoint1.DpInfo.InterativityAnimationState)
                {
                    if (!dataPoint1.DpInfo.InteractiveExplodeState)
                    {
                        dataPoint1.DpInfo.InterativityAnimationState = true;
                        if (allowToChangePropertyValue)
                        {
                            dataPoint1.DpInfo.IsNotificationEnable = false;
                            dataPoint1.Exploded = new bool?(true);
                            dataPoint1.DpInfo.IsNotificationEnable = true;
                        }
                        if (dataPoint1.DpInfo.ExplodeAnimation != null)
                        {
                            dataPoint1.DpInfo.ExplodeAnimation.Begin(dataPoint1.Chart._rootElement, true);
                            dataPoint1.DpInfo.IsAlreadyExploded = true;
                        }
                    }
                    if (dataPoint1.DpInfo.InteractiveExplodeState)
                    {
                        dataPoint1.DpInfo.InterativityAnimationState = true;
                        if (allowToChangePropertyValue)
                        {
                            dataPoint1.DpInfo.IsNotificationEnable = false;
                            dataPoint1.Exploded = new bool?(false);
                            dataPoint1.DpInfo.IsNotificationEnable = true;
                        }
                        DataPointHelper.UnExplodeFunnelSlices(dataPoint1);
                        if (dataPoint1.DpInfo.UnExplodeAnimation != null)
                        {
                            dataPoint1.DpInfo.UnExplodeAnimation.Begin(dataPoint1.Chart._rootElement, true);
                            return;
                        }
                    }
                }
            }
            else if (dataPoint1.Parent.RenderAs != RenderAs.SectionFunnel && dataPoint1.Parent.RenderAs != RenderAs.StreamLineFunnel && dataPoint1.Parent.RenderAs != RenderAs.Pyramid)
            {
                DataPointHelper.PieExplodeOrUnExplodeWithoutAnimation(dataPoint1);
            }
        }

        internal static void AttachEvent2DataPointVisualFaces(IDataPoint dp, VisifireElement obj)
        {
            DataSeries parent = dp.Parent;
            DpInfo dpInfo = dp.DpInfo;
            if (parent != null && RenderHelper.IsAxisIndependentCType(parent))
            {
                if (dpInfo.Faces != null)
                {
                    if ((parent.Chart as Chart).View3D)
                    {
                        using (List<FrameworkElement>.Enumerator enumerator = dpInfo.Faces.VisualComponents.GetEnumerator())
                        {
                            while (enumerator.MoveNext())
                            {
                                FrameworkElement current = enumerator.Current;
                                dp.AttachEvents2Visual(obj, dp, current);
                                if ((dp.Chart as Chart).ChartArea != null && (dp.Chart as Chart).ChartArea._isDefaultInteractivityAllowed)
                                {
                                    current.MouseLeftButtonUp -= new MouseButtonEventHandler(dp.DpFunc.Visual_ExplodeUnExplode);
                                    current.MouseLeftButtonUp += new MouseButtonEventHandler(dp.DpFunc.Visual_ExplodeUnExplode);
                                }
                            }
                            goto IL_162;
                        }
                    }
                    if (dpInfo.Faces.Visual != null)
                    {
                        dp.AttachEvents2Visual(obj, dp, dpInfo.Faces.Visual);
                        if ((dp.Chart as Chart).ChartArea != null && (dp.Chart as Chart).ChartArea._isDefaultInteractivityAllowed)
                        {
                            dpInfo.Faces.Visual.MouseLeftButtonUp -= new MouseButtonEventHandler(dp.DpFunc.Visual_ExplodeUnExplode);
                            dpInfo.Faces.Visual.MouseLeftButtonUp += new MouseButtonEventHandler(dp.DpFunc.Visual_ExplodeUnExplode);
                        }
                    }
                    IL_162:
                    if (dpInfo.ExplodeAnimation != null)
                    {
                        dpInfo.ExplodeAnimation.Completed -= new EventHandler(dp.DpFunc.ExplodeAnimation_Completed);
                        dpInfo.ExplodeAnimation.Completed += new EventHandler(dp.DpFunc.ExplodeAnimation_Completed);
                    }
                    if (dpInfo.UnExplodeAnimation != null)
                    {
                        dpInfo.UnExplodeAnimation.Completed -= new EventHandler(dp.DpFunc.UnExplodeAnimation_Completed);
                        dpInfo.UnExplodeAnimation.Completed += new EventHandler(dp.DpFunc.UnExplodeAnimation_Completed);
                    }
                }
                if ((parent.RenderAs == RenderAs.Pie || parent.RenderAs == RenderAs.Doughnut) && dpInfo.LabelVisual != null)
                {
                    dp.AttachEvents2Visual(obj, dp, dpInfo.LabelVisual);
                    return;
                }
            }
            else if (parent != null && (parent.RenderAs == RenderAs.StackedArea || parent.RenderAs == RenderAs.StackedArea100 || RenderHelper.IsLineCType(parent)))
            {
                if (!RenderHelper.IsLineCType(parent) && parent.Faces != null && (obj.GetType().Equals(typeof(DataPoint)) || obj.GetType().Equals(typeof(LightDataPoint))) && dpInfo.Faces != null && dpInfo.Faces.VisualComponents != null)
                {
                    foreach (FrameworkElement current2 in dpInfo.Faces.VisualComponents)
                    {
                        if (!dp.IsLightDataPoint)
                        {
                            AreaChart.AttachEvents2AreaVisual(obj, dp as DataPoint, current2);
                        }
                    }
                }
                if (dpInfo.Marker != null)
                {
                    dp.AttachEvents2Visual(obj, dp, dpInfo.Marker.Visual);
                    return;
                }
            }
            else if (dpInfo.Faces != null)
            {
                if (parent != null && (parent.RenderAs == RenderAs.Bubble || parent.RenderAs == RenderAs.Point || parent.RenderAs == RenderAs.Stock || parent.RenderAs == RenderAs.CandleStick || parent.RenderAs == RenderAs.SectionFunnel || parent.RenderAs == RenderAs.StreamLineFunnel || parent.RenderAs == RenderAs.Pyramid))
                {
                    foreach (FrameworkElement current3 in dpInfo.Faces.VisualComponents)
                    {
                        dp.AttachEvents2Visual(obj, dp, current3);
                    }
                    if ((parent.RenderAs == RenderAs.Stock || parent.RenderAs == RenderAs.CandleStick) && dpInfo.LabelVisual != null)
                    {
                        dp.AttachEvents2Visual(obj, dp, dpInfo.LabelVisual);
                        return;
                    }
                }
                else
                {
                    dp.AttachEvents2Visual(obj, dp, dpInfo.Faces.Visual);
                    if (dpInfo.Marker != null)
                    {
                        dp.AttachEvents2Visual(obj, dp, dpInfo.Marker.Visual);
                    }
                    if (dpInfo.LabelVisual != null)
                    {
                        dp.AttachEvents2Visual(obj, dp, dpInfo.LabelVisual);
                    }
                }
            }
        }

        internal static void PieDouExplodeWithoutAnimation(IDataPoint dataPoint)
        {
            if (dataPoint.Parent != null && (dataPoint.Parent.RenderAs == RenderAs.Pie || dataPoint.Parent.RenderAs == RenderAs.Doughnut))
            {
                if (!(dataPoint.Chart as Chart).View3D)
                {
                    (dataPoint.DpInfo.Faces.Visual.RenderTransform as TranslateTransform).X = (dataPoint.DpInfo.VisualParams as SectorChartShapeParams).OffsetX;
                    (dataPoint.DpInfo.Faces.Visual.RenderTransform as TranslateTransform).Y = (dataPoint.DpInfo.VisualParams as SectorChartShapeParams).OffsetY;
                    if (dataPoint.DpInfo.LabelVisual != null)
                    {
                        if ((LabelStyles)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelStyle) == LabelStyles.Inside)
                        {
                            double num = (double)(dataPoint.DpInfo.LabelVisual as Canvas).GetValue(Canvas.LeftProperty);
                            if (dataPoint.Chart != null && (dataPoint.Chart as Chart).ChartArea != null && (num + (dataPoint.DpInfo.VisualParams as SectorChartShapeParams).OffsetX < 0.0 || num + (dataPoint.DpInfo.LabelVisual as Canvas).Width + (dataPoint.DpInfo.VisualParams as SectorChartShapeParams).OffsetX > (dataPoint.Chart as Chart).ChartArea.ChartVisualCanvas.Width))
                            {
                                return;
                            }
                            TranslateTransform translateTransform = new TranslateTransform();
                            (dataPoint.DpInfo.LabelVisual as Canvas).RenderTransform = translateTransform;
                            translateTransform.X = (dataPoint.DpInfo.VisualParams as SectorChartShapeParams).OffsetX;
                            translateTransform.Y = (dataPoint.DpInfo.VisualParams as SectorChartShapeParams).OffsetY;
                        }
                        else
                        {
                            (dataPoint.DpInfo.LabelVisual as Canvas).SetValue(Canvas.LeftProperty, (dataPoint.DpInfo.VisualParams as SectorChartShapeParams).ExplodedPoints.LabelPosition.X);
                        }
                    }
                    if (dataPoint.DpInfo.LabelLine != null)
                    {
                        PathFigure pathFigure = (dataPoint.DpInfo.LabelLine.Data as PathGeometry).Figures[0];
                        PathSegmentCollection segments = pathFigure.Segments;
                        (segments[0] as LineSegment).Point = (dataPoint.DpInfo.VisualParams as SectorChartShapeParams).ExplodedPoints.LabelLineMidPoint;
                        (segments[1] as LineSegment).Point = (dataPoint.DpInfo.VisualParams as SectorChartShapeParams).ExplodedPoints.LabelLineEndPoint;
                    }
                    dataPoint.DpInfo.InteractiveExplodeState = true;
                }
                else
                {
                    using (List<FrameworkElement>.Enumerator enumerator = dataPoint.DpInfo.Faces.VisualComponents.GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                        {
                            Shape shape = (Shape)enumerator.Current;
                            if (shape != null)
                            {
                                (shape.RenderTransform as TranslateTransform).X = (dataPoint.DpInfo.VisualParams as SectorChartShapeParams).OffsetX;
                                (shape.RenderTransform as TranslateTransform).Y = (dataPoint.DpInfo.VisualParams as SectorChartShapeParams).OffsetY;
                            }
                        }
                    }
                    if (dataPoint.DpInfo.LabelVisual != null)
                    {
                        if ((LabelStyles)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LabelStyle) == LabelStyles.Inside)
                        {
                            double num2 = (double)(dataPoint.DpInfo.LabelVisual as Canvas).GetValue(Canvas.LeftProperty);
                            if (dataPoint.Chart != null && (dataPoint.Chart as Chart).ChartArea != null && (num2 + (dataPoint.DpInfo.VisualParams as SectorChartShapeParams).OffsetX < 0.0 || num2 + (dataPoint.DpInfo.LabelVisual as Canvas).Width + (dataPoint.DpInfo.VisualParams as SectorChartShapeParams).OffsetX > (dataPoint.Chart as Chart).ChartArea.ChartVisualCanvas.Width))
                            {
                                return;
                            }
                            TranslateTransform translateTransform2 = new TranslateTransform();
                            (dataPoint.DpInfo.LabelVisual as Canvas).RenderTransform = translateTransform2;
                            translateTransform2.X = (dataPoint.DpInfo.VisualParams as SectorChartShapeParams).OffsetX;
                            translateTransform2.Y = (dataPoint.DpInfo.VisualParams as SectorChartShapeParams).OffsetY;
                        }
                        else
                        {
                            (dataPoint.DpInfo.LabelVisual as Canvas).SetValue(Canvas.LeftProperty, (dataPoint.DpInfo.VisualParams as SectorChartShapeParams).ExplodedPoints.LabelPosition.X);
                        }
                    }
                    if (dataPoint.DpInfo.LabelLine != null)
                    {
                        (dataPoint.DpInfo.LabelLine.RenderTransform as TranslateTransform).X = (dataPoint.DpInfo.VisualParams as SectorChartShapeParams).OffsetX;
                        (dataPoint.DpInfo.LabelLine.RenderTransform as TranslateTransform).Y = (dataPoint.DpInfo.VisualParams as SectorChartShapeParams).OffsetY;
                        PathFigure pathFigure2 = (dataPoint.DpInfo.LabelLine.Data as PathGeometry).Figures[0];
                        PathSegmentCollection segments2 = pathFigure2.Segments;
                        (segments2[0] as LineSegment).Point = (dataPoint.DpInfo.VisualParams as SectorChartShapeParams).ExplodedPoints.LabelLineMidPoint;
                        (segments2[1] as LineSegment).Point = (dataPoint.DpInfo.VisualParams as SectorChartShapeParams).ExplodedPoints.LabelLineEndPoint;
                    }
                    dataPoint.DpInfo.InteractiveExplodeState = true;
                }
                dataPoint.DpInfo.IsAlreadyExploded = true;
            }
        }

        internal static void PieDouUnExplodeWithoutAnimation(IDataPoint dp)
        {
            if (dp.Parent != null && (dp.Parent.RenderAs == RenderAs.Pie || dp.Parent.RenderAs == RenderAs.Doughnut))
            {
                if (!(dp.Chart as Chart).View3D)
                {
                    (dp.DpInfo.Faces.Visual.RenderTransform as TranslateTransform).X = 0.0;
                    (dp.DpInfo.Faces.Visual.RenderTransform as TranslateTransform).Y = 0.0;
                    if (dp.DpInfo.LabelVisual != null)
                    {
                        if ((LabelStyles)DataPointHelper.GetDataPointValueFromProperty(dp, VcProperties.LabelStyle) == LabelStyles.Inside)
                        {
                            TranslateTransform translateTransform = new TranslateTransform();
                            (dp.DpInfo.LabelVisual as Canvas).RenderTransform = translateTransform;
                            translateTransform.X = 0.0;
                            translateTransform.Y = 0.0;
                        }
                        else
                        {
                            (dp.DpInfo.LabelVisual as Canvas).SetValue(Canvas.LeftProperty, (dp.DpInfo.VisualParams as SectorChartShapeParams).UnExplodedPoints.LabelPosition.X);
                        }
                    }
                    if (dp.DpInfo.LabelLine != null)
                    {
                        PathFigure pathFigure = (dp.DpInfo.LabelLine.Data as PathGeometry).Figures[0];
                        PathSegmentCollection segments = pathFigure.Segments;
                        (segments[0] as LineSegment).Point = (dp.DpInfo.VisualParams as SectorChartShapeParams).UnExplodedPoints.LabelLineMidPoint;
                        (segments[1] as LineSegment).Point = (dp.DpInfo.VisualParams as SectorChartShapeParams).UnExplodedPoints.LabelLineEndPoint;
                    }
                    dp.DpInfo.InteractiveExplodeState = false;
                }
                else
                {
                    using (List<FrameworkElement>.Enumerator enumerator = dp.DpInfo.Faces.VisualComponents.GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                        {
                            Shape shape = (Shape)enumerator.Current;
                            if (shape != null)
                            {
                                (shape.RenderTransform as TranslateTransform).X = 0.0;
                                (shape.RenderTransform as TranslateTransform).Y = 0.0;
                            }
                        }
                    }
                    if (dp.DpInfo.LabelVisual != null)
                    {
                        if ((LabelStyles)DataPointHelper.GetDataPointValueFromProperty(dp, VcProperties.LabelStyle) == LabelStyles.Inside)
                        {
                            TranslateTransform translateTransform2 = new TranslateTransform();
                            (dp.DpInfo.LabelVisual as Canvas).RenderTransform = translateTransform2;
                            translateTransform2.X = 0.0;
                            translateTransform2.Y = 0.0;
                        }
                        else
                        {
                            (dp.DpInfo.LabelVisual as Canvas).SetValue(Canvas.LeftProperty, (dp.DpInfo.VisualParams as SectorChartShapeParams).UnExplodedPoints.LabelPosition.X);
                        }
                    }
                    if (dp.DpInfo.LabelLine != null)
                    {
                        (dp.DpInfo.LabelLine.RenderTransform as TranslateTransform).X = 0.0;
                        (dp.DpInfo.LabelLine.RenderTransform as TranslateTransform).Y = 0.0;
                        PathFigure pathFigure2 = (dp.DpInfo.LabelLine.Data as PathGeometry).Figures[0];
                        PathSegmentCollection segments2 = pathFigure2.Segments;
                        (segments2[0] as LineSegment).Point = (dp.DpInfo.VisualParams as SectorChartShapeParams).UnExplodedPoints.LabelLineMidPoint;
                        (segments2[1] as LineSegment).Point = (dp.DpInfo.VisualParams as SectorChartShapeParams).UnExplodedPoints.LabelLineEndPoint;
                    }
                    dp.DpInfo.InteractiveExplodeState = false;
                }
                dp.DpInfo.IsAlreadyExploded = false;
            }
        }

        internal static void PieExplodeOrUnExplodeWithoutAnimation(IDataPoint dataPoint)
        {
            dataPoint.DpInfo.IsNotificationEnable = true;
            if (dataPoint.DpInfo.Faces != null && dataPoint.DpInfo.Faces.Visual != null)
            {
                if (dataPoint.Exploded.Value)
                {
                    DataPointHelper.PieDouExplodeWithoutAnimation(dataPoint);
                    return;
                }
                DataPointHelper.PieDouUnExplodeWithoutAnimation(dataPoint);
            }
        }

        internal static void PartialUpdateOfColorProperty(IDataPoint dataPoint, Brush newValue)
        {
            if (dataPoint.DpInfo.Faces != null && dataPoint.DpInfo.Faces.Parts != null)
            {
                Brush brush = (newValue != null) ? newValue : dataPoint.Color;
                RenderAs renderAs = dataPoint.Parent.RenderAs;
                if (renderAs <= RenderAs.Doughnut)
                {
                    if (renderAs != RenderAs.Pie && renderAs != RenderAs.Doughnut)
                    {
                        goto IL_7B0;
                    }
                    SectorChartShapeParams sectorChartShapeParams = (SectorChartShapeParams)dataPoint.DpInfo.VisualParams;
                    if (sectorChartShapeParams == null)
                    {
                        goto IL_7B0;
                    }
                    sectorChartShapeParams.Background = brush;
                    if (!(dataPoint.Parent.Chart as Chart).View3D)
                    {
                        if (dataPoint.DpInfo.Faces.Parts.Count > 0 && dataPoint.DpInfo.Faces.Parts[0] != null)
                        {
                            (dataPoint.DpInfo.Faces.Parts[0] as Shape).Fill = (dataPoint.Parent.LightingEnabled.Value ? Graphics.GetLightingEnabledBrush(brush, "Radial", null) : brush);
                        }
                        if (dataPoint.DpInfo.Faces.Parts.Count > 1 && dataPoint.DpInfo.Faces.Parts[1] != null)
                        {
                            (dataPoint.DpInfo.Faces.Parts[1] as Shape).Fill = ((sectorChartShapeParams.StartAngle > 1.5707963267948966 && sectorChartShapeParams.StartAngle <= 4.71238898038469) ? PieChart.GetDarkerBevelBrush(sectorChartShapeParams.Background, sectorChartShapeParams.StartAngle * 180.0 / 3.1415926535897931 + 135.0) : PieChart.GetLighterBevelBrush(sectorChartShapeParams.Background, -sectorChartShapeParams.StartAngle * 180.0 / 3.1415926535897931));
                        }
                        if (dataPoint.DpInfo.Faces.Parts.Count > 2 && dataPoint.DpInfo.Faces.Parts[2] != null)
                        {
                            (dataPoint.DpInfo.Faces.Parts[2] as Shape).Fill = ((sectorChartShapeParams.StopAngle > 1.5707963267948966 && sectorChartShapeParams.StopAngle <= 4.71238898038469) ? PieChart.GetLighterBevelBrush(sectorChartShapeParams.Background, sectorChartShapeParams.StopAngle * 180.0 / 3.1415926535897931 + 135.0) : PieChart.GetDarkerBevelBrush(sectorChartShapeParams.Background, -sectorChartShapeParams.StopAngle * 180.0 / 3.1415926535897931));
                        }
                        if (dataPoint.DpInfo.Faces.Parts.Count > 3 && dataPoint.DpInfo.Faces.Parts[3] != null)
                        {
                            (dataPoint.DpInfo.Faces.Parts[3] as Shape).Fill = ((sectorChartShapeParams.MeanAngle > 0.0 && sectorChartShapeParams.MeanAngle < 3.1415926535897931) ? PieChart.GetCurvedBevelBrush(sectorChartShapeParams.Background, sectorChartShapeParams.MeanAngle * 180.0 / 3.1415926535897931 + 90.0, Graphics.GenerateDoubleCollection(new double[]
                            {
                                -0.745,
                                -0.85
                            }), Graphics.GenerateDoubleCollection(new double[]
                            {
                                0.0,
                                1.0
                            })) : ((dataPoint.DpInfo.Faces.Parts[3] as Shape).Fill = PieChart.GetCurvedBevelBrush(sectorChartShapeParams.Background, sectorChartShapeParams.MeanAngle * 180.0 / 3.1415926535897931 + 90.0, Graphics.GenerateDoubleCollection(new double[]
                            {
                                0.745,
                                -0.99
                            }), Graphics.GenerateDoubleCollection(new double[]
                            {
                                0.0,
                                1.0
                            }))));
                        }
                        if (dataPoint.Parent.RenderAs == RenderAs.Doughnut && dataPoint.DpInfo.Faces.Parts.Count > 4 && dataPoint.DpInfo.Faces.Parts[4] != null)
                        {
                            (dataPoint.DpInfo.Faces.Parts[4] as Shape).Fill = ((sectorChartShapeParams.MeanAngle > 0.0 && sectorChartShapeParams.MeanAngle < 3.1415926535897931) ? PieChart.GetCurvedBevelBrush(sectorChartShapeParams.Background, sectorChartShapeParams.MeanAngle * 180.0 / 3.1415926535897931 + 90.0, Graphics.GenerateDoubleCollection(new double[]
                            {
                                -0.745,
                                -0.85
                            }), Graphics.GenerateDoubleCollection(new double[]
                            {
                                0.0,
                                1.0
                            })) : ((dataPoint.DpInfo.Faces.Parts[4] as Shape).Fill = PieChart.GetCurvedBevelBrush(sectorChartShapeParams.Background, sectorChartShapeParams.MeanAngle * 180.0 / 3.1415926535897931 + 90.0, Graphics.GenerateDoubleCollection(new double[]
                            {
                                0.745,
                                -0.99
                            }), Graphics.GenerateDoubleCollection(new double[]
                            {
                                0.0,
                                1.0
                            }))));
                            goto IL_7B0;
                        }
                        goto IL_7B0;
                    }
                    else
                    {
                        int num = 0;
                        using (List<DependencyObject>.Enumerator enumerator = dataPoint.DpInfo.Faces.Parts.GetEnumerator())
                        {
                            while (enumerator.MoveNext())
                            {
                                FrameworkElement frameworkElement = (FrameworkElement)enumerator.Current;
                                PieFaceTypes faceType = (PieFaceTypes)Enum.Parse(typeof(PieFaceTypes), Enum.GetName(typeof(PieFaceTypes), num), true);
                                if (frameworkElement != null)
                                {
                                    (frameworkElement as Shape).Fill = PieChart.Get3DFaceColor(dataPoint.Parent.RenderAs, sectorChartShapeParams.Lighting, sectorChartShapeParams.Background, faceType, sectorChartShapeParams.StartAngle, sectorChartShapeParams.StopAngle, sectorChartShapeParams.TiltAngle);
                                }
                                num++;
                                if (num >= 4)
                                {
                                    num = 4;
                                }
                            }
                            goto IL_7B0;
                        }
                    }
                }
                else
                {
                    switch (renderAs)
                    {
                        case RenderAs.StreamLineFunnel:
                        case RenderAs.SectionFunnel:
                            break;
                        default:
                            if (renderAs != RenderAs.Pyramid)
                            {
                                goto IL_7B0;
                            }
                            goto IL_72E;
                    }
                }
                TriangularChartSliceParms triangularChartSliceParms = (TriangularChartSliceParms)dataPoint.DpInfo.VisualParams;
                if (triangularChartSliceParms == null)
                {
                    goto IL_7B0;
                }
                using (List<DependencyObject>.Enumerator enumerator2 = dataPoint.DpInfo.Faces.Parts.GetEnumerator())
                {
                    while (enumerator2.MoveNext())
                    {
                        Shape shape = (Shape)enumerator2.Current;
                        FunnelChart.ReCalculateAndApplyTheNewBrush(shape, brush, (bool)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LightingEnabled), (dataPoint.Parent.Chart as Chart).View3D, triangularChartSliceParms);
                    }
                    goto IL_7B0;
                }
                IL_72E:
                TriangularChartSliceParms triangularChartSliceParms2 = (TriangularChartSliceParms)dataPoint.DpInfo.VisualParams;
                if (triangularChartSliceParms2 != null)
                {
                    using (List<DependencyObject>.Enumerator enumerator3 = dataPoint.DpInfo.Faces.Parts.GetEnumerator())
                    {
                        while (enumerator3.MoveNext())
                        {
                            Shape shape2 = (Shape)enumerator3.Current;
                            PyramidChart.ReCalculateAndApplyTheNewBrush(shape2, brush, (bool)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LightingEnabled), (dataPoint.Parent.Chart as Chart).View3D, triangularChartSliceParms2);
                        }
                    }
                }
            }
            IL_7B0:
            DataPointHelper.UpdateMarkerAndLegend(dataPoint, newValue);
        }

        internal static void UpdateMarkerAndLegend(IDataPoint dataPoint, object colorValue)
        {
            Marker marker = dataPoint.DpInfo.Marker;
            if (marker != null && marker.Visual != null && (bool)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.MarkerEnabled))
            {
                if (dataPoint.Parent.RenderAs == RenderAs.Point)
                {
                    marker.MarkerFillColor = dataPoint.Color;
                    if (marker.MarkerType != MarkerTypes.Cross)
                    {
                        if (DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.BorderColor) != null)
                        {
                            marker.BorderColor = (Brush)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.BorderColor);
                        }
                    }
                    else
                    {
                        marker.BorderColor = dataPoint.Color;
                    }
                }
                else
                {
                    marker.BorderColor = ((DataPointHelper.GetMarkerBorderColor_DataPoint(dataPoint) == null) ? ((colorValue != null) ? (colorValue as Brush) : ((Brush)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.MarkerBorderColor))) : ((Brush)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.MarkerBorderColor)));
                }
                if (!dataPoint.IsLightDataPoint && !(dataPoint as DataPoint).Selected)
                {
                    marker.UpdateMarker();
                }
            }
            DataPointHelper.UpdateLegendMarker(dataPoint, colorValue as Brush);
        }

        internal static void UpdateLegendMarker(IDataPoint dataPoint, Brush colorValue)
        {
            Brush brush = (colorValue != null) ? colorValue : dataPoint.DpInfo.InternalColor;
            if (DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LegendMarkerColor) != null)
            {
                brush = (Brush)DataPointHelper.GetDataPointValueFromProperty(dataPoint, VcProperties.LegendMarkerColor);
            }
            Marker legendMarker = dataPoint.DpInfo.LegendMarker;
            if (legendMarker != null && legendMarker.Visual != null)
            {
                RenderAs renderAs = dataPoint.Parent.RenderAs;
                RenderAs renderAs2 = renderAs;
                if (renderAs2 != RenderAs.Line)
                {
                    switch (renderAs2)
                    {
                        case RenderAs.Stock:
                        case RenderAs.CandleStick:
                            legendMarker.BorderColor = brush;
                            if (legendMarker.Visual.Parent != null && legendMarker.Visual.Parent is Canvas && (legendMarker.Visual.Parent as Canvas).Children[0] is Line)
                            {
                                ((legendMarker.Visual.Parent as Canvas).Children[0] as Line).Stroke = brush;
                                goto IL_21B;
                            }
                            goto IL_21B;
                        case RenderAs.StepLine:
                        case RenderAs.Spline:
                        case RenderAs.QuickLine:
                            goto IL_8A;
                    }
                    legendMarker.BorderColor = brush;
                    legendMarker.MarkerFillColor = brush;
                    goto IL_21B;
                }
                IL_8A:
                bool value = dataPoint.Parent.LightingEnabled.Value;
                if (!dataPoint.IsLightDataPoint)
                {
                    value = (dataPoint as DataPoint).LightingEnabled.Value;
                }
                legendMarker.BorderColor = (value ? Graphics.GetLightingEnabledBrush(brush, "Linear", new double[]
                {
                    0.65,
                    0.55
                }) : brush);
                if (legendMarker.Visual.Parent != null && legendMarker.Visual.Parent is Canvas && (legendMarker.Visual.Parent as Canvas).Children[0] is Line)
                {
                    ((legendMarker.Visual.Parent as Canvas).Children[0] as Line).Stroke = (value ? Graphics.GetLightingEnabledBrush(brush, "Linear", new double[]
                    {
                        0.65,
                        0.55
                    }) : brush);
                }
                IL_21B:
                legendMarker.UpdateMarker();
            }
        }

        private static void ResetVisualParams4NonPartialUpdateChartTypes(IDataPoint dataPoint)
        {
            if (dataPoint.Parent != null)
            {
                foreach (IDataPoint current in dataPoint.Parent.InternalDataPoints)
                {
                    if (double.IsNaN(current.DpInfo.InternalYValue))
                    {
                        dataPoint.DpInfo.VisualParams = null;
                    }
                }
            }
        }

        internal static bool UpdateVisual(IDataPoint dp1, VcProperties property, object newValue, bool recursive)
        {
            Chart chart = dp1.Chart as Chart;
            DataSeries parent = dp1.Parent;
            if (chart.IsInDesignMode)
            {
                if (chart != null)
                {
                    chart.RENDER_LOCK = false;
                }
                dp1.FirePropertyChanged(property);
                return false;
            }
            if (chart != null && chart.IS_FULL_RENDER_PENDING)
            {
                return false;
            }
            if (!recursive && ObservableObject.NonPartialUpdateChartTypes(parent.RenderAs))
            {
                if (property == VcProperties.Color)
                {
                    DataPointHelper.PartialUpdateOfColorProperty(dp1, newValue as Brush);
                }
                else
                {
                    DataPointHelper.ResetVisualParams4NonPartialUpdateChartTypes(dp1);
                    dp1.FirePropertyChanged(property);
                }
                return false;
            }
            if (((property == VcProperties.YValue || property == VcProperties.Enabled) && (parent.IncludePercentageInLegend || parent.IncludeYValueInLegend) && parent.ShowInLegend.Value) || (parent != null && !parent.InternalDataPoints.Contains(dp1)) || property == VcProperties.LegendMarkerType)
            {
                dp1.FirePropertyChanged(property);
                return false;
            }
            if (chart != null)
            {
                chart._internalPartialUpdateEnabled = true;
            }
            if (!chart.PARTIAL_DP_RENDER_LOCK || recursive)
            {
                bool flag = false;
                bool flag2 = false;
                PlotGroup plotGroup = parent.PlotGroup;
                AxisRepresentations renderAxisType = AxisRepresentations.AxisX;
                if (RenderHelper.IsRasterRenderSupported(parent) || parent.RenderAs == RenderAs.Spline || parent.RenderAs == RenderAs.QuickLine || (parent.RenderAs != RenderAs.Line && parent.RenderAs != RenderAs.StepLine) || ((parent.RenderAs == RenderAs.Line || parent.RenderAs == RenderAs.StepLine) && chart.AnimatedUpdate == false))
                {
                    if ((!recursive && property == VcProperties.YValue && (parent.RenderAs == RenderAs.Spline || parent.RenderAs == RenderAs.QuickLine)) || (!recursive && RenderHelper.IsRasterRenderSupported(parent) && (property == VcProperties.YValue || property == VcProperties.YValues || property == VcProperties.XValue || property == VcProperties.Enabled)) || (!recursive && property == VcProperties.YValue && (chart.PlotDetails.ListOfAllDataPoints.Count > 1000 || !chart.AnimatedUpdate.Value)) || (!recursive && property == VcProperties.XValue && (!RenderHelper.IsLineCType(parent) || parent.RenderAs != RenderAs.Bubble || parent.RenderAs != RenderAs.Point)))
                    {
                        chart.PARTIAL_DP_RENDER_LOCK = true;
                        chart._partialRenderBlockedCount = 0.0;
                        chart._datapoint2UpdatePartially = new Dictionary<IDataPoint, VcProperties>();
                        chart._datapoint2UpdatePartially.Add(dp1, property);
                    }
                    else
                    {
                        chart.PARTIAL_DP_RENDER_LOCK = false;
                    }
                }
                if (chart.ChartArea != null)
                {
                    chart.ChartArea._axisUpdated = false;
                    chart.ChartArea._plotDetailsReCreated = false;
                    chart.ChartArea._seriesListUpdated = false;
                }
                if (!chart.PARTIAL_DP_RENDER_LOCK || recursive)
                {
                    if (parent.RenderAs == RenderAs.Area && (property == VcProperties.BorderThickness || property == VcProperties.BorderColor || property == VcProperties.BorderStyle))
                    {
                        return false;
                    }
                    if (chart.PlotDetails == null || plotGroup == null)
                    {
                        return true;
                    }
                    if (property == VcProperties.YValue || property == VcProperties.YValues)
                    {
                        double axisYMaximumDataValue = chart.PlotDetails.GetAxisYMaximumDataValue(plotGroup.AxisY);
                        double axisYMinimumDataValue = chart.PlotDetails.GetAxisYMinimumDataValue(plotGroup.AxisY);
                        object oldValue;


                        if (parent.RenderAs == RenderAs.CandleStick || parent.RenderAs == RenderAs.Stock)
                        {
                            oldValue = dp1.DpInfo.OldYValues;
                        }
                        else
                        {
                            oldValue = dp1.DpInfo.OldYValue;
                        }
                        chart.PlotDetails.ReCreate(dp1, ElementTypes.DataPoint, property, oldValue, newValue);
                        double axisYMaximumDataValue2 = chart.PlotDetails.GetAxisYMaximumDataValue(plotGroup.AxisY);
                        double axisYMinimumDataValue2 = chart.PlotDetails.GetAxisYMinimumDataValue(plotGroup.AxisY);
                        if ((plotGroup.AxisY.AxisMinimum == null || plotGroup.AxisY.AxisMaximum == null) && (axisYMaximumDataValue2 != axisYMaximumDataValue || axisYMinimumDataValue2 != axisYMinimumDataValue))
                        {
                            flag2 = true;
                            renderAxisType = AxisRepresentations.AxisY;
                        }
                        if (plotGroup.AxisY.ViewportRangeEnabled)
                        {
                            flag2 = true;
                            renderAxisType = AxisRepresentations.AxisY;
                        }
                    }
                    else if (property == VcProperties.XValue)
                    {
                        double axisXMaximumDataValue = chart.PlotDetails.GetAxisXMaximumDataValue(plotGroup.AxisX);
                        double axisXMinimumDataValue = chart.PlotDetails.GetAxisXMinimumDataValue(plotGroup.AxisX);
                        double axisYMaximumDataValue3 = chart.PlotDetails.GetAxisYMaximumDataValue(plotGroup.AxisY);
                        double axisYMinimumDataValue3 = chart.PlotDetails.GetAxisYMinimumDataValue(plotGroup.AxisY);
                        chart.PlotDetails.ReCreate(dp1, ElementTypes.DataPoint, property, null, newValue);
                        double axisXMaximumDataValue2 = chart.PlotDetails.GetAxisXMaximumDataValue(plotGroup.AxisX);
                        double axisXMinimumDataValue2 = chart.PlotDetails.GetAxisXMinimumDataValue(plotGroup.AxisX);
                        double axisYMaximumDataValue4 = chart.PlotDetails.GetAxisYMaximumDataValue(plotGroup.AxisY);
                        double axisYMinimumDataValue4 = chart.PlotDetails.GetAxisYMinimumDataValue(plotGroup.AxisY);
                        if ((plotGroup.AxisX.AxisMinimum == null || plotGroup.AxisX.AxisMaximum == null) && (axisXMaximumDataValue2 != axisXMaximumDataValue || axisXMinimumDataValue2 != axisXMinimumDataValue))
                        {
                            flag2 = true;
                            renderAxisType = AxisRepresentations.AxisX;
                        }
                        if ((plotGroup.AxisY.AxisMinimum == null || plotGroup.AxisY.AxisMaximum == null) && (axisYMaximumDataValue4 != axisYMaximumDataValue3 || axisYMinimumDataValue4 != axisYMinimumDataValue3))
                        {
                            flag2 = true;
                            renderAxisType = AxisRepresentations.AxisY;
                        }
                    }
                    double num = 0.0;
                    double num2 = 0.0;
                    if (flag2)
                    {
                        if (property == VcProperties.YValue)
                        {
                            num = plotGroup.AxisY._zeroBaseLinePixPosition;
                        }
                        else if (property == VcProperties.XValue)
                        {
                            num2 = plotGroup.AxisX._zeroBaseLinePixPosition;
                        }
                    }
                    PartialUpdateConfiguration partialUpdateConfiguration = new PartialUpdateConfiguration
                    {
                        Sender = dp1,
                        ElementType = ElementTypes.DataPoint,
                        Property = property,
                        OldValue = null,
                        NewValue = null,
                        IsUpdateLists = false,
                        IsCalculatePlotDetails = false,
                        IsUpdateAxis = flag2,
                        RenderAxisType = renderAxisType,
                        IsPartialUpdate = true
                    };
                    chart.ChartArea.PrePartialUpdateConfiguration(partialUpdateConfiguration);
                    if (chart.ChartArea.ChartVisualCanvas == null)
                    {
                        return false;
                    }
                    if (property == VcProperties.YValue)
                    {
                        if (plotGroup.AxisY._oldInternalAxisMinimum == plotGroup.AxisY.InternalAxisMinimum && plotGroup.AxisY._oldInternalAxisMaximum == plotGroup.AxisY.InternalAxisMaximum)
                        {
                            flag2 = false;
                        }
                    }
                    else if (property == VcProperties.XValue && plotGroup.AxisX._oldInternalAxisMinimum == plotGroup.AxisX.InternalAxisMinimum && plotGroup.AxisX._oldInternalAxisMaximum == plotGroup.AxisX.InternalAxisMaximum)
                    {
                        flag2 = false;
                    }
                    if (flag2)
                    {
                        if (property == VcProperties.YValue)
                        {
                            if (num == plotGroup.AxisY._zeroBaseLinePixPosition)
                            {
                                flag2 = false;
                            }
                        }
                        else if (property == VcProperties.XValue && num2 == plotGroup.AxisX._zeroBaseLinePixPosition)
                        {
                            flag2 = false;
                        }
                        flag = true;
                    }
                }
                parent._isZooming = false;
                if (chart.PARTIAL_DP_RENDER_LOCK)
                {
                    chart.Dispatcher.BeginInvoke(new Action<RenderHelperInfo>(RenderHelper.UpdateVisualObject), new object[]
                    {
                        new RenderHelperInfo(chart, property, newValue, true, RenderHelper.IsRasterRenderSupported(parent), false)
                    });
                    chart.Dispatcher.BeginInvoke(new Action<Chart>(DataPointHelper.ActivePartialUpdateRenderLock), new object[]
                    {
                        chart
                    });
                }
                else
                {
                    if (flag)
                    {
                        if (recursive && (!chart.AnimatedUpdate.Value || chart.PlotDetails.ListOfAllDataPoints.Count > 1000))
                        {
                            if (property == VcProperties.YValue && double.IsNaN((double)newValue))
                            {
                                dp1.DpInfo.Faces = null;
                            }
                            return true;
                        }
                        using (List<DataSeries>.Enumerator enumerator = chart.InternalSeries.GetEnumerator())
                        {
                            while (enumerator.MoveNext())
                            {
                                DataSeries current = enumerator.Current;
                                if (RenderHelper.IsRasterRenderSupported(current))
                                {
                                    foreach (IDataPoint current2 in current.InternalDataPoints)
                                    {
                                        if (current.RenderAs == RenderAs.CandleStick || current.RenderAs == RenderAs.Stock)
                                        {
                                            current2.DpInfo.OldYValues = (double[])DataPointHelper.ConvertActualOldYValueToLogarithmicValue(current2, current2.DpInfo.OldYValues);
                                        }
                                        else
                                        {
                                            current2.DpInfo.OldYValue = (double)DataPointHelper.ConvertActualOldYValueToLogarithmicValue(current2, current2.DpInfo.OldYValue);
                                        }
                                    }
                                    RenderHelper.UpdateVisualObject(current.RenderAs, new RenderHelperInfo(chart, current, ElementTypes.DataSeries, property, newValue, !flag));
                                }
                                else
                                {
                                    foreach (IDataPoint current3 in current.InternalDataPoints)
                                    {
                                        if (current.RenderAs == RenderAs.CandleStick || current.RenderAs == RenderAs.Stock)
                                        {
                                            current3.DpInfo.OldYValues = (double[])DataPointHelper.ConvertActualOldYValueToLogarithmicValue(current3, current3.DpInfo.OldYValues);
                                        }
                                        else
                                        {
                                            current3.DpInfo.OldYValue = (double)DataPointHelper.ConvertActualOldYValueToLogarithmicValue(current3, current3.DpInfo.OldYValue);
                                        }
                                        RenderHelper.UpdateVisualObject(current.RenderAs, new RenderHelperInfo(chart, current3, ElementTypes.DataPoint, property, newValue, !flag));
                                        if (current.RenderAs == RenderAs.StackedArea || current.RenderAs == RenderAs.StackedArea100)
                                        {
                                            break;
                                        }
                                    }
                                }
                            }
                            goto IL_94A;
                        }
                    }
                    if (parent.RenderAs == RenderAs.CandleStick || parent.RenderAs == RenderAs.Stock)
                    {
                        dp1.DpInfo.OldYValues = (double[])DataPointHelper.ConvertActualOldYValueToLogarithmicValue(dp1, dp1.DpInfo.OldYValues);
                    }
                    else
                    {
                        dp1.DpInfo.OldYValue = (double)DataPointHelper.ConvertActualOldYValueToLogarithmicValue(dp1, dp1.DpInfo.OldYValue);
                    }
                    if (RenderHelper.IsRasterRenderSupported(parent) && (property == VcProperties.YValue || property == VcProperties.YValues || property == VcProperties.XValue || property == VcProperties.Enabled))
                    {
                        RenderHelper.UpdateVisualObject(parent.RenderAs, new RenderHelperInfo(chart, dp1.Parent, ElementTypes.DataSeries, property, newValue, flag2));
                    }
                    else
                    {
                        RenderHelper.UpdateVisualObject(parent.RenderAs, new RenderHelperInfo(chart, dp1, ElementTypes.DataPoint, property, newValue, flag2));
                    }
                }
                IL_94A:
                if (parent.ListOfSelectedDataPoints != null && !dp1.IsLightDataPoint)
                {
                    if ((dp1 as DataPoint).Selected)
                    {
                        if (!parent.ListOfSelectedDataPoints.Contains(dp1))
                        {
                            parent.ListOfSelectedDataPoints.Add(dp1);
                        }
                    }
                    else if (parent.ListOfSelectedDataPoints.Contains(dp1))
                    {
                        parent.ListOfSelectedDataPoints.Remove(dp1);
                    }
                }
                if ((property == VcProperties.Color || property == VcProperties.LegendMarkerColor) && parent.RenderAs != RenderAs.QuickLine)
                {
                    DataPointHelper.UpdateLegendMarker(dp1, (Brush)newValue);
                }
                if (property == VcProperties.LightingEnabled && parent.RenderAs != RenderAs.QuickLine && parent.RenderAs != RenderAs.CandleStick)
                {
                    DataPointHelper.UpdateLegendMarker(dp1, null);
                }
                if (parent != null)
                {
                    chart.Dispatcher.BeginInvoke(new Action<IDataPoint>(parent.AttachOrDetachInteractivity4DataPoint), new object[]
                    {
                        dp1
                    });
                }
            }
            else if (!chart._datapoint2UpdatePartially.Keys.Contains(dp1))
            {
                chart._datapoint2UpdatePartially.Add(dp1, property);
                chart._partialRenderBlockedCount += 1.0;
            }
            return false;
        }

        internal static void ActivePartialUpdateRenderLock(Chart chart)
        {
            if (chart != null)
            {
                chart.PARTIAL_DP_RENDER_LOCK = false;
            }
        }

        internal static object ConvertActualOldYValueToLogarithmicValue(IDataPoint dp, object oldValue)
        {
            VisifireControl arg_06_0 = dp.Chart;
            DataSeries parent = dp.Parent;
            if (parent.RenderAs == RenderAs.CandleStick || parent.RenderAs == RenderAs.Stock)
            {
                if (oldValue != null)
                {
                    oldValue = (from value in new double[]
                    {
                        (((double[])oldValue).Length > 0) ? DataPointHelper.ConvertYValue2LogarithmicValue(parent, ((double[])oldValue)[0]) : double.NaN,
                        (((double[])oldValue).Length > 1) ? DataPointHelper.ConvertYValue2LogarithmicValue(parent, ((double[])oldValue)[1]) : double.NaN,
                        (((double[])oldValue).Length > 2) ? DataPointHelper.ConvertYValue2LogarithmicValue(parent, ((double[])oldValue)[2]) : double.NaN,
                        (((double[])oldValue).Length > 3) ? DataPointHelper.ConvertYValue2LogarithmicValue(parent, ((double[])oldValue)[3]) : double.NaN
                    }
                                where !double.IsNaN(value)
                                select value).ToArray<double>();
                }
            }
            else
            {
                oldValue = DataPointHelper.ConvertYValue2LogarithmicValue(parent, Convert.ToDouble(oldValue));
            }
            return oldValue;
        }

        private static string FormatDate4Labels(IDataPoint dataPoint, DateTime dt, Axis axis)
        {
            string text = (axis.XValueType == ChartValueTypes.Date) ? "d" : ((axis.XValueType == ChartValueTypes.Time) ? "t" : "G");
            text = (string.IsNullOrEmpty(dataPoint.Parent.XValueFormatString) ? text : dataPoint.Parent.XValueFormatString);
            return axis.AddPrefixAndSuffix(dt.ToString(text, CultureInfo.CurrentCulture));
        }

        private static double Percentage(IDataPoint dataPoint)
        {
            double num = 0.0;
            DataSeries parent = dataPoint.Parent;
            if (parent.RenderAs == RenderAs.Column || parent.RenderAs == RenderAs.Bar || parent.RenderAs == RenderAs.Area || RenderHelper.IsLineCType(parent) || parent.RenderAs == RenderAs.SectionFunnel || parent.RenderAs == RenderAs.Pyramid || parent.RenderAs == RenderAs.Point || parent.RenderAs == RenderAs.Bubble || parent.RenderAs == RenderAs.Pie || parent.RenderAs == RenderAs.Doughnut)
            {
                if ((parent.Chart as Chart).PlotDetails != null)
                {
                    double absoluteSumOfDataPoints = (parent.Chart as Chart).PlotDetails.GetAbsoluteSumOfDataPoints(parent.InternalDataPoints.ToList<IDataPoint>());
                    if (absoluteSumOfDataPoints > 0.0)
                    {
                        num = dataPoint.YValue / absoluteSumOfDataPoints * 100.0;
                    }
                    else
                    {
                        num = 0.0;
                    }
                }
            }
            else if (parent.RenderAs == RenderAs.CandleStick || parent.RenderAs == RenderAs.Stock)
            {
                if ((parent.Chart as Chart).PlotDetails != null)
                {
                    double absoluteSumOfDataPoints2 = (parent.Chart as Chart).PlotDetails.GetAbsoluteSumOfDataPoints(parent.InternalDataPoints.ToList<IDataPoint>());
                    if (absoluteSumOfDataPoints2 > 0.0)
                    {
                        if (dataPoint.YValues != null && dataPoint.YValues.Length > 1)
                        {
                            num = dataPoint.YValues[1] / absoluteSumOfDataPoints2 * 100.0;
                        }
                        else
                        {
                            num = 0.0;
                        }
                    }
                    else
                    {
                        num = 0.0;
                    }
                }
            }
            else if (parent.RenderAs == RenderAs.StreamLineFunnel)
            {
                num = dataPoint.YValue / parent.PlotGroup.MaximumY * 100.0;
            }
            else if (parent.RenderAs == RenderAs.StackedArea || parent.RenderAs == RenderAs.StackedBar || parent.RenderAs == RenderAs.StackedColumn || parent.RenderAs == RenderAs.StackedArea100 || parent.RenderAs == RenderAs.StackedBar100 || parent.RenderAs == RenderAs.StackedColumn100)
            {
                num = dataPoint.YValue / parent.PlotGroup.XWiseStackedDataList[dataPoint.DpInfo.InternalXValue].AbsoluteYValueSum * 100.0;
            }
            if (!double.IsNaN(num))
            {
                return num;
            }
            return 0.0;
        }

        public static string TextParser(IDataPoint dp, string str)
        {
            DataSeries parent = dp.Parent;
            if (string.IsNullOrEmpty(str) || dp.Enabled == false)
            {
                return "";
            }
            if (str.Contains("##XValue"))
            {
                str = str.Replace("##XValue", "#XValue");
            }
            else if (str.Contains("#XValue"))
            {
                if (string.IsNullOrEmpty(parent.XValueFormatString))
                {
                    if (parent.PlotGroup != null)
                    {
                        if ((dp.Chart as Chart).ChartArea.AxisX != null && (dp.Chart as Chart).ChartArea.AxisX.XValueType != ChartValueTypes.Numeric)
                        {
                            str = str.Replace("#XValue", DataPointHelper.FormatDate4Labels(dp, Convert.ToDateTime(dp.DpInfo.InternalXValueAsDateTime), (dp.Chart as Chart).ChartArea.AxisX));
                        }
                        else if ((dp.Parent.RenderAs == RenderAs.Pie || dp.Parent.RenderAs == RenderAs.Doughnut || dp.Parent.RenderAs == RenderAs.SectionFunnel || dp.Parent.RenderAs == RenderAs.StreamLineFunnel || dp.Parent.RenderAs == RenderAs.Pyramid) && parent.InternalXValueType != ChartValueTypes.Numeric)
                        {
                            str = str.Replace("#XValue", DataPointHelper.FormatDate4Labels(dp, Convert.ToDateTime(dp.DpInfo.InternalXValueAsDateTime), parent.PlotGroup.AxisX));
                        }
                        else
                        {
                            str = str.Replace("#XValue", parent.PlotGroup.AxisX.GetFormattedString(dp.DpInfo.ActualNumericXValue));
                        }
                    }
                }
                else if (parent.PlotGroup != null)
                {
                    if ((dp.Chart as Chart).ChartArea.AxisX != null && (dp.Chart as Chart).ChartArea.AxisX.XValueType != ChartValueTypes.Numeric)
                    {
                        str = str.Replace("#XValue", DataPointHelper.FormatDate4Labels(dp, Convert.ToDateTime(dp.DpInfo.InternalXValueAsDateTime), (dp.Chart as Chart).ChartArea.AxisX));
                    }
                    else if ((dp.Parent.RenderAs == RenderAs.Pie || dp.Parent.RenderAs == RenderAs.Doughnut || dp.Parent.RenderAs == RenderAs.SectionFunnel || dp.Parent.RenderAs == RenderAs.StreamLineFunnel || dp.Parent.RenderAs == RenderAs.Pyramid) && parent.InternalXValueType != ChartValueTypes.Numeric)
                    {
                        str = str.Replace("#XValue", DataPointHelper.FormatDate4Labels(dp, Convert.ToDateTime(dp.DpInfo.InternalXValueAsDateTime), parent.PlotGroup.AxisX));
                    }
                    else
                    {
                        str = str.Replace("#XValue", dp.DpInfo.ActualNumericXValue.ToString(parent.XValueFormatString));
                    }
                }
            }
            if (str.Contains("##YValue"))
            {
                str = str.Replace("##YValue", "#YValue");
            }
            else if (str.Contains("#YValue"))
            {
                if (string.IsNullOrEmpty(parent.YValueFormatString))
                {
                    if (parent.PlotGroup != null)
                    {
                        str = str.Replace("#YValue", parent.PlotGroup.AxisY.GetFormattedString(dp.YValue));
                    }
                }
                else
                {
                    str = str.Replace("#YValue", dp.YValue.ToString(parent.YValueFormatString));
                }
            }
            if (str.Contains("##ZValue"))
            {
                str = str.Replace("##ZValue", "#ZValue");
            }
            else if (str.Contains("#ZValue"))
            {
                str = str.Replace("#ZValue", dp.ZValue.ToString(parent.ZValueFormatString));
            }
            if (str.Contains("##Series"))
            {
                str = str.Replace("##Series", "#Series");
            }
            else if (str.Contains("#Series"))
            {
                if (parent._isAutoName)
                {
                    string[] array = parent.Name.Split(new char[]
                    {
                        '_'
                    });
                    str = str.Replace("#Series", array[0]);
                }
                else
                {
                    str = str.Replace("#Series", parent.Name);
                }
            }
            if (str.Contains("##LegendText"))
            {
                str = str.Replace("##LegendText", "#LegendText");
            }
            else if (str.Contains("#LegendText"))
            {
                str = str.Replace("#LegendText", DataPointHelper.GetLegendText(dp));
            }
            if (str.Contains("##AxisXLabel"))
            {
                str = str.Replace("##AxisXLabel", "#AxisXLabel");
            }
            else if (str.Contains("#AxisXLabel"))
            {
                string axisXLabel = dp.AxisXLabel;
                str = str.Replace("#AxisXLabel", string.IsNullOrEmpty(axisXLabel) ? DataPointHelper.GetAxisXLabelString(dp) : axisXLabel);
            }
            if (str.Contains("##Percentage"))
            {
                str = str.Replace("##Percentage", "#Percentage");
            }
            else if (str.Contains("#Percentage"))
            {
                if (string.IsNullOrEmpty(parent.PercentageFormatString))
                {
                    str = str.Replace("#Percentage", DataPointHelper.Percentage(dp).ToString("#0.##"));
                }
                else
                {
                    str = str.Replace("#Percentage", DataPointHelper.Percentage(dp).ToString(parent.PercentageFormatString));
                }
            }
            if (str.Contains("##Sum"))
            {
                str = str.Replace("##Sum", "#Sum");
            }
            else if (str.Contains("#Sum") && parent.PlotGroup != null && parent.PlotGroup.XWiseStackedDataList != null && parent.PlotGroup.XWiseStackedDataList.ContainsKey(dp.DpInfo.InternalXValue))
            {
                double num = 0.0;
                num += parent.PlotGroup.XWiseStackedDataList[dp.DpInfo.InternalXValue].PositiveYValueSum;
                num += parent.PlotGroup.XWiseStackedDataList[dp.DpInfo.InternalXValue].NegativeYValueSum;
                str = str.Replace("#Sum", parent.PlotGroup.AxisY.GetFormattedString(num));
            }
            if (parent != null && (parent.RenderAs == RenderAs.Stock || parent.RenderAs == RenderAs.CandleStick))
            {
                if (str.Contains("##High"))
                {
                    str = str.Replace("##High", "#High");
                }
                else if (str.Contains("#High"))
                {
                    if (string.IsNullOrEmpty(parent.YValueFormatString))
                    {
                        if (parent.PlotGroup != null && dp.YValues != null && dp.YValues.Length > 2)
                        {
                            str = str.Replace("#High", parent.PlotGroup.AxisY.GetFormattedString(dp.YValues[2]));
                        }
                        else
                        {
                            str = str.Replace("#High", "");
                        }
                    }
                    else if (dp.YValues != null && dp.YValues.Length > 2)
                    {
                        str = str.Replace("#High", dp.YValues[2].ToString(parent.YValueFormatString));
                    }
                    else
                    {
                        str = str.Replace("#High", "");
                    }
                }
                if (str.Contains("##Low"))
                {
                    str = str.Replace("##Low", "#Low");
                }
                else if (str.Contains("#Low"))
                {
                    if (string.IsNullOrEmpty(parent.YValueFormatString))
                    {
                        if (parent.PlotGroup != null && dp.YValues != null && dp.YValues.Length > 3)
                        {
                            str = str.Replace("#Low", parent.PlotGroup.AxisY.GetFormattedString(dp.YValues[3]));
                        }
                        else
                        {
                            str = str.Replace("#Low", "");
                        }
                    }
                    else if (dp.YValues != null && dp.YValues.Length > 3)
                    {
                        str = str.Replace("#Low", dp.YValues[3].ToString(parent.YValueFormatString));
                    }
                    else
                    {
                        str = str.Replace("#Low", "");
                    }
                }
                if (str.Contains("##Open"))
                {
                    str = str.Replace("##Open", "#Open");
                }
                else if (str.Contains("#Open"))
                {
                    if (string.IsNullOrEmpty(parent.YValueFormatString))
                    {
                        if (parent.PlotGroup != null && dp.YValues != null && dp.YValues.Length > 0)
                        {
                            str = str.Replace("#Open", parent.PlotGroup.AxisY.GetFormattedString(dp.YValues[0]));
                        }
                        else
                        {
                            str = str.Replace("#Open", "");
                        }
                    }
                    else if (dp.YValues != null && dp.YValues.Length > 0)
                    {
                        str = str.Replace("#Open", dp.YValues[0].ToString(parent.YValueFormatString));
                    }
                    else
                    {
                        str = str.Replace("#Open", "");
                    }
                }
                if (str.Contains("##Close"))
                {
                    str = str.Replace("##Close", "#Close");
                }
                else if (str.Contains("#Close"))
                {
                    if (string.IsNullOrEmpty(parent.YValueFormatString))
                    {
                        if (parent.PlotGroup != null && dp.YValues != null && dp.YValues.Length > 1)
                        {
                            str = str.Replace("#Close", parent.PlotGroup.AxisY.GetFormattedString(dp.YValues[1]));
                        }
                        else
                        {
                            str = str.Replace("#Close", "");
                        }
                    }
                    else if (dp.YValues != null && dp.YValues.Length > 1)
                    {
                        str = str.Replace("#Close", dp.YValues[1].ToString(parent.YValueFormatString));
                    }
                    else
                    {
                        str = str.Replace("#Close", "");
                    }
                }
            }
            return ObservableObject.GetFormattedMultilineText(str);
        }

        private static string GetAxisXLabelString(IDataPoint dp)
        {
            Chart chart = dp.Chart as Chart;
            DataSeries parent = dp.Parent;
            string result;
            if (chart.PlotDetails.ChartOrientation == ChartOrientationType.NoAxis)
            {
                if (string.IsNullOrEmpty(dp.AxisXLabel))
                {
                    if (parent.PlotGroup.AxisX.IsDateTimeAxis)
                    {
                        result = AxisLabels.FormatDate(dp.DpInfo.InternalXValueAsDateTime, parent.PlotGroup.AxisX);
                    }
                    else
                    {
                        result = parent.PlotGroup.AxisX.GetFormattedString(dp.DpInfo.ActualNumericXValue);
                    }
                }
                else
                {
                    result = dp.AxisXLabel;
                }
            }
            else if (parent.PlotGroup != null && parent.PlotGroup.AxisX != null && parent.PlotGroup.AxisX.AxisLabels != null)
            {
                if (parent.PlotGroup.AxisX.AxisLabels.AxisLabelContentDictionary != null && parent.PlotGroup.AxisX.AxisLabels.AxisLabelContentDictionary.ContainsKey(dp.DpInfo.ActualNumericXValue))
                {
                    result = parent.PlotGroup.AxisX.AxisLabels.AxisLabelContentDictionary[dp.DpInfo.ActualNumericXValue];
                }
                else if (parent.RenderAs == RenderAs.Radar)
                {
                    result = parent.PlotGroup.AxisX.GetFormattedString(dp.DpInfo.ActualNumericXValue + 1.0);
                }
                else if (parent.PlotGroup.AxisX.IsDateTimeAxis)
                {
                    result = AxisLabels.FormatDate(dp.DpInfo.InternalXValueAsDateTime, parent.PlotGroup.AxisX);
                }
                else
                {
                    result = parent.PlotGroup.AxisX.GetFormattedString(dp.DpInfo.ActualNumericXValue);
                }
            }
            else if (string.IsNullOrEmpty(dp.AxisXLabel) && parent.PlotGroup != null && parent.PlotGroup.AxisX != null)
            {
                if (parent.RenderAs == RenderAs.Radar)
                {
                    result = parent.PlotGroup.AxisX.GetFormattedString(dp.DpInfo.ActualNumericXValue + 1.0);
                }
                else if (parent.PlotGroup.AxisX.IsDateTimeAxis)
                {
                    result = AxisLabels.FormatDate(dp.DpInfo.InternalXValueAsDateTime, parent.PlotGroup.AxisX);
                }
                else
                {
                    result = parent.PlotGroup.AxisX.GetFormattedString(dp.DpInfo.ActualNumericXValue);
                }
            }
            else
            {
                result = dp.AxisXLabel;
            }
            return result;
        }

        private static string GetLegendText(IDataPoint dp)
        {
            Chart chart = dp.Chart as Chart;
            if (chart == null)
            {
                return null;
            }
            if (chart.Series.Count > 1)
            {
                return dp.TextParser(dp.Parent.LegendText);
            }
            return dp.TextParser((string)DataPointHelper.GetDataPointValueFromProperty(dp, VcProperties.LegendText));
        }

        internal static void UpdateExplodedPropertyForSelection(IDataPoint dataPoint, bool exploded, bool selfAction)
        {
            if (dataPoint.Parent.RenderAs == RenderAs.SectionFunnel || dataPoint.Parent.RenderAs == RenderAs.StreamLineFunnel || dataPoint.Parent.RenderAs == RenderAs.Pyramid)
            {
                if (dataPoint.Exploded != exploded)
                {
                    if (selfAction)
                    {
                        dataPoint.Exploded = new bool?(exploded);
                        return;
                    }
                    dataPoint.DpInfo.IsNotificationEnable = false;
                    dataPoint.Exploded = new bool?(exploded);
                    dataPoint.DpInfo.IsNotificationEnable = true;
                    return;
                }
            }
            else if (dataPoint.Exploded != exploded)
            {
                dataPoint.Exploded = new bool?(exploded);
            }
        }
    }
}
