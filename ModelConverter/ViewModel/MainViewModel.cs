using Collada141;
using Common.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TPresenter.Filesystem;

namespace ModelConverter.ViewModel
{
    public class MainViewModel: INotifyPropertyChanged
    {
        private Boolean _areButtonsEnable;
        private Boolean _isModelsListEnabled;
        private Int32 _progressBarValue;
        private RelayCommand _fixOpenColladaAction;
        private RelayCommand _modelsListDropFieldDropAction;
        private RelayCommand _clearModelsListAction;
        private Dictionary<String, Boolean> _internalCollection = new Dictionary<string, bool>();

        public event PropertyChangedEventHandler PropertyChanged;

        #region Properties

        public ObservableCollection<ModelsListEntryViewModel> ModelsListDropFieldItems { get; set; } = new ObservableCollection<ModelsListEntryViewModel>();
        public ObservableCollection<ResultBarViewModel> ResultsBarItem { get; set; } = new ObservableCollection<ResultBarViewModel>();

        public Boolean AreButtonsEnable
        {
            get { return _areButtonsEnable; }
            set
            {
                if(_areButtonsEnable != value)
                {
                    _areButtonsEnable = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("AreButtonsEnable"));
                }
            }
        }

        public Boolean IsModelsListEnabled
        {
            get { return _isModelsListEnabled; }
            set
            {
                if(_isModelsListEnabled != value)
                {
                    _isModelsListEnabled = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsModelsListEnabled"));
                }
            }
        }

        public Int32 ProgressBarValue
        {
            get { return _progressBarValue; }
            set
            {
                if(_progressBarValue != value)
                {
                    _progressBarValue = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ProgressBarValue"));
                }
            }
        }

        public ICommand FixOpenColladaAction
        {
            get
            {
                if(_fixOpenColladaAction == null)
                {
                    _fixOpenColladaAction = new RelayCommand(param => OnTediousActionCall(FixOpenColladaMethod));
                }
                return _fixOpenColladaAction;
            }
        }

        public ICommand ModelsListDropFieldDropAction
        {
            get
            {
                if(_modelsListDropFieldDropAction == null)
                {
                    _modelsListDropFieldDropAction = new RelayCommand(param => OnModelsListDropFieldDrop(param));
                }
                return _modelsListDropFieldDropAction;
            }
        }

        public ICommand ClearModelsListAction
        {
            get
            {
                if(_clearModelsListAction == null)
                {
                    _clearModelsListAction = new RelayCommand(param => OnClearModelsListAction());
                }
                return _clearModelsListAction;
            }
        }

        #endregion

        public MainViewModel()
        {
            AreButtonsEnable = true;
            IsModelsListEnabled = true;

            ColladaProcessor.OverwriteFile = true;
        }

        #region RelayCommand Methods

        private async Task OnTediousActionCall(Func<Task<IResult>> action)
        {
            if(ModelsListDropFieldItems.Count == 0)
            {
                return;
            }

            AreButtonsEnable = false;
            IsModelsListEnabled = false;
            ChangeModelsListEntryBackground();

            ShowResults(await action.Invoke());
        }

        private void OnModelsListDropFieldDrop(Object param)
        {
            var data = (String[])(param as DragEventArgs).Data.GetData(DataFormats.FileDrop);
            foreach(String entry in data)
            {
                if (Directory.Exists(entry))
                {
                    ProcessDirectory(entry);
                }
                else
                {
                    ProcessFile(entry);
                }
            }
        }

        private void OnClearModelsListAction()
        {
            _internalCollection.Clear();
            ModelsListDropFieldItems.Clear();
        }

        #endregion

        #region Drag and Drop Processing Methods

        private void ProcessDirectory(String directoryPath)
        {
            var subDirs = Directory.GetDirectories(directoryPath);
            var files = Directory.GetFiles(directoryPath);
            foreach(String subdir in subDirs)
            {
                ProcessDirectory(subdir);
            }
            foreach(String file in files)
            {
                ProcessFile(file);
            }
        }

        private void ProcessFile(String filePath)
        {
            if (!_internalCollection.ContainsKey(filePath))
            {
                _internalCollection.Add(filePath, true);
                var entry = new ModelsListEntryViewModel(ModelsListDropFieldItems.Count);
                entry.ModelRelativePath = ToRelativePath(filePath);
                entry.ModelFullPath = filePath;
                entry.IsModelSelected = true;
                ModelsListDropFieldItems.Add(entry);
            }
        }

        #endregion

        private async Task<IResult> FixOpenColladaMethod()
        {
            ColladaProcessorResult results = new ColladaProcessorResult("");
            var step = GetProgressStepValue();
            foreach (ModelsListEntryViewModel modelsListEntry in ModelsListDropFieldItems)
            {
                if (modelsListEntry.IsModelSelected)
                {
                    modelsListEntry.UpdateState(ModelsListEntryProgressEnum.INPROGRESS);
                    var result = await ColladaProcessor.ProcessColladaFile(modelsListEntry.ModelFullPath);
                    if (result.IsSuccess)
                    {
                        modelsListEntry.UpdateState(ModelsListEntryProgressEnum.COMPLETE);
                    }
                    else
                    {
                        modelsListEntry.UpdateState(ModelsListEntryProgressEnum.ERROR);
                        results.IsSuccess = false;
                        results.InternalResults.Add(result);
                    }
                }
                ProgressBarValue += step;
            }
            if (results.IsSuccess)
            {
                results.Message = "OPENCOllada skeleton update complete!";
            }
            else
            {
                results.Message = "OPENCOllada skeleton update failed!";
            }
            return results;
        }

        private void OnHideResultBar(object sender, EventArgs args)
        {
            foreach (ModelsListEntryViewModel modelsListEntry in ModelsListDropFieldItems)
            {
                modelsListEntry.UpdateState(ModelsListEntryProgressEnum.CLEAR);
            }
            ResultsBarItem.Clear();
            AreButtonsEnable = true;
            IsModelsListEnabled = true;
            ProgressBarValue = 0;
        }

        private void ChangeModelsListEntryBackground()
        {
            foreach (ModelsListEntryViewModel modelsListEntry in ModelsListDropFieldItems)
            {
                modelsListEntry.UpdateState(modelsListEntry.IsModelSelected ? ModelsListEntryProgressEnum.PEDING : ModelsListEntryProgressEnum.SKIPPED);
            }
        }

        private void ShowResults(IResult result)
        {
            ProgressBarValue = 100;
            ResultsBarItem.Add(ResultPresenter.ShowResult(result, OnHideResultBar));
        }

        private Int32 GetProgressStepValue()
        {
            Int32 activeCount = 0;
            foreach (ModelsListEntryViewModel modelsListEntry in ModelsListDropFieldItems)
            {
                if (modelsListEntry.IsModelSelected)
                {
                    activeCount++;
                }
            }
            if(activeCount > 0)
            {
                return (Int32)(100 / activeCount);
            }
            else
            {
                return 100;
            }
        }

        private String ToRelativePath(String modelPath)
        {
            return modelPath.Split(new String[] { @"\Content\Models\" }, StringSplitOptions.RemoveEmptyEntries).Last();
        }
    }
}
