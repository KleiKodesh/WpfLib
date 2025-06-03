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
        private static SolidColorBrush _Background;
        private static SolidColorBrush _Foreground;

        public static string LightForegroundColorString => "#000000";
        public static string LightBackgroundColorString => "#FFFFFF";
        public static string DarkForegroundColorString => "#FFFFFF";
        public static string DarkBackgroundColorString => "#1E1E1E";

        public static SolidColorBrush Background
        {
            get
            {
                if (_Background == null)
                    DetectSystemTheme();
                return _Background;
            }
            set
            {
                _Background = value;
                OnStaticPropertyChanged(nameof(Background));
            }
        }

        public static SolidColorBrush Foreground
        {
            get
            {
                if (_Foreground == null)
                    DetectSystemTheme();
                return _Foreground;
            }
            set
            {
                _Foreground = value;
                OnStaticPropertyChanged(nameof(Foreground));
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
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(DarkBackgroundColorString));
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(DarkForegroundColorString));
            }
            else
            {
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(LightBackgroundColorString));
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(LightForegroundColorString));
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
                            Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(isLight ? LightBackgroundColorString : DarkBackgroundColorString));
                            Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(isLight ? LightForegroundColorString : DarkForegroundColorString));
                        }
                    }
                }
            }
            catch
            {
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(LightBackgroundColorString));
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(LightForegroundColorString));
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
