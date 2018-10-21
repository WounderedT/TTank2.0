using ProfilerViewer.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ProfilerViewer.ViewModel
{
    public class StepDetailsViewModel : INotifyPropertyChanged
    {
        private static Brush _stepDetailsBackgroundBrush;
        private int _stepPadding;
        private string _stepNameText;
        private double _totalDurationText;
        private double _netDurationText;

        public ObservableCollection<StepDetailsViewModel> SubstepDetails { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public Brush StepDetailsBackgroundBrush
        {
            get { return _stepDetailsBackgroundBrush; }
            set
            {
                if (_stepDetailsBackgroundBrush != value)
                {
                    _stepDetailsBackgroundBrush = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("StepDetailsBackgroundBrush"));
                }
            }
        }

        public int StepPadding
        {
            get { return _stepPadding; }
            set
            {
                if(_stepPadding != value)
                {
                    _stepPadding = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("StepPadding"));
                }
            }
        }

        public string StepNameText
        {
            get { return _stepNameText; }
            set
            {
                if(_stepNameText != value)
                {
                    _stepNameText = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("StepNameText"));
                }
            }
        }

        public double TotalDurationText
        {
            get { return _totalDurationText; }
            set
            {
                if (_totalDurationText != value)
                {
                    _totalDurationText = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TotalDurationText"));
                }
            }
        }

        public double NetDurationText
        {
            get { return _netDurationText; }
            set
            {
                if (_netDurationText != value)
                {
                    _netDurationText = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("NetDurationText"));
                }
            }
        }

        public static Brush GetBackgroundColor()
        {
            return (_stepDetailsBackgroundBrush == Brushes.LightGray) ? Brushes.DarkGray : Brushes.LightGray;
        }
    }
}
