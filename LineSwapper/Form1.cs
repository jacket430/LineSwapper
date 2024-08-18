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
            // Move all selected items to the right
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
            // Move all items to the right
            foreach (var item in listBox1.Items)
            {
                listBox2.Items.Add(item);
            }
            listBox1.Items.Clear();
            UpdateButtonStates();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // Move all items to the left
            foreach (var item in listBox2.Items)
            {
                listBox1.Items.Add(item);
            }
            listBox2.Items.Clear();
            UpdateButtonStates();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            // Move all selected items to the left
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
                openFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // Get the path of the selected file
                    currentFilePath = openFileDialog.FileName;

                    // Load the contents of the selected file into listBox1
                    var linesListBox1 = System.IO.File.ReadAllLines(currentFilePath);
                    listBox1.Items.Clear();
                    listBox1.Items.AddRange(linesListBox1);

                    // Create the stashed file path
                    string stashedFilePath = System.IO.Path.Combine(
                        System.IO.Path.GetDirectoryName(currentFilePath)!,
                        System.IO.Path.GetFileNameWithoutExtension(currentFilePath) + ".stashed" + System.IO.Path.GetExtension(currentFilePath)
                    );

                    // Check if the stashed file exists
                    if (System.IO.File.Exists(stashedFilePath))
                    {
                        // Load the contents of the stashed file into listBox2
                        var linesListBox2 = System.IO.File.ReadAllLines(stashedFilePath);
                        listBox2.Items.Clear();
                        listBox2.Items.AddRange(linesListBox2);
                    }
                    else
                    {
                        // Clear listBox2 if no stashed file is found
                        listBox2.Items.Clear();
                    }
                }
            }
        }

        private void openFileDialog1_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Get the selected file path
            currentFilePath = openFileDialog1.FileName;

            // Read all lines from the file
            string[] lines = System.IO.File.ReadAllLines(currentFilePath);

            // Clear existing items in listBox1
            listBox1.Items.Clear();
            listBox2.Items.Clear();

            // Add each line to listBox1
            foreach (string line in lines)
            {
                listBox1.Items.Add(line);
            }

            // Update button states
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
                // Get all items from listBox1
                var linesListBox1 = listBox1.Items.Cast<string>().ToArray();

                // Write all lines to the original file
                System.IO.File.WriteAllLines(currentFilePath, linesListBox1);

                // Create the stashed file path
                string stashedFilePath = System.IO.Path.Combine(
                    System.IO.Path.GetDirectoryName(currentFilePath)!,
                    System.IO.Path.GetFileNameWithoutExtension(currentFilePath) + ".stashed" + System.IO.Path.GetExtension(currentFilePath)
                );

                // Get all items from listBox2
                var linesListBox2 = listBox2.Items.Cast<string>().ToArray();

                // Write all lines to the stashed file
                System.IO.File.WriteAllLines(stashedFilePath, linesListBox2);
            }
            else
            {
                MessageBox.Show("No file is currently loaded.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                // Get the selected file path
                string filePath = saveFileDialog1.FileName;

                // Get all items from listBox1
                var lines = listBox1.Items.Cast<string>().ToArray();

                // Write all lines to the file
                System.IO.File.WriteAllLines(filePath, lines);

                // Update the current file path
                currentFilePath = filePath;
            }
        }
    }
}
