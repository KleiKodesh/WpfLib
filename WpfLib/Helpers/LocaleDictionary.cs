using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows.Data;
using Office = Microsoft.Office.Core;
using System.Globalization;
using WpfLib.Extensions;

namespace WpfLib.Helpers
{
    public static class LocaleDictionary
    {
        public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;
        private static void OnStaticPropertyChanged(string propertyName)
        {
            StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(propertyName));
        }

        static Dictionary<string, string> _translations;
        static string _locale;

        public static string Locale
        {
            get => _locale;
            set
            {
                if (value != _locale)
                {
                    _locale = value;
                    ChangeDictionary(value);
                }    
            }
        }
        public static Dictionary<string , string> Translations 
        {
            get
            {
                if (_translations == null)
                    _translations = new Dictionary<string , string>();
                return _translations;
            }
            set
            {
                if (_translations != value)
                    _translations = value;
                OnStaticPropertyChanged(nameof(Translations));
            }
        }

        public static Dictionary<string, Binding> Bindings { get; private set; } = new Dictionary<string, Binding>();

        public static string LocaleDirectory =>
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "Locale");

        public static List<string> LocaleList => (Directory.Exists(LocaleDirectory)) ?
           Directory.GetFiles(LocaleDirectory, "*.json").Select(Path.GetFileNameWithoutExtension).ToList() : new List<string>();

        public static bool? IsRtl => Locale?.StartsWith("he");

        public static void UseOfficeLocale(Microsoft.Office.Interop.Word.Application application)
        {
            try
            {
                int lcid = application.LanguageSettings.get_LanguageID(Office.MsoAppLanguageID.msoLanguageIDUI);
                Locale = new CultureInfo(lcid).TwoLetterISOLanguageName;
            }
            catch { }
        }

        public static void ChangeDictionary(string locale)
        {
            if (string.IsNullOrWhiteSpace(locale)) 
                return;

            LoadDictionary(locale);
            UpdateLocalization();
            OnStaticPropertyChanged(nameof(Locale));
            OnStaticPropertyChanged(nameof(IsRtl));
        }

        static void LoadDictionary(string locale)
        {
            try
            {
                string filePath = Path.Combine(LocaleDirectory, locale + ".json");
                if (File.Exists(filePath))
                {
                    string json = File.ReadAllText(filePath);
                    Translations = JsonSerializer.Deserialize<Dictionary<string, string>>(json) ?? new Dictionary<string, string>();
                }
            }
            catch { }
        }

        public static void UpdateLocalization()
        {
            foreach (var binding in Bindings.Values)
            {
                if (binding.Source is LocaleExtension localeExtension)
                    localeExtension.OnTranslationChanged();
            }
        }

        public static string Translate(string input)
        {
            if (input == null)
                return string.Empty;

            if (Translations.TryGetValue(input.Trim().Replace(" ", "_"), out string translation))
                return translation;

            return input.Replace("_", " ");
        }
    }
}
