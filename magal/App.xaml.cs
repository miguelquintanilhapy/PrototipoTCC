using System.Configuration;
using System.Data;
using System.Windows;
using System.Windows.Markup; 
using System.Globalization;  

namespace magal
{

    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            // Garante que todos os bindings de data, hora e moeda (R$) 
            // utilizem a configuração regional do Windows do usuário.
            FrameworkElement.LanguageProperty.OverrideMetadata(
                typeof(FrameworkElement),
                new FrameworkPropertyMetadata(
                    XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));

            base.OnStartup(e);
        }
    }
}