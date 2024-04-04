using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Newtonsoft.Json;

namespace MM4RawSocketAPI
{
    public partial class Form1 : Form
    {
        private string _lastErrorMsg;
        private MM4RemoteError _lastError;
        private TcpClient _tcpClient;
        private MM4InteropResponse _response = new MM4InteropResponse();
        private int _notificationPort = 0;
        private IPAddress _serverAddr;
        private IPAddress _clientAddr;
        private int _serverPort;
        public const int BUFF_SZ = 16384;
        private TcpListener _listener = null;
        private volatile bool _listening = false;
        public bool Listening { get { return _listening; } }

        public Form1()
        {
            InitializeComponent();

            _tcpClient = new TcpClient();
        }

        private void SetControlText(Control ctrl, string text, bool append = false)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<Control, string, bool>(SetControlText), new object[] { ctrl, text, append });
                return;
            }

            if (append)
            {
                TextBox t = (TextBox)ctrl;
                t.AppendText(text + Environment.NewLine);
            }
            else
            {
                ctrl.Text = text;
            }
        }

        public void LogMessage(string msg)
        {
            SetControlText(txtMessages, msg, true);
        }

        private bool ProcessTransaction(MM4InteropCommand cmd)
        {
            _lastErrorMsg = "";
            _response = new MM4InteropResponse();
            _lastError = _response.Error = MM4RemoteError.ClientSideError;
            if (_tcpClient == null)
            {
                _lastErrorMsg = "Client not active. Call Initialize() with appropriate IP addresses.";
                return false;
            }
            string err = "";
            if (!CommandTransaction(cmd, ref _response, out err))
            {
                _lastErrorMsg = "Client-side transaction failure: " + err;
                return false;
            }
            _lastError = _response.Error;
            if (_response.Error != MM4RemoteError.OK)
            {
                _lastErrorMsg = _response.Error.ToString() +
                    (!string.IsNullOrEmpty(_response.Result) ? (", Hint: " + _response.Result) : "");
                return false;
            }
            return true;
        }

        public bool CommandTransaction(MM4InteropCommand cmd, ref MM4InteropResponse response, out string errorMsg)
        {
            errorMsg = "";
            IAsyncResult result = null;
            try
            {
                // String to store the response ASCII representation.
                String responseData = String.Empty;

                _serverAddr = IPAddress.Parse(txtIPAddress.Text);
                _serverPort = Convert.ToUInt16(txtPort.Text);

                using (TcpClient client = new TcpClient())
                {
                    // -- Try Asynch --
                    result = client.BeginConnect(_serverAddr, _serverPort, null, null);

                    // Small delay
                    Thread.Sleep(100);

                    if (!result.AsyncWaitHandle.WaitOne(3000))
                    {
                        LogMessage("TC: connection timeout");
                        // Close the socket and bail.                        
                        client.Client.Close();
                        client.Close();
                        result.AsyncWaitHandle.Close();
                        result.AsyncWaitHandle.Dispose();
                        result = null;
                        LogMessage("TC: Connection cleanup");
                        throw new Exception("Server connection timeout");
                    }

                    client.EndConnect(result);
                    LogMessage("TC: Connected");

                    // Translate the passed message into ASCII and store it as a Byte array.
                    string dataString = JsonConvert.SerializeObject(cmd);
                    Byte[] data = System.Text.Encoding.ASCII.GetBytes(dataString);

                    // Get a client stream for reading and writing. 
                    using (NetworkStream stream = client.GetStream())
                    {
                        // Send the message to the connected TcpServer. 
                        LogMessage(">> " + dataString);
                        stream.Write(data, 0, data.Length);

                        // Receive the TcpServer.response. 
                        // Buffer to store the response bytes.
                        data = new Byte[BUFF_SZ];

                        // Read the first batch of the TcpServer response bytes.
                        Int32 bytes = stream.Read(data, 0, data.Length);
                        responseData = Encoding.ASCII.GetString(data, 0, bytes).Trim();

                        LogMessage("<< " + responseData);
                        response = JsonConvert.DeserializeObject<MM4InteropResponse>(responseData);
                        // Close everything.
                        stream.Close();
                    }
                    client.Client.Close();
                    client.Close();

                    LogMessage("TC: Closed");
                }
                // The JSON deserialization might fail.
                if (response == null)
                {
                    LogMessage("TC: Response data not deserialized: " + responseData);
                }
                return (response != null);
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
                LogMessage("TC: AsyncWaitHandle exception: " + errorMsg);
                if (ex.InnerException != null)
                {
                    LogMessage("TC: inner exception: " + ex.InnerException.Message);
                }

                if (result != null)
                {
                    result.AsyncWaitHandle.Close();
                    result.AsyncWaitHandle.Dispose();
                    result = null;
                }
            }
            return false;
        }

        public MM4RemoteError GetLastError(out string lastError)
        {
            lastError = _lastErrorMsg;
            return _lastError;
        }
        public MM4RemoteError StartMethod(string methodName)
        {
            ProcessTransaction(new MM4InteropCommand(MM4RemoteCommand.StartMethod)
            {
                ItemName = methodName,
                Password = txtUserOrPassword.Text
            });
            return _lastError;
        }
        public MM4RemoteError StartMethodAtStep(string methodName, string step)
        {
            ProcessTransaction(new MM4InteropCommand(MM4RemoteCommand.StartMethodAtStep)
            {
                ItemName = methodName,
                ItemValue = step,
                Password = txtUserOrPassword.Text
            });
            return _lastError;
        }
        public MM4RemoteError StartMethodSingleStep(string methodName, string step)
        {
            ProcessTransaction(new MM4InteropCommand(MM4RemoteCommand.StartMethodSingleStep)
            {
                ItemName = methodName,
                ItemValue = step,
                Password = txtUserOrPassword.Text
            });
            return _lastError;
        }
        public MM4RemoteError StopMethod()
        {
            ProcessTransaction(new MM4InteropCommand(MM4RemoteCommand.StopMethod)
            {
                Password = txtUserOrPassword.Text
            });
            return _lastError;
        }
        public MM4RemoteError GetMethodState(out string activeMethodName, out MM4RemoteMethodState methodState)
        {
            activeMethodName = "";
            methodState = MM4RemoteMethodState.Unknown;
            if (ProcessTransaction(new MM4InteropCommand(MM4RemoteCommand.GetMethodState)))
            {
                activeMethodName = _response.Result;
                methodState = _response.MethodState;
            }
            return _lastError;
        }
        public MM4RemoteError GetLastMethodResult(out string lastMethodName, out MM4RemoteLastMethodResult lastMethodResult)
        {
            lastMethodName = "";
            lastMethodResult = MM4RemoteLastMethodResult.None;
            if (ProcessTransaction(new MM4InteropCommand(MM4RemoteCommand.GetLastMethodResult)))
            {
                lastMethodName = _response.Result;
                lastMethodResult = _response.LastMethodResult;
            }
            return _lastError;
        }
        public MM4RemoteError GetApplicationState(out MM4RemoteApplicationState applicationState, out string workspaceName, out string worktableNames)
        {
            applicationState = MM4RemoteApplicationState.None;
            workspaceName = "";
            worktableNames = "";
            if (ProcessTransaction(new MM4InteropCommand(MM4RemoteCommand.GetApplicationState)))
            {
                applicationState = _response.ApplicationState;
                workspaceName = _response.Item;
                worktableNames = _response.Result;
            }
            return _lastError;
        }
        public MM4RemoteError SetVariable(string variableName, string value)
        {
            ProcessTransaction(new MM4InteropCommand(MM4RemoteCommand.SetVariable)
            {
                ItemName = variableName,
                ItemValue = value,
                Password = txtUserOrPassword.Text
            });
            return _lastError;
        }
        public MM4RemoteError GetVariable(string variableName, out string value)
        {
            value = "";
            if (ProcessTransaction(new MM4InteropCommand(MM4RemoteCommand.GetVariable)
            {
                ItemName = variableName
            }))
            {
                value = _response.Result;
            }
            return _lastError;
        }
        public MM4RemoteError VariableWatch(string variableName, bool watch)
        {
            if (_notificationPort == 0)
            {
                _lastErrorMsg = "Watch is not allowed because there is no valid client notification port.";
                return MM4RemoteError.ClientSideError;
            }
            ProcessTransaction(new MM4InteropCommand(MM4RemoteCommand.VariableWatch)
            {
                ItemName = variableName,
                ItemValue = (watch ? MM4InteropCommand.WATCH : MM4InteropCommand.DONT_WATCH) + _notificationPort.ToString()
            });
            return _lastError;
        }
        public MM4RemoteError MethodWatch(bool watch)
        {
            if (_notificationPort == 0)
            {
                _lastErrorMsg = "Watch is not allowed because there is no valid client notification port.";
                return MM4RemoteError.ClientSideError;
            }
            ProcessTransaction(new MM4InteropCommand(MM4RemoteCommand.MethodWatch)
            {
                ItemValue = (watch ? MM4InteropCommand.WATCH : MM4InteropCommand.DONT_WATCH) + _notificationPort.ToString()
            });
            return _lastError;
        }
        public MM4RemoteError GetInput(string inputName, out bool active)
        {
            active = false;
            if (ProcessTransaction(new MM4InteropCommand(MM4RemoteCommand.GetInput)
            {
                ItemName = inputName
            }))
            {
                active = bool.Parse(_response.Result);
            }
            return _lastError;
        }
        public MM4RemoteError QueryWorktablePlate(string worktableFullName, string plateName, out string queryResults)
        {
            queryResults = "";
            if (ProcessTransaction(new MM4InteropCommand(MM4RemoteCommand.QueryWorktablePlate)
            {
                ItemName = worktableFullName,
                ItemValue = plateName
            }))
            {
                queryResults = _response.Result;
            }
            return _lastError;
        }
        public MM4RemoteError QueryWorktableBarcode(string worktableFullName, string barCode, out string queryResults)
        {
            queryResults = "";
            if (ProcessTransaction(new MM4InteropCommand(MM4RemoteCommand.QueryWorktablePlate)
            {
                ItemName = worktableFullName,
                ItemValue = barCode
            }))
            {
                queryResults = _response.Result;
            }
            return _lastError;
        }
        public MM4RemoteError QueryWorktableLocation(string worktableFullName, string locationName, out string queryResults)
        {
            queryResults = "";
            if (ProcessTransaction(new MM4InteropCommand(MM4RemoteCommand.QueryWorktableLocation)
            {
                ItemName = worktableFullName,
                ItemValue = locationName
            }))
            {
                queryResults = _response.Result;
            }
            return _lastError;
        }
        public MM4RemoteError InitializeHardware()
        {
            ProcessTransaction(new MM4InteropCommand(MM4RemoteCommand.InitializeHardware)
            {
                Password = txtUserOrPassword.Text,
                ItemValue = _notificationPort.ToString()
            });
            return _lastError;
        }
        public MM4RemoteError ClearErrors()
        {
            ProcessTransaction(new MM4InteropCommand(MM4RemoteCommand.ClearErrors)
            {
                Password = txtUserOrPassword.Text
            });
            return _lastError;
        }
        public MM4RemoteError ConnectHardware()
        {
            ProcessTransaction(new MM4InteropCommand(MM4RemoteCommand.ConnectHardware)
            {
                Password = txtUserOrPassword.Text,
                ItemValue = _notificationPort.ToString()
            });
            return _lastError;
        }
        public MM4RemoteError SetExecuteMode(bool on)
        {
            ProcessTransaction(new MM4InteropCommand(MM4RemoteCommand.SetExecuteMode)
            {
                Password = txtUserOrPassword.Text,
                ItemValue = on.ToString()
            });
            return _lastError;
        }
        public MM4RemoteError GetExecuteMode(out bool executeMode)
        {
            ProcessTransaction(new MM4InteropCommand(MM4RemoteCommand.GetExecuteMode)
            {
                Password = txtUserOrPassword.Text,
            });
            {
                executeMode = bool.Parse(_response.Result);
            }
            return _lastError; 
        }
        public MM4RemoteError SetWorktablePersistMode(bool on)
        {
            ProcessTransaction(new MM4InteropCommand(MM4RemoteCommand.SetWorktablePersistModeOn)
            {
                Password = txtUserOrPassword.Text,
                ItemValue = on.ToString()
            });
            return _lastError;
        }
        public MM4RemoteError GetWorktablePersistModeOn(out bool persistModeOn)
        {
            ProcessTransaction(new MM4InteropCommand(MM4RemoteCommand.GetWorktablePersistModeOn)
            {
                Password = txtUserOrPassword.Text,
            });
            {
                persistModeOn = bool.Parse(_response.Result);
            }
            return _lastError;
        }
        public MM4RemoteError GetProcessPausedFormActive(out bool active, out bool continueAvailable, out bool retryAvailable)
        {
            ProcessTransaction(new MM4InteropCommand(MM4RemoteCommand.GetProcessPausedFormActive)
            {
                Password = txtUserOrPassword.Text,
            });
            {
                string[] results = _response.Result.Split(';');
                active = bool.Parse(results[0]);
                continueAvailable = bool.Parse(results[1]);
                retryAvailable = bool.Parse(results[2]);
            }
            return _lastError;
        }
        public MM4RemoteError CloseActiveProcessPausedForm(bool continueOption, bool retryOption)
        {
            ProcessTransaction(new MM4InteropCommand(MM4RemoteCommand.CloseActiveProcessPausedForm)
            {
                ItemName = continueOption.ToString(),
                ItemValue = retryOption.ToString(),
                Password = txtUserOrPassword.Text
            });
            return _lastError;
        }
        public MM4RemoteError PauseMethod()
        {
            ProcessTransaction(new MM4InteropCommand(MM4RemoteCommand.PauseMethod)
            {
                Password = txtUserOrPassword.Text
            });
            return _lastError;
        }

        public void StartNotificationListener()
        {
            _clientAddr = IPAddress.Parse(txtListenerIPAddress.Text);
            _notificationPort = Convert.ToUInt16(txtListenerPort.Text);

            if (_clientAddr == IPAddress.None)
            {
                return;
            }
            (new Thread(() =>
            {
                string errMsg = "";
                try
                {
                    // TcpListener server = new TcpListener(port);
                    _listener = new TcpListener(_clientAddr, _notificationPort);

                    // Start listening for client requests.
                    _listener.Start();
                    LogMessage("TC: Notification listener started.");

                    // Buffer for reading data
                    Byte[] bytes = new Byte[BUFF_SZ];
                    _listening = true;
                    // Enter the listening loop. 

                    SetControlText(btnNotificationListenerControl, "Stop Notification Listener");
                    while (Listening)
                    {
                        // Perform a blocking call to accept requests. 
                        // You could also user server.AcceptSocket() here.
                        using (TcpClient client = _listener.AcceptTcpClient())
                        {
                            LogMessage("TC: Notification.");

                            // Get a stream object for reading and writing
                            int i = 0;
                            using (NetworkStream stream = client.GetStream())
                            {
                                i = stream.Read(bytes, 0, bytes.Length);
                                stream.Close();
                            }
                            // Shutdown and end connection
                            client.Client.Close();
                            client.Close();
                            if (i != 0)
                            {
                                // Translate data bytes to a ASCII string.
                                ProcessNotification(Encoding.ASCII.GetString(bytes, 0, i));
                            }
                            LogMessage("TC: Notification processed.");
                        }
                    }
                }
                catch (SocketException ex)
                {
                    if (ex.SocketErrorCode != SocketError.Interrupted)
                    {
                        errMsg = ex.Message;
                    }
                }
                catch (Exception ex)
                {
                    errMsg = ex.Message;
                }
                finally
                {
                    // Stop listening for new clients.
                    if (_listener != null)
                    {
                        _listener.Stop();
                    }
                }
                _listening = false;
                _listener = null;
                if (!string.IsNullOrEmpty(errMsg))
                {
                    LogMessage("TC: Notification listener failed: " + errMsg);
                }
            }) { Name = "Notification Listener" }).Start();
        }

        public void StopNotificationListener()
        {
            if (_listening)
            {
                _listening = false;
                if (_listener != null)
                {
                    _listener.Stop();
                }
                Thread.Sleep(100);
                LogMessage("TC: Notification listener stopped");
            }
        }

        private void ProcessNotification(string message)
        {
            LogMessage("Client ProcessNotification: " + message);

            if ((message.Replace('{', '\0').Length - message.Replace('}', '\0').Length) == 0)
            {
                try
                {
                    MM4InteropNotification notification = JsonConvert.DeserializeObject<MM4InteropNotification>(message.Trim());

                    if (notification.NotificationType == MM4InteropNotificationType.MethodComplete)
                    {
                        LogMessage("NotificationType: MethodComplete");
                        if (!string.IsNullOrEmpty(notification.ItemName))
                            LogMessage("ItemName: " + notification.ItemName);
                        if (!string.IsNullOrEmpty(notification.ItemValue))
                            LogMessage("ItemValue: " + notification.ItemValue);
                    }
                    else if (notification.NotificationType == MM4InteropNotificationType.InitializationComplete)
                    {
                        LogMessage("NotificationType: InitializationComplete");
                        if (!string.IsNullOrEmpty(notification.ItemName))
                            LogMessage("ItemName: " + notification.ItemName);
                        LogMessage("ItemValue: " + notification.ItemValue);
                    }
                    else if (notification.NotificationType == MM4InteropNotificationType.ConnectionComplete)
                    {
                        LogMessage("NotificationType: ConnectionComplete");
                        if (!string.IsNullOrEmpty(notification.ItemName))
                            LogMessage("ItemName: " + notification.ItemName);
                        if (!string.IsNullOrEmpty(notification.ItemValue))
                            LogMessage("ItemValue: " + notification.ItemValue);
                    }
                    else if (notification.NotificationType == MM4InteropNotificationType.VariableChanged)
                    {
                        LogMessage("NotificationType: VariableChanged");
                        if (!string.IsNullOrEmpty(notification.ItemName))
                            LogMessage("ItemName: " + notification.ItemName);
                        if (!string.IsNullOrEmpty(notification.ItemValue))
                            LogMessage("ItemValue: " + notification.ItemValue);
                    }
                }
                catch { }
            }
        }

        private void btnGetApplicationState_Click(object sender, EventArgs e)
        {
            string foo = "";
            MM4RemoteApplicationState currentApplicationState = MM4RemoteApplicationState.None;
            GetApplicationState(out currentApplicationState, out foo, out foo);
        }

        private void btnGetVariable_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(txtVariableName.Text))
            {
                string value;
                SetControlText(txtVariableValue, "");
                _lastError = GetVariable(txtVariableName.Text, out value);
                if (_lastError == MM4RemoteError.OK)
                    SetControlText(txtVariableValue, value);
                else
                    LogMessage(_lastError.ToString());
            }
        }

        private void btnSetVariable_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(txtVariableName.Text) && !String.IsNullOrEmpty(txtVariableValue.Text))
            {
                _lastError = SetVariable(txtVariableName.Text, txtVariableValue.Text);
                if (_lastError != MM4RemoteError.OK)
                    LogMessage(_lastError.ToString());
            }
        }

        private void btnNotificationListenerControl_Click(object sender, EventArgs e)
        {
            if (btnNotificationListenerControl.Text == "Start Notification Listener")
            {
                StartNotificationListener();
                btnNotificationListenerControl.Text = "Stop Notification Listener";
            }
            else
            {
                StopNotificationListener();
                btnNotificationListenerControl.Text = "Start Notification Listener";
            }
        }

        private void btnMonitorVariable_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(txtVariableName.Text))
            {
                _lastError = VariableWatch(txtVariableName.Text, true);
                if (_lastError != MM4RemoteError.OK)
                    LogMessage(_lastError.ToString());
            }
        }

        private void btnUnwatchVariable_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(txtVariableName.Text))
            {
                _lastError = VariableWatch(txtVariableName.Text, false);
                if (_lastError != MM4RemoteError.OK)
                    LogMessage(_lastError.ToString());
            }
        }

        private void btnWatchMethod_Click(object sender, EventArgs e)
        {
            _lastError = MethodWatch(true);
            if (_lastError != MM4RemoteError.OK)
                LogMessage(_lastError.ToString());
        }

        private void btnUnwatchMethod_Click(object sender, EventArgs e)
        {
            _lastError = MethodWatch(false);
            if (_lastError != MM4RemoteError.OK)
                LogMessage(_lastError.ToString());
        }

        private void btnConnectHardware_Click(object sender, EventArgs e)
        {
            _lastError = ConnectHardware();
            if (_lastError != MM4RemoteError.OK)
                LogMessage(_lastError.ToString());
        }

        private void btnRunMethod_Click(object sender, EventArgs e)
        {
            _lastError = StartMethod(txtMethodName.Text);
            if (_lastError != MM4RemoteError.OK)
                LogMessage(_lastError.ToString());
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            _lastError = StopMethod();
            if (_lastError != MM4RemoteError.OK)
                LogMessage(_lastError.ToString());
        }

        private void btnStartMethodAtStep_Click(object sender, EventArgs e)
        {
            _lastError = StartMethodAtStep(txtMethodName.Text, txtStepIndex.Text);
            if (_lastError != MM4RemoteError.OK)
                LogMessage(_lastError.ToString());
        }

        private void btnStartMethodSingleStep_Click(object sender, EventArgs e)
        {
            _lastError = StartMethodSingleStep(txtMethodName.Text, txtStepIndex.Text);
            if (_lastError != MM4RemoteError.OK)
                LogMessage(_lastError.ToString());
        }

        private void btnSetExecuteMode_Click(object sender, EventArgs e)
        {
            _lastError = SetExecuteMode(true);
            if (_lastError != MM4RemoteError.OK)
                LogMessage(_lastError.ToString());
        }

        private void btnSetTestMode_Click(object sender, EventArgs e)
        {
            _lastError = SetExecuteMode(false);
            if (_lastError != MM4RemoteError.OK)
                LogMessage(_lastError.ToString());
        }

        private void btnWorktablePersistModeON_Click(object sender, EventArgs e)
        {
            _lastError = SetWorktablePersistMode(true);
            if (_lastError != MM4RemoteError.OK)
                LogMessage(_lastError.ToString());
        }

        private void btnWorktablePersistModeOFF_Click(object sender, EventArgs e)
        {
            _lastError = SetWorktablePersistMode(false);
            if (_lastError != MM4RemoteError.OK)
                LogMessage(_lastError.ToString());
        }

        private void btnGetExecuteMode_Click(object sender, EventArgs e)
        {
            bool executeMode;
            _lastError = GetExecuteMode(out executeMode);
            if (_lastError != MM4RemoteError.OK)
                LogMessage(_lastError.ToString());
        }

        private void btnGetWorktablePersistModeOn_Click(object sender, EventArgs e)
        {
            bool persistMode;
            _lastError = GetWorktablePersistModeOn(out persistMode);
            if (_lastError != MM4RemoteError.OK)
                LogMessage(_lastError.ToString());
        }

        private void btnInitHardware_Click(object sender, EventArgs e)
        {
            _lastError = InitializeHardware();
            if (_lastError != MM4RemoteError.OK)
                LogMessage(_lastError.ToString());
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_listening)
                StopNotificationListener();

            Properties.Settings.Default.ServerIP = txtIPAddress.Text;
            Properties.Settings.Default.ServerPort = txtPort.Text;
            Properties.Settings.Default.ClientIP = txtListenerIPAddress.Text;
            Properties.Settings.Default.ClientPort = txtListenerPort.Text;
            Properties.Settings.Default.LastMethod = txtMethodName.Text;
            Properties.Settings.Default.LastVariable = txtVariableName.Text;
            Properties.Settings.Default.Save();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            txtIPAddress.Text = Properties.Settings.Default.ServerIP;
            txtPort.Text = Properties.Settings.Default.ServerPort;
            txtListenerIPAddress.Text = Properties.Settings.Default.ClientIP;
            txtListenerPort.Text = Properties.Settings.Default.ClientPort;
            txtMethodName.Text = Properties.Settings.Default.LastMethod;
            txtVariableName.Text = Properties.Settings.Default.LastVariable;
        }

        private void btnGetProcessPausedFormActive_Click(object sender, EventArgs e)
        {
            bool active;
            bool continueAvailable;
            bool retryAvailable;
            _lastError = GetProcessPausedFormActive(out active, out continueAvailable, out retryAvailable);
            if (_lastError != MM4RemoteError.OK)
                LogMessage(_lastError.ToString());
            else
            {
                MessageBox.Show("Process Paused Form is " + (active ? "active," : "inactive,") + Environment.NewLine +
                "Continue Option is " + (continueAvailable ? "available, " : "not available,") + Environment.NewLine +
                "Retry Option is " + (continueAvailable ? "available. " : "not available."));

                btnCloseActiveProcessPausedForm.Enabled = active;
                chkContinue.Enabled = continueAvailable;
                chkRetry.Enabled = retryAvailable;
            }
        }

        private void btnCloseActiveProcessPausedForm_Click(object sender, EventArgs e)
        {
            _lastError = CloseActiveProcessPausedForm((chkContinue.Enabled & chkContinue.Checked), (chkRetry.Enabled & chkRetry.Checked));
            if (_lastError != MM4RemoteError.OK)
                LogMessage(_lastError.ToString());

            btnCloseActiveProcessPausedForm.Enabled = chkContinue.Enabled = chkRetry.Enabled = false;
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            _lastError = PauseMethod();
            if (_lastError != MM4RemoteError.OK)
                LogMessage(_lastError.ToString());
        }
    }

    #region MM4 Classes

    public enum MM4InteropNotificationType
    {
        Unknown = 0,
        MethodComplete,
        VariableChanged,
        InitializationComplete,
        ConnectionComplete
    }

    public class MM4InteropNotification
    {
        public const char VALUE_SEPARATOR = '|';
        public MM4InteropNotificationType NotificationType { get; set; }
        // This names a variable or a method name.
        public string ItemName { get; set; }
        // Can be a completion code or variable value.
        public string ItemValue { get; set; }
        public MM4RemoteApplicationState ApplicationState { get; set; }
    }

    public class MM4InteropCommand : MM4InteropHeader
    {
        public const string WATCH = "WATCH";
        public const string DONT_WATCH = "DONT_WATCH";
        // This names a variable, a method, an IO, or a worktable name.
        public string ItemName { get; set; }
        // can be a variable name, a method name, or "true"/"false".
        public string ItemValue { get; set; }
        // password only required for control, not monitoring.
        public string Password { get; set; }

        public MM4InteropCommand() : base() { }

        public MM4InteropCommand(MM4RemoteCommand commandType) :
            base(commandType)
        {
        }

    }    

    public class MM4InteropResponse : MM4InteropHeader
    {
        public const char METHOD_STACK_COMMAND_SEPARATOR = ';';
        public const char METHOD_STACK_PROCESS_SEPARATOR = '|';
        public MM4RemoteError Error { get; set; }
        public MM4RemoteApplicationState ApplicationState { get; set; }
        public string Item { get; set; }
        public MM4RemoteMethodState MethodState { get; set; }
        public MM4RemoteLastMethodResult LastMethodResult { get; set; }
        public string Result { get; set; }

        public MM4InteropResponse() { }

        public MM4InteropResponse(MM4InteropHeader header)
        {
            Command = header.Command;
        }
    }

    public enum MM4RemoteLastMethodResult
    {
        /// <summary>
        /// There is no record of a previous method
        /// </summary>
        None = 0,
        /// <summary>
        /// The last method executed completed without error.
        /// </summary>
        Success,
        /// <summary>
        /// A non-error interruption that prevented completion.
        /// Can be for a variety of reasons.
        /// </summary>
        Interrupted,
        /// <summary>
        /// The last method executed terminated due to an error.
        /// </summary>
        Error
    }

    public class MM4InteropHeader
    {
        /// <summary>
        /// Transaction header
        /// </summary>
        public MM4RemoteCommand Command { get; set; }

        protected MM4InteropHeader() { }

        protected MM4InteropHeader(MM4RemoteCommand commandId)
        {
            Command = commandId;
        }
    }

    public enum MM4RemoteCommand
    {
        Unknown = 0,
        StartMethod,                        // 1
        StopMethod,                         // 2
        GetMethodState,                     // 3
        GetLastMethodResult,                // 4
        GetApplicationState,                // 5
        SetVariable,                        // 6
        GetVariable,                        // 7
        VariableWatch,                      // 8
        MethodWatch,                        // 9
        GetInput,                           // 10
        QueryWorktablePlate,                // 11
        QueryWorktableBarcode,              // 12
        QueryWorktableLocation,             // 13
        InitializeHardware,                 // 14
        ConnectHardware,                    // 15
        ClearErrors,                        // 16
        StartScheduledMethod,               // 17
        GetLastErrorMessage,                // 18
        GetExecuteMode,                     // 19
        SetExecuteMode,                     // 20
        StartMethodAtStep,                  // 21
        StartMethodSingleStep,              // 22
        GetWorktablePersistModeOn,          // 23
        SetWorktablePersistModeOn,          // 24
        GetProcessPausedFormActive,         // 25
        CloseActiveProcessPausedForm,       // 26
        PauseMethod                         // 27
    }

    public enum MM4RemoteApplicationState
    {
        None = 0,
        WorkspaceLoaded = 0x0001,
        SimulationMode = 0x0002,
        MethodRunning = 0x0004,
        MethodPaused = 0x0008,
        MethodErrorPaused = 0x0010,
        ApplicationBlocked = 0x0020,
        EStopEngaged = 0x0040,
        DevicesReady = 0x0080,
        InitializationInProgress = 0x0100
    }

    public enum MM4RemoteMethodState
    {
        /// <summary>
        /// Unknown state.
        /// </summary>
        Unknown = 0,
        /// <summary>
        /// There is no active method.
        /// </summary>
        NoActiveMethod, // Nothing running
        /// <summary>
        /// A method is executing.
        /// </summary>
        Busy, // MMProcessState: Created, Finished, Busy, MainProcessWaiting, CommandInUseWaiting
        /// <summary>
        /// A method is paused due to a non-error condition.
        /// </summary>
        Paused, // MMProcessState: Paused
        /// <summary>
        /// A method is paused due to an error condition.
        /// </summary>
        ErrorPaused // MMProcessState: ErrorStopped
    }

    public enum MM4RemoteError
    {
        /// <summary>
        /// The server understood and executed the command without error.
        /// </summary>
        OK = 0,
        /// <summary>
        /// The server does not have a Workspace loaded.
        /// </summary>
        NoWorkspace,
        /// <summary>
        /// The application is performing a task that prevents the server from responding correctly.
        /// </summary>
        ApplicationBlocked,
        /// <summary>
        /// The Emergency-Stop is engaged on the server and no commands can be performed.
        /// </summary>
        EStopEngaged,
        /// <summary>
        /// The current user level on the server is other than 'User', which is required to permit remote commands.
        /// </summary>
        PermissionLevelNotUser,
        /// <summary>
        /// A method start was requested but the method does not have the 'User Permission Level' flag selected.
        /// </summary>
        MethodPermissionLevelNotUser,
        /// <summary>
        /// The Devices in the Workspace are not initialized and no methods can be performed.
        /// </summary>
        DevicesNotReady,
        /// <summary>
        /// The server is in Test mode and can not execute a command.
        /// </summary>
        NotExecutionMode,
        /// <summary>
        /// A method is currently running and a new method can not be started or last method results can not be reported.
        /// </summary>
        MethodAlreadyRunning,
        /// <summary>
        /// A method variable name was not recognized on the server.
        /// </summary>
        UnknownVariable,
        /// <summary>
        /// An attempt was made to write to a read-only method variable on the remote server.
        /// </summary>
        VariableIsReadOnly,
        /// <summary>
        /// A client command referenced a Device that that the server does not recognize.
        /// </summary>
        UnknownDevice,
        /// <summary>
        /// A client command referenced a Worktable that that the server does not recognize.
        /// </summary>
        UnknownWorktable,
        /// <summary>
        /// A client command requested a query that the server does not recognize.
        /// </summary>
        UnknownQuery,
        /// <summary>
        /// A client command requested the state of an input that the server does not recognize.
        /// </summary>
        UnknownInput,
        /// <summary>
        /// A client command referenced a Worktable that that the server does not recognize.
        /// </summary>
        UnknownMethod,
        /// <summary>
        /// A client issues a command using an incorrect Remote Password.
        /// </summary>
        RemoteControlPswdNotValid,
        /// <summary>
        /// A client requested a stop method command but no method was running.
        /// </summary>
        NoMethodRunning,
        /// <summary>
        /// A command format was not recognized.
        /// </summary>
        BadCommandFormat,
        /// <summary>
        /// The server encountered an error that prevented a correct response.
        /// </summary>
        ApplicationError,
        /// <summary>
        /// The client was unable to establish a connection to the server.
        /// </summary>
        ClientSideError,
        /// <summary>
        /// The method is flagged to only ever run as a sub-method.
        /// </summary>
        SubMethodOnly
    }

    #endregion
}
