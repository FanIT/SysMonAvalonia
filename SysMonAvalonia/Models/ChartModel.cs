using System.Collections.ObjectModel;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.Drawing;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;

namespace SysMonAvalonia.Models
{
    public class ChartModel
    {
        private int _index = 0;
        private ObservableCollection<ObservablePoint> _observablePoints;
        private LineSeries<ObservablePoint> _lineSeries;

        public ISeries[] Series { get; set; }
        public Axis[] XAxis { get; set; }
        public Axis[] YAxis { get; set; }
        public SolidColorPaint Fill { 
            set => _lineSeries.Fill = value;
        }
        public double Value
        {
            set => AddPoint(value);
        }
        
        public ChartModel()
        {
            _observablePoints = new();

            FillPoints();

            _lineSeries = new LineSeries<ObservablePoint>
            {
                Values = _observablePoints,
                Stroke = null,
                GeometryFill = null,
                GeometryStroke = null,
                GeometrySize = 0,
                DataPadding = new LvcPoint(0, 0)
            };
            
            Series = new[] { _lineSeries };

            XAxis = new[]
            {
                new Axis { IsVisible = false }
            };

            YAxis = new[]
            {
                new Axis
                {
                    IsVisible = false,
                    MinLimit = 0,
                    MaxLimit = 100
                }
            };
        }

        private void AddPoint(double point)
        {
            if (point > YAxis[0].MaxLimit) YAxis[0].MaxLimit = point;

            _observablePoints.Add(new ObservablePoint(_index++, point));
            _observablePoints.RemoveAt(0);
        }

        public void ClearPoints()
        {
            _observablePoints.Clear();
            _index = 0;
        }

        public void FillPoints(double point = 0)
        {
            for (; _index < 30; _index++)
            {
                _observablePoints.Add(new ObservablePoint(_index, point));
            }
        }
    }
}