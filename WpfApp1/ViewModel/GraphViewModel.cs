using ProfilerViewer.Helpers;
using ProfilerViewer.Structures;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ProfilerViewer.ViewModel
{
    public class GraphViewModel: INotifyPropertyChanged
    {
        private int _axisX;
        private int _axisY;
        private double _stepDetailsSelectorX;
        private int _stepDetailsSelectorWidth = 1;
        private double _minValue;
        private double _maxValue;
        private int _minLineY;
        private int _maxLineY;
        private Thickness _minValueLabelMargin;
        private Thickness _maxValueLabelMargin;
        private PointCollection _graphPoints;
        private string _selectedProfilingStep;
        private int _selectedGrapScale;
        private double _absoluteMinimumLabelText;
        private double _absoluteMaximumLabelText;
        private Visibility _stepDetailsSelectorVisibility = Visibility.Visible;
        private ICommand _updateSelectorPositionCommand;
        private ICommand _selectorMouseClickCommand;

        bool isUpdating = false;

        public event PropertyChangedEventHandler PropertyChanged;
        public ObservableCollection<string> ProfilingSteps
        {
            get { return ProfilerViewerViewModel.ProfilingSteps; }
        }
        public ObservableCollection<int> GrapScales { get; set; }
        public ObservableCollection<StepDetailsViewModel> LineSteps { get; set; }

        public int AxisX
        {
            get { return _axisX; }
            set
            {
                if(_axisX != value)
                {
                    _axisX = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("AxisX"));
                }
            }
        }

        public int AxisY
        {
            get { return _axisY; }
            set
            {
                if (_axisY != value)
                {
                    _axisY = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("AxisY"));
                }
            }
        }

        public double StepDetailsSelectorX
        {
            get { return _stepDetailsSelectorX; }
            set
            {
                if (_stepDetailsSelectorX != value)
                {
                    _stepDetailsSelectorX = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("StepDetailsSelectorX"));
                    AbsoluteMaximumLabelText = _stepDetailsSelectorX.ToString();
                }
            }
        }

        public int StepDetailsSelectorWidth
        {
            get { return _stepDetailsSelectorWidth; }
            set
            {
                if (_stepDetailsSelectorWidth != value)
                {
                    _stepDetailsSelectorWidth = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("StepDetailsSelectorWidth"));
                }
            }
        }

        public Visibility StepDetailsSelectorVisibility
        {
            get { return _stepDetailsSelectorVisibility; }
            set
            {
                if (_stepDetailsSelectorVisibility != value)
                {
                    _stepDetailsSelectorVisibility = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("StepDetailsSelectorVisibility"));
                }
            }
        }

        public ICommand SelectorEventHandlerCommand
        {
            get
            {
                if (_selectorMouseClickCommand == null)
                {
                    _selectorMouseClickCommand = new RelayCommand(param => SelectorEventHandlerExecutor(param));
                }
                return _selectorMouseClickCommand;
            }
        }

        public double MinValue
        {
            get { return _minValue; }
            set
            {
                if (_minValue != value)
                {
                    _minValue = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MinValue"));
                }
            }
        }

        public double MaxValue
        {
            get { return _maxValue; }
            set
            {
                if (_maxValue != value)
                {
                    _maxValue = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MaxValue"));
                }
            }
        }

        public int MixLineY
        {
            get { return _minLineY; }
            set
            {
                if (_minLineY != value)
                {
                    _minLineY = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MixLineY"));
                }
            }
        }

        public int MaxLineY
        {
            get { return _maxLineY; }
            set
            {
                if (_maxLineY != value)
                {
                    _maxLineY = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MaxLineY"));
                }
            }
        }

        public Thickness MinValueLabelMargin
        {
            get { return _minValueLabelMargin; }
            set
            {
                if (_minValueLabelMargin != value)
                {
                    _minValueLabelMargin = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MinValueLabelMargin"));
                }
            }
        }

        public Thickness MaxValueLabelMargin
        {
            get { return _maxValueLabelMargin; }
            set
            {
                if (_maxValueLabelMargin != value)
                {
                    _maxValueLabelMargin = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MaxValueLabelMargin"));
                }
            }
        }

        public string SelectedProfilingStep
        {
            get { return _selectedProfilingStep; }
            set
            {
                if (_selectedProfilingStep != value)
                {
                    _selectedProfilingStep = value;
                    UpdateAbsolutes();
                    if (!isUpdating)
                        UpdateCanvas();
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedProfilingStep"));
                }
            }
        }

        public int SelectedGrapScale
        {
            get { return _selectedGrapScale; }
            set
            {
                if (_selectedGrapScale != value)
                {
                    _selectedGrapScale = value;
                    if(!isUpdating)
                        UpdateCanvas();
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedGrapScale"));
                }
            }
        }

        public PointCollection GraphPoints
        {
            get { return _graphPoints; }
            set
            {
                if (_graphPoints != value)
                {
                    _graphPoints = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("GraphPoints"));
                }
            }
        }

        public string AbsoluteMinimumLabelText
        {
            get { return "Absolute Minimum: " + _absoluteMinimumLabelText; }
            set
            {
                var parsed = double.Parse(value);
                if (_absoluteMinimumLabelText != parsed)
                {
                    _absoluteMinimumLabelText = parsed;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("AbsoluteMinimumLabelText"));
                }
            }
        }

        public string AbsoluteMaximumLabelText
        {
            get { return "Absolute Maximum: " + _absoluteMaximumLabelText; }
            set
            {
                var parsed = double.Parse(value);
                if (_absoluteMaximumLabelText != parsed)
                {
                    _absoluteMaximumLabelText = parsed;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("AbsoluteMaximumLabelText"));
                }
            }
        }

        public GraphViewModel()
        {
            AxisX = Settings.AxisX;
            AxisY = Settings.AxisY;
            MixLineY = 20;
            MaxLineY = _axisY - 20;
            MinValueLabelMargin = new Thickness(0, 0, 0, _minLineY - 13);
            MaxValueLabelMargin = new Thickness(0, _minLineY - 13, 0, 0);
            GrapScales = new ObservableCollection<int>(Settings.Scale);
            LineSteps = new ObservableCollection<StepDetailsViewModel>();
            UpdateLineSteps();
        }

        void UpdateAbsolutes()
        {
            List<double> values;
            if(!ProfilerViewerViewModel.AbsoluteValues.TryGetValue(_selectedProfilingStep, out values))
            {
                values = new List<double>(2);
                values.Add(ProfilerViewerViewModel.ProfilerData[_selectedProfilingStep].Min());
                values.Add(ProfilerViewerViewModel.ProfilerData[_selectedProfilingStep].Max());
                ProfilerViewerViewModel.AbsoluteValues.Add(_selectedProfilingStep, values);
            }
            AbsoluteMinimumLabelText = values[0].ToString();
            AbsoluteMaximumLabelText = values[1].ToString();
        }

        void UpdateCanvas()
        {
            if (string.IsNullOrEmpty(_selectedProfilingStep))
                return;
            isUpdating = true;
            var graph = GetOrBuildGraph();
            MaxValue = graph.Max;
            MinValue = graph.Min;
            GraphPoints = graph.Line.Points;
            isUpdating = false;
        }

        void UpdateLineSteps()
        {
            LineSteps.Add(new StepDetailsViewModel());
        }

        private void ShowIterationDetails()
        {
            //if (StepDetailsSelectorVisibility == Visibility.Visible)
            //    StepDetailsSelectorVisibility = Visibility.Hidden;
            //else
            //    StepDetailsSelectorVisibility = Visibility.Visible;

            Console.WriteLine(_stepDetailsSelectorX);
        }

        private void SelectorEventHandlerExecutor(object param)
        {
            Type type = this.GetType();
            MethodInfo methodInfo = type.GetMethod(param.ToString(), BindingFlags.NonPublic | BindingFlags.Instance);
            methodInfo.Invoke(this, null);
        }

        private void StepDetailsSelectorHide()
        {
            SelectorVisibilityChange(Visibility.Hidden);
        }

        private void StepDetailsSelectorShow()
        {
            SelectorVisibilityChange(Visibility.Visible);
        }

        private void SelectorVisibilityChange(Visibility visibility)
        {
            StepDetailsSelectorVisibility = visibility;
        }

        GraphObject GetOrBuildGraph()
        {
            SelectedGrapScale = _selectedGrapScale == 0 ? GrapScales.First() : _selectedGrapScale;
            if (ProfilerViewerViewModel.Graphs.ContainsKey(_selectedProfilingStep))
            {
                var result = ProfilerViewerViewModel.Graphs[_selectedProfilingStep].Where(g => g.Scale == _selectedGrapScale).FirstOrDefault();
                if (result != default(GraphObject))
                    return result;
            }

            var graph = new GraphObject();
            graph.BuildGraph(ProfilerViewerViewModel.ProfilerData[_selectedProfilingStep], _selectedGrapScale, _axisX, _minLineY, _maxLineY);
            if(!ProfilerViewerViewModel.Graphs.ContainsKey(_selectedProfilingStep))
                ProfilerViewerViewModel.Graphs.Add(_selectedProfilingStep, new List<GraphObject>(GrapScales.Count));
            ProfilerViewerViewModel.Graphs[_selectedProfilingStep].Add(graph);
            return graph;
        }
    }
}
