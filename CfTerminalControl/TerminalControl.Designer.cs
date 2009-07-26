namespace CfTerminalControl
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
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
         this.BackColor = System.Drawing.Color.Black;
         this.Font = new System.Drawing.Font( "Courier New", 9F, System.Drawing.FontStyle.Regular );
         this.ForeColor = System.Drawing.Color.White;
         this.Name = "TerminalControl";
         this.OnPaintBuffered += new System.Windows.Forms.PaintEventHandler( this.TerminalControl_PaintBuffered );
         this.Resize += new System.EventHandler( this.TerminalControl_Resize );
         this.ResumeLayout( false );

      }

      #endregion
   }
}
