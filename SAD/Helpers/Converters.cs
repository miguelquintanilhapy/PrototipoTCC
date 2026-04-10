using System;
using System.Globalization;
using System.Windows.Data;

namespace SAD.Helpers
{
    /// <summary>
    /// Converter para exibir valores decimais como moeda BRL no DataGrid e labels.
    /// Uso no XAML: Text="{Binding Total, Converter={StaticResource CurrencyConverter}}"
    /// </summary>
    public class CurrencyConverter : IValueConverter
    {
        private static readonly CultureInfo _ptBR = new CultureInfo("pt-BR");

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is decimal d) return d.ToString("C2", _ptBR);
            if (value is double dbl) return dbl.ToString("C2", _ptBR);
            return value?.ToString() ?? string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var str = value?.ToString()?.Replace("R$", "").Trim();
            if (decimal.TryParse(str, NumberStyles.Currency, _ptBR, out var result))
                return result;
            return 0m;
        }
    }

    /// <summary>
    /// Converter para formatar percentual (0.20 → "20,00%")
    /// </summary>
    public class PercentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double d) return $"{d:0.##}%";
            return value?.ToString() ?? string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => value;
    }
}
