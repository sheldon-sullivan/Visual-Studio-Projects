using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net.Mail;

namespace SDMS_AppointmentReminder
{
    public partial class AutomatedTextMsgs : Form
    {
        public AutomatedTextMsgs()
        {
            InitializeComponent();
        }

        private string username;
        private string password;
        private string mMailServer;
        private string txtPhoneNumber;
        private string carrier;
        private string smtpPort;
        private int smtpPortVal;
        private string mTo;
        private string mFrom;
        private string mMsg;
        private string mSubject;
        private string interval;

        private void CreateNewAppointment_Load(object sender, EventArgs e)
        {
            lblStatus.Text = string.Empty;
            lblMessageSrvc.Text = string.Empty;

            this.tbTextMessage.MaxLength = 124;

            cmbCarrier.Items.Add("@messaging.sprintpcs.com");
            cmbCarrier.Items.Add("@att.net");
            cmbCarrier.Items.Add("@vtext.com");
        }

        private void sendMessage(object sender, System.Timers.ElapsedEventArgs args)
        {
            username = "sheldon.dot.sullivan@gmail.com";
            password = "Angelia110779";
            smtpPort = Properties.Settings.Default.smtpPort;
            smtpPortVal = Convert.ToInt32(smtpPort);
            txtPhoneNumber = tbTextNum1.Text + tbTextNum2.Text + tbTextNum3.Text;
            carrier = cmbCarrier.SelectedItem.ToString();
            Invoke(new MethodInvoker( delegate() { carrier = cmbCarrier.SelectedItem.ToString();}));
            //this.Invoke(new MethodInvoker(delegate() { text = combobox.Text; }));
            cmbCarrier.BeginInvoke(new MethodInvoker(delegate() { cmbCarrier.Text = cmbCarrier.SelectedItem.ToString(); }));      
            mTo = txtPhoneNumber.Trim() + carrier.Trim();
            mFrom = tbTextSender.Text.Trim();
            mSubject = tbTextSubject.Text.Trim();
            mMailServer = tbMailServer.Text.Trim();
            mMsg = tbTextMessage.Text;

            try
            {
                MailMessage message = new MailMessage(mFrom, mTo, mSubject, mMsg);
                SmtpClient mySmtpClient = new SmtpClient(mMailServer);
                mySmtpClient.Credentials = new System.Net.NetworkCredential(username, password);
                mySmtpClient.Port = (smtpPortVal);
                mySmtpClient.EnableSsl = true;
                mySmtpClient.Send(message);
            }

            catch (FormatException ex)
            {
                MessageBox.Show(ex.StackTrace, ex.Message,
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            catch (SmtpException ex)
            {
                MessageBox.Show(ex.StackTrace, ex.Message,
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.StackTrace, ex.Message,
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            //sendMessage();
        }

        private void tbTextMessage_TextChanged(object sender, EventArgs e)
        {
            lblCharacterCount.Text = (tbTextMessage.MaxLength - tbTextMessage.Text.Length).ToString();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                if (!bgWorker.IsBusy)
                {
                    bgWorker.RunWorkerAsync();
                }
            }

            if (checkBox1.Checked == false)
            {
                if ( scheduleTimer.Enabled == true)
                {
                    scheduleTimer.Stop();
                    lblStatus.Text = "Timer disabled...timer stopped.";
                    lblMessageSrvc.Text = "Message service stopped.";
                    lblMessageSrvc.Update();
                }
            }
        }

        System.Timers.Timer scheduleTimer = new System.Timers.Timer();

        private void bgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            interval = Properties.Settings.Default.timerInterval;
            int intervalValue = Convert.ToInt32(interval);

            scheduleTimer.Elapsed += new System.Timers.ElapsedEventHandler(sendMessage);
            scheduleTimer.Interval = (intervalValue);
            scheduleTimer.Start();
        }

        private void bgWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {

        }

        private void bgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            lblStatus.Text = "Timer enabled...Timer started.";
            lblMessageSrvc.Text = "Message service running.";
            lblMessageSrvc.Update();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnStopTimer_Click(object sender, EventArgs e)
        {
            scheduleTimer.Stop();
        }
    }
}
