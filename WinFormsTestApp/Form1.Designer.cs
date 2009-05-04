namespace WinFormsTestApp
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
		protected override void Dispose( bool disposing )
		{
			if( disposing && (components != null) )
			{
				components.Dispose();
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.m_terminal = new WinFormsTerminalControl.TerminalControl();
			this.SuspendLayout();
			// 
			// m_terminal
			// 
			this.m_terminal.BackColor = System.Drawing.Color.Black;
			this.m_terminal.Dock = System.Windows.Forms.DockStyle.Fill;
			this.m_terminal.Font = new System.Drawing.Font( "Courier New", 9F );
			this.m_terminal.ForeColor = System.Drawing.Color.White;
			this.m_terminal.Location = new System.Drawing.Point( 0, 0 );
			this.m_terminal.Margin = new System.Windows.Forms.Padding( 4 );
			this.m_terminal.Name = "m_terminal";
			this.m_terminal.Size = new System.Drawing.Size( 784, 564 );
			this.m_terminal.TabIndex = 0;
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size( 784, 564 );
			this.Controls.Add( this.m_terminal );
			this.Name = "Form1";
			this.Text = "WinForms Terminal";
			this.ResumeLayout( false );

		}

		#endregion

		private WinFormsTerminalControl.TerminalControl m_terminal;
	}
}

