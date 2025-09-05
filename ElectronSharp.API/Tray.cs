using ElectronSharp.API.Entities;
using ElectronSharp.API.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Runtime.Versioning;
using System.Threading.Tasks;

namespace ElectronSharp.API
{
    /// <summary>
    /// Add icons and context menus to the system's notification area.
    /// </summary>
    [SupportedOSPlatform("macos")]
    [SupportedOSPlatform("windows")]
    public sealed class Tray
    {
        /// <summary>
        /// Emitted when the tray icon is clicked.
        /// </summary>
        public event Action<TrayClickEventArgs, Rectangle> OnClick
        {
            add => ElectronEventManager.AddTrayEvent("tray-click-event", GetHashCode(), _click, value);
            remove => ElectronEventManager.RemoveTrayEvent("tray-click-event", GetHashCode(), _click, value);
        }

        private event Action<TrayClickEventArgs, Rectangle> _click;

        /// <summary>
        /// macOS, Windows: Emitted when the tray icon is right clicked.
        /// </summary>
        [SupportedOSPlatform("windows")]
        [SupportedOSPlatform("macos")]
        public event Action<TrayClickEventArgs, Rectangle> OnRightClick
        {
            add => ElectronEventManager.AddTrayEvent("tray-right-click-event", GetHashCode(), _rightClick, value);
            remove => ElectronEventManager.RemoveTrayEvent("tray-right-click-event", GetHashCode(), _rightClick, value);
        }

        private event Action<TrayClickEventArgs, Rectangle> _rightClick;

        /// <summary>
        /// macOS, Windows: Emitted when the tray icon is double clicked.
        /// </summary>
        [SupportedOSPlatform("windows")]
        [SupportedOSPlatform("macos")]
        public event Action<TrayClickEventArgs, Rectangle> OnDoubleClick
        {
            add => ElectronEventManager.AddTrayEvent("tray-double-click-event", GetHashCode(), _doubleClick, value);
            remove => ElectronEventManager.RemoveTrayEvent("tray-double-click-event", GetHashCode(), _doubleClick, value);
        }

        private event Action<TrayClickEventArgs, Rectangle> _doubleClick;

        /// <summary>
        /// Windows: Emitted when the tray balloon shows.
        /// </summary>
        [SupportedOSPlatform("windows")]
        public event Action OnBalloonShow
        {
            add => ElectronEventManager.AddEventNoSuffix("tray-balloon-show-event", GetHashCode(), _balloonShow, value);
            remove => ElectronEventManager.RemoveEvent("tray-balloon-show-event", GetHashCode(), _balloonShow, value);
        }

        private event Action _balloonShow;

        /// <summary>
        /// Windows: Emitted when the tray balloon is clicked.
        /// </summary>
        [SupportedOSPlatform("windows")]
        public event Action OnBalloonClick
        {
            add => ElectronEventManager.AddEventNoSuffix("tray-balloon-click-event", GetHashCode(), _balloonClick, value);
            remove => ElectronEventManager.RemoveEvent("tray-balloon-click-event", GetHashCode(), _balloonClick, value);
        }

        private event Action _balloonClick;

        /// <summary>
        /// Windows: Emitted when the tray balloon is closed 
        /// because of timeout or user manually closes it.
        /// </summary>
        [SupportedOSPlatform("windows")]
        public event Action OnBalloonClosed
        {
            add => ElectronEventManager.AddEventNoSuffix("tray-balloon-closed-event", GetHashCode(), _balloonClosed, value);
            remove => ElectronEventManager.RemoveEvent("tray-balloon-closed-event", GetHashCode(), _balloonClosed, value);
        }

        private event Action _balloonClosed;

        // TODO: Implement macOS Events

        private static          Tray   _tray;
        private static readonly object _syncRoot = new();

        internal Tray() { }

        internal static Tray Instance
        {
            get
            {
                if (_tray == null)
                {
                    lock (_syncRoot)
                    {
                        if (_tray == null)
                        {
                            _tray = new Tray();
                        }
                    }
                }

                return _tray;
            }
        }

        /// <summary>
        /// Gets the menu items.
        /// </summary>
        /// <value>
        /// The menu items.
        /// </value>
        public IReadOnlyCollection<MenuItem> MenuItems { get { return _items.AsReadOnly(); } }
        private readonly List<MenuItem> _items = new();

