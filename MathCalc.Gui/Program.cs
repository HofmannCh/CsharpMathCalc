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
            new Program();
        }

        private Form1 Form;

        public Program()
        {
            Form = new Form1();
            Application.Run(Form);
        }
    }
}