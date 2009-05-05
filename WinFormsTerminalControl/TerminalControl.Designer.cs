namespace WinFormsTerminalControl
{
	partial class TerminalControl
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

		#region Component Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.SuspendLayout();
			// 
			// TerminalControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF( 8F, 16F );
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.Black;
			this.Font = new System.Drawing.Font( "Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)) );
			this.ForeColor = System.Drawing.Color.White;
			this.Margin = new System.Windows.Forms.Padding( 4 );
			this.Name = "TerminalControl";
			this.Size = new System.Drawing.Size( 365, 309 );
			this.Load += new System.EventHandler( this.TerminalControl_Load );
			this.Paint += new System.Windows.Forms.PaintEventHandler( this.TerminalControl_Paint );
			this.Resize += new System.EventHandler( this.TerminalControl_Resize );
			this.ResumeLayout( false );

		}

		#endregion
	}
}
