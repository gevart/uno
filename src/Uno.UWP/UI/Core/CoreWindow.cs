using System;
using System.Runtime.InteropServices;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Foundation.Metadata;

namespace Windows.UI.Core
{
	public partial class CoreWindow
	{
		[ThreadStatic]
		private static CoreWindow _current;

		public static CoreWindow GetForCurrentThread()
			=> _current; // UWP returns 'null' if on a BG thread

		private Point? _pointerPosition;
		private IPointerEventArgs _lastPointerEventArgs;
		private static Action _invalidateRender;

		internal CoreWindow()
		{
			_current = this;
			Main ??= this;

			InitializePartial();
		}

		internal static CoreWindow Main { get; private set; }

		internal static void SetInvalidateRender(Action invalidateRender) => _invalidateRender = invalidateRender;

		internal static void QueueInvalidateRender() => _invalidateRender?.Invoke();

		partial void InitializePartial();
    
		public event TypedEventHandler<CoreWindow, WindowSizeChangedEventArgs> SizeChanged;

		public CoreDispatcher Dispatcher
			=> CoreDispatcher.Main;

		public Point PointerPosition
		{
			get => _pointerPosition ?? _lastPointerEventArgs?.GetLocation() ?? new Point();
			set => _pointerPosition = value;
		}

#if !__WASM__ && !__MACOS__ && !__SKIA__
		[Uno.NotImplemented("__ANDROID__", "__IOS__", "NET461", "__NETSTD_REFERENCE__")]
		public CoreCursor PointerCursor { get; set; } = new CoreCursor(CoreCursorType.Arrow, 0);
#endif

		[Uno.NotImplemented]
		public CoreVirtualKeyStates GetAsyncKeyState(System.VirtualKey virtualKey)
			=> CoreVirtualKeyStates.None;

		[Uno.NotImplemented]
		public CoreVirtualKeyStates GetKeyState(System.VirtualKey virtualKey)
			=> CoreVirtualKeyStates.None;

		internal void SetLastPointerEvent(IPointerEventArgs args)
			=> _lastPointerEventArgs = args;

		internal interface IPointerEventArgs
		{
			Point GetLocation();
		}

		internal void OnSizeChanged(WindowSizeChangedEventArgs windowSizeChangedEventArgs)
		{
			SizeChanged?.Invoke(this, windowSizeChangedEventArgs);
		}
	}
}
