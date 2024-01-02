using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace calc
{
    public partial class Form1 : Form
    {
        private Button closeButton;
        private TextBox allOneTextBox;
        private Label draggableSpace; // This label will act as the draggable space

        public Form1()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.Black;
            allOneTextBox.KeyDown += AllOneTextBox_KeyDown;

            // Create the draggable space, same height as the TextBox and the same width as closeButton
            draggableSpace = new Label
            {
                Width = 30,
                Height = allOneTextBox.Height,
                Location = new Point(allOneTextBox.Right, 1), // position it right of the TextBox
                BackColor = this.BackColor // you can set a different color to make it visible during design
            };
            this.Controls.Add(draggableSpace);

            // Adjust the closeButton location to be to the right of the draggable space
            closeButton = new Button
            {
                Text = "X",
                ForeColor = Color.White,
                BackColor = Color.Red,
                Width = 30,
                Height = allOneTextBox.Height,
                Location = new Point(draggableSpace.Right, 0),
                TextAlign = ContentAlignment.MiddleCenter
            };
            closeButton.Click += (s, e) => this.Close();
            this.Controls.Add(closeButton);

            // Resize the form to fit the TextBox, draggableSpace, and closeButton with no extra space
            this.ClientSize = new Size(allOneTextBox.Width + closeButton.Width + draggableSpace.Width, allOneTextBox.Height);

            // Set the Anchor properties to keep controls at the right position on resize
            allOneTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            draggableSpace.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            closeButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;

            // Attach the mouse down event to the draggable space
            draggableSpace.MouseDown += new MouseEventHandler(Form_MouseDown);

            this.MouseDown += new MouseEventHandler(Form_MouseDown);
        }

        // Enter and Backspace key-bindings
        private void AllOneTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                PerformCalculation();
            }
            else if (e.KeyCode == Keys.Back)
            {
                e.SuppressKeyPress = true;
                allOneTextBox.Clear();
            }
        }

        private void PerformCalculation()
        {
            string input = allOneTextBox.Text;

            try
            {
                var resultObject = new System.Data.DataTable().Compute(input, null);
                long result;
                if (resultObject is decimal || resultObject is double)
                {
                    decimal decimalResult = Convert.ToDecimal(resultObject);
                    if (decimalResult % 1 == 0) // Check if it's a whole number
                    {
                        result = Convert.ToInt64(decimalResult); // Explicitly cast to Int64
                        allOneTextBox.Text = result.ToString();
                    }
                    else
                    {
                        allOneTextBox.Text = decimalResult.ToString(); // Keep as decimal
                    }
                }
                else
                {
                    allOneTextBox.Text = resultObject.ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                allOneTextBox.Clear();
            }
        }



        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            // Adjust form size if necessary
            this.Size = new Size(allOneTextBox.Width + closeButton.Width + draggableSpace.Width, allOneTextBox.Height);
        }


        private void Form_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HT_CAPTION = 0x2;

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();
    }
}