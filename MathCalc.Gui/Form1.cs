using GlobalHotkeysRX;
using MathCalc.Logic;

namespace MathCalc.Gui
{
    public partial class Form1 : Form
    {
        private Calculatur Calculator = new Calculatur();
        private GlobalHotkeys AppHotkeys;
        public Form1()
        {
            InitializeComponent();

            AppHotkeys = new GlobalHotkeys(Handle,
                  //new Hotkey(Modifier.Control, VKey.C, HotkeyOpenFormC),
                  new Hotkey(Modifier.Control, VKey.X, HotkeyOpenFormX)
                );

            Load += (_, _) => expression_input.Focus();
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WinApi.WM_HOTKEY)
                AppHotkeys.OnWinFormMesssage(m);  // Replace by your own instance name

            // (Optional) Stops any default or additional processing of the message.
            m.Result = (IntPtr)0;

            base.WndProc(ref m);
        }

        private void Calculate()
        {
            string result;
            try
            {
                result = Calculator.EvaluateExpression(expression_input.Text).ToString();
            }
            catch (Exception ex)
            {
                result = ex.GetType().Name + ": " + ex.Message;
            }
            result_output.Text = result;
        }

        private void calculate_button_Click(object sender, EventArgs e)
        {
            Calculate();
        }

        private void calculate_button_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Calculate();
            }
        }

        private void calculate_button_TextChanged(object sender, EventArgs e)
        {
            Calculate();
        }

        private static DateTime LatestCClick = DateTime.MinValue;
        internal void HotkeyOpenFormC()
        {
            LatestCClick = DateTime.UtcNow;
        }
        internal void HotkeyOpenFormX()
        {
            if (Focused && expression_input.Focused)
            {
                if (!string.IsNullOrWhiteSpace(result_output.Text))
                    Clipboard.SetText(result_output.Text, TextDataFormat.Text);
                Hide();
                SendToBack();
            }
            else if((LatestCClick - DateTime.UtcNow).TotalMilliseconds < 200)
            {
                //var text = Win32.GetSelectedTextFromActiveWindow();
                //Win32.CopyTextFromActiveWindow();
                var txt = Clipboard.GetText();
                expression_input.Text = txt;
                Show();
                BringToFront();
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            //if (e.Control && e.KeyCode == Keys.X)
            //{
            //    Clipboard.SetText(result_output.Text, TextDataFormat.Text);
            //    Hide();
            //}
        }
    }
}
