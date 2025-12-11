using System;//Mostafa_Desha [ 01006596987 ]
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace AccServer
{
    public struct ErrorCodes
    {
        public const int INVALID_HANDLE_VALUE = -1;
    }
    public class MyConsole
    {
        private const int MF_BYCOMMAND = 0x00000000;
        public const int SC_CLOSE = 0xF060;

        [DllImport("user32.dll")]
        public static extern int DeleteMenu(IntPtr hMenu, int nPosition, int wFlags);

        [DllImport("user32.dll")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GetConsoleWindow();

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr GetStdHandle(int nStdHandle);
        [DllImport("kernel32.dll")]
        public static extern bool SetConsoleTitle(string lpConsoleTitle);
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool ReadConsole(IntPtr hConsoleInput, [Out] StringBuilder lpBuffer, uint nNumberOfCharsToRead, out uint lpNumberOfCharsRead, IntPtr lpReserved);
        [DllImport("kernel32.dll")]
        public static extern bool SetStdHandle(int nStdHandle, IntPtr hHandle);
        [DllImport("kernel32.dll")]
        public static extern bool WriteConsole(IntPtr hConsoleOutput, string lpBuffer, uint nNumberOfCharsToWrite, out uint lpNumberOfCharsWritten, IntPtr lpReserved);

        [DllImport("kernel32.dll")]
        public static extern uint GetConsoleTitle([Out] StringBuilder lpConsoleTitle, uint nSize);


        [DllImport("kernel32")]
        public static extern bool AllocConsolee();

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool AttachConsolee(uint dwProcessId);
        public static void DissableButton()
        {
            DeleteMenu(GetSystemMenu(GetConsoleWindow(), false), SC_CLOSE, MF_BYCOMMAND);

        }

        private static IntPtr _InputHandle;
        private static IntPtr _OutputHandle;


        private static Counter ExceptionsCounter = new Counter(1);

        /// <summary> The handle Input handle is not recommended </summary>
        public static IntPtr InputHandle
        {
            get { return _InputHandle; }
            set
            {
                if (!SetStdHandle(STD_INPUT_HANDLE, value))
                    throw new Exception("Unable to set the new Input handle");
                _InputHandle = value;
            }
        }

        /// <summary> handle Output handle is not recommended </summary>
        public static IntPtr OutputHandle
        {
            get { return _OutputHandle; }
            set
            {
                if (!SetStdHandle(STD_OUTPUT_HANDLE, value))
                    throw new Exception("Unable to set the new Output handle");
                _OutputHandle = value;
            }
        }

        public static string Title
        {
            get
            {
                StringBuilder sb = new StringBuilder(32767);
                uint size = (uint)sb.Capacity;
                GetConsoleTitle(sb, size);
                sb.Capacity = sb.Length;
                return sb.ToString();
            }
            set { SetConsoleTitle(value); }
        }

        public const int STD_INPUT_HANDLE = -10;
        public const int STD_OUTPUT_HANDLE = -11;
        public const int STD_ERROR_HANDLE = -12; //Not being used yet

        static MyConsole()
        {
            InputHandle = GetStdHandle(STD_INPUT_HANDLE);
            OutputHandle = GetStdHandle(STD_OUTPUT_HANDLE);

            if (InputHandle.ToInt32() == ErrorCodes.INVALID_HANDLE_VALUE ||
                OutputHandle.ToInt32() == ErrorCodes.INVALID_HANDLE_VALUE)
            {
                throw new Exception("Unable to get the Console Handle");
            }
        }

        public static void Write(string value)
        {
            uint written = 0;
            WriteConsole(OutputHandle, value, (uint)value.Length, out written, IntPtr.Zero);
        }
        private static DateTime NOW = DateTime.Now;
        private static Time32 NOW32 = Time32.Now;
        public static string TimeStamp()
        {
            return "[" + NOW.AddMilliseconds((Time32.Now - NOW32).GetHashCode()).ToString("hh:mm:ss") + "]";
        }
        public static void WriteLine(object value, ConsoleColor color = ConsoleColor.White)
        {
            System.Console.ForegroundColor = ConsoleColor.Red;
            System.Console.Write(TimeStamp() + " ");
            System.Console.ForegroundColor = color;
            System.Console.Write(value.ToString() + "\n");
        }

        public static ConsoleKeyInfo ReadKey()
        {
            return Console.ReadKey();
        }
        ///<summary> Attach a console to a specified process, CAUTION: If the console will be closed the process will close! </summary>
        public static void AttachConsole(uint PID)
        {
            if (!AttachConsolee(PID))
                throw new Exception("Unable to attach the console.");
        }

        /// <summary>
        /// Create a console for the current process, Caution: Closing the console will also close the process!
        /// </summary>
        public static void AllocConsole()
        {
            if (!AllocConsolee())
                throw new Exception("Unable to allocate a console.");
            //System.Console.read
        }
        public static void WriteException(Exception e)
        {
            try
            {
                MyConsole.WriteLine(e.ToString());
                SaveException(e);
            }
            catch (Exception ex)
            {
                WriteLine(ex.ToString());
            }
        }
        public static void SaveException(Exception e)
        {


            const string UnhandledExceptionsPath = "Exceptions\\";

            var dt = DateTime.Now;
            string date = dt.Month + "-" + dt.Day + "//";

            if (!Directory.Exists(Application.StartupPath + UnhandledExceptionsPath))
                Directory.CreateDirectory(Application.StartupPath + "\\" + UnhandledExceptionsPath);
            if (!Directory.Exists(Application.StartupPath + "\\" + UnhandledExceptionsPath + date))
                Directory.CreateDirectory(Application.StartupPath + "\\" + UnhandledExceptionsPath + date);
            if (!Directory.Exists(Application.StartupPath + "\\" + UnhandledExceptionsPath + date + e.TargetSite.Name))
                Directory.CreateDirectory(Application.StartupPath + "\\" + UnhandledExceptionsPath + date + e.TargetSite.Name);

            string fullPath = Application.StartupPath + "\\" + UnhandledExceptionsPath + date + e.TargetSite.Name + "\\";

            string date2 = dt.DayOfYear + "-" + dt.Hour + "-" + dt.Minute + "-" + dt.Second + "E" + ExceptionsCounter.Next;
            List<string> Lines = new List<string>();

            Lines.Add("----Exception message----");
            Lines.Add(e.Message);
            Lines.Add("----End of exception message----\r\n");

            Lines.Add("----Stack trace----");
            Lines.Add(e.StackTrace);
            Lines.Add("----End of stack trace----\r\n");

            File.WriteAllLines(fullPath + date2 + ".txt", Lines.ToArray());
            WriteLine(e.ToString());
        }
        public static string ReadLine()
        {
            StringBuilder sb = new StringBuilder();
            uint read = 0;

            ReadConsole(InputHandle, sb, 100, out read, IntPtr.Zero);
            if (read == 0)
                return "";
            return sb.ToString(0, (int)read - 2);
        }
    }
}
