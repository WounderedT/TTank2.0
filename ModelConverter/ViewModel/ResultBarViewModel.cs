using Common.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ModelConverter.ViewModel
{
    public enum MessageStyle
    {
        INFO,
        ERROR
    }

    public class ResultBarViewModel : INotifyPropertyChanged
    {
        private const String InfoImagePath = "/AssemblyName;Resources/Images/info_icon.png";
        private const String ErrorMessagePath = "/AssemblyName;Resources/Images/error_icon.png";

        private String _message;
        private String _messageStyleImageSource;
        private MessageStyle _messageStyle;
        private Visibility _viewLogButtonVisibility;
        private RelayCommand _viewLogAction;
        private RelayCommand _okAction;

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler ResultsCleared;

        #region Properties

        public String Message
        {
            get { return _message; }
            set
            {
                _message = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Message"));
            }
        }

        public String MessageStyleImageSource
        {
            get { return _messageStyleImageSource; }
            set
            {
                _messageStyleImageSource = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MessageStyleImageSource"));
            }
        }

        public Visibility ViewLogButtonVisibility
        {
            get { return _viewLogButtonVisibility; }
            set
            {
                if(_viewLogButtonVisibility != value)
                {
                    _viewLogButtonVisibility = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ViewLogButtonVisibility"));
                }
            }
        }

        public ICommand ViewLogAction
        {
            get
            {
                if(_viewLogAction == null)
                {
                    _viewLogAction = new RelayCommand(param => OnViewLog());
                }
                return _viewLogAction;
            }
        }

        public ICommand OkAction
        {
            get
            {
                if (_okAction == null)
                {
                    _okAction = new RelayCommand(param => OnOk());
                }
                return _okAction;
            }
        }

        public String LogFilePath { get; set; }

        public MessageStyle MessageStyle
        {
            get { return _messageStyle; }
            set
            {
                _messageStyle = value;
                UpdateMessageStyle();
            }
        }

        #endregion

        private void OnViewLog()
        {
            System.Diagnostics.Process.Start(LogFilePath);
        }

        private void OnOk()
        {
            ResultsCleared?.Invoke(this, new EventArgs());
        }

        private void UpdateMessageStyle()
        {
            switch (_messageStyle)
            {
                case MessageStyle.INFO:
                    MessageStyleImageSource = InfoImagePath;
                    ViewLogButtonVisibility = Visibility.Collapsed;
                    break;
                case MessageStyle.ERROR:
                    MessageStyleImageSource = ErrorMessagePath;
                    ViewLogButtonVisibility = Visibility.Visible;
                    break;
            }
        }
    }
}
