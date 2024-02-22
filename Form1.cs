using System;
using Microsoft.VisualBasic.ApplicationServices;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.AspNet.SignalR.Messaging;
using System.Drawing;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNet.SignalR.Hosting;

namespace CalculateLeftTime
{
    public partial class Form1 : Form
    {
        public static HubConnection SignalRConn;
        private static int WM_QUERYENDSESSION = 0x11;

        public static int tenantId,userId;
        private static System.Windows.Forms.Timer timer;
        public static string accessToken = "";
        public Form1()
        {
            InitializeComponent();
            this.FormClosing += Form1_FormClosingAsync;
            this.Activated += new System.EventHandler(this.Form1_Activated);
          

           /* PostUserStatusToWeb();*/

        }
        private void Form1_Activated(object sender, EventArgs e)
        {
            /*this.Hide()*/
            ;
        }
  

        private void btnReadFile_Click()
        {
            // Get the path of the application's startup folder
            string appFolderPath = Application.StartupPath;

            // Specify the filename you want to read (change it accordingly)
            string filename = "userDetails.dat";

            // Combine the folder path and filename to get the full file path
            string filePath = Path.Combine(appFolderPath, filename);
            Console.WriteLine(filePath);

            try
            {
                // Check if the file exists
                if (File.Exists(filePath))
                {
                    // Read the contents of the file
                    string fileContent = File.ReadAllText(filePath);
                    JObject jsonObject = JObject.Parse(fileContent);
                    string userIdString = jsonObject["userId"]?.ToString();
                    string tenantIdString = jsonObject["tenantId"]?.ToString();
                    if (!string.IsNullOrEmpty(userIdString) && !string.IsNullOrEmpty(tenantIdString))
                    {
                        userId = int.Parse(userIdString);
                        tenantId = int.Parse(tenantIdString);
                        accessToken = jsonObject["accessToken"]?.ToString();
                        Console.Write($" userid {userId}");
                        System.Diagnostics.Debug.WriteLine($" userid {userId}");
                        ConnectSignalRServer();
                      // TrigerSignalRServerUserStatus("SignalRAgentUserStatus");
                    } else
                    {
                        Console.Write($" userid/tenantid null");
                        System.Diagnostics.Debug.WriteLine($"userid/tenantid null");
                        AddLogs(" userid/tenantid null");
                    }
                   

                    // Process accessToken
                 
                    HideApp();
                  
                }
                else
                {
                  //  MessageBox.Show("File not found.", "File Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    System.Diagnostics.Debug.WriteLine($" userid not found");
                    HideApp();
                    AddLogs(" userid/tenantid null");

                }
            }
            catch (Exception ex)
            {
                /*MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);*/
            }
        }

        private void HideApp()
        {
            this.Hide();
            this.Visible = false;
            this.Opacity = 0;
            this.ShowInTaskbar = false;
            this.ShowIcon = false;
        }
      /*  protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            if (m.Msg == WM_QUERYENDSESSION)
            {
                PostUserStatusToWeb();
                System.Threading.Timer timer = new System.Threading.Timer(TimerCallback, null, 10000, System.Threading.Timeout.Infinite);
                
                //  MessageBox.Show("queryendsession: this is a logoff, shutdown, or reboot");

            }

            base.WndProc(ref m);
        }*/
       /* private void TimerCallback(object state)
        {
            // Code to execute after 10 seconds
            // This can include any additional logic or actions you want to perform
            // For example, you can display a message, close the application, etc.

            // Make sure to use Invoke if you need to update UI elements from a different thread
            this.Invoke((MethodInvoker)delegate {
                // Your code to be executed after the delay
                System.Diagnostics.Debug.WriteLine("Code executed after 10 seconds.");
                string message = "System is shutting down or user is logging off.1 WndProc";

                // Get the path to the Documents folder
                string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

                // Specify the file path in the Documents folder
                string filePath = Path.Combine(documentsPath, "shutdown_log1.txt");

                // Write the message to a text file
                using (StreamWriter writer = new StreamWriter(filePath, true))
                {
                    writer.WriteLine($"{DateTime.Now} - {message}");
                }
                System.Diagnostics.Debug.WriteLine("System is shutting down or user is logging off.1 WndProc");
            });
        }*/

        /*  private void Form1_Load(object sender, EventArgs e)
          {
              Console.WriteLine("Hello");
              this.Hide();
          }*/
        private static async Task Form1_FormClosedAsync(object sender, FormClosedEventArgs e)

        {
            PostUserStatusToWeb(5);
            Console.WriteLine("Hello Form1_FormClosed");

            string message = "System is shutting down or user is logging off.2 WndProc";

            // Get the path to the Documents folder
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            // Specify the file path in the Documents folder
            string filePath = Path.Combine(documentsPath, "shutdown_log2.txt");

            // Write the message to a text file
            using (StreamWriter writer = new StreamWriter(filePath, true))
            {
                writer.WriteLine($"{DateTime.Now} - {message}");
            }
            // MessageBox.Show("queryendsession: this is a logoff, shutdown, or reboot");
            System.Diagnostics.Debug.WriteLine("System is shutting down or user is logging off.2 WndProc");
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            // Code to be executed when the timer ticks
            //  MessageBox.Show("Timeout reached!");
            System.Diagnostics.Debug.WriteLine("Timeout reached!");
            // Stop the timer
            timer.Stop();
        }
        private static  async void Form1_FormClosingAsync(Object sender, FormClosingEventArgs e)
        {
            PostUserStatusToWeb(5);
            //In case windows is trying to shut down, don't hold the process up
            if (e.CloseReason == CloseReason.UserClosing)
            {
               
                timer.Start();

                /*  MessageBox.Show("queryendsession: this is a logoff, shutdown, or reboot UserClosing");*/
                AddLogs("Hello Form1_FormClosing UserClosing");
            }
            // Prompt user to save his data

            if (e.CloseReason == CloseReason.WindowsShutDown)
            {
               
                Console.WriteLine("Hello Form1_FormClosing WindowsShutDown");

                string message = "System is shutting down or user is logging off.4 Form1_FormClosingAsync";
                AddLogs(message);
                // Get the path to the Documents folder
               
                System.Diagnostics.Debug.WriteLine("System is shutting down or user is logging off.4 Form1_FormClosingAsync");

               
                /* MessageBox.Show("queryendsession: this is a logoff, shutdown, or reboot WindowsShutDown");*/
            }
        }

