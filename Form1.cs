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

namespace CalculateLeftTime
{
    public partial class Form1 : Form
    {
        public static HubConnection SignalRConn;
        private static int WM_QUERYENDSESSION = 0x11;

        public int tenantId,userId;
        private System.Windows.Forms.Timer timer;
        public string accessToken = "";
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
        private void Form1_Load(object sender, EventArgs e)
        {
            this.Hide();
            this.Visible = false;
            this.Opacity = 0;
            btnReadFile_Click();
            this.ShowInTaskbar = false;
            this.ShowIcon = false;
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

                    // Display the content in a MessageBox (you can use any other way to display it)
                   userId = (int)jsonObject["userId"];
                   tenantId = (int)jsonObject["tenantId"];
                   accessToken = (string)jsonObject["accessToken"];
                    Console.Write($" userid {userId}");
                    System.Diagnostics.Debug.WriteLine($" userid {userId}");
                    HideApp();
                    ConnectSignalRServer();
                    TrigerSignalRServerUserStatus("SignalRAgentUserStatus");
                }
                else
                {
                    MessageBox.Show("File not found.", "File Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error);

                   System.Diagnostics.Debug.WriteLine($" userid not found");
                    HideApp();

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            if (m.Msg == WM_QUERYENDSESSION)
            {
                PostUserStatusToWeb();
                System.Threading.Timer timer = new System.Threading.Timer(TimerCallback, null, 10000, System.Threading.Timeout.Infinite);
                
                //  MessageBox.Show("queryendsession: this is a logoff, shutdown, or reboot");

            }

            base.WndProc(ref m);
        }
        private void TimerCallback(object state)
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
        }

        /*  private void Form1_Load(object sender, EventArgs e)
          {
              Console.WriteLine("Hello");
              this.Hide();
          }*/
        private async Task Form1_FormClosedAsync(object sender, FormClosedEventArgs e)

