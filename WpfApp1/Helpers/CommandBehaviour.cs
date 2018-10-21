using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Shapes;

namespace ProfilerViewer.Helpers
{
    public static class CommandBehaviour
    {
        public static readonly DependencyProperty MouseEnterCommandProperty =
            DependencyProperty.RegisterAttached("MouseEnterCommand", typeof(ICommand), typeof(CommandBehaviour), new PropertyMetadata(null, OnMouseEnterCommand));

        //public static readonly DependencyProperty MouseLeaveCommandProperty =
        //    DependencyProperty.RegisterAttached("MouseLeaveCommand", typeof(ICommand), typeof(CommandBehaviour), new PropertyMetadata(null, OnMouseLeaveCommand));

        public static readonly DependencyProperty CommnadParameterProperty = 
            DependencyProperty.RegisterAttached("CommnadParameter", typeof(object), typeof(CommandBehaviour), new PropertyMetadata(null));

        public static ICommand GetCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(MouseEnterCommandProperty);
        }

        public static void SetCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(MouseEnterCommandProperty, value);
        }

        public static object GetCommnadParameter(DependencyObject obj)
        {
            return (object)obj.GetValue(CommnadParameterProperty);
        }

        public static void SetCommnadParameter(DependencyObject obj, object value)
        {
            obj.SetValue(CommnadParameterProperty, value);
        }

        public static void OnMouseEnterCommand(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Canvas c = d as Canvas;
            if (c == null)
                return;

            c.MouseEnter += OnMouseEnter;
        }

        public static void OnMouseLeaveCommand(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Canvas c = d as Canvas;
            if (c == null)
                return;

            c.MouseEnter += OnMouseEnter;
        }

        static void OnMouseEnter(object sender, MouseEventArgs args)
        {
            Canvas c = (Canvas)sender;
            ICommand command = c.GetValue(CommandBehaviour.MouseEnterCommandProperty) as ICommand;
            //object param = c.GetValue(CommandBehavior.CommnadParameterProperty);
            if (command != null && command.CanExecute(null))
                command.Execute(null);
        }
    }
}