        private void Form1_Load_1(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Form1_Load_1");
            this.Hide();
            this.Visible = false;
            this.Opacity = 0;
            btnReadFile_Click();
        }
        

        private static async void ConnectSignalRServer()
        {
            try
            {
                //string signalRUrl = "https://localhost:44301/notifyclient";
                string signalRUrl = "https://stagapi.intimepro.io/notifyclient";
                //string signalRUrl = "https://uatapi.intimepro.io/notifyclient";
                //string signalRUrl = "https://api.intimepro.io/notifyclient",
                //-------------------
                SignalRConn = new HubConnectionBuilder()
                  .WithUrl(signalRUrl)
                  .Build();

                SignalRConn.Closed += async (error) =>
                {
                    await Task.Delay(new Random().Next(0, 5) * 1000);
                    await SignalRConn.StartAsync();
                    var data = new RegisterSignalRDto();
                    data.userId = 45;
                    data.tenantId = (int)1;
                    data.device = "W";
                    await SignalRConn.SendAsync("RegisterClientId", data);
                    System.Diagnostics.Debug.WriteLine("RegisterClientId");
                    AddLogs("RegisterClientId");
                };

                try
                {
                    await SignalRConn.StartAsync();
                    System.Diagnostics.Debug.WriteLine("StartAsync RegisterClientId");
                    AddLogs("StartAsync RegisterClientId");
                    TrigerSignalRServerUserStatus("SignalRAgentUserStatus");
                    PostUserStatusToWeb(4);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("RegisterClientId" + ex);
                    AddLogs("RegisterClientId" + ex);
                }
                //-------------------
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(" RegisterClientId" + ex);
                AddLogs("StartAsync RegisterClientId" + ex);
            }
        }
        public static void AddLogs(string message)
        {
            try
            {

                // Get the path to the Documents folder
                string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

                // Specify the file path in the Documents folder
                string filePath = Path.Combine(documentsPath, "IsSuccessStatusCode.txt");

                // Check if the file exists
                bool fileExists = File.Exists(filePath);

                // Write the message to a text file
                using (StreamWriter writer = new StreamWriter(filePath, true))
                {
                    // If the file already exists and it's not empty, move to the next line
                    if (fileExists && new FileInfo(filePath).Length > 0)
                    {
                        writer.WriteLine();
                    }
                    writer.WriteLine($"{DateTime.Now} - {message}");
                }
            }
            catch (Exception ex)
            {
                string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

                // Specify the file path in the Documents folder
                string filePath = Path.Combine(documentsPath, "IsSuccessStatusCode.txt");

                // Write the message to a text file
                using (StreamWriter writer = new StreamWriter(filePath, true))
                {
                    writer.WriteLine($"{DateTime.Now} - {ex.Message} ");
                }
            }
        }
        public static void TrigerSignalRServerUserStatus(string SignalREvent)
        {
            try
            {
                AddLogs("in TrigerSignalRServerUserStatus");
                #region snippet_ConnectionOn
                if (SignalRConn == null) 
                {

                    AddLogs("SignalRConn null TrigerSignalRServerUserStatus");
                    return;
                }
                SignalRConn.On<UserStatusDto>(SignalREvent, (data) =>
                {
                    Console.WriteLine("gfgggjfjhgjhghjgjhgjhgkg");
                    System.Diagnostics.Debug.WriteLine("gfgggjfjhgjhghjgjhgjhgkg");
                    AddLogs("TrigerSignalRServerUserStatus");
                });
                AddLogs("endregion SignalRConn null TrigerSignalRServerUserStatus");
                #endregion
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("TrigerSignalRServerUserStatus" + ex);
                AddLogs("TrigerSignalRServerUserStatus" + ex);
            }
        }
        public static async Task<HttpResponseMessage> PostUserStatusToWeb(int UserStatus)
        {
            try
            {
                if (tenantId != 0 && userId != 0)
                {
                    UserStatusDto userStatusDto = new UserStatusDto();
                    userStatusDto.TenantId = tenantId;
                    userStatusDto.UserId = userId;
                    userStatusDto.UserStatus = UserStatus;

                    await SignalRConn.InvokeAsync("SignalRetOrUpdateUserStatusFromAgent", userStatusDto);
                    System.Diagnostics.Debug.WriteLine("SignalRetOrUpdateUserStatusFromAgent");
                    AddLogs("Signal R status send");
                    return new HttpResponseMessage();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("SignalRetOrUpdateUserStatusFromAgent" + ex);
                AddLogs("SignalRetOrUpdateUserStatusFromAgent" + ex);
            }
            return null;
        }
    }
    public class RegisterSignalRDto
    {
        public long userId { get; set; }
        public int tenantId { get; set; }
        public string device { get; set; }
    }
    public class UserStatusDto
    {
        public long TenantId { get; set; }
        public long UserId { get; set; }
        public int UserStatus { get; set; }
    }
}

