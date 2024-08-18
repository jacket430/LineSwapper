namespace LineSwapper
{
    public partial class Form1 : Form
    {
        private string? currentFilePath;
        private bool isModified = false;
        private List<string> recentFiles = new List<string>();
        private const string RecentFilesPath = "recentFiles.txt";

        public Form1()
        {
            InitializeComponent();
            UpdateButtonStates();
            LoadRecentFiles();
            this.FormClosing += new FormClosingEventHandler(Form1_FormClosing);
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

        private void MarkAsModified()
        {
            isModified = true;
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
            MarkAsModified();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            foreach (var item in listBox1.Items)
            {
                listBox2.Items.Add(item);
            }
            listBox1.Items.Clear();
            UpdateButtonStates();
            MarkAsModified();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            foreach (var item in listBox2.Items)
            {
                listBox1.Items.Add(item);
            }
            listBox2.Items.Clear();
            UpdateButtonStates();
            MarkAsModified();
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
            MarkAsModified();
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
                        MessageBox.Show("Cannot open .stashed.txt files.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

                    toolStripStatusLabel1.Text = $"Loaded file: {currentFilePath}";
                    isModified = false;

                    AddToRecentFiles(currentFilePath);
                    UpdateRecentFilesMenu();

                    if (listBox1.Items.Count > 0)
                    {
                        listBox1.SelectedIndex = 0;
                        listBox1.Focus();
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
            toolStripStatusLabel1.Text = $"Loaded file: {currentFilePath}";
            isModified = false;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
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

                toolStripStatusLabel1.Text = $"Saved file: {currentFilePath}";
                isModified = false;
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

                toolStripStatusLabel1.Text = $"Saved file as: {currentFilePath}";
                isModified = false;
            }
        }

        private void Form1_FormClosing(object? sender, FormClosingEventArgs e)
        {
            if (isModified)
            {
                var result = MessageBox.Show("You have unsaved changes. Do you want to save before exiting?", "Unsaved Changes", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    saveToolStripMenuItem_Click(this, e);
                }
                else if (result == DialogResult.Cancel)
                {
                    e.Cancel = true;
                }
            }
            SaveRecentFiles();
        }

        private void AddToRecentFiles(string filePath)
        {
            if (recentFiles.Contains(filePath))
            {
                recentFiles.Remove(filePath);
            }
            recentFiles.Insert(0, filePath);
            if (recentFiles.Count > 10)
            {
                recentFiles.RemoveAt(10);
            }
        }

        private void UpdateRecentFilesMenu()
        {
            openRecentToolStripMenuItem.DropDownItems.Clear();
            openRecentToolStripMenuItem.Visible = recentFiles.Count > 0;
            foreach (var file in recentFiles)
            {
                var item = new ToolStripMenuItem(file);
                item.Click += (s, e) => OpenRecentFile(file);
                openRecentToolStripMenuItem.DropDownItems.Add(item);
            }
        }

        private void OpenRecentFile(string filePath)
        {
            if (System.IO.File.Exists(filePath))
            {
                currentFilePath = filePath;
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

                toolStripStatusLabel1.Text = $"Loaded file: {currentFilePath}";
                isModified = false;

                // Auto focus the first item in listBox1
                if (listBox1.Items.Count > 0)
                {
                    listBox1.SelectedIndex = 0;
                    listBox1.Focus();
                }
            }
            else
            {
                MessageBox.Show("File not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                recentFiles.Remove(filePath);
                UpdateRecentFilesMenu();
            }
        }

        private void SaveRecentFiles()
        {
            System.IO.File.WriteAllLines(RecentFilesPath, recentFiles);
        }

        private void LoadRecentFiles()
        {
            if (System.IO.File.Exists(RecentFilesPath))
            {
                recentFiles = System.IO.File.ReadAllLines(RecentFilesPath).ToList();
                UpdateRecentFilesMenu();
            }
        }

        private void listBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int index = listBox1.IndexFromPoint(e.Location);
            if (index != ListBox.NoMatches)
            {
                var selectedItem = listBox1.Items[index];
                listBox2.Items.Add(selectedItem);
                listBox1.Items.RemoveAt(index);
                UpdateButtonStates();
                MarkAsModified();
            }
        }

        private void listBox2_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int index = listBox2.IndexFromPoint(e.Location);
            if (index != ListBox.NoMatches)
            {
                var selectedItem = listBox2.Items[index];
                listBox1.Items.Add(selectedItem);
                listBox2.Items.RemoveAt(index);
                UpdateButtonStates();
                MarkAsModified();
            }
        }

        private void listBox2_Enter(object sender, EventArgs e)
        {
            if (sender == listBox1)
            {
                listBox2.ClearSelected();
            }
            else if (sender == listBox2)
            {
                listBox1.ClearSelected();
            }
        }

        private void listBox1_Enter(object sender, EventArgs e)
        {
            if (sender == listBox1)
            {
                listBox2.ClearSelected();
            }
            else if (sender == listBox2)
            {
                listBox1.ClearSelected();
            }
        }
    }
}
