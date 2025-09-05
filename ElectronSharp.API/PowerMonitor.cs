using System;
using System.Runtime.Versioning;
using System.Threading.Tasks;

namespace ElectronSharp.API
{
    /// <summary>
    /// Monitor power state changes..
    /// </summary>
    public sealed class PowerMonitor
    {
        /// <summary>
        /// Emitted when the system is about to lock the screen. 
        /// </summary>
        [SupportedOSPlatform("windows")]
        [SupportedOSPlatform("macos")]
        public event Action OnLockScreen
        {
            add => ElectronEventManager.AddEvent("pm-lock-screen", string.Empty, _lockScreen, value);
            remove => ElectronEventManager.RemoveEvent("pm-lock-screen", string.Empty, _lockScreen, value);
        }

        private event Action _lockScreen;

        /// <summary>
        /// Emitted when the system is about to unlock the screen. 
        /// </summary>
        [SupportedOSPlatform("windows")]
        [SupportedOSPlatform("macos")]
        public event Action OnUnLockScreen
        {
            add => ElectronEventManager.AddEvent("pm-unlock-screen", string.Empty, _unlockScreen, value);
            remove => ElectronEventManager.RemoveEvent("pm-unlock-screen", string.Empty, _unlockScreen, value);
        }

        private event Action _unlockScreen;

        /// <summary>
        /// Emitted when the system is suspending.
        /// </summary>
        [SupportedOSPlatform("windows")]
        [SupportedOSPlatform("macos")]
        public event Action OnSuspend
        {
            add => ElectronEventManager.AddEvent("pm-suspend", string.Empty, _suspend, value);
            remove => ElectronEventManager.RemoveEvent("pm-suspend", string.Empty, _suspend, value);
        }

        private event Action _suspend;

        /// <summary>
        /// Emitted when system is resuming.
        /// </summary>
        [SupportedOSPlatform("windows")]
        [SupportedOSPlatform("macos")]
        public event Action OnResume
        {
            add => ElectronEventManager.AddEvent("pm-resume", string.Empty, _resume, value);
            remove => ElectronEventManager.RemoveEvent("pm-resume", string.Empty, _resume, value);
        }

        private event Action _resume;

        /// <summary>
        /// Emitted when the system changes to AC power.
        /// </summary>
        [SupportedOSPlatform("windows")]
        [SupportedOSPlatform("macos")]
        public event Action OnAC
        {
            add => ElectronEventManager.AddEvent("pm-on-ac", string.Empty, _onAC, value);
            remove => ElectronEventManager.RemoveEvent("pm-on-ac", string.Empty, _onAC, value);
        }

        private event Action _onAC;

        /// <summary>
        /// Emitted when system changes to battery power.
        /// </summary>
        [SupportedOSPlatform("windows")]
        [SupportedOSPlatform("macos")]
        public event Action OnBattery
        {
            add => ElectronEventManager.AddEvent("pm-on-battery", string.Empty, _onBattery, value);
            remove => ElectronEventManager.RemoveEvent("pm-on-battery", string.Empty, _onBattery, value);
        }

        private event Action _onBattery;


        /// <summary>
        /// Emitted when the system is about to reboot or shut down. If the event handler
        /// invokes `e.preventDefault()`, Electron will attempt to delay system shutdown in
        /// order for the app to exit cleanly.If `e.preventDefault()` is called, the app
        /// should exit as soon as possible by calling something like `app.quit()`.
        /// </summary>
        [SupportedOSPlatform("linux")]
        [SupportedOSPlatform("macos")]
        public event Action OnShutdown
        {
            add => ElectronEventManager.AddEvent("pm-shutdown", string.Empty, _shutdown, value);
            remove => ElectronEventManager.RemoveEvent("pm-shutdown", string.Empty, _shutdown, value);
        }

        private event Action _shutdown;

        private static          PowerMonitor _powerMonitor;
        private static readonly object       _syncRoot = new();

        internal PowerMonitor() { }

        internal static PowerMonitor Instance
        {
            get
            {
                if (_powerMonitor == null)
                {
                    lock (_syncRoot)
                    {
                        if (_powerMonitor == null)
                        {
                            _powerMonitor = new PowerMonitor();
                        }
                    }
                }

                return _powerMonitor;
            }
        }
    }
}