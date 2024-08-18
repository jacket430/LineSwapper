namespace LineSwapper
{
    public partial class Form1 : Form
    {
        private string? currentFilePath;

        public Form1()
        {
            InitializeComponent();
            UpdateButtonStates();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Shown += new EventHandler(Form1_Shown);
        }

        private void Form1_Shown(object? sender, EventArgs e)
        {
            UpdateButtonStates();
            listBox1.SelectedIndexChanged += ListBox1_SelectedIndexChanged;
            listBox2.SelectedIndexChanged += ListBox2_SelectedIndexChanged;
        }

        private void UpdateButtonStates()
        {
            button1.Enabled = listBox1.SelectedItem != null;
            button2.Enabled = listBox1.Items.Count > 0;
            button3.Enabled = listBox2.Items.Count > 0;
            button4.Enabled = listBox2.SelectedItem != null;
        }

        private void ListBox1_SelectedIndexChanged(object? sender, EventArgs e)
        {
            UpdateButtonStates();
        }

        private void ListBox2_SelectedIndexChanged(object? sender, EventArgs e)
        {
            UpdateButtonStates();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var selectedItems = listBox1.SelectedItems.Cast<object>().ToList();
            foreach (var item in selectedItems)
            {
                listBox2.Items.Add(item);
                listBox1.Items.Remove(item);
            }
            UpdateButtonStates();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            foreach (var item in listBox1.Items)
            {
                listBox2.Items.Add(item);
            }
            listBox1.Items.Clear();
            UpdateButtonStates();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            foreach (var item in listBox2.Items)
            {
                listBox1.Items.Add(item);
            }
            listBox2.Items.Clear();
            UpdateButtonStates();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            var selectedItems = listBox2.SelectedItems.Cast<object>().ToList();
            foreach (var item in selectedItems)
            {
                listBox1.Items.Add(item);
                listBox2.Items.Remove(item);
            }
            UpdateButtonStates();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Text files (*.txt)|*.txt";
                openFileDialog.FileOk += (s, args) =>
                {
                    if (openFileDialog.FileName.EndsWith(".stashed.txt"))
                    {
                        MessageBox.Show("Open the original file.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        args.Cancel = true;
                    }
                };

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    currentFilePath = openFileDialog.FileName;
                    var linesListBox1 = System.IO.File.ReadAllLines(currentFilePath);
                    listBox1.Items.Clear();
                    listBox1.Items.AddRange(linesListBox1);

                    string stashedFilePath = System.IO.Path.Combine(
                        System.IO.Path.GetDirectoryName(currentFilePath)!,
                        System.IO.Path.GetFileNameWithoutExtension(currentFilePath) + ".stashed" + System.IO.Path.GetExtension(currentFilePath)
                    );

                    if (System.IO.File.Exists(stashedFilePath))
                    {
                        var linesListBox2 = System.IO.File.ReadAllLines(stashedFilePath);
                        listBox2.Items.Clear();
                        listBox2.Items.AddRange(linesListBox2);
                    }
                    else
                    {
                        listBox2.Items.Clear();
                    }
                }
            }
        }

        private void openFileDialog1_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            currentFilePath = openFileDialog1.FileName;
            string[] lines = System.IO.File.ReadAllLines(currentFilePath);
            listBox1.Items.Clear();
            listBox2.Items.Clear();
            foreach (string line in lines)
            {
                listBox1.Items.Add(line);
            }
            UpdateButtonStates();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void fileToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(currentFilePath))
            {
                var linesListBox1 = listBox1.Items.Cast<string>().ToArray();
                System.IO.File.WriteAllLines(currentFilePath, linesListBox1);

                string stashedFilePath = System.IO.Path.Combine(
                    System.IO.Path.GetDirectoryName(currentFilePath)!,
                    System.IO.Path.GetFileNameWithoutExtension(currentFilePath) + ".stashed" + System.IO.Path.GetExtension(currentFilePath)
                );

                var linesListBox2 = listBox2.Items.Cast<string>().ToArray();
                System.IO.File.WriteAllLines(stashedFilePath, linesListBox2);
            }
            else
            {
                MessageBox.Show("No file is currently loaded.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Filter = "Text files (*.txt)|*.txt";
            saveFileDialog1.FileOk += (s, args) =>
            {
                if (saveFileDialog1.FileName.EndsWith(".stashed.txt"))
                {
                    MessageBox.Show("Cannot save as .stashed.txt files.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    args.Cancel = true;
                }
            };

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string filePath = saveFileDialog1.FileName;
                var linesListBox1 = listBox1.Items.Cast<string>().ToArray();
                System.IO.File.WriteAllLines(filePath, linesListBox1);
                currentFilePath = filePath;

                string stashedFilePath = System.IO.Path.Combine(
                    System.IO.Path.GetDirectoryName(currentFilePath)!,
                    System.IO.Path.GetFileNameWithoutExtension(currentFilePath) + ".stashed" + System.IO.Path.GetExtension(currentFilePath)
                );

                var linesListBox2 = listBox2.Items.Cast<string>().ToArray();
                System.IO.File.WriteAllLines(stashedFilePath, linesListBox2);
            }
        }
    }
}
