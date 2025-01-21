using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using Guna.UI2.WinForms;
using ruhan;
using Memory;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Helper;

namespace ROYAL_REGEDIT___
{

    public partial class MainFrom : Form
    {
        KeyHelper kh = new KeyHelper();
        bool shift;

        private const int ParticleCount = 80;
        private const int DrawCount = 80;
        private readonly Random _random = new Random();
        private readonly PointF[] _particlePositions = new PointF[ParticleCount];
        private readonly PointF[] _particleTargetPositions = new PointF[ParticleCount];
        private readonly float[] _particleSpeeds = new float[ParticleCount];
        private readonly float[] _particleSizes = new float[ParticleCount];
        private readonly float[] _particleRadii = new float[ParticleCount];
        private readonly float[] _particleRotations = new float[ParticleCount];
        private readonly PointF[] _vertices = new PointF[3];

        public MainFrom()
        {
            kh.KeyDown += Kh_KeyDown;
            kh.KeyUp += Kh_KeyUp;

            InitializeComponent();


            DoubleBuffered = true;

            InitializeParticles();

            Timer timer = new Timer
            {
                Interval = 3
            };
            timer.Tick += (sender, args) =>
            {
                UpdateParticles();
                Invalidate();
            };
            timer.Start();




        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }








        private void timer1_Tick(object sender, EventArgs e)
        {

            UpdateParticles();
            Invalidate();




        }




        

       

