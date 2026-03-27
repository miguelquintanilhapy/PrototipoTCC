using System;
using System.Globalization;
using System.Windows.Data;

namespace SAD.Helpers
{
    public class CurrencyConverter : IValueConverter
    {
        private static readonly CultureInfo _ptBR = new("pt-BR");

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
}