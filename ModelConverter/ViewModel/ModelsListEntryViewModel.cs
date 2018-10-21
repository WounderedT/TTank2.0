using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ModelConverter.ViewModel
{
    public enum ModelsListEntryProgressEnum
    {
        CLEAR,
        PEDING,
        INPROGRESS,
        COMPLETE,
        ERROR,
        SKIPPED
    }

    public class ModelsListEntryViewModel: INotifyPropertyChanged
    {
        private Boolean _isModelSelected;
        private String _modelRelativePath;
        private Brush _modelsListEntryBackgroundBrush;
        private Brush _clearColour;

        public event PropertyChangedEventHandler PropertyChanged;

        public Boolean IsModelSelected
        {
            get { return _isModelSelected; }
            set
            {
                if(_isModelSelected != value)
                {
                    _isModelSelected = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsModelSelected"));
                }
            }
        }

        public String ModelRelativePath
        {
            get { return _modelRelativePath; }
            set
            {
                _modelRelativePath = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ModelRelativePath"));
            }
        }

        public Brush ModelsListEntryBackgroundBrush
        {
            get { return _modelsListEntryBackgroundBrush; }
            set
            {
                if(_modelsListEntryBackgroundBrush != value)
                {
                    _modelsListEntryBackgroundBrush = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ModelsListEntryBackgroundBrush"));
                }
            }
        }

        public String ModelFullPath { get; set; }

        public ModelsListEntryViewModel(Int32 position)
        {
            _clearColour = ChooseColour(position);
            ModelsListEntryBackgroundBrush = _clearColour;
        }

        public void UpdateState(ModelsListEntryProgressEnum progress)
        {
            switch (progress)
            {
                case ModelsListEntryProgressEnum.CLEAR:
                    ModelsListEntryBackgroundBrush = _clearColour;
                    break;
                case ModelsListEntryProgressEnum.COMPLETE:
                    //ModelsListEntryBackgroundBrush = Brushes.Green;
                    ModelsListEntryBackgroundBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#4fff5e"));
                    break;
                case ModelsListEntryProgressEnum.ERROR:
                    //ModelsListEntryBackgroundBrush = Brushes.Red;
                    ModelsListEntryBackgroundBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#ff6f67"));
                    break;
                case ModelsListEntryProgressEnum.INPROGRESS:
                    ModelsListEntryBackgroundBrush = Brushes.Gold;
                    break;
                case ModelsListEntryProgressEnum.PEDING:
                    ModelsListEntryBackgroundBrush = Brushes.LightBlue;
                    break;
                case ModelsListEntryProgressEnum.SKIPPED:
                    ModelsListEntryBackgroundBrush = Brushes.LightSlateGray;
                    break;
            }
        }

        private Brush ChooseColour(Int32 position)
        {
            return (position % 2 != 0) ? Brushes.DarkGray : Brushes.GhostWhite;
        }

    }
}