        private async void guna2ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {


             






        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {

            if (guna2Button1.Checked)
            {
                RestoreMemoryValues2();
                Console.Beep(1000, 200);
            }
            else
            {
                RestoreMemoryValues();
                Console.Beep(1000, 200);
            }
        }
        private static Qᴜᴀɴᴛᴜᴍ x = new Qᴜᴀɴᴛᴜᴍ();

        // AIM BOT CODE HARE 
        string AOB = "";
        string readFORhead = "70";
        string write = "6C";

        // SPEED CODE HARE 

     

        // SNIPER SCOPE CODE HARE 
        string SNIPERS = "";
        string SNIPERR = "";

        // AWM SWITCH CODE HARE 

        string AWMS = "";
        string AWMR = "";

        // M82B SWITCH CODE HARE

        string M82B = "";
        string M82BR = "";

        //  SNIPER AMMO CODE HARE 

        string AMMO = "";
        string AMMOR = "";

        // M82B LOCATION CODE HARE 

        string M82BLC = "";
        string M82BLCR = "";

        // AIM FOV CODE HARE 

        string AIMFOV = "";
        string AIMFOVR = "";

        // WALL HACK CODE HARE 

        string WALL = "";
        string WALLR = "";

        // RUN MEDKIT CODE HARE 

        string RUN = "";
        string RUNR = "";

        // 2 GUN CODE HARE 

        string GUN = "";
        string GUNR = "";

        // CAMARA HACK CODE HARE 


        string CMA = "";
        string CMAR = "";

        // GUEST RESET CODE HARE 

        string gst = " ";
        string gstr = "";


     
        string t = "";
        string tt = "";


        // SOUND CODE HARE 



        static void sound()
        {
            var b7ae = Assembly.GetCallingAssembly();
            var s = "ROYALREGEDIT.1280207838064541810.wav";
            using (Stream str = b7ae.GetManifestResourceStream(s))
            {
                SoundPlayer plr = new SoundPlayer(str);
                plr.Play();
            }
        }




































        private Dictionary<long, int> originalValues = new Dictionary<long, int>();
        private Dictionary<long, int> originallValues = new Dictionary<long, int>();
        private Dictionary<long, int> originalValues2 = new Dictionary<long, int>();
        private Dictionary<long, int> originallValues2 = new Dictionary<long, int>();

        public void RestoreMemoryValues()
        {
            foreach (var entry in originalValues)
            {
                x.WriteMemory(entry.Key.ToString("X"), "int", entry.Value.ToString());
            }
            {
                foreach (var entry in originallValues)
                {
                    x.WriteMemory(entry.Key.ToString("X"), "int", entry.Value.ToString());
                }
            }
        }

        public void RestoreMemoryValues2()
        {
            foreach (var entry in originalValues2)
            {
                x.WriteMemory(entry.Key.ToString("X"), "int", entry.Value.ToString());
            }
            {
                foreach (var entry in originallValues2)
                {
                    x.WriteMemory(entry.Key.ToString("X"), "int", entry.Value.ToString());
                }
            }
        }









        private void InitializeParticles()
        {
            Size screenSize = Screen.PrimaryScreen.Bounds.Size;
            for (int i = 0; i < ParticleCount; i++)
            {
                _particlePositions[i] = new PointF(0, 0);
                _particleTargetPositions[i] = new PointF(_random.Next(screenSize.Width), screenSize.Height * 2);
                _particleSpeeds[i] = 1 + _random.Next(25);
                _particleSizes[i] = _random.Next(8);
                _particleRadii[i] = _random.Next(4);
                _particleRotations[i] = 0;
            }
        }

        private void UpdateParticles()
        {
            Size screenSize = Screen.PrimaryScreen.Bounds.Size;
            for (int i = 0; i < ParticleCount; i++)
            {
                if (_particlePositions[i].X == 0 || _particlePositions[i].Y == 0)
                {
                    _particlePositions[i] = new PointF(_random.Next(screenSize.Width + 1), 15f);
                    _particleSpeeds[i] = 1 + _random.Next(25);
                    _particleRadii[i] = _random.Next(4);
                    _particleSizes[i] = _random.Next(8);
                    _particleTargetPositions[i] = new PointF(_random.Next(screenSize.Width), screenSize.Height * 2);
                }

                float deltaTime = 2.5f / 60;
                _particlePositions[i] = Lerp(_particlePositions[i], _particleTargetPositions[i], deltaTime * (_particleSpeeds[i] / 60));
                _particleRotations[i] += deltaTime;

                if (_particlePositions[i].Y > screenSize.Height)
                {
                    _particlePositions[i] = new PointF(0, 0);
                    _particleRotations[i] = 0;
                }
            }
        }

        private PointF Lerp(PointF start, PointF end, float t)
        {
            return new PointF(start.X + (end.X - start.X) * t, start.Y + (end.Y - start.Y) * t);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            for (int i = 0; i < DrawCount; i++)
            {
                DrawTriangleWithGlow(e.Graphics, _particlePositions[i], _particleSizes[i], _particleRotations[i]);
            }
        }

        private void DrawTriangleWithGlow(Graphics graphics, PointF position, float size, float rotation)
        {
            float angle = (float)(Math.PI * 2 / 3);
            PointF[] vertices = new PointF[3];

            for (int i = 0; i < 3; i++)
            {
                vertices[i] = new PointF(
                    position.X + size * (float)Math.Cos(rotation + i * angle),
                    position.Y + size * (float)Math.Sin(rotation + i * angle)
                );
            }

            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;


            int maxGlowLayers = 10;
            for (int j = 0; j < maxGlowLayers; j++)
            {
                int alpha = 25 - 2 * j;
                using (Brush glowBrush = new SolidBrush(Color.FromArgb(alpha, 50, 205, 50)))
                {
                    float glowSize = size + j * 4;
                    graphics.FillEllipse(glowBrush, position.X - glowSize / 2, position.Y - glowSize / 2, glowSize, glowSize);
                }
            }


            using (Brush brush = new SolidBrush(Color.FromArgb(50, 205, 50)))
            {
                graphics.FillPolygon(brush, vertices);
            }
        }





        public class Particle
        {
            public PointF Position { get; set; }
            public PointF Velocity { get; set; }
            public int Radius { get; set; }
            public Color Color { get; set; }

        }































        private async void guna2ComboBox1_SelectedIndexChanged_1(object sender, EventArgs e)
        {
           
        }

        private void guna2CheckBox4_CheckedChanged(object sender, EventArgs e)
        {
            if (guna2CheckBox4.Checked)
            {

                string processName = "HD-Player";
                string dllResourceName = "ROYALREGEDIT.Properties.chams3d.dll";
                string tempDllPath = Path.Combine(Path.GetTempPath(), "chams3d.dll");
                admin(dllResourceName, tempDllPath); ;
                Process[] targetProcesses = Process.GetProcessesByName(processName);
                if (targetProcesses.Length == 0)
                {
                    Console.WriteLine($"Waiting for {processName}.exe...");
                }
                if (targetProcesses.Length == 0)
                {




                }
                else
                {
                    Process targetProcess = targetProcesses[0];
                    IntPtr hProcess = OpenProcess(PROCESS_CREATE_THREAD | PROCESS_QUERY_INFORMATION | PROCESS_VM_OPERATION | PROCESS_VM_WRITE | PROCESS_VM_READ, false, targetProcess.Id);
                    IntPtr loadLibraryAddr = GetProcAddress(GetModuleHandle("kernel32.dll"), "LoadLibraryA");
                    IntPtr allocMemAddress = VirtualAllocEx(hProcess, IntPtr.Zero, (IntPtr)tempDllPath.Length, MEM_COMMIT, PAGE_READWRITE);
                    IntPtr bytesWritten;
                    WriteProcessMemory(hProcess, allocMemAddress, System.Text.Encoding.ASCII.GetBytes(tempDllPath), (uint)tempDllPath.Length, out bytesWritten);
                    CreateRemoteThread(hProcess, IntPtr.Zero, IntPtr.Zero, loadLibraryAddr, allocMemAddress, 0, IntPtr.Zero);
                }

                sound();
            }
            else
            {

            }
        }


        public static string PID;


        private void guna2ControlBox1_Click(object sender, EventArgs e)
        {
            this.Close();
            Application.Exit();

        }

        private void guna2CheckBox10_CheckedChanged(object sender, EventArgs e)
        {
            if (guna2CheckBox10.Checked)
            {

                string string_0;

                Int32 proc = Process.GetProcessesByName("HD-Player")[0].Id;
                  

                x.OpenProcess(proc);
                var enumerable = x.AoBScan(0x0000000000000000, 0x00007fffffffffff, SNIPERS, true, true, string.Empty).Result;
                string_0 = "0x" + enumerable.FirstOrDefault().ToString("X");
                foreach (long num in enumerable)
                {
                    x.WriteMemory(num.ToString("X"), "bytes", SNIPERR, string.Empty, null);
                }
                sound();
                 


            }
            else
            {
                
                sound();
                 
            }
        }

        private void guna2CheckBox9_CheckedChanged(object sender, EventArgs e)
        {
            if (guna2CheckBox9.Checked)
            {


                string string_0;

                Int32 proc = Process.GetProcessesByName("HD-Player")[0].Id;
                  

                x.OpenProcess(proc);
                var enumerable = x.AoBScan(0x0000000000000000, 0x00007fffffffffff, AWMS, true, true, string.Empty).Result;
                string_0 = "0x" + enumerable.FirstOrDefault().ToString("X");
                foreach (long num in enumerable)
                {
                    x.WriteMemory(num.ToString("X"), "bytes", AWMR, string.Empty, null);
                }
                sound();
                 


            }
            else
            {

            }
        }

        private void guna2CheckBox8_CheckedChanged(object sender, EventArgs e)
        {
            if (guna2CheckBox8.Checked)
            {
                string string_0;

                Int32 proc = Process.GetProcessesByName("HD-Player")[0].Id;
                  

                x.OpenProcess(proc);
                var enumerable = x.AoBScan(0x0000000000000000, 0x00007fffffffffff, M82B, true, true, string.Empty).Result;
                string_0 = "0x" + enumerable.FirstOrDefault().ToString("X");
                foreach (long num in enumerable)
                {
                    x.WriteMemory(num.ToString("X"), "bytes", M82BR, string.Empty, null);
                }
                sound();

                 

            }
            else
            {

            }
        }

        private void guna2CheckBox6_CheckedChanged(object sender, EventArgs e)
        {
            if (guna2CheckBox6.Checked)
            {
                string string_0;

                Int32 proc = Process.GetProcessesByName("HD-Player")[0].Id;
                  

                x.OpenProcess(proc);
                var enumerable = x.AoBScan(0x0000000000000000, 0x00007fffffffffff, AMMO, true, true, string.Empty).Result;
                string_0 = "0x" + enumerable.FirstOrDefault().ToString("X");
                foreach (long num in enumerable)
                {
                    x.WriteMemory(num.ToString("X"), "bytes", AMMOR, string.Empty, null);
                }
                sound();
                 

            }
            else
            {

            }
        }

        private void guna2CheckBox7_CheckedChanged(object sender, EventArgs e)
        {
            if (guna2CheckBox7.Checked)
            {
                string string_0;

                Int32 proc = Process.GetProcessesByName("HD-Player")[0].Id;
                  

                x.OpenProcess(proc);
                var enumerable = x.AoBScan(0x0000000000000000, 0x00007fffffffffff, M82BLC, true, true, string.Empty).Result;
                string_0 = "0x" + enumerable.FirstOrDefault().ToString("X");
                foreach (long num in enumerable)
                {
                    x.WriteMemory(num.ToString("X"), "bytes", M82BLCR, string.Empty, null);
                }
                sound();
                 

            }
            else
            {
                string string_0;

                Int32 proc = Process.GetProcessesByName("HD-Player")[0].Id;
                  

                x.OpenProcess(proc);
                var enumerable = x.AoBScan(0x0000000000000000, 0x00007fffffffffff, RUN, true, true, string.Empty).Result;
                string_0 = "0x" + enumerable.FirstOrDefault().ToString("X");
                foreach (long num in enumerable)
                {
                    x.WriteMemory(num.ToString("X"), "bytes", RUNR, string.Empty, null);
                }
                sound();
                 
            }
        }

        private void guna2CheckBox15_CheckedChanged(object sender, EventArgs e)
        {
            if (guna2CheckBox15.Checked)
            {
                string string_0;

                Int32 proc = Process.GetProcessesByName("HD-Player")[0].Id;
                  

                x.OpenProcess(proc);
                var enumerable = x.AoBScan(0x0000000000000000, 0x00007fffffffffff, AIMFOV, true, true, string.Empty).Result;
                string_0 = "0x" + enumerable.FirstOrDefault().ToString("X");
                foreach (long num in enumerable)
                {
                    x.WriteMemory(num.ToString("X"), "bytes", AIMFOVR, string.Empty, null);
                }
                sound();
                 

            }
            else
            {

            }
        }

        private void guna2CheckBox14_CheckedChanged(object sender, EventArgs e)
        {
            if (guna2CheckBox14.Checked)
            {
                string string_0;

                Int32 proc = Process.GetProcessesByName("HD-Player")[0].Id;
                  

                x.OpenProcess(proc);
                var enumerable = x.AoBScan(0x0000000000000000, 0x00007fffffffffff, RUN, true, true, string.Empty).Result;
                string_0 = "0x" + enumerable.FirstOrDefault().ToString("X");
                foreach (long num in enumerable)
                {
                    x.WriteMemory(num.ToString("X"), "bytes", RUNR, string.Empty, null);
                }
                sound();
                 

            }
            else
            {

            }
        }
        Mem rr = new Mem(); 
        private  void guna2CheckBox13_CheckedChanged(object sender, EventArgs e)
        {


            if (guna2CheckBox13.Checked)
            {

                string string_0;

                Int32 proc = Process.GetProcessesByName("HD-Player")[0].Id;
                  

                x.OpenProcess(proc);
                var enumerable = x.AoBScan(0x0000000000000000, 0x00007fffffffffff, GUN, true, true, string.Empty).Result;
                string_0 = "0x" + enumerable.FirstOrDefault().ToString("X");
                foreach (long num in enumerable)
                {
                    x.WriteMemory(num.ToString("X"), "bytes", GUNR, string.Empty, null);
                }
                sound();
                 
            }
            else
            {
               
                sound();
                 
            }



        }

        private void guna2CheckBox12_CheckedChanged(object sender, EventArgs e)
        {

            if (guna2CheckBox12.Checked)
            {
                string string_0;

                Int32 proc = Process.GetProcessesByName("HD-Player")[0].Id;


                x.OpenProcess(proc);
                var enumerable = x.AoBScan(0x0000000000000000, 0x00007fffffffffff, WALL, true, true, string.Empty).Result;
                string_0 = "0x" + enumerable.FirstOrDefault().ToString("X");
                foreach (long num in enumerable)
                {
                    x.WriteMemory(num.ToString("X"), "bytes", WALLR, string.Empty, null);
                }
                sound();


            }
            else
            {

            }
        }

        private void guna2CheckBox11_CheckedChanged(object sender, EventArgs e)
        {
            if (guna2CheckBox11.Checked)
            {
                string string_0;

                Int32 proc = Process.GetProcessesByName("HD-Player")[0].Id;
                  

                x.OpenProcess(proc);
                var enumerable = x.AoBScan(0x0000000000000000, 0x00007fffffffffff, CMA, true, true, string.Empty).Result;
                string_0 = "0x" + enumerable.FirstOrDefault().ToString("X");
                foreach (long num in enumerable)
                {
                    x.WriteMemory(num.ToString("X"), "bytes", CMAR, string.Empty, null);
                }
                sound();
                 

            }
            else
            {

            }
        }

       

      






        private async void guna2Button2_Click(object sender, EventArgs e)
        {

            sound();

        }

       

        private void guna2CheckBox20_CheckedChanged(object sender, EventArgs e)
        {
            if (guna2CheckBox20.Checked)
            {
                this.ExecuteCommand("netsh advfirewall firewall add rule name=\"TemporaryBlock2\" dir=in action=block profile=any program=\"C:\\Program Files\\BlueStacks_nxt\\HD-Player.exe");
                this.ExecuteCommand("netsh advfirewall firewall add rule name=\"TemporaryBlock2\" dir=out action=block profile=any program=\"C:\\Program Files\\BlueStacks_nxt\\HD-Player.exe");
                this.ExecuteCommand("netsh advfirewall firewall add rule name=\"TemporaryBlock2\" dir=in action=block profile=any program=\"C:\\Program Files\\BlueStacks\\HD-Player.exe");
                this.ExecuteCommand("netsh advfirewall firewall add rule name=\"TemporaryBlock2\" dir=out action=block profile=any program=\"C:\\Program Files\\BlueStacks\\HD-Player.exe");
                this.ExecuteCommand("netsh advfirewall firewall add rule name=\"TemporaryBlock2\" dir=in action=block profile=any program=\"C:\\Program Files\\BlueStacks_msi2\\HD-Player.exe");
                this.ExecuteCommand("netsh advfirewall firewall add rule name=\"TemporaryBlock2\" dir=out action=block profile=any program=\"C:\\Program Files\\BlueStacks_msi2\\HD-Player.exe");
                this.ExecuteCommand("netsh advfirewall firewall add rule name=\"TemporaryBlock2\" dir=in action=block profile=any program=\"C:\\Program Files\\BlueStacks_msi5\\HD-Player.exe");
                this.ExecuteCommand("netsh advfirewall firewall add rule name=\"TemporaryBlock2\" dir=out action=block profile=any program=\"C:\\Program Files\\BlueStacks_msi5\\HD-Player.exe");
                sound();

            }
            else
            {

                this.ExecuteCommand("netsh advfirewall firewall delete rule name=all program=\"C:\\Program Files\\BlueStacks_nxt\\HD-Player.exe");
                this.ExecuteCommand("netsh advfirewall firewall delete rule name=all program=\"C:\\Program Files\\BlueStacks\\HD-Player.exe");
                this.ExecuteCommand("netsh advfirewall firewall delete rule name=all program=\"C:\\Program Files\\BlueStacks_msi2\\HD-Player.exe");
                this.ExecuteCommand("netsh advfirewall firewall delete rule name=all program=\"C:\\Program Files\\BlueStacks_msi5\\HD-Player.exe");

                Console.Beep(1000, 300);
            }
        }





        public void ExecuteCommand(string command)
        {
            ProcessStartInfo processStartInfo = new ProcessStartInfo("cmd.exe")
            {
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            using (Process process = new Process
            {
                StartInfo = processStartInfo
            })
            {
                process.Start();
                process.StandardInput.WriteLine(command);
                process.StandardInput.Flush();
                process.StandardInput.Close();
                process.WaitForExit();
            }
        }


        public static bool Streaming;
        [DllImport("user32.dll")]
        public static extern uint SetWindowDisplayAffinity(IntPtr hwnd, uint dwAffinity);



        private void guna2CheckBox19_CheckedChanged(object sender, EventArgs e)
        {
            if (guna2CheckBox19.Checked)
            {
                base.ShowInTaskbar = false;
                MainFrom.Streaming = true;
                MainFrom.SetWindowDisplayAffinity(base.Handle, 17U);

            }

            else
            {
                base.ShowInTaskbar = true;
                MainFrom.Streaming = false;
                MainFrom.SetWindowDisplayAffinity(base.Handle, 0U);
            }
        }


        private static void admin(string resourceName, string outputPath)
        {
            Assembly executingAssembly = Assembly.GetExecutingAssembly();
            using (Stream manifestResourceStream = executingAssembly.GetManifestResourceStream(resourceName))
            {
                bool flag = manifestResourceStream == null;
                if (flag)
                {
                    throw new ArgumentException("Resource '" + resourceName + "' not found.");
                }
                using (FileStream fileStream = new FileStream(outputPath, FileMode.Create))
                {
                    byte[] array = new byte[manifestResourceStream.Length];
                    manifestResourceStream.Read(array, 0, array.Length);
                    fileStream.Write(array, 0, array.Length);
                }
            }
        }
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr OpenProcess(uint processAccess, bool bInheritHandle, int processId);
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr GetModuleHandle(string lpModuleName);
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, IntPtr dwSize, uint flAllocationType, uint flProtect);
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, uint nSize, out IntPtr lpNumberOfBytesWritten);
        [DllImport("kernel32.dll")]
        static extern IntPtr CreateRemoteThread(IntPtr hProcess, IntPtr lpThreadAttribute, IntPtr dwStackSize, IntPtr lpStartAddress, IntPtr lpParameter, uint dwCreationFlags, IntPtr lpThreadId);
        const uint PROCESS_CREATE_THREAD = 0x2;
        const uint PROCESS_QUERY_INFORMATION = 0x400;
        const uint PROCESS_VM_OPERATION = 0x8;
        const uint PROCESS_VM_WRITE = 0x20;
        const uint PROCESS_VM_READ = 0x10;
        const uint MEM_COMMIT = 0x1000;
        const uint PAGE_READWRITE = 4;




