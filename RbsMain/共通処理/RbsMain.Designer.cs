namespace RbsMain
{
    partial class RbsMain
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージド リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.panel1 = new System.Windows.Forms.Panel();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.timerログ監視 = new System.Windows.Forms.Timer(this.components);
            this.timer集配信監視 = new System.Windows.Forms.Timer(this.components);
            this.timerRBSD010 = new System.Windows.Forms.Timer(this.components);
            this.timerRBSD020 = new System.Windows.Forms.Timer(this.components);
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.textBox1);
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(878, 544);
            this.panel1.TabIndex = 3;
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.textBox1.Location = new System.Drawing.Point(0, 0);
            this.textBox1.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox1.Size = new System.Drawing.Size(878, 544);
            this.textBox1.TabIndex = 0;
            this.textBox1.TabStop = false;
            // 
            // timerログ監視
            // 
            this.timerログ監視.Interval = 60000;
            this.timerログ監視.Tick += new System.EventHandler(this.timerログ監視_Tick);
            // 
            // timer集配信監視
            // 
            this.timer集配信監視.Interval = 1000;
            this.timer集配信監視.Tick += new System.EventHandler(this.timer集配信監視_Tick);
            // 
            // timerRBSD010
            // 
            this.timerRBSD010.Interval = 1000;
            this.timerRBSD010.Tick += new System.EventHandler(this.timerRBSD010_Tick);
            // 
            // timerRBSD020
            // 
            this.timerRBSD020.Interval = 1000;
            this.timerRBSD020.Tick += new System.EventHandler(this.timerRBSD020_Tick);
            // 
            // RbsMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(878, 544);
            this.Controls.Add(this.panel1);
            this.Name = "RbsMain";
            this.Text = "RBSインターフェース";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.RbsClosing);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Timer timerログ監視;
        private System.Windows.Forms.Timer timer集配信監視;
        private System.Windows.Forms.Timer timerRBSD010;
        private System.Windows.Forms.Timer timerRBSD020;
    }
}

