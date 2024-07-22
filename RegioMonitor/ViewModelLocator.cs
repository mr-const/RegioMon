using System;
using Avalonia;
using Avalonia.Data;
using Avalonia.Interactivity;

namespace RegioMon
{
    public class ViewModelLocator : AvaloniaObject
    {
        static ViewModelLocator()
        {
            AutoWireViewModelProperty.Changed.AddClassHandler<Interactive>(AutoWireViewModelChanged);
        }

        public static void SetAutoWireViewModel(AvaloniaObject element, bool autowireValue)
        {
            element.SetValue(AutoWireViewModelProperty, autowireValue);
        }

        public static bool GetAutoWireViewModel(AvaloniaObject element)
        {
            return element.GetValue(AutoWireViewModelProperty);
        }

        /// <summary>  
        /// AutoWireViewModel attached property  
        /// </summary>  
        public static readonly AttachedProperty<bool> AutoWireViewModelProperty =
            AvaloniaProperty.RegisterAttached<ViewModelLocator, Interactive, bool>(
                "AutoWireViewModel",
                default(bool),
                false,
                BindingMode.OneTime);

        /// <summary>  
        /// Step 5 approach to hookup View with ViewModel  
        /// </summary>  
        /// <param name="d"></param>  
        /// <param name="e"></param>  
        private static void AutoWireViewModelChanged(Interactive interactElem, AvaloniaPropertyChangedEventArgs args)
        {
            // if (DesignerProperties.GetIsInDesignMode(d)) return;
            var viewType = interactElem.GetType();
            var viewTypeName = viewType.ToString().Replace(".View", ".ViewModel");

            var viewModelTypeName = viewTypeName.EndsWith("View")
                ? viewTypeName.Replace("View", "ViewModel")
                : viewTypeName + "ViewModel";
#pragma warning disable IL2057 // Unrecognized value passed to the parameter of method. It's not possible to guarantee the availability of the target type.
            var viewModelType = Type.GetType(viewModelTypeName);
#pragma warning restore IL2057 // Unrecognized value passed to the parameter of method. It's not possible to guarantee the availability of the target type.
            if (viewModelType == null)
                throw new Exception(viewModelTypeName + " does not exist");

            var viewModel = ((App)Application.Current!).Locator?.GetService(viewModelType);
            interactElem.DataContext = viewModel;
        }
    }
}
