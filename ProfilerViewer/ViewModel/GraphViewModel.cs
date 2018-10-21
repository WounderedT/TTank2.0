using ProfilerViewer.Helpers;
using ProfilerViewer.Structures;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using TPresenter.Profiler;

namespace ProfilerViewer.ViewModel
{
    public class GraphViewModel: INotifyPropertyChanged
    {
        private static GraphBox _graphBox;

        private double _minValue;
        private double _maxValue;
        private double _zoomStepSelectorCurrentPositionX;
        private double _zoomStepSelectorNewPositionX;
        private Int32 _zoomStepSelectorWidth = 1;
        private PointCollection _graphPoints;
        private string _selectedProfilingStep;
        private string _profilerDataPath;
        private int _selectedGrapScale;
        private double _absoluteMinimumLabelText;
        private double _absoluteMaximumLabelText;
        private Visibility _zoomStepSelectorVisibility = Visibility.Visible;
        private ICommand _updateSelectorPositionCommand;
        private ICommand _selectorMouseClickCommand;
        private ICommand _changeGraphModeActionButton;
        private ICommand _backToPreviousGraphActionButton;
        private ICommand _zoomGraphActionButton;
        private ICommand _updateSubstepGraphObjectsListAction;
        private Dictionary<string, GraphObject> _graphObjects = new Dictionary<string, GraphObject>();
        private bool _isUpdating = false;
        private Boolean _isSelectingSteps = false;
        private Boolean _isSelectionActive = false;
        private Boolean _isBackToPreviousGraphButtonEnabled = false;
        private Boolean _isNetValueGraphModeOn = false;
        private Double _zoomStepSelectorBasePositionX;
        private Brush _graphLineColour;
        private GraphObject _currentGraphObject;
        private Task _prepareGraphDetailsTask;
        private CancellationTokenSource _prepareGraphDetailsTokenSource;

        private List<StepViewerState> _previousStates = new List<StepViewerState>();
        private Dictionary<string, List<MenuItem>> _substepsGraphObjects = new Dictionary<string, List<MenuItem>>();
        private Dictionary<string, List<MenuItem>> _stepsZoomedGraphObjects = new Dictionary<string, List<MenuItem>>();
        private Dictionary<StepViewerState, Boolean> _substepDetailsLoadedStates = new Dictionary<StepViewerState, Boolean>();

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler StepDetailsEvent;

        private StepViewerState CurrentState
        {
            get
            {
                return new StepViewerState(_selectedProfilingStep, _selectedGrapScale, _currentGraphObject.CurrentZoom);
            }
        }

        private Boolean IsSubstepDetaisLoaded
        {
            get { return _substepDetailsLoadedStates.ContainsKey(CurrentState); }
            set
            {
                Debug.Assert(value, "False cannot be set as IsSubstepDetaisLoaded");
                _substepDetailsLoadedStates.Add(CurrentState, value);
            }
        }

        public ObservableCollection<string> ProfilingSteps { get; set; }
        public ObservableCollection<int> GrapScales { get; set; }
        public ObservableCollection<StepDetailsEntryViewModel> LineSteps { get; set; }

        public ObservableCollection<MenuItem> SupstepGraphObjects { get; set; } = new ObservableCollection<MenuItem>();
        public ObservableCollection<MenuItem> StepZoomedGraphObjects { get; set; } = new ObservableCollection<MenuItem>();
        public ObservableCollection<MenuItem> PreviousSteps { get; set; } = new ObservableCollection<MenuItem>();

        public static int AxisX { get; private set; }
        public static int AxisY { get; private set; }
        public static int MinLineY
        {
            get { return _graphBox.Y0; }
        }
        public static int MaxLineY
        {
            get { return _graphBox.Y1; }
        }
        public static GraphBox GraphBox { get => _graphBox; private set => _graphBox = value; }

        public double ZoomStepSelectorCurrentPositionX
        {
            get { return _zoomStepSelectorCurrentPositionX; }
            set
            {
                if (!_isSelectionActive)
                {
                    if (_zoomStepSelectorCurrentPositionX != value)
                    {
                        _zoomStepSelectorCurrentPositionX = value;
                        if (!_isSelectingSteps)
                            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ZoomStepSelectorCurrentPositionX"));
                    }
                } else
                {
                    _zoomStepSelectorNewPositionX = value;
                }
            }
        }

