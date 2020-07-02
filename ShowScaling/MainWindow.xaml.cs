using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ShowScaling
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            this.LocationChanged += MainWindow_LocationChanged;
        }
        public const int MonitorDefaultToNearest = 0x2;

        private void MainWindow_LocationChanged(object sender, EventArgs e)
        {
            IntPtr windowHandle = new WindowInteropHelper(Application.Current.MainWindow).Handle;
            var monitorHandle = MonitorFromWindow(windowHandle, MonitorDefaultToNearest);

            // Get the logical width and height of the monitor.
            var monitorInfo = new MonitorInfoEx();
            monitorInfo.Size = Marshal.SizeOf(monitorInfo);

            GetMonitorInfoEx(monitorHandle, ref monitorInfo);
            int cxLogical = (monitorInfo.MonitorArea.Right - monitorInfo.MonitorArea.Left);

            // Get the physical width and height of the monitor.
            DevMode dm = new DevMode();
            dm.dmSize = (short)Marshal.SizeOf(dm);
            dm.dmDriverExtra = 0;
            EnumDisplaySettings(monitorInfo.DeviceName, EnumCurrentSettings, ref dm);
            int cxPhysical = dm.dmPelsWidth;

            // Calculate the scaling factor.
            double scaleFactor = ((double)cxPhysical / (double)cxLogical);

            Scale.Text = scaleFactor.ToString();
        }



        public const int EnumCurrentSettings = -1;

        [DllImport("user32.dll", SetLastError = true, EntryPoint = "GetMonitorInfo")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetMonitorInfoEx(IntPtr monitorHandle, ref MonitorInfoEx monitorInfo);

        [DllImport("Shcore.dll")]
        static extern long GetScaleFactorForMonitor(IntPtr hMon, ref Int32 pScale);

        [DllImport("user32.dll")]
        static extern IntPtr MonitorFromWindow(IntPtr hwnd, uint dwFlags);

        [DllImport("user32.dll", SetLastError = true, EntryPoint = "EnumDisplaySettings", BestFitMapping = false)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool EnumDisplaySettings([MarshalAs(UnmanagedType.AnsiBStr)] string deviceName, int modeNum, ref DevMode devMode);


        [StructLayout(LayoutKind.Sequential)]
        public struct MonitorInfoEx
        {
            /// <summary>
            /// The size, in bytes, of the structure. Set this member to sizeof(MONITORINFO) (40) before calling the GetMonitorInfo function. 
            /// Doing so lets the function determine the type of structure you are passing to it.
            /// </summary>
            [SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "Native struct")]
            public int Size;

            /// <summary>
            /// A RECT structure that specifies the display monitor rectangle, expressed in virtual-screen coordinates. 
            /// Note that if the monitor is not the primary display monitor, some of the rectangle's coordinates may be negative values.
            /// </summary>
            [SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "Native struct")]
            public Win32Rect MonitorArea;

            /// <summary>
            /// A RECT structure that specifies the work area rectangle of the display monitor that can be used by applications, 
            /// expressed in virtual-screen coordinates. Windows uses this rectangle to maximize an application on the monitor. 
            /// The rest of the area in rcMonitor contains system windows such as the task bar and side bars. 
            /// Note that if the monitor is not the primary display monitor, some of the rectangle's coordinates may be negative values.
            /// </summary>
            [SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "Native struct")]
            public Win32Rect WorkArea;

            /// <summary>
            /// The attributes of the display monitor.
            /// 
            /// This member can be the following value:
            ///   1 : MONITORINFOF_PRIMARY
            /// </summary>
            [SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flags", Justification = "Native definition")]
            [SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "Native struct")]
            public uint Flags;

            /// <summary>
            /// A string that specifies the device name of the monitor being used. 
            /// </summary>
            [SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "Native struct")]
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string DeviceName;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Win32Rect
        {
            private readonly int _left;
            private readonly int _top;
            private readonly int _right;
            private readonly int _bottom;

            /// <summary>
            /// Initializes a new instance of the <see cref="Win32Rect"/> struct.
            /// </summary>
            /// <param name="left">The left.</param>
            /// <param name="top">The top.</param>
            /// <param name="right">The right.</param>
            /// <param name="bottom">The bottom.</param>
            public Win32Rect(int left, int top, int right, int bottom)
            {
                _left = left;
                _top = top;
                _right = right;
                _bottom = bottom;
            }

            /// <summary>
            /// Gets the x-coordinate of the upper-left corner of the rectangle.
            /// </summary>
            public int Left
            {
                get { return _left; }
            }

            /// <summary>
            /// Gets the y-coordinate of the upper-left corner of the rectangle.
            /// </summary>
            public int Top
            {
                get { return _top; }
            }

            /// <summary>
            /// Gets the x-coordinate of the lower-right corner of the rectangle.
            /// </summary>
            public int Right
            {
                get { return _right; }
            }

            /// <summary>
            /// Gets the y-coordinate of the lower-right corner of the rectangle.
            /// </summary>
            public int Bottom
            {
                get { return _bottom; }
            }
        }


        [StructLayout(LayoutKind.Sequential)]
        public struct DevMode
        {
            private const int CCHDEVICENAME = 0x20;
            private const int CCHFORMNAME = 0x20;

#pragma warning disable SA1307
            [SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "Native struct")]
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x20)]
            public string dmDeviceName;
            [SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "Native struct")]
            public short dmSpecVersion;
            [SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "Native struct")]
            public short dmDriverVersion;
            [SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "Native struct")]
            public short dmSize;
            [SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "Native struct")]
            public short dmDriverExtra;
            [SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "Native struct")]
            public int dmFields;
            [SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "Native struct")]
            public int dmPositionX;
            [SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "Native struct")]
            public int dmPositionY;
            [SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "Native struct")]
            public int dmDisplayOrientation;
            [SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "Native struct")]
            public int dmDisplayFixedOutput;
            [SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "Native struct")]
            public short dmColor;
            [SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "Native struct")]
            public short dmDuplex;
            [SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "Native struct")]
            public short dmYResolution;
            [SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "Native struct")]
            public short dmTTOption;
            [SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "Native struct")]
            public short dmCollate;
            [SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "Native struct")]
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x20)]
            public string dmFormName;
            [SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "Native struct")]
            public short dmLogPixels;
            [SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "Native struct")]
            [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Pel", Justification = "Native definition")]
            public int dmBitsPerPel;
            [SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "Native struct")]
            [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Pels", Justification = "Native definition")]
            public int dmPelsWidth;
            [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Pels", Justification = "Native definition")]
            [SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "Native struct")]
            public int dmPelsHeight;
            [SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "Native struct")]
            [SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flags", Justification = "Native definition.")]
            public int dmDisplayFlags;
            [SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "Native struct")]
            public int dmDisplayFrequency;
            [SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "Native struct")]
            public int dmICMMethod;
            [SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "Native struct")]
            public int dmICMIntent;
            [SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "Native struct")]
            public int dmMediaType;
            [SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "Native struct")]
            public int dmDitherType;
            [SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "Native struct")]
            public int dmReserved1;
            [SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "Native struct")]
            public int dmReserved2;
            [SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "Native struct")]
            public int dmPanningWidth;
            [SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "Native struct")]
            public int dmPanningHeight;
#pragma warning restore SA1307
        }
    }
}
