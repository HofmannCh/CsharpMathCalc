//using GlobalHotkeysRX;
using MathCalc.Logic;
using System.Diagnostics;
using WK.Libraries.HotkeyListenerNS;
using static System.Net.Mime.MediaTypeNames;

namespace MathCalc.Gui
{
    public partial class Form1 : Form
    {
        private Calculatur Calculator = new Calculatur();
        //private GlobalHotkeys AppHotkeys;
        internal HotkeyListener hotkeyListener = new HotkeyListener();
        public Form1()
        {
            InitializeComponent();


            WindowState = Debugger.IsAttached ? FormWindowState.Normal : FormWindowState.Minimized;

            //AppHotkeys = new GlobalHotkeys(Handle,
            //      //new Hotkey(Modifier.Control, VKey.C, HotkeyOpenFormC),
            //      new Hotkey(Modifier.Control, VKey.X, HotkeyOpenFormX)
            //    );
            InitHotkey();

            Load += (_, _) => expression_input.Focus();


        }

        private void InitHotkey()
        {
            var clippingHotkey = new Hotkey(Keys.Control, Keys.Y);
            hotkeyListener.Add(clippingHotkey);

            // Suspend listening to hotkeys when the Form is active.
            hotkeyListener.SuspendOn(this);

            // This event is used to listen to any hotkey presses.
            hotkeyListener.HotkeyPressed += (object sender, HotkeyEventArgs e) =>
            {
                if (e.Hotkey != clippingHotkey)
                    return;

                var txt = e.SourceApplication.Selection;
                Debug.WriteLine("Hotkey pressed with selection:" + txt);

                expression_input.Text = txt;

                Activate();
            };

            // This event is used to listen to any updated hotkeys.
            //hotkeyListener.HotkeyUpdated += (object sender, HotkeyListener.HotkeyUpdatedEventArgs e) =>
            //{
            //    // OK
            //};
        }

        //protected override void WndProc(ref Message m)
        //{
        //    if (m.Msg == WinApi.WM_HOTKEY)
        //        AppHotkeys.OnWinFormMesssage(m);  // Replace by your own instance name

        //    // (Optional) Stops any default or additional processing of the message.
        //    m.Result = (IntPtr)0;

        //    base.WndProc(ref m);
        //}

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
                if (history_list.SelectedIndex >= 0)
                {
                    SetExpression(history_list.Items[history_list.SelectedIndex].ToString()!);
                    history_list.SelectedIndex = -1;
                    Calculate();
                }
                else
                {
                    Calculate();
                }
            }
            else if (e.Control && e.KeyCode == Keys.Y)
            {
                Clipboard.SetText(result_output.Text);
                Hide();
            }
            else if (e.KeyCode == Keys.Delete && history_list.SelectedIndex >= 0)
            {
                e.SuppressKeyPress = true;
                var selectedIndex = history_list.SelectedIndex;
                history_list.Items.RemoveAt(history_list.SelectedIndex);
                history_list.SelectedIndex = Math.Min(selectedIndex, history_list.Items.Count - 1);
            }
            else if (e.Control && e.KeyCode == Keys.Delete)
            {
                history_list.Items.Clear();
            }
            else if (e.Control && e.KeyCode == Keys.Tab)
            {
                e.SuppressKeyPress = true;
                if (history_list.Items.Count >= 1)
                    history_list.SelectedIndex = 0;
            }
            else if (e.KeyCode == Keys.Down)
            {
                e.SuppressKeyPress = true;
                if (history_list.SelectedIndex >= 0)
                {
                    if (history_list.Items.Count > history_list.SelectedIndex + 1)
                        history_list.SelectedIndex++;
                }
                else if(!string.IsNullOrWhiteSpace(expression_input.Text))
                {
                    var isEqual = false;
                    if (history_list.Items.Count >= 1)
                    {
                        var lastItem = history_list.Items[history_list.Items.Count - 1];
                        isEqual = StringComparer.OrdinalIgnoreCase.Equals(lastItem.ToString(), expression_input.Text);
                    }

                    if (isEqual)
                    {
                        history_list.SelectedIndex = history_list.Items.Count - 1;
                    }
                    else
                    {
                        var newIndex = history_list.Items.Add(expression_input.Text);
                        history_list.SelectedIndex = newIndex;
                        SetExpression(result_output.Text, true);
                        Calculate();
                    }
                }
            }
            else if (e.KeyCode == Keys.Up)
            {
                e.SuppressKeyPress = true;
                if (history_list.SelectedIndex >= 1)
                {
                    history_list.SelectedIndex--;
                }
                else if (history_list.Items.Count >= 1)
                {
                    history_list.SelectedIndex = history_list.Items.Count - 1;
                }
            }
        }

        private void SetExpression(string expression, bool isExpressionInput = false)
        {
            if (!isExpressionInput || expression_input.SelectionLength <= 0)
            {
                expression_input.Text = expression;
                expression_input.SelectionStart = expression.Length;
                expression_input.SelectionLength = 0;
            }
            else
            {
                var orig = expression_input.Text;
                var sS = expression_input.SelectionStart;
                var sL = expression_input.SelectionLength;
                try
                {
                    var result = Calculator.EvaluateExpression(orig.Substring(sS, sL)).ToString();
                    expression_input.Text = orig.Substring(0, sS) + result + orig.Substring(sS + sL);
                    expression_input.SelectionStart = sS;
                    expression_input.SelectionLength = result.Length;
                }
                catch (Exception)
                {
                    return;
                }
            }
            Calculate();
        }

        private void calculate_button_TextChanged(object sender, EventArgs e)
        {
            history_list.SelectedIndex = -1;
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
            else if ((LatestCClick - DateTime.UtcNow).TotalMilliseconds < 200)
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
