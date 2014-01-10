using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Microsoft.Win32;

[StructLayout(LayoutKind.Sequential)]
struct MIDIOUTCAPS
{
    public ushort wMid;
    public ushort wPid;
    public uint vDriverVersion;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
    public string szPname;
    public ushort wTechnology;
    public ushort wVoices;
    public ushort wNotes;
    public ushort wChannelMask;
    public uint dwSupport;
}

public enum MMRESULT : uint
{
    MMSYSERR_NOERROR = 0,
    MMSYSERR_ERROR = 1,
    MMSYSERR_BADDEVICEID = 2,
    MMSYSERR_NOTENABLED = 3,
    MMSYSERR_ALLOCATED = 4,
    MMSYSERR_INVALHANDLE = 5,
    MMSYSERR_NODRIVER = 6,
    MMSYSERR_NOMEM = 7,
    MMSYSERR_NOTSUPPORTED = 8,
    MMSYSERR_BADERRNUM = 9,
    MMSYSERR_INVALFLAG = 10,
    MMSYSERR_INVALPARAM = 11,
    MMSYSERR_HANDLEBUSY = 12,
    MMSYSERR_INVALIDALIAS = 13,
    MMSYSERR_BADDB = 14,
    MMSYSERR_KEYNOTFOUND = 15,
    MMSYSERR_READERROR = 16,
    MMSYSERR_WRITEERROR = 17,
    MMSYSERR_DELETEERROR = 18,
    MMSYSERR_VALNOTFOUND = 19,
    MMSYSERR_NODRIVERCB = 20,
    WAVERR_BADFORMAT = 32,
    WAVERR_STILLPLAYING = 33,
    WAVERR_UNPREPARED = 34
}      

namespace com.wibblr.windows7_midi_mapper
{
    /// <summary>
    /// GUI to allow user to select a MIDI output device.
    /// </summary>
    public partial class MainWindow : Window
    {
        [DllImport("winmm.dll", SetLastError = true)]
        public static extern uint midiOutGetNumDevs();

        [DllImport("winmm.dll", SetLastError = true)]
        static extern MMRESULT midiOutGetDevCaps(UIntPtr uDeviceID, ref MIDIOUTCAPS lpMidiOutCaps, uint cbMidiOutCaps);

        public MainWindow()
        {
            InitializeComponent();

            uint numDevs = midiOutGetNumDevs();
            for (int i = 0; i < numDevs; i++)
            {
                MIDIOUTCAPS caps = new MIDIOUTCAPS();
                if (MMRESULT.MMSYSERR_NOERROR == midiOutGetDevCaps((UIntPtr)i, ref caps, (uint)Marshal.SizeOf(typeof(MIDIOUTCAPS))))
                {
                    cbOutputDevice.Items.Add(caps.szPname);
                }
            }

            cbOutputDevice.SelectedValue = GetCurrentOutputDevice();
        }

        private void OK_Clicked(object sender, RoutedEventArgs e)
        {
            ApplyChanges();
            Close();
        }

        private void Cancel_Clicked(object sender, RoutedEventArgs e)
        {   
            Close();
        }

        private void Apply_Clicked(object sender, RoutedEventArgs e)
        {
            ApplyChanges();
        }

        private void ApplyChanges()
        {
            SetCurrentOutputDevice(cbOutputDevice.SelectedValue.ToString());
        }

        private string GetCurrentOutputDevice() 
        {
            try {
                return (string) Registry.GetValue("HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\Multimedia\\MIDIMap", "szPname", "");
            }
            catch (Exception) {
                return "";    
            }            
        }

        private void SetCurrentOutputDevice(string outputDevice)
        {
            Registry.SetValue("HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\Multimedia\\MIDIMap", "szPname", outputDevice);
        }
    }
}
