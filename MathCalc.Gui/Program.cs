using System.Diagnostics;
using System.Windows;
using WK.Libraries.HotkeyListenerNS;

namespace MathCalc.Gui
{
    internal class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            
            Form = new Form1();
            var trayIcon = new TrayIcon(Form);

            RegisterHotkey();

            System.Windows.Forms.Application.Run(trayIcon);
        }

        private static Form1 Form;
        private static HotkeyListener hotkeyListener = new HotkeyListener();
        private static void RegisterHotkey()
        {
            var clippingHotkey = new Hotkey(Keys.Control, Keys.Oem102 /* Hotkey "<>/" (key between left shift and Y) */);
            hotkeyListener.Add(clippingHotkey);

            // Suspend listening to hotkeys when the Form is active.
            hotkeyListener.SuspendOn(Form);

            // This event is used to listen to any hotkey presses.
            hotkeyListener.HotkeyPressed += (object sender, HotkeyEventArgs e) =>
            {
                if (e.Hotkey != clippingHotkey)
                    return;

                var txt = e.SourceApplication.Selection;
                Debug.WriteLine("Hotkey pressed with selection:" + txt);

                Form.Show();
                Form.OpenForm(txt);
            };
        }

    }

    internal class TrayIcon : ApplicationContext
    {
        private NotifyIcon trayIcon;
        private Form1 form1;
        public TrayIcon(Form1 form)
        {
            form1 = form;
            var ctxMenu = new ContextMenuStrip();
            var ctxMenuExit = new ToolStripMenuItem("Exit", null, Exit, "Exit");
            ctxMenu.Items.Add(ctxMenuExit);

            // Initialize Tray Icon
            trayIcon = new NotifyIcon()
            {
                Icon = form.Icon,
                ContextMenuStrip = ctxMenu,
                Visible = true,
                Text = "MathCalc"
            };
        }

        void Exit(object? sender, EventArgs e)
        {
            // Hide tray icon, otherwise it will remain shown until user mouses over it
            trayIcon.Visible = false;
            form1.Dispose();
            System.Windows.Forms.Application.Exit();
        }
    }
}