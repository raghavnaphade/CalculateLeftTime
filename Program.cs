namespace CalculateLeftTime
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Form1 form1 = new Form1();
            form1.Hide();
            Application.Run(form1);
            Console.WriteLine("Hello");
            Task.Run(() => GetLocation());
        }
        static void GetLocation()
        {
            Console.WriteLine("LeftTime: ABC");

            // Keep the application running
            while (true)
            {
                Thread.Sleep(1000);
            }
        }
    }
}