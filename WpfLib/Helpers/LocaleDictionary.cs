﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows.Data;
using Office = Microsoft.Office.Core;
using System.Globalization;
using WpfLib.Extensions;
using System.Windows;

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
        static string _locale = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
        static string _localDirectory;

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
                    LoadDictionary(Locale);
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
            _localDirectory ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Locale");

        public static List<string> LocaleList => (Directory.Exists(LocaleDirectory)) ?
           Directory.GetFiles(LocaleDirectory, "*.json").Select(Path.GetFileNameWithoutExtension).ToList() : new List<string>();

        public static bool? IsRtl => Locale?.StartsWith("he");

        public static FlowDirection FlowDirection => Locale != null && Locale.StartsWith("he") ?
            FlowDirection.RightToLeft : FlowDirection.LeftToRight;

        public static void UseOfficeLocale(Microsoft.Office.Interop.Word.Application application, string basDirectory)
        {
            try
            {
                _localDirectory = Path.Combine(Path.GetDirectoryName(basDirectory), "Locale");
                int lcid = application.LanguageSettings.get_LanguageID(Office.MsoAppLanguageID.msoLanguageIDUI);
                Locale = (new CultureInfo(lcid).TwoLetterISOLanguageName);
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
            OnStaticPropertyChanged(nameof(FlowDirection));
        }

        static void LoadDictionary(string localePrefix)
        {
            Translations = new Dictionary<string, string>();

            try
            {
                var files = Directory.GetFiles(LocaleDirectory, localePrefix + "*.json");

                foreach (var filePath in files)
                {
                    string json = File.ReadAllText(filePath);
                    var partialTranslations = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
                    if (partialTranslations != null)
                        foreach (var kvp in partialTranslations)
                            Translations[kvp.Key] = kvp.Value;
                }
            }
            catch
            {
                // Optionally log or handle errors
            }
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
