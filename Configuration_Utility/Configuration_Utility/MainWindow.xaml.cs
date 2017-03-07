using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Configuration_Utility.Utils;
using System.Windows.Forms;
using MahApps.Metro;
using TUIO;
using MahApps.Metro.Controls;
using System.Drawing;
using System.Runtime.InteropServices;
using System.IO;
using System.ServiceProcess;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Diagnostics;
using System.Threading;
namespace Configuration_Utility
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SW_HIDE = 0;
        const int SW_SHOW = 5;

        [DllImport("user32.dll")]
        static extern bool EnumDisplayDevices(string lpDevice, uint iDevNum, byte[] lpDisplayDevice, uint dwFlags);
        //private object cursorSync = new object();
        Monitor Monitor1;
        Sensor Sensor1;
        System.Windows.Controls.Button DetectDisplays;
        public double width;
        public double height;

        public MainWindow()
        {

            InitializeComponent();

            MinimizeToTray.Enable(this);
            width = System.Windows.Application.Current.MainWindow.Width;
            height = System.Windows.Application.Current.MainWindow.Height;
            Console.WriteLine("Adding Displays");
            adddisplays();
            Console.WriteLine("Adding Touch sensor for every Display");
            addsensors();

            //uncommment if you don't want to show the console .
            //var handle = GetConsoleWindow();
            //// Hide
            //ShowWindow(handle, SW_HIDE);

            Properties.Settings.Default.NumberOfMonitors = ScreenUtil.Screens.Count();
            Properties.Settings.Default.Save();
            Monitor1.border.BorderBrush = new SolidColorBrush(Colors.SteelBlue);
            try
            {
                Console.WriteLine("0 >" + ScreenUtil.Screens[0].WorkingArea.X + " " + ScreenUtil.Screens[0].WorkingArea.Y + " " + ScreenUtil.Screens[0].WorkingArea.Top + " " + ScreenUtil.Screens[0].WorkingArea.Left + " " + ScreenUtil.Screens[0].WorkingArea.Bottom + " " + ScreenUtil.Screens[0].WorkingArea.Right);
            }
            catch { }   //  Console.WriteLine("1 >" + ScreenUtil.Screens[1].WorkingArea.X + " " + ScreenUtil.Screens[1].WorkingArea.Y + " " + ScreenUtil.Screens[1].WorkingArea.Top + " " + ScreenUtil.Screens[1].WorkingArea.Left + " " + ScreenUtil.Screens[1].WorkingArea.Bottom + " " + ScreenUtil.Screens[1].WorkingArea.Right);

        }

        public void addsensors()
        {
            sensor_stackpanel.Children.Clear();
            Sensor1 = new Sensor();
            Sensor1.UpdateStatusBar += new Sensor.UpdateStatusBarEventHandler(Sensor_UpdateStatusBar);
            Sensor1.service_name = "Tuio-to-Vmulti-Service-1.exe";

            load_values_from_config_files();

            Sensor1.service.Click += new RoutedEventHandler(service_Click1);

            //Get service status and display it in the sesnor area . 
            Sensor1.service_status.Content = get_service_status("Tuio-To-vmulti-Device1");

            sensor_stackpanel.Children.Insert(0, Sensor1);
            Sensor1.Visibility = Visibility.Visible;

        }

        void Sensor_UpdateStatusBar(bool message)
        {
            apply.IsEnabled = message;
            ok.IsEnabled = message;
            Sensor1.service.IsEnabled = message;
        }

        private void install_service(string service_name, string file_name, string port)
        {
            //Process Process_Remove = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = file_name;
            startInfo.Arguments = "remove " + "3";
            startInfo.CreateNoWindow = true;
            startInfo.UseShellExecute = true;

            startInfo.WindowStyle = ProcessWindowStyle.Hidden;

            using (Process exeProcess = Process.Start(startInfo))
            {
                exeProcess.WaitForExit();
            }


            //status = sc.Status.ToString();
            ProcessStartInfo startInfo2 = new ProcessStartInfo();
            startInfo2.FileName = file_name;
            startInfo2.Arguments = "install " + port;
            startInfo2.UseShellExecute = true;
            startInfo2.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo2.CreateNoWindow = true;
            using (Process exeProcess2 = Process.Start(startInfo2))
            {
                exeProcess2.WaitForExit();
            }

            if (System.IO.File.ReadAllText("C:\\Users\\AppData\\TUIO-To-Vmulti\\Data\\service1.txt") == "The Service is Running" && service_name == "Tuio-To-vmulti-Device1")
            {
                start_service("Tuio-To-vmulti-Device1");
            }

            Sensor1.service_status.Content = get_service_status("Tuio-To-vmulti-Device1");
        }

        private void start_feedback(Sensor sensor, string service_name)
        {

            ServiceController sc = new ServiceController();
            sc.ServiceName = service_name;
            string status = "";
            try
            {

                status = sc.Status.ToString();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                status = "Does not exist";
            }

            if (status == "Stopped")
            {
                sensor.removetuioclinet();
                Thread.Sleep(200);
                sensor.addtuioclient();
                sensor.service_status.Content = get_service_status("Tuio-To-vmulti-Device1");
            }

            if (status == "Running")
            {
                sensor.service_status.Content = get_service_status("Tuio-To-vmulti-Device1");
                sensor.removetuioclinet();
                Thread.Sleep(500);
            }


        }

        private void service_Click1(object sender, RoutedEventArgs e)
        {
            
            try
            {
                Console.WriteLine("Toggle Service button clicked");
            }
            catch
            {
            }
            //  Console.WriteLine("test 1"); 
            ServiceController sc = new ServiceController();
            sc.ServiceName = "Tuio-To-vmulti-Device1";
            string status = "";
            // Console.WriteLine("test 2");
            try
            {
                status = sc.Status.ToString();
            }
            catch
            {
                status = "Does not exist";
                Sensor1.service_status.Content = "Does not exist";
                // Console.WriteLine("Does not exist");
            }


            if (status == "Running")
            {
                //    Console.WriteLine("test 3");
                stop_service(sc.ServiceName);
                Thread.Sleep(500);
                Sensor1.service_status.Content = get_service_status(sc.ServiceName);
                Sensor1.addtuioclient();
                //   Console.WriteLine("test 4");

            }
            if (status == "Stopped")
            {
                do_apply_stuff_for_individual_service("Tuio-To-vmulti-Device1", Sensor1);
                // Console.WriteLine("test 5");
                Sensor1.removetuioclinet();
                Thread.Sleep(200);
                // Console.WriteLine("test 6");
                start_service(sc.ServiceName);
                Thread.Sleep(500);
                // Console.WriteLine("test 7");
                Sensor1.service_status.Content = get_service_status(sc.ServiceName);
                //ar 
                Console.WriteLine("test 8");
            }
        }


        public string get_service_status(string service_name)
        {
            ServiceController sc = new ServiceController();
            sc.ServiceName = service_name;
            try
            {
                return "The Service is " + sc.Status.ToString();
            }
            catch
            { return service_name + "Does not exist"; }
        }



        public void adddisplays()
        {
            display_stackpanel.Children.Clear();

                int monWidth = 95;
                int monHeight = 65;

                Monitor1 = new Monitor();
                Monitor1.border.Width = monWidth;
                Monitor1.border.Height = monHeight;
                Monitor1.text.Content = "1";
                //Monitor1.device_name.Content = "1";
                // Console.WriteLine("Primary" + ScreenUtil.Screens[1].Primary.ToString() + "Workign Area x" + ScreenUtil.Screens[1].WorkingArea.X + "Workign Area y" + ScreenUtil.Screens[1].WorkingArea.Y);
                // EnumDisplayDevices(
                Monitor1.primary.Visibility = Visibility.Visible;
                Monitor1.Active.Content = true;
                Monitor1.MouseDown += new MouseButtonEventHandler(Monitor1_MouseDown);
                display_stackpanel.Children.Add(Monitor1);
             //   Monitor1.Margin = new Thickness(ScreenUtil.Screens[0].WorkingArea.X / 10, ScreenUtil.Screens[0].WorkingArea.Y / 10, 0, 0);

        }



        void Monitor1_MouseDown(object sender, MouseButtonEventArgs e)
        {
            adddisplays();
            Monitor1.border.BorderBrush = new SolidColorBrush(Colors.SteelBlue);
            Sensor1.Visibility = Visibility.Visible;
        }

        private void DetectDisplays_Click(object sender, RoutedEventArgs e)
        {
            adddisplays();
            Monitor1.border.BorderBrush = new SolidColorBrush(Colors.SteelBlue);
            Sensor1.Visibility = Visibility.Visible;
        }

        private void apply_Click(object sender, RoutedEventArgs e)
        {
            do_apply_stuff();
        }

        private void do_apply_stuff()
        {

            System.IO.Directory.CreateDirectory("C:\\Users\\AppData\\TUIO-To-Vmulti\\Data");

            System.IO.File.WriteAllText("C:\\Users\\AppData\\TUIO-To-Vmulti\\Data\\tuioport1.txt", Sensor1.tuio_port.Text);
            System.IO.File.WriteAllText("C:\\Users\\AppData\\TUIO-To-Vmulti\\Data\\inverthorizontal1.txt", Sensor1.invert_horizontal.IsChecked.ToString());
            System.IO.File.WriteAllText("C:\\Users\\AppData\\TUIO-To-Vmulti\\Data\\invertverticle1.txt", Sensor1.invert_verticle.IsChecked.ToString());
            System.IO.File.WriteAllText("C:\\Users\\AppData\\TUIO-To-Vmulti\\Data\\swapxy1.txt", Sensor1.swap_xy.IsChecked.ToString());
            System.IO.File.WriteAllText("C:\\Users\\AppData\\TUIO-To-Vmulti\\Data\\xrange_min1.txt", Sensor1.xrange_min.Text.ToString());
            System.IO.File.WriteAllText("C:\\Users\\AppData\\TUIO-To-Vmulti\\Data\\xrange_max1.txt", Sensor1.xrange_max.Text.ToString());
            System.IO.File.WriteAllText("C:\\Users\\AppData\\TUIO-To-Vmulti\\Data\\yrange_min1.txt", Sensor1.yrange_min.Text.ToString());
            System.IO.File.WriteAllText("C:\\Users\\AppData\\TUIO-To-Vmulti\\Data\\yrange_max1.txt", Sensor1.yrange_max.Text.ToString());
            System.IO.File.WriteAllText("C:\\Users\\AppData\\TUIO-To-Vmulti\\Data\\x01.txt", Sensor1.x_offset.Text);
            System.IO.File.WriteAllText("C:\\Users\\AppData\\TUIO-To-Vmulti\\Data\\y01.txt", Sensor1.y_offset.Text);
            System.IO.File.WriteAllText("C:\\Users\\AppData\\TUIO-To-Vmulti\\Data\\service1.txt", get_service_status("Tuio-To-vmulti-Device1"));

            //Installs service if it's not already installed . 
            install_service("Tuio-To-vmulti-Device1", Sensor1.service_name, Sensor1.tuio_port.Text);

            start_feedback(Sensor1, "Tuio-To-vmulti-Device1");

        }

        private void do_apply_stuff_for_individual_service(string service_name,Sensor sensor)
        {

            System.IO.Directory.CreateDirectory("C:\\Users\\AppData\\TUIO-To-Vmulti\\Data");

            System.IO.File.WriteAllText("C:\\Users\\AppData\\TUIO-To-Vmulti\\Data\\tuioport1.txt", Sensor1.tuio_port.Text);
            System.IO.File.WriteAllText("C:\\Users\\AppData\\TUIO-To-Vmulti\\Data\\inverthorizontal1.txt", Sensor1.invert_horizontal.IsChecked.ToString());
            System.IO.File.WriteAllText("C:\\Users\\AppData\\TUIO-To-Vmulti\\Data\\invertverticle1.txt", Sensor1.invert_verticle.IsChecked.ToString());
            System.IO.File.WriteAllText("C:\\Users\\AppData\\TUIO-To-Vmulti\\Data\\x01.txt", Sensor1.x_offset.Text);
            System.IO.File.WriteAllText("C:\\Users\\AppData\\TUIO-To-Vmulti\\Data\\y01.txt", Sensor1.y_offset.Text);
            System.IO.File.WriteAllText("C:\\Users\\AppData\\TUIO-To-Vmulti\\Data\\swapxy1.txt", Sensor1.swap_xy.IsChecked.ToString());
            System.IO.File.WriteAllText("C:\\Users\\AppData\\TUIO-To-Vmulti\\Data\\xrange_min1.txt", Sensor1.xrange_min.Text.ToString());
            System.IO.File.WriteAllText("C:\\Users\\AppData\\TUIO-To-Vmulti\\Data\\xrange_max1.txt", Sensor1.xrange_max.Text.ToString());
            System.IO.File.WriteAllText("C:\\Users\\AppData\\TUIO-To-Vmulti\\Data\\yrange_min1.txt", Sensor1.yrange_min.Text.ToString());
            System.IO.File.WriteAllText("C:\\Users\\AppData\\TUIO-To-Vmulti\\Data\\yrange_max1.txt", Sensor1.yrange_max.Text.ToString());
            //Installs service if it's not already installed . 
            install_service(service_name, sensor.service_name, sensor.tuio_port.Text);
            

            start_feedback(sensor, service_name);

            System.IO.File.WriteAllText("C:\\Users\\AppData\\TUIO-To-Vmulti\\Data\\service1.txt", get_service_status("Tuio-To-vmulti-Device1"));

        }

        private void load_values_from_config_files()
        {
            if (System.IO.File.Exists("C:\\Users\\AppData\\TUIO-To-Vmulti\\Data\\tuioport1.txt") == true)
            {
                Sensor1.tuio_port.Text = System.IO.File.ReadAllText("C:\\Users\\AppData\\TUIO-To-Vmulti\\Data\\tuioport1.txt");
                if (System.IO.File.Exists("C:\\Users\\AppData\\TUIO-To-Vmulti\\Data\\xrange_min1.txt") == true)
                {
                    Sensor1.xrange_min.Text = System.IO.File.ReadAllText("C:\\Users\\AppData\\TUIO-To-Vmulti\\Data\\xrange_min1.txt");

                    Sensor1.xrange_max.Text = System.IO.File.ReadAllText("C:\\Users\\AppData\\TUIO-To-Vmulti\\Data\\xrange_max1.txt");

                    Sensor1.yrange_min.Text = System.IO.File.ReadAllText("C:\\Users\\AppData\\TUIO-To-Vmulti\\Data\\yrange_min1.txt");

                    Sensor1.yrange_max.Text = System.IO.File.ReadAllText("C:\\Users\\AppData\\TUIO-To-Vmulti\\Data\\yrange_max1.txt");

                    if (System.IO.File.ReadAllText("C:\\Users\\AppData\\TUIO-To-Vmulti\\Data\\swapxy1.txt") == "True")
                        Sensor1.swap_xy.IsChecked = true;
                }
                if (System.IO.File.ReadAllText("C:\\Users\\AppData\\TUIO-To-Vmulti\\Data\\inverthorizontal1.txt") == "True")
                    Sensor1.invert_horizontal.IsChecked = true;

                if (System.IO.File.ReadAllText("C:\\Users\\AppData\\TUIO-To-Vmulti\\Data\\invertverticle1.txt") == "True")
                    Sensor1.invert_verticle.IsChecked = true;

                Sensor1.x_offset.Text = System.IO.File.ReadAllText("C:\\Users\\AppData\\TUIO-To-Vmulti\\Data\\x01.txt");
                Sensor1.y_offset.Text = System.IO.File.ReadAllText("C:\\Users\\AppData\\TUIO-To-Vmulti\\Data\\y01.txt");
                System.IO.File.WriteAllText("C:\\Users\\AppData\\TUIO-To-Vmulti\\Data\\service1.txt", get_service_status("Tuio-To-vmulti-Device1"));
            }

            install_service("Tuio-To-vmulti-Device1", Sensor1.service_name, Sensor1.tuio_port.Text);

            start_feedback(Sensor1, "Tuio-To-vmulti-Device1");

        }

        public string start_service(string service_name)
        {

            ServiceController sc = new ServiceController();
            sc.ServiceName = service_name;
            if (sc.Status == ServiceControllerStatus.Stopped)
            {
                // Start the service if the current status is stopped.
                try
                {
                    Console.WriteLine("Starting the " + service_name + " service...");
                }
                catch { }
                try
                {
                    // Start the service, and wait until its status is "Running".
                    sc.Start();
                    sc.WaitForStatus(ServiceControllerStatus.Running);
                    Thread.Sleep(500);
                    // Display the current service status.
                    try
                    {
                        Console.WriteLine("The " + service_name + " service status is now set to {0}.",
                                           sc.Status.ToString());
                    }
                    catch { }
                    return "The " + service_name + " service status is now set to " + sc.Status.ToString();
                }
                catch (InvalidOperationException)
                {
                    try
                    {
                        Console.WriteLine("Could not start the " + service_name + " service.");
                    }
                    catch { }
                    return "Could not start the " + service_name + " service.";
                }
                Thread.Sleep(500);
            }
            return "Service is already running";

        }

        public string stop_service(string service_name)
        {

            ServiceController sc = new ServiceController();
            sc.ServiceName = service_name;
            if (sc.Status == ServiceControllerStatus.Running)
            {
                // Start the service if the current status is stopped.
                try
                {
                    Console.WriteLine("Stopping the " + service_name + " service...");
                }
                catch { }
                try
                {
                    // Start the service, and wait until its status is "Running".
                    sc.Stop();
                    sc.WaitForStatus(ServiceControllerStatus.Stopped);
                    Thread.Sleep(500);
                    // Display the current service status.
                    try
                    {
                        Console.WriteLine("The " + service_name + " service status is now set to {0}.",
                                           sc.Status.ToString());
                    }
                    catch { }
                    return "The " + service_name + " service status is now set to " + sc.Status.ToString();
                }
                catch (InvalidOperationException)
                {
                    try
                    {
                        Console.WriteLine("Could not Stop the " + service_name + " service.");
                    }
                    catch { }
                    return "Could not Stop the " + service_name + " service.";
                }
            }
            return "Service is already Stopped";
            Thread.Sleep(500);
        }

        private void ok_Click(object sender, RoutedEventArgs e)
        {
            do_apply_stuff();
            Process.GetCurrentProcess().Kill();

        }

        private void cancel_Click(object sender, RoutedEventArgs e)
        {
            Process.GetCurrentProcess().Kill(); ;
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Process.GetCurrentProcess().Kill();
        }

        private void assign_displays_Click(object sender, RoutedEventArgs e)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();

            if (Utils.Wow.Is64BitOperatingSystem)
            {
                startInfo.FileName = "C:\\Windows\\Sysnative\\MultiDigiMon.exe";
            }
            else
            {
                startInfo.FileName = "C:\\Windows\\System32\\MultiDigiMon.exe";
            }

            startInfo.Arguments = " -touch";
            startInfo.CreateNoWindow = true;
            startInfo.UseShellExecute = true;
            startInfo.Verb = "runas";

            startInfo.WindowStyle = ProcessWindowStyle.Normal;
            using (Process exeProcess = Process.Start(startInfo))
            {
                exeProcess.WaitForExit();
            }

        }
    }
}