        private void guna2CheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (guna2CheckBox1.Checked)
            {

                string processName = "HD-Player";
                string dllResourceName = "ROYALREGEDIT.Properties.lc.dll";
                string tempDllPath = Path.Combine(Path.GetTempPath(), "lc.dll");
                admin(dllResourceName, tempDllPath); ;
                Process[] targetProcesses = Process.GetProcessesByName(processName);
                if (targetProcesses.Length == 0)
                {
                    Console.WriteLine($"Waiting for {processName}.exe...");
                }
                if (targetProcesses.Length == 0)
                {
                    



                }
                else
                {
                    Process targetProcess = targetProcesses[0];
                    IntPtr hProcess = OpenProcess(PROCESS_CREATE_THREAD | PROCESS_QUERY_INFORMATION | PROCESS_VM_OPERATION | PROCESS_VM_WRITE | PROCESS_VM_READ, false, targetProcess.Id);
                    IntPtr loadLibraryAddr = GetProcAddress(GetModuleHandle("kernel32.dll"), "LoadLibraryA");
                    IntPtr allocMemAddress = VirtualAllocEx(hProcess, IntPtr.Zero, (IntPtr)tempDllPath.Length, MEM_COMMIT, PAGE_READWRITE);
                    IntPtr bytesWritten;
                    WriteProcessMemory(hProcess, allocMemAddress, System.Text.Encoding.ASCII.GetBytes(tempDllPath), (uint)tempDllPath.Length, out bytesWritten);
                    CreateRemoteThread(hProcess, IntPtr.Zero, IntPtr.Zero, loadLibraryAddr, allocMemAddress, 0, IntPtr.Zero);
                }

                sound();
            }
            else
            {

            }
        }

        





