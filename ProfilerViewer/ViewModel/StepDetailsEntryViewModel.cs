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
    public class StepDetailsEntryViewModel : INotifyPropertyChanged
    {
        private static Brush _stepDetailsBackgroundBrush;
        private int _stepPadding;
        private string _stepNameText;
        private double _totalDurationText;
        private double _netDurationText;
        private Boolean _isChecked = false;
        private Visibility _stepDetailsDefaultVisibility = Visibility.Collapsed;

        public ObservableCollection<StepDetailsEntryViewModel> SubstepDetails { get; set; } = new ObservableCollection<StepDetailsEntryViewModel>();

        public event PropertyChangedEventHandler PropertyChanged;

        public Visibility StepDetailsDefaultVisibility
        {
            get { return _stepDetailsDefaultVisibility; }
            set
            {
                if(_stepDetailsDefaultVisibility != value)
                {
                    _stepDetailsDefaultVisibility = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("StepDetailsDefaultVisibility"));
                }
            }
        }

        public Visibility ChangeSubstepsVisibilityButtonVisibility
        {
            get
            {
                return SubstepDetails.Count != 0 ? Visibility.Visible : Visibility.Hidden;
            }
        }

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

        public Boolean IsChecked
        {
            get { return _isChecked; }
            set
            {
                if (_isChecked != value)
                {
                    _isChecked = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsChecked"));
                }
            }
        }

        public int StepPadding
        {
            get { return _stepPadding * 15; }
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

        private ICommand _changeSubstepsVisibilityButton;
        public ICommand ChangeSubstepsVisibilityButton
        {
            get
            {
                if(_changeSubstepsVisibilityButton == null)
                {
                    _changeSubstepsVisibilityButton = new RelayCommand(param => OnChangeSubstepsVisibilityAction());
                }
                return _changeSubstepsVisibilityButton;
            }
        }

        private void OnChangeSubstepsVisibilityAction()
        {
            var visibility = _isChecked ? Visibility.Visible : Visibility.Collapsed;
            foreach (var substep in SubstepDetails)
                substep.StepDetailsDefaultVisibility = visibility;
        }

        public static Brush GetBackgroundColor()
        {
            return (_stepDetailsBackgroundBrush == Brushes.LightGray) ? Brushes.DarkGray : Brushes.LightGray;
        }
    }
}