        {
            PostUserStatusToWeb();
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
        private async void Form1_FormClosingAsync(Object sender, FormClosingEventArgs e)
        {
            PostUserStatusToWeb();
            //In case windows is trying to shut down, don't hold the process up
            if (e.CloseReason == CloseReason.UserClosing)
            {
               
                timer.Start();

                Console.WriteLine("Hello Form1_FormClosing UserClosing");

                string message = "System is shutting down or user is logging off.3 WndProc";

                // Get the path to the Documents folder
                string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

                // Specify the file path in the Documents folder
                string filePath = Path.Combine(documentsPath, "shutdown_log3.txt");

                // Write the message to a text file
                using (StreamWriter writer = new StreamWriter(filePath, true))
                {
                    writer.WriteLine($"{DateTime.Now} - {message}");
                }
                System.Diagnostics.Debug.WriteLine("System is shutting down or user is logging off.3 WndProc");
                /*  MessageBox.Show("queryendsession: this is a logoff, shutdown, or reboot UserClosing");*/
            }
            // Prompt user to save his data

            if (e.CloseReason == CloseReason.WindowsShutDown)
            {
               
                Console.WriteLine("Hello Form1_FormClosing WindowsShutDown");

                string message = "System is shutting down or user is logging off.4 WndProc";

                // Get the path to the Documents folder
                string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

                // Specify the file path in the Documents folder
                string filePath = Path.Combine(documentsPath, "shutdown_log4.txt");

                // Write the message to a text file
                using (StreamWriter writer = new StreamWriter(filePath, true))
                {
                    writer.WriteLine($"{DateTime.Now} - {message}");
                }
                System.Diagnostics.Debug.WriteLine("System is shutting down or user is logging off.4 WndProc");

               
                /* MessageBox.Show("queryendsession: this is a logoff, shutdown, or reboot WindowsShutDown");*/
            }
        }

        private void Form1_Load_1(object sender, EventArgs e)
        {
            this.Hide();
            this.Visible = false;
            this.Opacity = 0;
            btnReadFile_Click();
        }
        private async Task btnPost_Click()
        {
            try
            {
                string apiUrl = "https://uatapi.intimepro.io/api/services/app/AgentHelper/SignalRetOrUpdateUserStatusFromAgentAPI";

                // Create an instance of HttpClient
                using (HttpClient httpClient = new HttpClient())
                {

                    int status = 5;
                    DateTime utcDate = DateTime.UtcNow;

                    // Create JSON data
                    string postData = $"{{\"tenantId\":\"{tenantId}\", \"userId\":\"{userId}\", \"userStatus\":\"{status}\", \"creationTime\":\"{utcDate:s}\"}}";

                    httpClient.DefaultRequestHeaders.Add("Abp.TenantId", tenantId.ToString());  // Replace with your actual tenantId
                    httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
                    // Create a StringContent object with the JSON data
                    StringContent content = new StringContent(postData, Encoding.UTF8, "application/json");

                    try
                    {
                        // Make the POST request
                        HttpResponseMessage response = await httpClient.PostAsync(apiUrl, content);
                      

                        // Check if the request was successful (status code 200-299)
                        if (response.IsSuccessStatusCode)
                        {
                            // Read the response content
                            string responseContent = await response.Content.ReadAsStringAsync();
                            string message = responseContent + "";

                            // Get the path to the Documents folder
                            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

                            // Specify the file path in the Documents folder
                            string filePath = Path.Combine(documentsPath, "IsSuccessStatusCode.txt");

                            // Write the message to a text file
                            using (StreamWriter writer = new StreamWriter(filePath, true))
                            {
                                writer.WriteLine($"{DateTime.Now} - {message} ");
                            }

                            // Process the response as needed
                            /*  MessageBox.Show("POST request successful. Response: " + responseContent);*/
                        }
                        else
                        {
                            // Handle the error response
                            /*  MessageBox.Show("POST request failed. Status Code: " + response.StatusCode);*/
                            string message = "POST request failed. Status Code: " + response.StatusCode;

                            // Get the path to the Documents folder
                            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

                            // Specify the file path in the Documents folder
                            string filePath = Path.Combine(documentsPath, "IsSuccessStatusCodeFalse.txt");

                            // Write the message to a text file
                            using (StreamWriter writer = new StreamWriter(filePath, true))
                            {
                                writer.WriteLine($"{DateTime.Now} - {message}");
                            }
                        }
                    }
                    catch (Exception ex)    
                    {
                        // Handle any exceptions that occur during the request
                        string message = ex.Message + "sec last Exp";

                        // Get the path to the Documents folder
                        string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

                        // Specify the file path in the Documents folder
                        string filePath = Path.Combine(documentsPath, "Exp.txt");

                        // Write the message to a text file
                        using (StreamWriter writer = new StreamWriter(filePath, true))
                        {
                            writer.WriteLine($"{DateTime.Now} - {message}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string message = ex.Message + "last Exp";

                // Get the path to the Documents folder
                string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

                // Specify the file path in the Documents folder
                string filePath = Path.Combine(documentsPath, "shutdown_log4.txt");

                // Write the message to a text file
                using (StreamWriter writer = new StreamWriter(filePath, true))
                {
                    writer.WriteLine($"{DateTime.Now} - {message}");
                }
            }
           
        }


        private static async void ConnectSignalRServer()
        {
            try
            {
                //string signalRUrl = "https://localhost:44301/notifyclient";
                //string signalRUrl = "https://stagapi.intimepros.com/notifyclient";

                //-------------------
               SignalRConn = new HubConnectionBuilder()
                  .WithUrl("https://uatapi.intimepro.io/notifyclient")
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
                };

                try
                {
                    await SignalRConn.StartAsync();
                    System.Diagnostics.Debug.WriteLine("StartAsync RegisterClientId");

                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("RegisterClientId" + ex);
                }
                //-------------------
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("StartAsync RegisterClientId" + ex);
            }
        }

        public void TrigerSignalRServerUserStatus(string SignalREvent)
        {
            try
            {
                #region snippet_ConnectionOn
                if (SignalRConn == null) return;
                SignalRConn.On<UserStatusDto>(SignalREvent, (data) =>
                {
                    Console.WriteLine("gfgggjfjhgjhghjgjhgjhgkg");
                    System.Diagnostics.Debug.WriteLine("gfgggjfjhgjhghjgjhgjhgkg");
                });
                #endregion
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("TrigerSignalRServerUserStatus" + ex);
            }
        }
        public async Task<HttpResponseMessage> PostUserStatusToWeb()
        {
            try
            {
                UserStatusDto userStatusDto = new UserStatusDto();
                userStatusDto.TenantId = tenantId;
                userStatusDto.UserId = userId;
                userStatusDto.UserStatus = 5;

                await SignalRConn.InvokeAsync("SignalRetOrUpdateUserStatusFromAgent", userStatusDto);
                System.Diagnostics.Debug.WriteLine("SignalRetOrUpdateUserStatusFromAgent");
                return new HttpResponseMessage();
              
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("SignalRetOrUpdateUserStatusFromAgent" + ex);
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

