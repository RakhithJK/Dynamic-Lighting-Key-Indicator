using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml;
using System;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Controls;

namespace Dynamic_Lighting_Key_Indicator
{
    using VK = KeyStatesHandler.ToggleAbleKeys;

    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly DispatcherQueue _dispatcherQueue;

        public MainViewModel()
        {
            _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
        }

        private string _deviceStatusMessage;
        public string DeviceStatusMessage
        {
            get => _deviceStatusMessage;
            set => SetProperty(ref _deviceStatusMessage, value);
        }

        private string _attachedDevicesMessage;
        public string AttachedDevicesMessage
        {
            get => _attachedDevicesMessage;
            set => SetProperty(ref _attachedDevicesMessage, value);
        }

        private bool _hasAttachedDevices;
        public bool HasNoAttachedDevices => !HasAttachedDevices;
        public bool HasAttachedDevices
        {
            get => _hasAttachedDevices;
            set
            {
                if (SetProperty(ref _hasAttachedDevices, value))
                {
                    // Notify that HasNoAttachedDevices has also changed
                    OnPropertyChanged(nameof(HasNoAttachedDevices));
                }
            }
        }

        private string _deviceWatcherStatusMessage;
        public string DeviceWatcherStatusMessage
        {
            get => _deviceWatcherStatusMessage;
            set => SetProperty(ref _deviceWatcherStatusMessage, value);
        }

        private bool _isWatcherRunning;
        public bool IsWatcherStopped => !IsWatcherRunning;
        public bool IsWatcherRunning
        {
            get => _isWatcherRunning;
            set
            {
                if (SetProperty(ref _isWatcherRunning, value))
                {
                    // Notify that IsWatcherStopped has also changed
                    OnPropertyChanged(nameof(IsWatcherStopped));
                    OnPropertyChanged(nameof(WatcherRunningVisibilityBool));
                }
            }
        }

        public Visibility WatcherRunningVisibilityBool
        {
            get
            {
                if (IsWatcherRunning)
                    return Visibility.Visible;
                else
                    return Visibility.Collapsed;
            }
        }

        private int _selectedDeviceIndex;
        public int SelectedDeviceIndex
        {
            get => _selectedDeviceIndex;
            set
            {
                _selectedDeviceIndex = value;
                OnPropertyChanged();
            }
        }

        // Custom object for storing colors for scroll lock, caps lock, and num lock. Contains Hex strings for on and off status of each
        private ColorSettings _colorSettings;
        public ColorSettings ColorSettings
        {
            get => _colorSettings;
            set => SetProperty(ref _colorSettings, value);
        }

