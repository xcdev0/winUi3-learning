using System;
using Windows.Storage;

namespace App1.Utilities
{
    internal class SettingsManager
    {

        private static readonly ISettingsService _service = new LocalSettingsService();

        public static ApplicationSettings Settings { get; } = new ApplicationSettings(_service);

    }


    public class ApplicationSettings
    {
        private readonly ISettingsService _storage;

        public ApplicationSettings(ISettingsService storage)
        {
            _storage = storage;
        }

        // إعدادات المستخدم
        public string? Username
        {
            get => _storage.Get<string>("User.Username");
            set => _storage.Set("User.Username", value);
        }

        public string? Email
        {
            get => _storage.Get<string>("User.Email", isEncrypted: true);
            set => _storage.Set("User.Email", value, isEncrypted: true);
        }

        // إعدادات الواجهة
        public bool IsDarkMode
        {
            get => _storage.Get<bool>("Ui.IsDarkMode", false);
            set => _storage.Set("Ui.IsDarkMode", value);
        }

        public string Language
        {
            get => _storage.Get<string>("Ui.Language", "en-US") ?? "en-US";
            set => _storage.Set("Ui.Language", value);
        }

        public double WindowWidth
        {
            get => _storage.Get<double>("Window.Width", 1200.0);
            set => _storage.Set("Window.Width", value);
        }

        public double WindowHeight
        {
            get => _storage.Get<double>("Window.Height", 800.0);
            set => _storage.Set("Window.Height", value);
        }

        // إعدادات التطبيق
        public DateTime LastLoginDate
        {
            get => _storage.Get<DateTime>("App.LastLogin", DateTime.Now);
            set => _storage.Set("App.LastLogin", value);
        }

        public bool IsFirstRun
        {
            get => _storage.Get<bool>("App.IsFirstRun", true);
            set => _storage.Set("App.IsFirstRun", value);
        }

    }

    public interface ISettingsService
    {
        void Set<T>(string key, T value, bool isEncrypted = false);
        T? Get<T>(string key, T? defaultValue = default, bool isEncrypted = false);
        bool Remove(string key);
        bool Clear();
    }
    public class LocalSettingsService : ISettingsService
    {
        private readonly ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;

        public void Set<T>(string key, T value, bool isEncrypt = false)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Key cannot be null or empty", nameof(key));

            try
            {
                object? storedValue = isEncrypt ? Crypto.Encrypt(value?.ToString() ?? "") : value;

                if (localSettings.Values.ContainsKey(key)) localSettings.Values[key] = storedValue;

                else localSettings.Values.Add(key, storedValue);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error setting value for key '{key}': {ex.Message}");
                throw;
            }
        }

        public T? Get<T>(string key, T? defaultValue = default, bool isEncrypt = false)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Key cannot be null or empty", nameof(key));

            try
            {
                if (localSettings.Values.TryGetValue(key, out object? storedValue))
                {
                    object? value = isEncrypt ? Crypto.Decrypt(storedValue?.ToString() ?? "") : storedValue;
                    return (T)value;
                }
                else return defaultValue;
            }

            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting value for key '{key}': {ex.Message}");
                return defaultValue;
            }
        }

        public bool Remove(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return false;

            try
            {
                if (localSettings.Values.ContainsKey(key))
                {
                    return localSettings.Values.Remove(key);
                }
                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error removing key '{key}': {ex.Message}");
                return false;
            }
        }

        public bool Clear()
        {
            try
            {
                localSettings.Values.Clear();
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error clearing settings: {ex.Message}");
                return false;
            }

        }


    }
}

// usage example 
/*
 <Page
    x:Class="App1.GUI.HomePage"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:local="using:App1.GUI"
    xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
    xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
    mc:Ignorable="d">

    <StackPanel Spacing="10" Padding="20">
        
        <TextBlock x:Name="CurrentUsernameTextBlock"
                   FontSize="18"
                   Text="Current username will appear here" />

        <TextBox x:Name="UsernameInput"
                 PlaceholderText="Enter new username" />

        <Button Content="Save Username"
                Click="SaveUsername_Click" />

    </StackPanel>
</Page>




using App1.Utilities;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace App1.GUI
{

    public sealed partial class HomePage : Page
    {
        public HomePage()
        {


            InitializeComponent();
            CurrentUsernameTextBlock.Text = $"Current Username: {SettingsManager.Settings.Username ?? "(not set)"}";

        }

        private void SaveUsername_Click(object sender, RoutedEventArgs e)
        {
            var newUsername = UsernameInput.Text;

            if (!string.IsNullOrWhiteSpace(newUsername))
            {
                // حفظ الاسم في الإعدادات
                SettingsManager.Settings.Username = newUsername;

                // تحديث العرض
                CurrentUsernameTextBlock.Text = $"Current Username: {newUsername}";
            }
            else
            {
                CurrentUsernameTextBlock.Text = "⚠ Please enter a username.";
            }
        }
    }
}


 */