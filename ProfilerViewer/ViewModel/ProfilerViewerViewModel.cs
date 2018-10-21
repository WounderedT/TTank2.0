using Microsoft.WindowsAPICodePack.Dialogs;
using ProfilerViewer.Helpers;
using ProfilerViewer.Structures;
using ProfilerViewer.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using TPresenter.Profiler;

namespace ProfilerViewer.ViewModel
{
    public class ProfilerViewerViewModel: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<GraphViewModel> GraphData { get; set; }

        private bool _addGraphViewerButtonVisibility;
        private string _profilerViewerDefaultPath;
        private ICommand _loadProfilerDataAction;

        public static DataModel data;
        public static Dictionary<string, List<double>> ProfilerData = new Dictionary<string, List<double>>();

        public static ObservableCollection<string> ProfilingSteps { get; set; }

        public string ProfilerViewerDefaultPath
        {
            get
            {
                if (string.IsNullOrEmpty(_profilerViewerDefaultPath))
                {
                    var tmp = AppDomain.CurrentDomain.BaseDirectory.Split(new string[] { "\\" }, StringSplitOptions.None);
                    StringBuilder path = new StringBuilder();
                    for (int i = 0; i < tmp.Length - 4; i++)
                    {
                        path.Append(tmp[i]);
                        path.Append("\\");
                    }
                    _profilerViewerDefaultPath = System.IO.Path.Combine(path.ToString(), "Content", "_ProfilingData");
                }
                return _profilerViewerDefaultPath;
            }
        }

        public bool AddGraphViewerButtonVisibility
        {
            get { return _addGraphViewerButtonVisibility; }
            set
            {
                if (_addGraphViewerButtonVisibility != value)
                {
                    _addGraphViewerButtonVisibility = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("AddGraphViewerButtonVisibility"));
                }
            }
        }

        public ICommand LoadProfilerDataAction
        {
            get
            {
                if(_loadProfilerDataAction == null)
                {
                    _loadProfilerDataAction = new RelayCommand(param => OnLoadProfilerData());
                }
                return _loadProfilerDataAction;
            }
        }

        //public ObservableCollection<StepDetailsEntryViewModel> SelectedStepDetails { get; set; } = new ObservableCollection<StepDetailsEntryViewModel>();

        private StepDetailsEntryViewModel _selectedStepDetails;
        public StepDetailsEntryViewModel SelectedStepDetails
        {
            get { return _selectedStepDetails; }
            set
            {
                if (_selectedStepDetails != value)
                {
                    _selectedStepDetails = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedStepDetails"));
                }
            }
        }

        public ProfilerViewerViewModel()
        {
            ProfilingSteps = new ObservableCollection<string>();
            GraphData = new ObservableCollection<GraphViewModel>();
            //GraphData.Add(new GraphViewModel());
            //_addGraphViewerButtonVisibility = true;
            //data = new DataModel(Settings.Scale.Last());

            //ProfilingSteps.Add("TEST");
            //List<double> randData = new List<double>();
            //Random random = new Random();
            //for (int i = 0; i < GraphData[0].GrapScales[3]; i++)
            //    randData.Add(random.NextDouble());
            //ProfilerData.Add("TEST", randData);
        }

        public void ParseProfilingData(IDataObject dataObject)
        {
            if (dataObject.GetDataPresent(DataFormats.FileDrop))
            {
                string file = ((string[])dataObject.GetData(DataFormats.FileDrop))[0];
                if (!file.Contains(".profiler"))
                    return;

                List<ProfileMessage> messages;
                List<long> memoryPrint;
                MyProfiler.Load(file, out messages, out memoryPrint);

                data.Init(messages);
                foreach (string key in data.data.Keys)
                    ProfilingSteps.Add(key);

                //Graphs.Clear();
                //AbsoluteValues.Clear();
            }
        }

        private void OnLoadProfilerData()
        {
            var dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            dialog.InitialDirectory = ProfilerViewerDefaultPath;
            CommonFileDialogResult result = dialog.ShowDialog();
            if(result == CommonFileDialogResult.Ok)
            {
                GraphData.Clear();
                var model = new GraphViewModel(dialog.FileName);
                model.StepDetailsEvent += OnStepDetailsEvent;
                GraphData.Add(model);
                SelectedStepDetails = new StepDetailsEntryViewModel();
            }
        }

        private void OnStepDetailsEvent(object sender, EventArgs args)
        {
            SelectedStepDetails = ((StepDetailsEventArgs)args).Message;
            //SelectedStepDetails.Clear();
            //SelectedStepDetails.Add(((StepDetailsEventArgs)args).Message);
            //StepDetailsEntryViewModel stepDetails = new StepDetailsEntryViewModel(((StepDetailsEventArgs)args).Message, ((GraphObject)sender));
            //stepDetails.
            //StepDetails.Add()
        }

        //// Draw a simple graph.
        //private void Window_Loaded(object sender, RoutedEventArgs e)
        //{
        //    const double margin = 10;
        //    double xmin = margin;
        //    double xmax = _canvasWidth - margin;
        //    double ymin = margin;
        //    double ymax = _canvasHeight - margin;
        //    const double step = 10;

        //    // Make the X axis.
        //    GeometryGroup xaxis_geom = new GeometryGroup();
        //    xaxis_geom.Children.Add(new LineGeometry(
        //        new Point(0, ymax), new Point(_canvasWidth, ymax)));
        //    for (double x = xmin + step;
        //        x <= _canvasWidth - step; x += step)
        //    {
        //        xaxis_geom.Children.Add(new LineGeometry(
        //            new Point(x, ymax - margin / 2),
        //            new Point(x, ymax + margin / 2)));
        //    }

        //    Path xaxis_path = new Path();
        //    xaxis_path.StrokeThickness = 1;
        //    xaxis_path.Stroke = Brushes.Black;
        //    xaxis_path.Data = xaxis_geom;

        //    canGraph.Children.Add(xaxis_path);

        //    // Make the Y ayis.
        //    GeometryGroup yaxis_geom = new GeometryGroup();
        //    yaxis_geom.Children.Add(new LineGeometry(
        //        new Point(xmin, 0), new Point(xmin, _canvasHeight)));
        //    for (double y = step; y <= _canvasHeight - step; y += step)
        //    {
        //        yaxis_geom.Children.Add(new LineGeometry(
        //            new Point(xmin - margin / 2, y),
        //            new Point(xmin + margin / 2, y)));
        //    }

        //    Path yaxis_path = new Path();
        //    yaxis_path.StrokeThickness = 1;
        //    yaxis_path.Stroke = Brushes.Black;
        //    yaxis_path.Data = yaxis_geom;

        //    canGraph.Children.Add(yaxis_path);

            //// Make some data sets.
            //Brush[] brushes = { Brushes.Red, Brushes.Green, Brushes.Blue };
            //Random rand = new Random();
            //for (int data_set = 0; data_set < 3; data_set++)
            //{
            //    int last_y = rand.Next((int)ymin, (int)ymax);

            //    PointCollection points = new PointCollection();
            //    for (double x = xmin; x <= xmax; x += step)
            //    {
            //        last_y = rand.Next(last_y - 10, last_y + 10);
            //        if (last_y < ymin) last_y = (int)ymin;
            //        if (last_y > ymax) last_y = (int)ymax;
            //        points.Add(new Point(x, last_y));
            //    }

            //    Polyline polyline = new Polyline();
            //    polyline.StrokeThickness = 1;
            //    polyline.Stroke = brushes[data_set];
            //    polyline.Points = points;

            //    canGraph.Children.Add(polyline);
            //}
        //}
    }
}
