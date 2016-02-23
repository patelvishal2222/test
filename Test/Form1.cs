using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Deployment.Application;
using System.Security.Permissions;
using System.Security.Policy;
using System.Security;
using System.Threading;
using System.Diagnostics;
using System.DirectoryServices;
using Microsoft.Win32;
using System.Security.Principal;
using System.Net;
namespace Test
{
    public partial class Form1: Form
    {
        public Form1()
    {
      InitializeComponent();
    }
 
    private void LoginForm_Load(object sender, EventArgs e)
    {
      
    }
 
    private void CheckForUpdate()
    {
      var appDeploy = ApplicationDeployment.CurrentDeployment;
 
      //Check for update
      var update = appDeploy.CheckForDetailedUpdate();
      if(appDeploy.CheckForUpdate())
      {
        MessageBox.Show(@"The system needs to be updated to version: " + update.AvailableVersion, @"Update",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
        var deployForm = new AppDeployPopup {AppDeploy = appDeploy, IsUpdateAvailable = true};
        deployForm.ShowDialog();
 
        if (deployForm.ShouldAppRestart)
        {
          Visible = false;
          Close();  
          Application.Restart(); 
        }
      }
    }

    private void Form1_Load(object sender, EventArgs e)
    {
        SystemEvents.SessionSwitch += new SessionSwitchEventHandler(SystemEvents_SessionSwitch);
    }

        void SystemEvents_SessionSwitch(object sender, SessionSwitchEventArgs e)
  {

    string messageInfo = string.Empty;
            
         System.Text.StringBuilder    objStringBuilder=  new  System.Text.StringBuilder();
         String ipAddress=string.Empty ;
       foreach( IPAddress ip   in Dns.GetHostAddresses(Dns.GetHostName()) )
            {
                if( ip.AddressFamily ==System.Net.Sockets.AddressFamily.InterNetwork )
            {
                    objStringBuilder.AppendLine(String.Format("{0} : {1}", ip.AddressFamily.ToString(), ip.ToString()));
                    ipAddress=ip.Address.ToString();  
            }
            }

       string DATA =string.Empty ;
    switch (e.Reason)
    {
      case SessionSwitchReason.SessionLock:
            DATA = "{UserName:" + Environment.UserName + ",ComputerName:" + Environment.UserDomainName + ",IPAddress:" + ipAddress.ToString() + ",Status:SessionLock,Time:"+DateTime.Now.ToString("dd:MM:yyy  HH:mm:ss")+ " }"+Environment.NewLine  ;
        messageInfo = "Your computer Locked at {0:yyyy-MM-dd HH:mm:ss} by {1}.";
        break;
      case SessionSwitchReason.SessionLogoff:
        DATA = "{UserName:" + Environment.UserName + ",ComputerName:" + Environment.UserDomainName + ",IPAddress:" + ipAddress.ToString() + ",Status:Lock Off,Time:"+DateTime.Now.ToString("dd:MM:yyy  HH:mm:ss") +"  }" + Environment.NewLine;
        messageInfo = "Your computer Logoff at {0:yyyy-MM-dd HH:mm:ss} by {1}.";
        break;
      case SessionSwitchReason.SessionLogon:
        DATA = "{UserName:" + Environment.UserName + ",ComputerName:" + Environment.UserDomainName + ",IPAddress:" + ipAddress.ToString() + ",Status:Lock on,Time:" + DateTime.Now.ToString("dd:MM:yyy  HH:mm:ss") + "  }" + Environment.NewLine;
        messageInfo = "Your computer Logon at {0:yyyy-MM-dd HH:mm:ss} by {1}.";
        break;
      case SessionSwitchReason.SessionUnlock:
        DATA = "{UserName:" + Environment.UserName + ",ComputerName:" + Environment.UserDomainName + ",IPAddress:" + ipAddress.ToString() + ",Status:SessionUnlock,Time:" + DateTime.Now.ToString("dd:MM:yyy  HH:mm:ss") + "  }" + Environment.NewLine;
        messageInfo = "Your computer Unlock at {0:yyyy-MM-dd HH:mm:ss} by {1}.";
        break;
      case SessionSwitchReason.ConsoleConnect:
        messageInfo = "A session has been connected from the console at {0:yyyy-MM-dd HH:mm:ss} by {1}.";
        break;
      case SessionSwitchReason.ConsoleDisconnect:
        messageInfo = "A session has been disconnected from the console at {0:yyyy-MM-dd HH:mm:ss} by {1}.";
        break;
      case SessionSwitchReason.RemoteConnect:
        messageInfo = "A session has been connected from a remote connection at {0:yyyy-MM-dd HH:mm:ss} by {1}.";
        break;
      case SessionSwitchReason.RemoteDisconnect:
        messageInfo = "A session has been disconnected from a remote connection at {0:yyyy-MM-dd HH:mm:ss} by {1}.";
        break;
      case SessionSwitchReason.SessionRemoteControl:
        messageInfo = "A session has changed its status to or from remote controlled mode at {0:yyyy-MM-dd HH:mm:ss} by {1}.";
        break;
      default:
        break;
    }
            System.IO.File.AppendAllText("D:\\Status.txt",DATA);   
    if (messageInfo != string.Empty)
    {
      // WriteInfoToLog(string.Format(messageInfo, DateTime.Now, GetLoginUserName()));
      Console.WriteLine(string.Format(messageInfo, DateTime.Now, GetLoginUserName()));
    }
  }

  /// <summary>
  /// Get the current login user.
  /// </summary>
  public string GetLoginUserName()
  {
    return WindowsIdentity.GetCurrent().Name.ToString();      
  }
 
    private void button1_Click(object sender, EventArgs e)
    {
        try
        {
            
            ApplicationDeployment deploy = ApplicationDeployment.CurrentDeployment;
            MessageBox.Show(deploy.CurrentVersion.ToString());
            UpdateCheckInfo update = deploy.CheckForDetailedUpdate();
            
            if (deploy.CheckForUpdate())
            {
                MessageBox.Show("You can update to version: " + update.AvailableVersion.ToString());
                deploy.Update();
                
                Application.Restart();
                
            }
        }
        catch (Exception e1)
        {
            MessageBox.Show(e1.ToString());   
        }
    }

    private void button2_Click(object sender, EventArgs e)
    {
        System.IO.FileInfo file = new System.IO.FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location);
        if (System.IO.File.Exists(file.DirectoryName + "\\" + file.Name.Replace(file.Extension, "") + "-1" + file.Extension))
        {
            System.IO.File.Delete(file.DirectoryName + "\\" + file.Name.Replace(file.Extension, "") + "-1" + file.Extension);
        }
        System.IO.File.Move(file.FullName, file.DirectoryName + "\\" + file.Name.Replace(file.Extension, "") + "-1" + file.Extension);
        System.IO.File.Copy("\\\\gap-pc-6\\Debug\\"+file.Name  ,file.FullName);
        Application.Restart();
        /*
        Application.DoEvents();

        ProcessStartInfo Info = new ProcessStartInfo();
        Info.Arguments = "/C choice /C Y /N /D Y /T 3 & Del " +
                       Application.ExecutablePath;
        Info.WindowStyle = ProcessWindowStyle.Hidden;
        Info.CreateNoWindow = true;
        Info.FileName = "cmd.exe";
        Process.Start(Info);
        */
         //file = new System.IO.FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location);
        if (System.IO.File.Exists(file.DirectoryName + "\\" + file.Name.Replace(file.Extension, "") + "-1" + file.Extension))

        {
            //System.IO.File.Delete(file.DirectoryName + "\\" + file.Name.Replace(file.Extension, "") + "-1" + file.Extension);
            string FileName=file.DirectoryName + "\\" + file.Name.Replace(file.Extension, "") + "-1" + file.Extension;
            //Process.Start("cmd.exe", "/C ping 1.1.1.1 -n 1 -w 3000 > Nul & Del " + FileName );


            var info = new ProcessStartInfo("cmd.exe", "/C ping 1.1.1.1 -n 1 -w 3000 > Nul & Del \"" + FileName + "\"");
            info.WindowStyle = ProcessWindowStyle.Hidden;
            Process.Start(info).Dispose();
            //Application.Exit();
        }
    }

