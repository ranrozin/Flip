using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Flip
{
    public partial class Form1 : Form
    {
        public delegate void SetControlCallback();
        public ImageMan im;
        private ComboBox cmbFlip;
        private Label label1;
        private Button btnExit;
        private Button btnRevert;
        private volatile bool _shouldStop = true;
        public Form1()
        {
            Enroute.enr = Enroute.getEnroute();
            if (Enroute.enr == null)
            {
                MessageBox.Show("This Application can't work without EnRoute opened");
                Exit();
            }
            Enroute.drawing = Enroute.enr.ActiveDrawing;
            InitializeComponent();
            im = new ImageMan();
            Thread th = new Thread(CheckIfSelection);
            th.Start();

            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.TopMost = true;
            ToolTip toolTip1 = new ToolTip();
            toolTip1.AutoPopDelay = 5000;
            toolTip1.InitialDelay = 1000;
            toolTip1.ReshowDelay = 500;
            toolTip1.BackColor = Color.LightYellow;
            toolTip1.IsBalloon = true;
            // Force the ToolTip text to be displayed whether or not the form is active.
            toolTip1.ShowAlways = true;

            this.TopMost = true;
            // Set up the ToolTip text for the Button and Checkbox.
            toolTip1.SetToolTip(this.label1, "Flip image horizontally or vertically");
            cmbFlip.SelectedIndex = 0;

        }
        private void Exit()
        {
            _shouldStop = false;
            Application.Exit();

        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Exit();
        }

        private Image getImageFromClipboard()
        {
            Image cpImage = null;
            if (Clipboard.ContainsImage())
            {
                cpImage = Clipboard.GetImage();
            }
            return cpImage;
        }

        private bool CheckIfImageExist()

        {
            if (Clipboard.ContainsImage())
            {
                return true;
            }
            return false;
        }

        private void CheckIfSelection()
        {
            bool locked = false;
            SetControlCallback enable = new SetControlCallback(enableAllElements);
            SetControlCallback disable = new SetControlCallback(lockAllElements);
            while (_shouldStop)
            {
                try {

                    Enroute.drawing = Enroute.enr.ActiveDrawing;
                    Enroute.selection = Enroute.drawing.Selection;

                    if (Enroute.selection != null && Enroute.selection.Count == 0)
                    {
                        this.Invoke(disable);
                        locked = true;

                    }
                    else
                    {
                        this.Invoke(enable);
                        locked = false;

                    }
                    Thread.Sleep(1000);
                }
                catch
                {
                    if (!locked)
                    {
                        try
                        {
                            this.Invoke(disable);
                            locked = true;
                        }
                        catch { }
                    }

                }

            }
        }

        private void lockAllElements()
        {
            foreach (Control c in this.Controls)
            {
                if (!(c is Label || c.Name == "btnExit"))
                {
                    c.Enabled = false;
                }
            }
        }
        private void enableAllElements()
        {
            foreach (Control c in this.Controls)
            {
                if (!(c is Label))
                {
                    c.Enabled = true;
                }
            }
        }

        private void btnRevert_Click(object sender, EventArgs e)
        {
            if (im.OrgIamge != null)
            {
                Enroute.selection.Cut();
                Clipboard.SetImage(im.OrgIamge);
                Enroute.selection.Paste();
                //im.Reset();
            }

        }

        private void Flip(string name)
        {
            Cursor.Current = Cursors.WaitCursor;
            Enroute.drawing = Enroute.enr.ActiveDrawing;
            if (Enroute.drawing == null)
            // if there is no active drawing create one.
            {
                MessageBox.Show("Please open a document first ");
            }
            Enroute.selection = Enroute.drawing.Selection;
            //Enroute.selection.SelectAll();
            Enroute.selection.Copy();
            Image cpImage = getImageFromClipboard();
            if (cpImage != null)
            {
                im.Load(Utils.imageToByteArray(cpImage));
                switch (name)
                {
                    case "Horizontal":
                        im.FlipMe(false, false);
                        break;
                    case "Vertical":
                        im.FlipMe(true, false);
                        break;
                    case "Both":
                        im.FlipMe(true, true);
                        break;
                }
                Clipboard.SetImage(im.getImage());
                Enroute.selection.DeleteMembers();
                Enroute.selection.Paste();
            }
            else
            {
                MessageBox.Show("Please verify that an Image is selected");
            }
            Cursor.Current = Cursors.Default;

        }
   
        private void cmbFlip_SelectedIndexChanged(object sender, EventArgs e)
        {
            string val = cmbFlip.GetItemText(cmbFlip.SelectedItem);
            if (!string.IsNullOrEmpty(val) && val != "Select Flip")
            {
                Flip(val);
            }

        }

        private void btnRevert_Click_1(object sender, EventArgs e)
        {
            if (!(im.OrgIamge == null))
            {
                Enroute.selection.Cut();
                Clipboard.SetImage(im.OrgIamge);
                Enroute.selection.Paste();
            }

        }

        private void InitializeComponent()
        {
            this.cmbFlip = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnExit = new System.Windows.Forms.Button();
            this.btnRevert = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // cmbFlip
            // 
            this.cmbFlip.FormattingEnabled = true;
            this.cmbFlip.Items.AddRange(new object[] {
            "Horizontal",
            "Vertical",
            "Both"});
            this.cmbFlip.Location = new System.Drawing.Point(16, 32);
            this.cmbFlip.Name = "cmbFlip";
            this.cmbFlip.Size = new System.Drawing.Size(101, 28);
            this.cmbFlip.TabIndex = 0;
            this.cmbFlip.Text = "Select flip";
            this.cmbFlip.SelectedIndexChanged += new System.EventHandler(this.cmbFlip_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(145, 20);
            this.label1.TabIndex = 1;
            this.label1.Text = "Flip selected image";
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(133, 66);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(87, 25);
            this.btnExit.TabIndex = 2;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // btnRevert
            // 
            this.btnRevert.Location = new System.Drawing.Point(135, 32);
            this.btnRevert.Name = "btnRevert";
            this.btnRevert.Size = new System.Drawing.Size(85, 25);
            this.btnRevert.TabIndex = 3;
            this.btnRevert.Text = "Revert";
            this.btnRevert.UseVisualStyleBackColor = true;
            this.btnRevert.Click += new System.EventHandler(this.btnRevert_Click);
            // 
            // Form1
            // 
            this.ClientSize = new System.Drawing.Size(232, 103);
            this.Controls.Add(this.btnRevert);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cmbFlip);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Flip image";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

    }
}