        public Int32 ZoomStepSelectorWidth
        {
            get { return _zoomStepSelectorWidth; }
            set
            {
                if (_zoomStepSelectorWidth != value)
                {
                    _zoomStepSelectorWidth = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ZoomStepSelectorWidth"));
                }
            }
        }

        public Boolean IsBackToPreviousGraphButtonEnabled
        {
            get { return _isBackToPreviousGraphButtonEnabled; }
            set
            {
                if (_isBackToPreviousGraphButtonEnabled != value)
                {
                    _isBackToPreviousGraphButtonEnabled = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsBackToPreviousGraphButtonEnabled"));
                }
            }
        }

        public Visibility ZoomStepSelectorVisibility
        {
            get { return _zoomStepSelectorVisibility; }
            set
            {
                if (_zoomStepSelectorVisibility != value)
                {
                    _zoomStepSelectorVisibility = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ZoomStepSelectorVisibility"));
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

        public ICommand ChangeGraphModeActionButton
        {
            get
            {
                if(_changeGraphModeActionButton == null)
                {
                    _changeGraphModeActionButton = new RelayCommand(param => OnChangeGraphModeAction());
                }
                return _changeGraphModeActionButton;
            }
        }

        public ICommand BackToPreviousGraphActionButton
        {
            get
            {
                if (_backToPreviousGraphActionButton == null)
                {
                    _backToPreviousGraphActionButton = new RelayCommand(param => OnBackToPreviousGraphAction());
                }
                return _backToPreviousGraphActionButton;
            }
        }

        public ICommand ZoomGraphActionButton
        {
            get
            {
                if (_zoomGraphActionButton == null)
                {
                    _zoomGraphActionButton = new RelayCommand(param => OnZoomGraphAction());
                }
                return _zoomGraphActionButton;
            }
        }

        public ICommand UpdateSubstepGraphObjectsList
        {
            get
            {
                if (_updateSubstepGraphObjectsListAction == null)
                {
                    _updateSubstepGraphObjectsListAction = new RelayCommand(param => _prepareGraphDetailsTask.Wait());
                }
                return _updateSubstepGraphObjectsListAction;
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

        public static Thickness MinValueLabelMargin { get; private set; }

        public static Thickness MaxValueLabelMargin { get; private set; }

        public string SelectedProfilingStep
        {
            get { return _selectedProfilingStep; }
            set
            {
                if (_selectedProfilingStep != value)
                {
                    SaveCurrentState();
                    ChangeSelectedStep(value);
                    if (!_isUpdating)
                        UpdateCanvas();
                    UpdateObservables();
                    PrepareCurrectGraphDetailsAsync();
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
                    SaveCurrentState();
                    _selectedGrapScale = value;
                    if (!_isUpdating)
                        UpdateCanvas();
                    PrepareCurrectGraphDetailsAsync();
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

        public Boolean IsChangeGraphModeActionButtonChecked
        {
            get { return _isNetValueGraphModeOn; }
            set
            {
                if (_isNetValueGraphModeOn != value)
                {
                    _isNetValueGraphModeOn = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsChangeGraphModeActionButtonChecked"));
                }
            }
        }

        public Brush GraphLineColour
        {
            get { return _graphLineColour; }
            set
            {
                if (_graphLineColour != value)
                {
                    _graphLineColour = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("GraphLineColour"));
                }
            }
        }

        static GraphViewModel()
        {
            _graphBox = new GraphBox();
            AxisX = Settings.AxisX;
            AxisY = Settings.AxisY;
            _graphBox.Y1 = 20;
            _graphBox.Y0 = AxisY - 20;
            _graphBox.X1 = AxisX;
            MinValueLabelMargin = new Thickness(0, _graphBox.Y1 - 13, 0, 0);
            MaxValueLabelMargin = new Thickness(0, 0, 0, _graphBox.Y1 - 13);
        }

        public GraphViewModel(string profilerDataPath)
        {
            //This does not comply with MVVM pattern at all, but I don't care.
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("AxisX"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("AxisY"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MinLineY"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MaxLineY"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MinValueLabelMargin"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MaxValueLabelMargin"));

            GrapScales = new ObservableCollection<int>(Settings.Scale);
            LineSteps = new ObservableCollection<StepDetailsEntryViewModel>();
            _profilerDataPath = profilerDataPath;
            LoadStepNames();
            UpdateLineSteps();
            _selectedGrapScale = Settings.Scale.Last();
            SelectedProfilingStep = ProfilingSteps[0];
        }

        void LoadStepNames()
        {
            ProfilingSteps = new ObservableCollection<string>();
            using (StreamReader stream = new StreamReader(System.IO.Path.Combine(_profilerDataPath, "names")))
            {
                string name;
                Int32 id = 0;
                while ((name = stream.ReadLine()) != null)
                {
                    ProfilingSteps.Add(name);
                    _graphObjects.Add(name, new GraphObject(_profilerDataPath, id++));
                }
            }
        }

        void UpdateAbsolutes()
        {
            Double[] values = _graphObjects[SelectedProfilingStep].AbsoluteValues;
            AbsoluteMinimumLabelText = values[0].ToString();
            AbsoluteMaximumLabelText = values[1].ToString();
        }
        
        void UpdateCanvas()
        {
            if (_currentGraphObject == null)
                return;
            _isUpdating = true;
            var graph = _currentGraphObject.GetOrBuildGraph(SelectedGrapScale);
            UpdateCanvasObservables(graph);
            _isUpdating = false;
        }

        void UpdateCanvas(ZoomInfo graphZoom)
        {
            _isUpdating = true;
            var graph = _currentGraphObject.GetOrBuildGraph(graphZoom);
            UpdateCanvasObservables(graph);
            if(graph.Indexes.Length != _selectedGrapScale)
            {
                _selectedGrapScale = graph.Indexes.Length;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedGrapScale"));
            }
            _isUpdating = false;
        }

        void UpdateCanvasObservables(GraphLine graph, Boolean isNet = false)
        {
            if (isNet)
            {
                MaxValue = graph.NetMax;
                MinValue = graph.NetMin;
                GraphPoints = graph.NetLine.Points;
                GraphLineColour = graph.NetLineBrush;
            }
            else
            {
                MaxValue = graph.Max;
                MinValue = graph.Min;
                GraphPoints = graph.Line.Points;
                GraphLineColour = graph.LineBrush;
            }
        }

        void UpdateLineSteps()
        {
            LineSteps.Add(new StepDetailsEntryViewModel());
        }

        private void SelectorEventHandlerExecutor(object param)
        {
            Type type = this.GetType();
            MethodInfo methodInfo = type.GetMethod(param.ToString(), BindingFlags.NonPublic | BindingFlags.Instance);
            methodInfo.Invoke(this, null);
        }

        private void ShowIterationDetails()
        {
            _isSelectingSteps = false;
            if(ZoomStepSelectorWidth > 1)
            {
                if (_isSelectionActive)
                {
                    _isSelectionActive = false;
                    ZoomStepSelectorWidth = 1;
                    ZoomStepSelectorCurrentPositionX = _zoomStepSelectorNewPositionX;
                }
                else
                {
                    _isSelectionActive = true;
                    return;
                }
            }
            var detailsEntry = BuildStepDetailEntry((byte)ProfilingSteps.IndexOf(_selectedProfilingStep), 
                _currentGraphObject.RelativeIdToId(GetMessageRelativeIdFromPosition()));
            detailsEntry.StepDetailsDefaultVisibility = Visibility.Visible;
            StepDetailsEvent?.Invoke(this, new StepDetailsEventArgs(detailsEntry));
        }

        private void ZoomStepSelectorHide()
        {
            if(!_isSelectionActive)
                SelectorVisibilityChange(Visibility.Hidden);
        }

        private void ZoomStepSelectorShow()
        {
            SelectorVisibilityChange(Visibility.Visible);
        }

        private void SelectorVisibilityChange(Visibility visibility)
        {
            ZoomStepSelectorVisibility = visibility;
        }

        private void StepsSelectionStart()
        {
            _zoomStepSelectorBasePositionX = ZoomStepSelectorCurrentPositionX;
            _isSelectingSteps = true;
        }

        private void StepSelectionChange()
        {
            if (_isSelectingSteps)
            {
                var width = ZoomStepSelectorCurrentPositionX - _zoomStepSelectorBasePositionX;
                if (width == 0)
                    width = 1;
                else if(width < 0)
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ZoomStepSelectorCurrentPositionX"));
                ZoomStepSelectorWidth = (Int32)Math.Round(Math.Abs(width), MidpointRounding.AwayFromZero);
            }
        }

        private void OnChangeGraphModeAction()
        {
            _isUpdating = true;
            GraphLine graphLine;
            if (IsChangeGraphModeActionButtonChecked)
            {
                graphLine = _currentGraphObject.NetGraph;
                if(graphLine == null)
                {
                    _prepareGraphDetailsTask.Wait();
                    graphLine = _currentGraphObject.BuildNetGraph();
                }
                UpdateCanvasObservables(graphLine, true);
            }
            else
            {
                graphLine = _currentGraphObject.Graph;
                UpdateCanvasObservables(graphLine);
            }
            _isUpdating = false;
        }

        private void ChangeToSubstepGraphObject(string substepName)
        {

        }

        private void OnBackToPreviousGraphAction(Int32 step = 0, Boolean isCalledFromContextMenu = false)
        {
            if(!isCalledFromContextMenu)
                step = _previousStates.Count - 1;
            var state = _previousStates[step];
            _previousStates.RemoveRange(step, _previousStates.Count - step);
            PreviousSteps.RemoveRange(step, PreviousSteps.Count - step);

            if (_previousStates.Count == 0)
                IsBackToPreviousGraphButtonEnabled = false;
            LoadState(state);
        }

        private void OnZoomGraphAction()
        {
            if (!_isSelectionActive)
                return;
            SaveCurrentState();
            var zoom = new ZoomInfo();
            zoom.Scale = _selectedGrapScale;
            if(_zoomStepSelectorBasePositionX < _zoomStepSelectorCurrentPositionX)
            {
                zoom.LowerBound = _currentGraphObject.RelativeIdToId(GetMessageRelativeIdFromPosition(_zoomStepSelectorBasePositionX));
                zoom.UpperBound = _currentGraphObject.RelativeIdToId(GetMessageRelativeIdFromPosition());
            }
            else
            {
                zoom.LowerBound = _currentGraphObject.RelativeIdToId(GetMessageRelativeIdFromPosition());
                zoom.UpperBound = _currentGraphObject.RelativeIdToId(GetMessageRelativeIdFromPosition(_zoomStepSelectorBasePositionX));
            }
            if (!_prepareGraphDetailsTask.IsCompleted)
                while (!_prepareGraphDetailsTask.IsCompleted) { Thread.Yield(); }
            UpdateCanvas(zoom);

            var zoomedEntry = new MenuItem(zoom.ToString());
            zoomedEntry.Command = new RelayCommand(param => { UpdateCanvas(zoom); });
            _stepsZoomedGraphObjects[_selectedProfilingStep].Add(zoomedEntry);
            StepZoomedGraphObjects.Add(zoomedEntry);
            _isSelectionActive = false;
            ZoomStepSelectorWidth = 1;
            PrepareCurrectGraphDetailsAsync();
        }

        private void OnViewSubstepGraphAction(Byte substepNameId, List<Int32> substepStepIds, Guid graphId)
        {
            _isUpdating = true;
            SaveCurrentState();
            ChangeSelectedStep(ProfilingSteps[substepNameId]);
            _selectedGrapScale = substepStepIds.Count;
            var graph = _currentGraphObject.GetOrBuildGraph(graphId, substepStepIds);
            UpdateCanvasObservables(graph);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedProfilingStep"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedGrapScale"));
            _isUpdating = false;
        }

        private StepDetailsEntryViewModel BuildStepDetailEntry(Byte stepId, Int32 positionId, Byte nesting = 0)
        {
            StepDetailsEntryViewModel stepDetails = new StepDetailsEntryViewModel();
            stepDetails.StepNameText = ProfilingSteps[stepId];
            //this call might technically occur during background data loading. Waiter can be added here to prevent that. 
            var message = _graphObjects[ProfilingSteps[stepId]].GetDataMessageById(positionId);
            stepDetails.StepPadding = nesting;
            stepDetails.TotalDurationText = message.Duration;
            stepDetails.NetDurationText = message.Duration;
            StepDetailsEntryViewModel subStep;
            nesting++;
            foreach (var substep in message.Substeps)
            {
                subStep = BuildStepDetailEntry(substep.StepId, substep.PositionId, nesting);
                stepDetails.NetDurationText -= subStep.TotalDurationText;
                if (stepDetails.NetDurationText < 0)
                    stepDetails.NetDurationText = 0;
                stepDetails.SubstepDetails.Add(subStep);
            }
            return stepDetails;
        }

        private void SaveCurrentState()
        {
            if (string.IsNullOrEmpty(_selectedProfilingStep) || _selectedGrapScale == 0)
                return;
            var menuItem = new MenuItem();
            var currentZoom = _currentGraphObject.CurrentZoom;
            if (currentZoom == null)
                menuItem.Header = string.Format("{0} : {1}", _selectedProfilingStep, _selectedGrapScale);
            else
                menuItem.Header = string.Format("{0} : {1}", _selectedProfilingStep, currentZoom.ToString());
            IsBackToPreviousGraphButtonEnabled = true;
            var stepPosition = _previousStates.Count;
            menuItem.Command = new RelayCommand(param => { OnBackToPreviousGraphAction(stepPosition, true); });
            PreviousSteps.Add(menuItem);
            _previousStates.Add(new StepViewerState(_selectedProfilingStep, _selectedGrapScale, currentZoom));
        }

        private void LoadState(StepViewerState state)
        {
            if (_prepareGraphDetailsTask != null && !_prepareGraphDetailsTask.IsCompleted)
            {
                _prepareGraphDetailsTokenSource.Cancel(true);
                while (!_prepareGraphDetailsTask.IsCanceled) { Thread.Yield(); }
            }
            _selectedProfilingStep = state.ProfilingStepName;
            _currentGraphObject = _graphObjects[_selectedProfilingStep];
            _selectedGrapScale = state.Scale;
            IsChangeGraphModeActionButtonChecked = false;
            UpdateObservables();
            if (state.Zoom == null)
                UpdateCanvas();
            else
                UpdateCanvas(state.Zoom);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedProfilingStep"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedGrapScale"));
        }

        private Int32 GetMessageRelativeIdFromPosition()
        {
            return GetMessageRelativeIdFromPosition((Int32)Math.Round(ZoomStepSelectorCurrentPositionX, MidpointRounding.AwayFromZero));
        }

        private Int32 GetMessageRelativeIdFromPosition(Double position)
        {
            return GetMessageRelativeIdFromPosition((Int32)Math.Round(position, MidpointRounding.AwayFromZero));
        }

        private Int32 GetMessageRelativeIdFromPosition(Int32 position)
        {
            return position / (AxisX / _selectedGrapScale);
        }

        private void UpdateObservables()
        {
            UpdateAbsolutes();
            StepZoomedGraphObjects.Update(_stepsZoomedGraphObjects[_selectedProfilingStep]);
            SupstepGraphObjects.Update(_substepsGraphObjects[_selectedProfilingStep]);
        }

        private void PrepareCurrectGraphDetailsAsync()
        {
            _prepareGraphDetailsTokenSource = new CancellationTokenSource();
            _prepareGraphDetailsTask = Task.Run(() => LoadCurrectGraphDetails(), _prepareGraphDetailsTokenSource.Token);
        }

        private void CancelLoadGraphDetails()
        {
            if (_prepareGraphDetailsTask == null)
                return; 
            
            if(_prepareGraphDetailsTask.Status == TaskStatus.Created || _prepareGraphDetailsTask.Status == TaskStatus.WaitingToRun 
                || _prepareGraphDetailsTask.Status == TaskStatus.Running)
            {
                _prepareGraphDetailsTokenSource.Cancel();
                while(!_prepareGraphDetailsTask.IsCanceled) { Thread.Yield(); }
            }
        }

        private void LoadCurrectGraphDetails()
        {
            if (IsSubstepDetaisLoaded)
                return;
            DataSubmessage[] substeps;
            Dictionary<Byte, List<Int32>> substepIndexesById = new Dictionary<Byte, List<Int32>>();
            var currentIndexers = _currentGraphObject.GetCurrentGraphIndexes();
            foreach (var index in currentIndexers)
            {
                substeps = _currentGraphObject[index].Substeps;
                foreach(var substep in substeps)
                {
                    if (_prepareGraphDetailsTokenSource.IsCancellationRequested)
                        _prepareGraphDetailsTokenSource.Token.ThrowIfCancellationRequested();
                    if (!substepIndexesById.ContainsKey(substep.StepId))
                    {
                        substepIndexesById.Add(substep.StepId, new List<int>());
                    }
                    substepIndexesById[substep.StepId].Add(substep.PositionId);
                }
            }
            if (substepIndexesById.Count == 0 || _prepareGraphDetailsTokenSource.IsCancellationRequested)
                _prepareGraphDetailsTokenSource.Token.ThrowIfCancellationRequested();
            foreach (var key in substepIndexesById.Keys)
            {
                if (_prepareGraphDetailsTokenSource.IsCancellationRequested)
                    _prepareGraphDetailsTokenSource.Token.ThrowIfCancellationRequested();
                var graphId = Guid.NewGuid();
                var menuItem = new MenuItem() {
                    Header = ProfilingSteps[key],
                    Command = new RelayCommand(param => { OnViewSubstepGraphAction(key, substepIndexesById[key], graphId); })
                };
                SupstepGraphObjects.TryAdd(menuItem);
                _substepsGraphObjects[_selectedProfilingStep].TryAdd(menuItem);
                _graphObjects[ProfilingSteps[key]].LoadSteps(substepIndexesById[key]);
            }
            foreach (Int32 index in currentIndexers)
            {
                if (_prepareGraphDetailsTokenSource.IsCancellationRequested)
                    _prepareGraphDetailsTokenSource.Token.ThrowIfCancellationRequested();
                foreach (var substep in _currentGraphObject[index].Substeps)
                {
                    _currentGraphObject[index].NetDuration -= _graphObjects[ProfilingSteps[substep.StepId]][substep.PositionId].Duration;
                }
            }
            IsSubstepDetaisLoaded = true;
        }

        private void ChangeSelectedStep(string newSelectedStapName)
        {
            if(_prepareGraphDetailsTask != null)
            {
                CancelLoadGraphDetails();
            }

            _selectedProfilingStep = newSelectedStapName;
            _currentGraphObject = _graphObjects[_selectedProfilingStep];
            IsChangeGraphModeActionButtonChecked = false;
            if (!_stepsZoomedGraphObjects.ContainsKey(_selectedProfilingStep))
                _stepsZoomedGraphObjects.Add(_selectedProfilingStep, new List<MenuItem>());
            if (!_substepsGraphObjects.ContainsKey(_selectedProfilingStep))
                _substepsGraphObjects.Add(_selectedProfilingStep, new List<MenuItem>());
            if (_isSelectionActive)
            {
                _isSelectionActive = false;
                ZoomStepSelectorWidth = 1;
            }
            UpdateObservables();
        }
    }

    public struct GraphBox
    {
        public Int32 X0;
        public Int32 Y0;
        public Int32 X1;
        public Int32 Y1;
    }

    public class MenuItem
    {
        public string Header { get; set; }
        public ICommand Command { get; set; }

        public MenuItem() { }

        public MenuItem(string item)
        {
            Header = item;
        }

        public override bool Equals(object obj)
        {
            if (obj is MenuItem menuItem)
                return Header.Equals(menuItem.Header);
            else
                return false;
        }

        public override int GetHashCode()
        {
            return Header.GetHashCode();
        }
    }

    public class StepViewerState
    {
        public String ProfilingStepName;
        public Int32 Scale;
        public ZoomInfo Zoom;

        public StepViewerState(String profilingStepName, Int32 scale, ZoomInfo zoom = null)
        {
            ProfilingStepName = profilingStepName;
            Scale = scale;
            Zoom = zoom;
        }

        public override int GetHashCode()
        {
            return (ProfilingStepName + Scale + Zoom).GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if(obj is StepViewerState state)
            {
                return ProfilingStepName.Equals(state.ProfilingStepName) && Scale == state.Scale
                    && (Zoom != null ? Zoom.Equals(state.Zoom) : state.Zoom == null);
            }
            else
            {
                return false;
            }
        }
    }

    public class StepDetailsEventArgs : EventArgs
    {
        public StepDetailsEntryViewModel Message { get; }

        public StepDetailsEventArgs(StepDetailsEntryViewModel message)
        {
            Message = message;
        }
    }
}
