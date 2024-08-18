namespace LineSwapper
{
    public partial class Form1 : Form
    {
        private string? currentFilePath;
        private bool isModified = false;
        private List<string> recentFiles = new List<string>();
        private readonly string RecentFilesPath = Path.Combine(Application.StartupPath, "recentFiles.txt");
        private System.Windows.Forms.Timer doubleClickTimer;
        private bool isDoubleClick = false;

        public Form1()
        {
            InitializeComponent();
            UpdateButtonStates();
            LoadRecentFiles();
            this.FormClosing += Form1_FormClosing;
            listBox1.Enter += ListBox_Enter;
            listBox2.Enter += ListBox_Enter;
            listBox1.SelectedIndexChanged += ListBox_SelectedIndexChanged;
            listBox2.SelectedIndexChanged += ListBox_SelectedIndexChanged;
            listBox1.MouseDoubleClick += listBox1_MouseDoubleClick;
            listBox2.MouseDoubleClick += listBox2_MouseDoubleClick;

            doubleClickTimer = new System.Windows.Forms.Timer();
            doubleClickTimer.Interval = SystemInformation.DoubleClickTime;
            doubleClickTimer.Tick += DoubleClickTimer_Tick;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Shown += Form1_Shown;
        }

        private void Form1_Shown(object? sender, EventArgs e)
        {
            UpdateButtonStates();
        }

        private void UpdateButtonStates()
        {
            button1.Enabled = listBox1.SelectedItem != null;
            button2.Enabled = listBox1.Items.Count > 0;
            button3.Enabled = listBox2.Items.Count > 0;
            button4.Enabled = listBox2.SelectedItem != null;
        }

        private void ListBox_SelectedIndexChanged(object? sender, EventArgs e)
        {
            UpdateButtonStates();
        }

        private void MarkAsModified()
        {
            isModified = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MoveSelectedItems(listBox1, listBox2);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            MoveAllItems(listBox1, listBox2);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            MoveAllItems(listBox2, listBox1);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            MoveSelectedItems(listBox2, listBox1);
        }

        private void MoveSelectedItems(ListBox source, ListBox target)
        {
            var selectedItems = source.SelectedItems.Cast<object>().ToList();
            foreach (var item in selectedItems)
            {
                target.Items.Add(item);
                source.Items.Remove(item);
            }
            UpdateButtonStates();
            MarkAsModified();
        }

        private void MoveAllItems(ListBox source, ListBox target)
        {
            foreach (var item in source.Items)
            {
                target.Items.Add(item);
            }
            source.Items.Clear();
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
                    LoadFile(openFileDialog.FileName);
                }
            }
        }

        private void LoadFile(string filePath)
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

            AddToRecentFiles(currentFilePath);
            UpdateRecentFilesMenu();

            if (listBox1.Items.Count > 0)
            {
                listBox1.SelectedIndex = 0;
                listBox1.Focus();
            }

            UpdateButtonStates();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(currentFilePath))
            {
                SaveFile(currentFilePath);
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
                SaveFile(saveFileDialog1.FileName);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "<Pending>")]
        private void SaveFile(string filePath)
        {
            var linesListBox1 = listBox1.Items.Cast<string>().ToArray();
            System.IO.File.WriteAllLines(filePath, linesListBox1);
            currentFilePath = filePath;

            string stashedFilePath = System.IO.Path.Combine(
                System.IO.Path.GetDirectoryName(currentFilePath)!,
                System.IO.Path.GetFileNameWithoutExtension(currentFilePath) + ".stashed" + System.IO.Path.GetExtension(currentFilePath)
            );

            var linesListBox2 = listBox2.Items.Cast<string>().ToArray();
            System.IO.File.WriteAllLines(stashedFilePath, linesListBox2);

            toolStripStatusLabel1.Text = $"Saved file: {currentFilePath}";
            isModified = false;

            UpdateButtonStates();
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
                LoadFile(filePath);
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

        private void DoubleClickTimer_Tick(object? sender, EventArgs e)
        {
            doubleClickTimer.Stop();
            isDoubleClick = false;
        }

        private void listBox1_MouseDoubleClick(object? sender, MouseEventArgs e)
        {
            if (!isDoubleClick)
            {
                isDoubleClick = true;
                doubleClickTimer.Start();
                MoveItemOnDoubleClick(listBox1, listBox2, e);
            }
        }

        private void listBox2_MouseDoubleClick(object? sender, MouseEventArgs e)
        {
            if (!isDoubleClick)
            {
                isDoubleClick = true;
                doubleClickTimer.Start();
                MoveItemOnDoubleClick(listBox2, listBox1, e);
            }
        }

        private void MoveItemOnDoubleClick(ListBox source, ListBox target, MouseEventArgs e)
        {
            int index = source.IndexFromPoint(e.Location);
            if (index != ListBox.NoMatches)
            {
                var selectedItem = source.Items[index];
                source.Items.RemoveAt(index);
                target.Items.Add(selectedItem);
                UpdateButtonStates();
                MarkAsModified();
            }
        }

        private void ListBox_Enter(object? sender, EventArgs e)
        {
            if (sender == listBox1)
            {
                listBox2.ClearSelected();
            }
            else if (sender == listBox2)
            {
                listBox1.ClearSelected();
            }
            UpdateButtonStates();
        }
    }
}