        /// <summary>
        /// Shows the Traybar.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="menuItem">The menu item.</param>
        public void Show(string image, MenuItem menuItem)
        {
            Show(image, new MenuItem[] { menuItem });
        }

        /// <summary>
        /// Shows the Traybar.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="menuItems">The menu items.</param>
        public void Show(string image, MenuItem[] menuItems)
        {
            menuItems.AddMenuItemsId();
            BridgeConnector.Emit("create-tray", image, JArray.FromObject(menuItems, _jsonSerializer));
            _items.Clear();
            _items.AddRange(menuItems);

            BridgeConnector.Off("trayMenuItemClicked");

            BridgeConnector.On<string>("trayMenuItemClicked", (id) =>
            {
                MenuItem menuItem = _items.GetMenuItem(id.ToString());
                menuItem?.Click();
            });
        }

        /// <summary>
        /// Shows the Traybar (empty).
        /// </summary>
        /// <param name="image">The image.</param>
        public void Show(string image)
        {
            BridgeConnector.Emit("create-tray", image);
        }

        /// <summary>
        /// Destroys the tray icon immediately.
        /// </summary>
        public void Destroy()
        {
            BridgeConnector.Emit("tray-destroy");
            _items.Clear();
        }

        /// <summary>
        /// Sets the image associated with this tray icon.
        /// </summary>
        /// <param name="image"></param>
        public void SetImage(string image)
        {
            BridgeConnector.Emit("tray-setImage", image);
        }

        /// <summary>
        /// Sets the image associated with this tray icon when pressed on macOS.
        /// </summary>
        /// <param name="image"></param>
        [SupportedOSPlatform("macos")]
        public void SetPressedImage(string image)
        {
            BridgeConnector.Emit("tray-setPressedImage", image);
        }

        /// <summary>
        /// Sets the hover text for this tray icon.
        /// </summary>
        /// <param name="toolTip"></param>
        public void SetToolTip(string toolTip)
        {
            BridgeConnector.Emit("tray-setToolTip", toolTip);
        }

        /// <summary>
        /// macOS: Sets the title displayed aside of the tray icon in the status bar.
        /// </summary>
        /// <param name="title"></param>
        [SupportedOSPlatform("macos")]
        public void SetTitle(string title)
        {
            BridgeConnector.Emit("tray-setTitle", title);
        }

        /// <summary>
        /// Windows: Displays a tray balloon.
        /// </summary>
        /// <param name="options"></param>
        [SupportedOSPlatform("windows")]
        public void DisplayBalloon(DisplayBalloonOptions options)
        {
            BridgeConnector.Emit("tray-displayBalloon", options);
        }

        /// <summary>
        /// Whether the tray icon is destroyed.
        /// </summary>
        /// <returns></returns>
        public Task<bool> IsDestroyedAsync() => BridgeConnector.OnResult<bool>("tray-isDestroyed", "tray-isDestroyedCompleted");

        private const string ModuleName = "tray";

        /// <summary>
        /// Subscribe to an unmapped event on the <see cref="Tray"/> module.
        /// </summary>
        /// <param name="eventName">The event name</param>
        /// <param name="fn">The handler</param>
        public void On(string eventName, Action fn) => Events.Instance.On(ModuleName, eventName, fn);

        /// <summary>
        /// Subscribe to an unmapped event on the <see cref="Tray"/> module.
        /// </summary>
        /// <param name="eventName">The event name</param>
        /// <param name="fn">The handler</param>
        public void On(string eventName, Action<object> fn) => Events.Instance.On(ModuleName, eventName, fn);

        /// <summary>
        /// Subscribe to an unmapped event on the <see cref="Tray"/> module once.
        /// </summary>
        /// <param name="eventName">The event name</param>
        /// <param name="fn">The handler</param>
        public void Once(string eventName, Action fn) => Events.Instance.Once(ModuleName, eventName, fn);

        /// <summary>
        /// Subscribe to an unmapped event on the <see cref="Tray"/> module once.
        /// </summary>
        /// <param name="eventName">The event name</param>
        /// <param name="fn">The handler</param>
        public void Once(string eventName, Action<object> fn) => Events.Instance.Once(ModuleName, eventName, fn);

        private readonly JsonSerializer _jsonSerializer = new()
        {
            ContractResolver  = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore
        };
    }
}