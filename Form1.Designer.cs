namespace MM4RawSocketAPI
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.txtMessages = new System.Windows.Forms.TextBox();
            this.lblMessages = new System.Windows.Forms.Label();
            this.txtIPAddress = new System.Windows.Forms.TextBox();
            this.txtPort = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnGetApplicationState = new System.Windows.Forms.Button();
            this.btnGetVariable = new System.Windows.Forms.Button();
            this.btnSetVariable = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtVariableValue = new System.Windows.Forms.TextBox();
            this.txtVariableName = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtUserOrPassword = new System.Windows.Forms.TextBox();
            this.btnNotificationListenerControl = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.txtListenerPort = new System.Windows.Forms.TextBox();
            this.txtListenerIPAddress = new System.Windows.Forms.TextBox();
            this.btnWatchVariable = new System.Windows.Forms.Button();
            this.btnUnwatchVariable = new System.Windows.Forms.Button();
            this.btnUnwatchMethod = new System.Windows.Forms.Button();
            this.btnWatchMethod = new System.Windows.Forms.Button();
            this.btnConnectHardware = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtMessages
            // 
            this.txtMessages.Location = new System.Drawing.Point(212, 36);
            this.txtMessages.Multiline = true;
            this.txtMessages.Name = "txtMessages";
            this.txtMessages.Size = new System.Drawing.Size(582, 390);
            this.txtMessages.TabIndex = 1;
            // 
            // lblMessages
            // 
            this.lblMessages.AutoSize = true;
            this.lblMessages.Location = new System.Drawing.Point(209, 20);
            this.lblMessages.Name = "lblMessages";
            this.lblMessages.Size = new System.Drawing.Size(55, 13);
            this.lblMessages.TabIndex = 2;
            this.lblMessages.Text = "Messages";
            // 
            // txtIPAddress
            // 
            this.txtIPAddress.Location = new System.Drawing.Point(115, 20);
            this.txtIPAddress.Name = "txtIPAddress";
            this.txtIPAddress.Size = new System.Drawing.Size(88, 20);
            this.txtIPAddress.TabIndex = 3;
            this.txtIPAddress.Text = "192.168.10.7";
            this.txtIPAddress.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txtPort
            // 
            this.txtPort.Location = new System.Drawing.Point(115, 47);
            this.txtPort.Name = "txtPort";
            this.txtPort.Size = new System.Drawing.Size(52, 20);
            this.txtPort.TabIndex = 4;
            this.txtPort.Text = "47000";
            this.txtPort.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(17, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(92, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Server IP Address";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(49, 50);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(60, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Server Port";
            // 
            // btnGetApplicationState
            // 
            this.btnGetApplicationState.Location = new System.Drawing.Point(23, 195);
            this.btnGetApplicationState.Name = "btnGetApplicationState";
            this.btnGetApplicationState.Size = new System.Drawing.Size(165, 23);
            this.btnGetApplicationState.TabIndex = 7;
            this.btnGetApplicationState.Text = "Get Applicate State";
            this.btnGetApplicationState.UseVisualStyleBackColor = true;
            this.btnGetApplicationState.Click += new System.EventHandler(this.btnGetApplicationState_Click);
            // 
            // btnGetVariable
            // 
            this.btnGetVariable.Location = new System.Drawing.Point(23, 224);
            this.btnGetVariable.Name = "btnGetVariable";
            this.btnGetVariable.Size = new System.Drawing.Size(80, 23);
            this.btnGetVariable.TabIndex = 8;
            this.btnGetVariable.Text = "Get Variable";
            this.btnGetVariable.UseVisualStyleBackColor = true;
            this.btnGetVariable.Click += new System.EventHandler(this.btnGetVariable_Click);
            // 
            // btnSetVariable
            // 
            this.btnSetVariable.Location = new System.Drawing.Point(108, 224);
            this.btnSetVariable.Name = "btnSetVariable";
            this.btnSetVariable.Size = new System.Drawing.Size(80, 23);
            this.btnSetVariable.TabIndex = 9;
            this.btnSetVariable.Text = "Set Variable";
            this.btnSetVariable.UseVisualStyleBackColor = true;
            this.btnSetVariable.Click += new System.EventHandler(this.btnSetVariable_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(19, 336);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(75, 13);
            this.label3.TabIndex = 13;
            this.label3.Text = "Variable Value";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(18, 309);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(76, 13);
            this.label4.TabIndex = 12;
            this.label4.Text = "Variable Name";
            // 
            // txtVariableValue
            // 
            this.txtVariableValue.Location = new System.Drawing.Point(100, 333);
            this.txtVariableValue.Name = "txtVariableValue";
            this.txtVariableValue.Size = new System.Drawing.Size(88, 20);
            this.txtVariableValue.TabIndex = 11;
            this.txtVariableValue.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txtVariableName
            // 
            this.txtVariableName.Location = new System.Drawing.Point(100, 306);
            this.txtVariableName.Name = "txtVariableName";
            this.txtVariableName.Size = new System.Drawing.Size(88, 20);
            this.txtVariableName.TabIndex = 10;
            this.txtVariableName.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(29, 76);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(80, 13);
            this.label5.TabIndex = 15;
            this.label5.Text = "User/Password";
            // 
            // txtUserOrPassword
            // 
            this.txtUserOrPassword.Location = new System.Drawing.Point(115, 73);
            this.txtUserOrPassword.Name = "txtUserOrPassword";
            this.txtUserOrPassword.Size = new System.Drawing.Size(69, 20);
            this.txtUserOrPassword.TabIndex = 14;
            this.txtUserOrPassword.Text = "Remote";
            this.txtUserOrPassword.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // btnNotificationListenerControl
            // 
            this.btnNotificationListenerControl.Location = new System.Drawing.Point(23, 166);
            this.btnNotificationListenerControl.Name = "btnNotificationListenerControl";
            this.btnNotificationListenerControl.Size = new System.Drawing.Size(165, 23);
            this.btnNotificationListenerControl.TabIndex = 16;
            this.btnNotificationListenerControl.Text = "Start Notification Listener";
            this.btnNotificationListenerControl.UseVisualStyleBackColor = true;
            this.btnNotificationListenerControl.Click += new System.EventHandler(this.btnNotificationListenerControl_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(43, 129);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(66, 13);
            this.label6.TabIndex = 20;
            this.label6.Text = "Listener Port";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(11, 102);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(98, 13);
            this.label7.TabIndex = 19;
            this.label7.Text = "Listener IP Address";
            // 
            // txtListenerPort
            // 
            this.txtListenerPort.Location = new System.Drawing.Point(115, 126);
            this.txtListenerPort.Name = "txtListenerPort";
            this.txtListenerPort.Size = new System.Drawing.Size(52, 20);
            this.txtListenerPort.TabIndex = 18;
            this.txtListenerPort.Text = "47001";
            this.txtListenerPort.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txtListenerIPAddress
            // 
            this.txtListenerIPAddress.Location = new System.Drawing.Point(115, 99);
            this.txtListenerIPAddress.Name = "txtListenerIPAddress";
            this.txtListenerIPAddress.Size = new System.Drawing.Size(88, 20);
            this.txtListenerIPAddress.TabIndex = 17;
            this.txtListenerIPAddress.Text = "192.168.10.72";
            this.txtListenerIPAddress.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // btnWatchVariable
            // 
            this.btnWatchVariable.Location = new System.Drawing.Point(23, 253);
            this.btnWatchVariable.Name = "btnWatchVariable";
            this.btnWatchVariable.Size = new System.Drawing.Size(80, 38);
            this.btnWatchVariable.TabIndex = 21;
            this.btnWatchVariable.Text = "Watch Variable";
            this.btnWatchVariable.UseVisualStyleBackColor = true;
            this.btnWatchVariable.Click += new System.EventHandler(this.btnMonitorVariable_Click);
            // 
            // btnUnwatchVariable
            // 
            this.btnUnwatchVariable.Location = new System.Drawing.Point(109, 253);
            this.btnUnwatchVariable.Name = "btnUnwatchVariable";
            this.btnUnwatchVariable.Size = new System.Drawing.Size(80, 38);
            this.btnUnwatchVariable.TabIndex = 22;
            this.btnUnwatchVariable.Text = "Unwatch Variable";
            this.btnUnwatchVariable.UseVisualStyleBackColor = true;
            this.btnUnwatchVariable.Click += new System.EventHandler(this.btnUnwatchVariable_Click);
            // 
            // btnUnwatchMethod
            // 
            this.btnUnwatchMethod.Location = new System.Drawing.Point(109, 359);
            this.btnUnwatchMethod.Name = "btnUnwatchMethod";
            this.btnUnwatchMethod.Size = new System.Drawing.Size(80, 38);
            this.btnUnwatchMethod.TabIndex = 24;
            this.btnUnwatchMethod.Text = "Unwatch Method";
            this.btnUnwatchMethod.UseVisualStyleBackColor = true;
            this.btnUnwatchMethod.Click += new System.EventHandler(this.btnUnwatchMethod_Click);
            // 
            // btnWatchMethod
            // 
            this.btnWatchMethod.Location = new System.Drawing.Point(23, 359);
            this.btnWatchMethod.Name = "btnWatchMethod";
            this.btnWatchMethod.Size = new System.Drawing.Size(80, 38);
            this.btnWatchMethod.TabIndex = 23;
            this.btnWatchMethod.Text = "Watch Method";
            this.btnWatchMethod.UseVisualStyleBackColor = true;
            this.btnWatchMethod.Click += new System.EventHandler(this.btnWatchMethod_Click);
            // 
            // btnConnectHardware
            // 
            this.btnConnectHardware.Location = new System.Drawing.Point(24, 403);
            this.btnConnectHardware.Name = "btnConnectHardware";
            this.btnConnectHardware.Size = new System.Drawing.Size(165, 23);
            this.btnConnectHardware.TabIndex = 25;
            this.btnConnectHardware.Text = "Connect Hardware";
            this.btnConnectHardware.UseVisualStyleBackColor = true;
            this.btnConnectHardware.Click += new System.EventHandler(this.btnConnectHardware_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(808, 445);
            this.Controls.Add(this.btnConnectHardware);
            this.Controls.Add(this.btnUnwatchMethod);
            this.Controls.Add(this.btnWatchMethod);
            this.Controls.Add(this.btnUnwatchVariable);
            this.Controls.Add(this.btnWatchVariable);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.txtListenerPort);
            this.Controls.Add(this.txtListenerIPAddress);
            this.Controls.Add(this.btnNotificationListenerControl);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtUserOrPassword);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtVariableValue);
            this.Controls.Add(this.txtVariableName);
            this.Controls.Add(this.btnSetVariable);
            this.Controls.Add(this.btnGetVariable);
            this.Controls.Add(this.btnGetApplicationState);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtPort);
            this.Controls.Add(this.txtIPAddress);
            this.Controls.Add(this.lblMessages);
            this.Controls.Add(this.txtMessages);
            this.Name = "Form1";
            this.Text = "MM4 Raw Socket API Example";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtMessages;
        private System.Windows.Forms.Label lblMessages;
        private System.Windows.Forms.TextBox txtIPAddress;
        private System.Windows.Forms.TextBox txtPort;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnGetApplicationState;
        private System.Windows.Forms.Button btnGetVariable;
        private System.Windows.Forms.Button btnSetVariable;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtVariableValue;
        private System.Windows.Forms.TextBox txtVariableName;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtUserOrPassword;
        private System.Windows.Forms.Button btnNotificationListenerControl;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtListenerPort;
        private System.Windows.Forms.TextBox txtListenerIPAddress;
        private System.Windows.Forms.Button btnWatchVariable;
        private System.Windows.Forms.Button btnUnwatchVariable;
        private System.Windows.Forms.Button btnUnwatchMethod;
        private System.Windows.Forms.Button btnWatchMethod;
        private System.Windows.Forms.Button btnConnectHardware;
    }
}