        public class Snowflake
        {
            public int X { get; set; }
            public int Y { get; set; }
            public int Size { get; set; }
            public int Speed { get; set; }
        }

        private void guna2CheckBox17_CheckedChanged(object sender, EventArgs e)
        {
            if (guna2CheckBox17.Checked)
            {
                string string_0;

                Int32 proc = Process.GetProcessesByName("HD-Player")[0].Id;
                  

                x.OpenProcess(proc);
                var enumerable = x.AoBScan(0x0000000000010000, 0x00007ffffffeffff, t, true, true, string.Empty).Result;
                string_0 = "0x" + enumerable.FirstOrDefault().ToString("X");
                foreach (long num in enumerable)
                {
                    x.WriteMemory(num.ToString("X"), "bytes", tt, string.Empty, null);
                }
                sound();

                 

            }
            else
            {

            }
        }

        private void guna2CheckBox21_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void guna2CheckBox18_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void guna2CheckBox22_CheckedChanged(object sender, EventArgs e)
        {
            if (guna2CheckBox22.Checked)
            {
                

            }
            else
            {

            }
        }

   

        private void guna2CheckBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (guna2CheckBox2.Checked)
            {

                string processName = "HD-Player";
                string dllResourceName = "ROYALREGEDIT.Properties.ChamsRed.dll";
                string tempDllPath = Path.Combine(Path.GetTempPath(), "ChamsRed.dll");
                admin(dllResourceName, tempDllPath); ;
                Process[] targetProcesses = Process.GetProcessesByName(processName);
                if (targetProcesses.Length == 0)
                {
                    Console.WriteLine($"Waiting for {processName}.exe...");
                }
                if (targetProcesses.Length == 0)
                {
                  



                }
                else
                {
                    Process targetProcess = targetProcesses[0];
                    IntPtr hProcess = OpenProcess(PROCESS_CREATE_THREAD | PROCESS_QUERY_INFORMATION | PROCESS_VM_OPERATION | PROCESS_VM_WRITE | PROCESS_VM_READ, false, targetProcess.Id);
                    IntPtr loadLibraryAddr = GetProcAddress(GetModuleHandle("kernel32.dll"), "LoadLibraryA");
                    IntPtr allocMemAddress = VirtualAllocEx(hProcess, IntPtr.Zero, (IntPtr)tempDllPath.Length, MEM_COMMIT, PAGE_READWRITE);
                    IntPtr bytesWritten;
                    WriteProcessMemory(hProcess, allocMemAddress, System.Text.Encoding.ASCII.GetBytes(tempDllPath), (uint)tempDllPath.Length, out bytesWritten);
                    CreateRemoteThread(hProcess, IntPtr.Zero, IntPtr.Zero, loadLibraryAddr, allocMemAddress, 0, IntPtr.Zero);
                }

                sound();
            }
            else
            {

            }
        }

        private void guna2CheckBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (guna2CheckBox3.Checked)
            {

                string processName = "HD-Player";
                string dllResourceName = "ROYALREGEDIT.Properties.Red_Antena.dll";
                string tempDllPath = Path.Combine(Path.GetTempPath(), "Red_Antena.dll");
                admin(dllResourceName, tempDllPath); ;
                Process[] targetProcesses = Process.GetProcessesByName(processName);
                if (targetProcesses.Length == 0)
                {
                    Console.WriteLine($"Waiting for {processName}.exe...");
                }
                if (targetProcesses.Length == 0)
                {




                }
                else
                {
                    Process targetProcess = targetProcesses[0];
                    IntPtr hProcess = OpenProcess(PROCESS_CREATE_THREAD | PROCESS_QUERY_INFORMATION | PROCESS_VM_OPERATION | PROCESS_VM_WRITE | PROCESS_VM_READ, false, targetProcess.Id);
                    IntPtr loadLibraryAddr = GetProcAddress(GetModuleHandle("kernel32.dll"), "LoadLibraryA");
                    IntPtr allocMemAddress = VirtualAllocEx(hProcess, IntPtr.Zero, (IntPtr)tempDllPath.Length, MEM_COMMIT, PAGE_READWRITE);
                    IntPtr bytesWritten;
                    WriteProcessMemory(hProcess, allocMemAddress, System.Text.Encoding.ASCII.GetBytes(tempDllPath), (uint)tempDllPath.Length, out bytesWritten);
                    CreateRemoteThread(hProcess, IntPtr.Zero, IntPtr.Zero, loadLibraryAddr, allocMemAddress, 0, IntPtr.Zero);
                }

                sound();
            }
            else
            {

            }
        }

        private void guna2CheckBox5_CheckedChanged(object sender, EventArgs e)
        {
            if (guna2CheckBox5.Checked)
            {

                string processName = "HD-Player";
                string dllResourceName = "ROYALREGEDIT.Properties.boxrgb_1.dll";
                string tempDllPath = Path.Combine(Path.GetTempPath(), "boxrgb_1.dll");
                admin(dllResourceName, tempDllPath); ;
                Process[] targetProcesses = Process.GetProcessesByName(processName);
                if (targetProcesses.Length == 0)
                {
                    Console.WriteLine($"Waiting for {processName}.exe...");
                }
                if (targetProcesses.Length == 0)
                {




                }
                else
                {
                    Process targetProcess = targetProcesses[0];
                    IntPtr hProcess = OpenProcess(PROCESS_CREATE_THREAD | PROCESS_QUERY_INFORMATION | PROCESS_VM_OPERATION | PROCESS_VM_WRITE | PROCESS_VM_READ, false, targetProcess.Id);
                    IntPtr loadLibraryAddr = GetProcAddress(GetModuleHandle("kernel32.dll"), "LoadLibraryA");
                    IntPtr allocMemAddress = VirtualAllocEx(hProcess, IntPtr.Zero, (IntPtr)tempDllPath.Length, MEM_COMMIT, PAGE_READWRITE);
                    IntPtr bytesWritten;
                    WriteProcessMemory(hProcess, allocMemAddress, System.Text.Encoding.ASCII.GetBytes(tempDllPath), (uint)tempDllPath.Length, out bytesWritten);
                    CreateRemoteThread(hProcess, IntPtr.Zero, IntPtr.Zero, loadLibraryAddr, allocMemAddress, 0, IntPtr.Zero);
                }

                sound();
            }
            else
            {

            }
        }





        private void Kh_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.LShiftKey || e.KeyCode == Keys.RShiftKey) shift = false;

        }
        private async void Kh_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F4)
            {
               

            }


            if (e.KeyCode == Keys.F3)
            {
               

            }

        }


















    }

}

