namespace MathCalc.Gui
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            label1 = new Label();
            expression_input = new TextBox();
            calculate_button = new Button();
            label2 = new Label();
            result_output = new Label();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(10, 10);
            label1.Name = "label1";
            label1.Size = new Size(62, 15);
            label1.TabIndex = 0;
            label1.Text = "Expression";
            // 
            // expression_input
            // 
            expression_input.Location = new Point(150, 10);
            expression_input.Name = "expression_input";
            expression_input.Size = new Size(550, 23);
            expression_input.TabIndex = 1;
            expression_input.TextChanged += calculate_button_TextChanged;
            expression_input.KeyDown += calculate_button_KeyDown;
            // 
            // calculate_button
            // 
            calculate_button.Location = new Point(720, 10);
            calculate_button.Name = "calculate_button";
            calculate_button.Size = new Size(65, 23);
            calculate_button.TabIndex = 2;
            calculate_button.Text = "Calc";
            calculate_button.UseVisualStyleBackColor = true;
            calculate_button.Click += calculate_button_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(10, 40);
            label2.Name = "label2";
            label2.Size = new Size(39, 15);
            label2.TabIndex = 0;
            label2.Text = "Result";
            // 
            // result_output
            // 
            result_output.AutoSize = true;
            result_output.Location = new Point(150, 40);
            result_output.Name = "result_output";
            result_output.Size = new Size(0, 15);
            result_output.TabIndex = 3;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 68);
            Controls.Add(result_output);
            Controls.Add(calculate_button);
            Controls.Add(expression_input);
            Controls.Add(label2);
            Controls.Add(label1);
            Name = "Form1";
            Text = "Form1";
            KeyDown += Form1_KeyDown;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private TextBox expression_input;
        private Button calculate_button;
        private Label label2;
        private Label result_output;
    }
}
