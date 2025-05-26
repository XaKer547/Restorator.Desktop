using Restorator.Desktop.ViewModels.Abstract;
using System.Globalization;
using System.Windows;
using System.Windows.Markup;

namespace Restorator.Desktop.Infrastructure
{
    public class DataTemplateManager
    {
        private readonly ResourceDictionary _resources = [];
        private readonly Dictionary<Type, Type> _registeredTemplates = [];
        public DataTemplateManager RegisterDataTemplate<TViewModel, TView>()
         where TViewModel : ViewModelBase
         where TView : FrameworkElement
        {
            return RegisterDataTemplate(typeof(TViewModel), typeof(TView));
        }
        public DataTemplateManager RegisterDataTemplate(Type viewModelType, Type viewType)
        {
            var template = CreateTemplate(viewModelType, viewType);

            var key = template.DataTemplateKey;

            _resources.Add(key, template);

            return this;
        }

        public Type? TryGetRegisteredViewModel(Type viewType)
        {
            return _registeredTemplates[viewType];
        }

        public void InitilizeTemplates(ResourceDictionary resourceDictionary)
        {
            resourceDictionary.MergedDictionaries.Add(_resources);
        }
        public void SetControlsCulture(CultureInfo culture = default)
        {
            if (culture == default)
                culture = CultureInfo.CurrentCulture;

            FrameworkElement.LanguageProperty.OverrideMetadata(typeof(FrameworkElement), new FrameworkPropertyMetadata(
                XmlLanguage.GetLanguage(culture.IetfLanguageTag)));
        }
        private DataTemplate CreateTemplate(Type viewModelType, Type viewType)
        {
            const string xamlTemplate = "<DataTemplate DataType=\"{{x:Type vm:{0}}}\"><v:{1} /></DataTemplate>";
            var xaml = string.Format(xamlTemplate, viewModelType.Name, viewType.Name);

            var context = new ParserContext
            {
                XamlTypeMapper = new XamlTypeMapper([])
            };

            context.XamlTypeMapper.AddMappingProcessingInstruction("vm", viewModelType.Namespace, viewModelType.Assembly.FullName);
            context.XamlTypeMapper.AddMappingProcessingInstruction("v", viewType.Namespace, viewType.Assembly.FullName);

            context.XmlnsDictionary.Add("", "http://schemas.microsoft.com/winfx/2006/xaml/presentation");
            context.XmlnsDictionary.Add("x", "http://schemas.microsoft.com/winfx/2006/xaml");
            context.XmlnsDictionary.Add("vm", "vm");
            context.XmlnsDictionary.Add("v", "v");

            var template = (DataTemplate)XamlReader.Parse(xaml, context);

            _registeredTemplates.Add(viewType, viewModelType);

            return template;
        }
    }
}