        // ----------------------- Event Handlers -----------------------

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (_dispatcherQueue.HasThreadAccess)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            else
            {
                _ = _dispatcherQueue.TryEnqueue(() =>
                {
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
                });
            }
        }

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        // ------------------------------------- Text Box Text -------------------------------------
        public string TextScrollLockOnColor
        {
            get => ColorSettings.AsString(ColorSettings.ScrollLockOnColor);
            set => ColorSettings.ScrollLockOnColor = ColorSettings.GetColorFromString(value);
        }

        public string TextScrollLockOffColor
        {
            get => ColorSettings.AsString(ColorSettings.ScrollLockOffColor);
            set => ColorSettings.ScrollLockOffColor = ColorSettings.GetColorFromString(value);
        }

        public string TextCapsLockOnColor
        {
            get => ColorSettings.AsString(ColorSettings.CapsLockOnColor);
            set => ColorSettings.CapsLockOnColor = ColorSettings.GetColorFromString(value);
        }

        public string TextCapsLockOffColor
        {
            get => ColorSettings.AsString(ColorSettings.CapsLockOffColor);
            set => ColorSettings.CapsLockOffColor = ColorSettings.GetColorFromString(value);
        }

        public string TextNumLockOnColor
        {
            get => ColorSettings.AsString(ColorSettings.NumLockOnColor);
            set => ColorSettings.NumLockOnColor = ColorSettings.GetColorFromString(value);
        }

        public string TextNumLockOffColor
        {
            get => ColorSettings.AsString(ColorSettings.NumLockOffColor);
            set => ColorSettings.NumLockOffColor = ColorSettings.GetColorFromString(value);
        }

        public string TextDefaultColor
        {
            get => ColorSettings.AsString(ColorSettings.DefaultColor);
            set => ColorSettings.DefaultColor = ColorSettings.GetColorFromString(value);
        }

        public bool SyncScrollLockOnColor
        {
            get => ColorSettings.SyncScrollLockOnColor;
            set
            {
                if (ColorSettings.SyncScrollLockOnColor != value)
                {
                    ColorSettings.SyncScrollLockOnColor = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(ScrollLockOnGlyph));
                }
            }
        }

        public bool SyncScrollLockOffColor
        {
            get => ColorSettings.SyncScrollLockOffColor;
            set
            {
                if (ColorSettings.SyncScrollLockOffColor != value)
                {
                    ColorSettings.SyncScrollLockOffColor = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(ScrollLockOffGlyph));
                }
            }
        }

        public bool SyncCapsLockOnColor
        {
            get => ColorSettings.SyncCapsLockOnColor;
            set
            {
                if (ColorSettings.SyncCapsLockOnColor != value)
                {
                    ColorSettings.SyncCapsLockOnColor = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(CapsLockOnGlyph));
                }
            }
        }

        public bool SyncCapsLockOffColor
        {
            get => ColorSettings.SyncCapsLockOffColor;
            set
            {
                if (ColorSettings.SyncCapsLockOffColor != value)
                {
                    ColorSettings.SyncCapsLockOffColor = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(CapsLockOffGlyph));
                }
            }
        }

        public bool SyncNumLockOnColor
        {
            get => ColorSettings.SyncNumLockOnColor;
            set
            {
                if (ColorSettings.SyncNumLockOnColor != value)
                {
                    ColorSettings.SyncNumLockOnColor = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(NumLockOnGlyph));
                }
            }
        }

        public bool SyncNumLockOffColor
        {
            get => ColorSettings.SyncNumLockOffColor;
            set
            {
                if (ColorSettings.SyncNumLockOffColor != value)
                {
                    ColorSettings.SyncNumLockOffColor = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(NumLockOffGlyph));
                }
            }
        }

        private string _scrollLockOnGlyph;
        public string ScrollLockOnGlyph
        {
            get => GetSyncGlyph_ByPropertyName("ScrollLockOnColor");
            private set => SetProperty(ref _scrollLockOnGlyph, value);
        }

        private string _scrollLockOffGlyph;
        public string ScrollLockOffGlyph
        {
            get => GetSyncGlyph_ByPropertyName("ScrollLockOffColor");
            private set => SetProperty(ref _scrollLockOffGlyph, value);
        }

        private string _capsLockOnGlyph;
        public string CapsLockOnGlyph
        {
            get => GetSyncGlyph_ByPropertyName("CapsLockOnColor");
            private set => SetProperty(ref _capsLockOnGlyph, value);
        }

        private string _capsLockOffGlyph;
        public string CapsLockOffGlyph
        {
            get => GetSyncGlyph_ByPropertyName("CapsLockOffColor");
            private set => SetProperty(ref _capsLockOffGlyph, value);
        }

        private string _numLockOnGlyph;
        public string NumLockOnGlyph
        {
            get => GetSyncGlyph_ByPropertyName("NumLockOnColor");
            private set => SetProperty(ref _numLockOnGlyph, value);
        }

        private string _numLockOffGlyph;
        public string NumLockOffGlyph
        {
            get => GetSyncGlyph_ByPropertyName("NumLockOffColor");
            private set => SetProperty(ref _numLockOffGlyph, value);
        }


        // Color properties
        public Windows.UI.Color ScrollLockOnColor
        {
            get => ColorSettings.ScrollLockOnColor;
            set
            {
                if (ColorSettings.ScrollLockOnColor != value)
                {
                    ColorSettings.ScrollLockOnColor = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(ScrollLockOnColorHex));
                }
            }
        }

        public string ScrollLockOnColorHex => ColorSettings.AsString(ColorSettings.ScrollLockOnColor);

        public Windows.UI.Color ScrollLockOffColor
        {
            get => ColorSettings.ScrollLockOffColor;
            set
            {
                if (ColorSettings.ScrollLockOffColor != value)
                {
                    ColorSettings.ScrollLockOffColor = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(ScrollLockOffColorHex));
                }
            }
        }

        public string ScrollLockOffColorHex => ColorSettings.AsString(ColorSettings.ScrollLockOffColor);

        public Windows.UI.Color CapsLockOnColor
        {
            get => ColorSettings.CapsLockOnColor;
            set
            {
                if (ColorSettings.CapsLockOnColor != value)
                {
                    ColorSettings.CapsLockOnColor = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(CapsLockOnColorHex));
                }
            }
        }

        public string CapsLockOnColorHex => ColorSettings.AsString(ColorSettings.CapsLockOnColor);

        public Windows.UI.Color CapsLockOffColor
        {
            get => ColorSettings.CapsLockOffColor;
            set
            {
                if (ColorSettings.CapsLockOffColor != value)
                {
                    ColorSettings.CapsLockOffColor = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(CapsLockOffColorHex));
                }
            }
        }

        public string CapsLockOffColorHex => ColorSettings.AsString(ColorSettings.CapsLockOffColor);

        public Windows.UI.Color NumLockOnColor
        {
            get => ColorSettings.NumLockOnColor;
            set
            {
                if (ColorSettings.NumLockOnColor != value)
                {
                    ColorSettings.NumLockOnColor = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(NumLockOnColorHex));
                }
            }
        }

        public string NumLockOnColorHex => ColorSettings.AsString(ColorSettings.NumLockOnColor);

        public Windows.UI.Color NumLockOffColor
        {
            get => ColorSettings.NumLockOffColor;
            set
            {
                if (ColorSettings.NumLockOffColor != value)
                {
                    ColorSettings.NumLockOffColor = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(NumLockOffColorHex));
                }
            }
        }

        public string NumLockOffColorHex => ColorSettings.AsString(ColorSettings.NumLockOffColor);

        public Windows.UI.Color DefaultColor
        {
            get => ColorSettings.DefaultColor;
            set
            {
                if (ColorSettings.DefaultColor != value)
                {
                    ColorSettings.DefaultColor = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(DefaultColorHex));
                }
            }
        }

        public string DefaultColorHex => ColorSettings.AsString(ColorSettings.DefaultColor);

        // ------ Other methods ------
        public void UpdateSyncSetting(bool syncSetting, string colorPropertyName)
        {
            // Update the relevant sync setting in ViewModel based on the colorPropertyName
            switch (colorPropertyName)
            {
                case "NumLockOnColor":
                    SyncNumLockOnColor = syncSetting;
                    OnPropertyChanged(nameof(NumLockOnGlyph));
                    break;
                case "NumLockOffColor":
                    SyncNumLockOffColor = syncSetting;
                    OnPropertyChanged(nameof(NumLockOffGlyph));
                    break;
                case "CapsLockOnColor":
                    SyncCapsLockOnColor = syncSetting;
                    OnPropertyChanged(nameof(CapsLockOnGlyph));
                    break;
                case "CapsLockOffColor":
                    SyncCapsLockOffColor = syncSetting;
                    OnPropertyChanged(nameof(CapsLockOffGlyph));
                    break;
                case "ScrollLockOnColor":
                    SyncScrollLockOnColor = syncSetting;
                    OnPropertyChanged(nameof(ScrollLockOnGlyph));
                    break;
                case "ScrollLockOffColor":
                    SyncScrollLockOffColor = syncSetting;
                    OnPropertyChanged(nameof(ScrollLockOffGlyph));
                    break;
                default:
                    break;
            }
        }

        public bool GetSyncSettingByPropertyName(string colorPropertyName)
        {
            switch (colorPropertyName)
            {
                case "NumLockOnColor":
                    return SyncNumLockOnColor;
                case "NumLockOffColor":
                    return SyncNumLockOffColor;
                case "CapsLockOnColor":
                    return SyncCapsLockOnColor;
                case "CapsLockOffColor":
                    return SyncCapsLockOffColor;
                case "ScrollLockOnColor":
                    return SyncScrollLockOnColor;
                case "ScrollLockOffColor":
                    return SyncScrollLockOffColor;
                default:
                    return false;
            }
        }

        public bool GetSyncSetting_ByButtonObject(Button button)
        {
            return GetSyncSettingByPropertyName(button.Name);
        }

        //--------------------------------------------------------------

        public const string LinkedGlyph = "\uE71B";
        public const string UnlinkedGlyph = ""; // TESTING

        public string GetSyncGlyph_ByPropertyName(string colorPropertyName)
        {
            var glyph = GetSyncSettingByPropertyName(colorPropertyName) ? LinkedGlyph : UnlinkedGlyph;
            System.Diagnostics.Debug.WriteLine($"GetSyncGlyph_ByPropertyName({colorPropertyName}): {glyph}");
            return glyph;
        }

        public string GetSyncGlyph_ByButtonObject(Button button)
        {
            return GetSyncSetting_ByButtonObject(button) ? LinkedGlyph : UnlinkedGlyph;
        }

        


    } // ----------------------- End of MainViewModel -----------------------

    public class ColorSettings
    {
        // Properties as strings to store hex values for colors
        public Windows.UI.Color ScrollLockOnColor { get; set; }
        public Windows.UI.Color ScrollLockOffColor { get; set; }
        public Windows.UI.Color CapsLockOnColor { get; set; }
        public Windows.UI.Color CapsLockOffColor { get; set; }
        public Windows.UI.Color NumLockOnColor { get; set; }
        public Windows.UI.Color NumLockOffColor { get; set; }
        public Windows.UI.Color DefaultColor { get; set; }
        public int Brightness { get; set; }

        // Settings to sync keys to default
        public bool SyncScrollLockOnColor { get; set; }
        public bool SyncScrollLockOffColor { get; set; }
        public bool SyncCapsLockOnColor { get; set; }
        public bool SyncCapsLockOffColor { get; set; }
        public bool SyncNumLockOnColor { get; set; }
        public bool SyncNumLockOffColor { get; set; }

        public Windows.UI.Color GetColorFromString(string color)
        {
            if (string.IsNullOrEmpty(color))
            {
                // If the color is null or empty, return the default color
                return DefaultColor;
            }

            if (color.StartsWith("#"))
            {
                color = color.Substring(1);
            }
            byte r = Convert.ToByte(color.Substring(0, 2), 16);
            byte g = Convert.ToByte(color.Substring(2, 2), 16);
            byte b = Convert.ToByte(color.Substring(4, 2), 16);
            return Windows.UI.Color.FromArgb(255, r, g, b);
        }

        public string AsString(Windows.UI.Color color)
        {
            return "#" + color.R.ToString("X2") + color.G.ToString("X2") + color.B.ToString("X2");
        }

        // Set all the colors from the text boxes in the GUI
        public void SetAllColorsFromGUI(MainViewModel viewModel)
        {
            ScrollLockOnColor = GetColorFromString(viewModel.TextScrollLockOnColor);
            ScrollLockOffColor = GetColorFromString(viewModel.TextScrollLockOffColor);
            CapsLockOnColor = GetColorFromString(viewModel.TextCapsLockOnColor);
            CapsLockOffColor = GetColorFromString(viewModel.TextCapsLockOffColor);
            NumLockOnColor = GetColorFromString(viewModel.TextNumLockOnColor);
            NumLockOffColor = GetColorFromString(viewModel.TextNumLockOffColor);
            DefaultColor = GetColorFromString(viewModel.TextDefaultColor);
            Brightness = viewModel.ColorSettings.Brightness;

            SyncScrollLockOnColor = viewModel.SyncScrollLockOnColor;
            SyncScrollLockOffColor = viewModel.SyncScrollLockOffColor;
            SyncCapsLockOnColor = viewModel.SyncCapsLockOnColor;
            SyncCapsLockOffColor = viewModel.SyncCapsLockOffColor;
            SyncNumLockOnColor = viewModel.SyncNumLockOnColor;
            SyncNumLockOffColor = viewModel.SyncNumLockOffColor;
        }

        internal void SetAllColorsFromUserConfig(UserConfig userConfig)
        {
            if (userConfig == null || userConfig.MonitoredKeysAndColors == null)
            {
                throw new ArgumentNullException("UserConfig cannot be null.");
            }

            Brightness = userConfig.Brightness;
            DefaultColor = Windows.UI.Color.FromArgb(255, (byte)userConfig.StandardKeyColor.R, (byte)userConfig.StandardKeyColor.G, (byte)userConfig.StandardKeyColor.B);

            foreach (KeyStatesHandler.MonitoredKey monitoredKey in userConfig.MonitoredKeysAndColors)
            {
                Windows.UI.Color onColor;
                Windows.UI.Color offColor;
                bool onColorTiedToStandard = monitoredKey.onColorTiedToStandard;
                bool offColorTiedToStandard = monitoredKey.offColorTiedToStandard;

                // Sync colors to default if applicable
                if (onColorTiedToStandard)
                    onColor = DefaultColor;

                if (offColorTiedToStandard)
                    offColor = DefaultColor;

                // On color
                if (monitoredKey.onColor.Equals(default((int, int, int))))
                    onColor = Windows.UI.Color.FromArgb(255, DefaultColor.R, DefaultColor.G, DefaultColor.B);
                else
                    onColor = Windows.UI.Color.FromArgb(255, (byte)monitoredKey.onColor.R, (byte)monitoredKey.onColor.G, (byte)monitoredKey.onColor.B);

                // Off color
                if (monitoredKey.offColor.Equals(default((int, int, int))))
                    offColor = Windows.UI.Color.FromArgb(255, DefaultColor.R, DefaultColor.G, DefaultColor.B);
                else
                    offColor = Windows.UI.Color.FromArgb(255, (byte)monitoredKey.offColor.R, (byte)monitoredKey.offColor.G, (byte)monitoredKey.offColor.B);

                // Actually apply the colors to the correct properties in the view model after processing
                switch (monitoredKey.key)
                {
                    case VK.NumLock:
                        NumLockOnColor = onColor;
                        NumLockOffColor = offColor;
                        SyncNumLockOnColor = onColorTiedToStandard;
                        SyncNumLockOffColor = offColorTiedToStandard;
                        break;
                    case VK.CapsLock:
                        CapsLockOnColor = onColor;
                        CapsLockOffColor = offColor;
                        SyncCapsLockOnColor = onColorTiedToStandard;
                        SyncCapsLockOffColor = offColorTiedToStandard;
                        break;
                    case VK.ScrollLock:
                        ScrollLockOnColor = onColor;
                        ScrollLockOffColor = offColor;
                        SyncScrollLockOnColor = onColorTiedToStandard;
                        SyncScrollLockOffColor = offColorTiedToStandard;
                        break;
                }
            }
        }
    }

}