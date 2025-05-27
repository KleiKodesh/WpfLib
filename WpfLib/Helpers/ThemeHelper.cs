using Microsoft.Win32;
using System.ComponentModel;
using System.Windows.Media;
using System;

namespace WpfLib.Helpers
{
    public static class ThemeHelper
    {
        public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;
        private static void OnStaticPropertyChanged(string propertyName)
        {
            StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(propertyName));
        }

        private static bool _doNotChangeDocumentColors;
        private static SolidColorBrush _backGround;
        private static SolidColorBrush _foreGround;

        public static string LightForegroundColorString => "#000000";
        public static string LightBackgroundColorString => "#FFFFFF";
        public static string DarkForegroundColorString => "#FFFFFF";
        public static string DarkBackgroundColorString => "#1E1E1E";

        public static SolidColorBrush BackGround
        {
            get
            {
                if (_backGround == null)
                    DetectSystemTheme();
                return _backGround;
            }
            set
            {
                _backGround = value;
                OnStaticPropertyChanged(nameof(BackGround));
            }
        }

        public static SolidColorBrush ForeGround
        {
            get
            {
                if (_foreGround == null)
                    DetectSystemTheme();
                return _foreGround;
            }
            set
            {
                _foreGround = value;
                OnStaticPropertyChanged(nameof(ForeGround));
            }
        }

        public static bool DoNotChangeDocumentColors
        {
            get => _doNotChangeDocumentColors;
            set
            {
                _doNotChangeDocumentColors = value;
                OnStaticPropertyChanged(nameof(DoNotChangeDocumentColors));
            }
        }

        public static void ToggleDarkMode(bool isDarkMode)
        {
            if (isDarkMode)
            {
                BackGround = new SolidColorBrush((Color)ColorConverter.ConvertFromString(DarkBackgroundColorString));
                ForeGround = new SolidColorBrush((Color)ColorConverter.ConvertFromString(DarkForegroundColorString));
            }
            else
            {
                BackGround = new SolidColorBrush((Color)ColorConverter.ConvertFromString(LightBackgroundColorString));
                ForeGround = new SolidColorBrush((Color)ColorConverter.ConvertFromString(LightForegroundColorString));
            }
        }

        public static void DetectSystemTheme()
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize"))
                {
                    if (key != null)
                    {
                        object value = key.GetValue("AppsUseLightTheme");
                        if (value is int reg)
                        {
                            bool isLight = reg != 0;
                            BackGround = new SolidColorBrush((Color)ColorConverter.ConvertFromString(isLight ? LightBackgroundColorString : DarkBackgroundColorString));
                            ForeGround = new SolidColorBrush((Color)ColorConverter.ConvertFromString(isLight ? LightForegroundColorString : DarkForegroundColorString));
                        }
                    }
                }
            }
            catch
            {
                BackGround = new SolidColorBrush((Color)ColorConverter.ConvertFromString(LightBackgroundColorString));
                ForeGround = new SolidColorBrush((Color)ColorConverter.ConvertFromString(LightForegroundColorString));
            }
        }

        public static string ToRgbString(this SolidColorBrush brush)
        {
            Color color = brush.Color;
            return $"rgb({color.R}, {color.G}, {color.B})";
        }

        public static string ToRgbString(this Color color) =>
             $"rgb({color.R}, {color.G}, {color.B})";
    }
}
