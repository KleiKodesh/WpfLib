using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace WpfLib.Helpers
{
    public static class ThemeHelper
    {
        public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;

        private static void OnStaticPropertyChanged(string propertyName)
        {
            StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(propertyName));
        }

        private static SolidColorBrush _backGround = new SolidColorBrush(Colors.White);
        public static SolidColorBrush BackGround
        {
            get => _backGround;
            set
            {
                if (_backGround != value)
                {
                    _backGround = value;
                    OnStaticPropertyChanged(nameof(BackGround));
                }
            }
        }

        private static SolidColorBrush _foreGroundBrush = new SolidColorBrush(Colors.Black);
        public static SolidColorBrush ForeGround
        {
            get => _foreGroundBrush;
            set
            {
                if (_foreGroundBrush != value)
                {
                    _foreGroundBrush = value;
                    OnStaticPropertyChanged(nameof(ForeGround));
                }
            }
        }
    }
}
