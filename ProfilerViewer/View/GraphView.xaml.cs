using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ProfilerViewer.View
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class GraphView : UserControl
    {
        public GraphView()
        {
            InitializeComponent();
        }

        //This is just a workaround for empty list. This event should not be hanled like this. 
        //TODO: Find MVVM-compliant solution.
        private void ChangeGraphZoomButton_Opened(object sender, RoutedEventArgs e)
        {
            if(sender is ContextMenu contextMen)
            {
                if(contextMen.DataContext is ViewModel.GraphViewModel context)
                {
                    if(context.StepZoomedGraphObjects.Count == 0)
                    {
                        contextMen.IsOpen = false;
                    }
                }
            }
        }

        //This is just a workaround for empty list. This event should not be hanled like this. 
        //TODO: Find MVVM-compliant solution.
        private void SelectSubstepButton_Opened(object sender, RoutedEventArgs e)
        {
            if (sender is ContextMenu contextMen)
            {
                if (contextMen.DataContext is ViewModel.GraphViewModel context)
                {
                    if (context.SupstepGraphObjects.Count == 0)
                    {
                        contextMen.IsOpen = false;
                    }
                }
            }
        }
    }
}
