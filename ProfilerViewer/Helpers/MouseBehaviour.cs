using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace ProfilerViewer.Helpers
{
    public class MouseBehaviour : Behavior<FrameworkElement>
    {
        public static readonly DependencyProperty MousePropertyX =
            DependencyProperty.RegisterAttached("MouseX", typeof(double), typeof(MouseBehaviour), new PropertyMetadata(default(double)));

        public double MouseX
        {
            get { return (double)GetValue(MousePropertyX); }
            set { SetValue(MousePropertyX, value); }
        }

        protected override void OnAttached()
        {
            AssociatedObject.MouseMove += OnMouseMoveAction;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.MouseMove -= OnMouseMoveAction;
        }

        private void OnMouseMoveAction(object sender, MouseEventArgs args)
        {
            MouseX = args.GetPosition(AssociatedObject).X;
        }
    }
}