    private void button3_Click(object sender, EventArgs e)
    {
        System.IO.FileInfo file = new System.IO.FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location);
        if (System.IO.File.Exists(file.DirectoryName + "\\" + file.Name.Replace(file.Extension, "") + "-1" + file.Extension))
        {
            System.IO.File.Delete(file.DirectoryName + "\\" + file.Name.Replace(file.Extension, "") + "-1" + file.Extension);
        }
    }

    private void button4_Click(object sender, EventArgs e)
    {
       
    }

    private void button5_Click(object sender, EventArgs e)
    {
        

        DirectoryEntry dirs = new DirectoryEntry("WinNT://" + Environment.MachineName);
        foreach (DirectoryEntry de in dirs.Children)
        {
            if (de.SchemaClassName == "User")
            {
                Console.WriteLine(de.Name);
                if (de.Properties["lastlogin"].Value != null)
                {
                    Console.WriteLine(de.Properties["lastlogin"].Value.ToString());
                }
                if (de.Properties["lastlogoff"].Value != null)
                {
                    Console.WriteLine(de.Properties["lastlogoff"].Value.ToString());
                }
            }
        }
    }
  }

    public partial class AppDeployPopup : Form
    {
        public AppDeployPopup()
        {
            //InitializeComponent();
        }

        #region Properties

        public ApplicationDeployment AppDeploy { get; set; }

        public bool IsUpdateAvailable { get; set; }

        public bool ShouldAppRestart { get; set; }

        #endregion

        private void AppDeployPopup_Load(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;

            try
            {
                if (IsUpdateAvailable) BeginUpdate();
            }
            catch (Exception exception)
            {
                MessageBox.Show(@"Unexpected error during updating this software. \n" + exception.Message);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void BeginUpdate()
        {

            AppDeploy.UpdateCompleted += new AsyncCompletedEventHandler(AppDeploy_UpdateCompleted);

            AppDeploy.UpdateProgressChanged += new DeploymentProgressChangedEventHandler(AppDeploy_UpdateProgressChanged);

            AppDeploy.UpdateAsync();
        }

        private void AppDeploy_UpdateCompleted(object sender, AsyncCompletedEventArgs e)
        {
            ShouldAppRestart = false;
            if (e.Cancelled)
            {
                MessageBox.Show(@"The update of the application's latest version was cancelled.", @"Warning",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if (e.Error != null)
            {
                MessageBox.Show(@"There was an error during updating. Please contact your system administrator.", @"Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                ShouldAppRestart = true;
            }
            Close();
        }

        private void AppDeploy_UpdateProgressChanged(object sender, DeploymentProgressChangedEventArgs e)
        {
            var progressText = String.Format("Downloading: {0:D}K out of {1:D}K downloaded - {2:D}% complete", e.BytesCompleted / 1024,
                                                e.BytesTotal / 1024, e.ProgressPercentage);
          //  lblDownloadStatus.Text = progressText;
            //pgbDownload.Value = e.ProgressPercentage;
        }
    }
}
