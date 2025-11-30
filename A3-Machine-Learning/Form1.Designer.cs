namespace A3_Machine_Learning
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
            this.userInput = new System.Windows.Forms.TextBox();
            this.userLabel = new System.Windows.Forms.Label();
            this.outputBox = new System.Windows.Forms.RichTextBox();
            this.submit = new System.Windows.Forms.Button();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.trainingTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.checkBoxTrainModel = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // userInput
            // 
            this.userInput.Location = new System.Drawing.Point(138, 237);
            this.userInput.Name = "userInput";
            this.userInput.Size = new System.Drawing.Size(476, 22);
            this.userInput.TabIndex = 0;
            this.userInput.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // userLabel
            // 
            this.userLabel.AutoSize = true;
            this.userLabel.Location = new System.Drawing.Point(49, 237);
            this.userLabel.Name = "userLabel";
            this.userLabel.Size = new System.Drawing.Size(83, 16);
            this.userLabel.TabIndex = 1;
            this.userLabel.Text = "Enter prompt";
            this.userLabel.Click += new System.EventHandler(this.label1_Click);
            // 
            // outputBox
            // 
            this.outputBox.Location = new System.Drawing.Point(52, 317);
            this.outputBox.Name = "outputBox";
            this.outputBox.ReadOnly = true;
            this.outputBox.Size = new System.Drawing.Size(699, 421);
            this.outputBox.TabIndex = 2;
            this.outputBox.Text = "";
            this.outputBox.TextChanged += new System.EventHandler(this.richTextBox1_TextChanged);
            // 
            // submit
            // 
            this.submit.Location = new System.Drawing.Point(627, 269);
            this.submit.Name = "submit";
            this.submit.Size = new System.Drawing.Size(124, 23);
            this.submit.TabIndex = 3;
            this.submit.Text = "Submit";
            this.submit.UseVisualStyleBackColor = true;
            this.submit.Click += new System.EventHandler(this.button1_Click);
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.Checked = true;
            this.radioButton1.Location = new System.Drawing.Point(74, 51);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(214, 20);
            this.radioButton1.TabIndex = 4;
            this.radioButton1.TabStop = true;
            this.radioButton1.Text = "Train New Model and Use That";
            this.radioButton1.UseVisualStyleBackColor = true;
            // 
            // radioButton2
            // 
            this.radioButton2.AutoSize = true;
            this.radioButton2.Location = new System.Drawing.Point(74, 72);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(200, 20);
            this.radioButton2.TabIndex = 5;
            this.radioButton2.Text = "Load Existing External Model";
            this.radioButton2.UseVisualStyleBackColor = true;
            // 
            // trainingTextBox
            // 
            this.trainingTextBox.Enabled = false;
            this.trainingTextBox.Location = new System.Drawing.Point(517, 270);
            this.trainingTextBox.Name = "trainingTextBox";
            this.trainingTextBox.Size = new System.Drawing.Size(97, 22);
            this.trainingTextBox.TabIndex = 6;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(358, 273);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(136, 16);
            this.label1.TabIndex = 7;
            this.label1.Text = "Training Label (0 or 1)";
            this.label1.Click += new System.EventHandler(this.label1_Click_1);
            // 
            // checkBoxTrainModel
            // 
            this.checkBoxTrainModel.AutoSize = true;
            this.checkBoxTrainModel.Location = new System.Drawing.Point(253, 273);
            this.checkBoxTrainModel.Name = "checkBoxTrainModel";
            this.checkBoxTrainModel.Size = new System.Drawing.Size(108, 20);
            this.checkBoxTrainModel.TabIndex = 8;
            this.checkBoxTrainModel.Text = "Train Model?";
            this.checkBoxTrainModel.UseVisualStyleBackColor = true;
            this.checkBoxTrainModel.CheckedChanged += new System.EventHandler(this.checkBoxTrainModel_CheckedChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(821, 776);
            this.Controls.Add(this.checkBoxTrainModel);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.trainingTextBox);
            this.Controls.Add(this.radioButton2);
            this.Controls.Add(this.radioButton1);
            this.Controls.Add(this.submit);
            this.Controls.Add(this.outputBox);
            this.Controls.Add(this.userLabel);
            this.Controls.Add(this.userInput);
            this.Name = "Form1";
            this.Text = "Sentiment Analysis - ML Model Trainer";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox userInput;
        private System.Windows.Forms.Label userLabel;
        private System.Windows.Forms.RichTextBox outputBox;
        private System.Windows.Forms.Button submit;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.RadioButton radioButton2;
        private System.Windows.Forms.TextBox trainingTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox checkBoxTrainModel;
    }
}

