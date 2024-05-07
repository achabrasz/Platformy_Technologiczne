


using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using Application = System.Windows.Application;
using MessageBox = System.Windows.Forms.MessageBox;

namespace Lab8
{
    public class Program
    {
        [STAThread]
        public static void Main()
        {
            Application app = new Application();
            app.Run(new MainWindow());
        }
    }
    public partial class MainWindow : Window
    {
        private string selectedFolder = "";

        public MainWindow()
        {
            InitializeComponent();
            InitializeTreeView();
        }

        private void InitializeTreeView()
        {
            TreeViewItem root = new TreeViewItem
            {
                Header = "Root",
                Tag = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory)
            };
            treeView.Items.Add(root);
            PopulateTreeView(root, /*(string)root.Tag*/ "C:\\Users\\Adam\\Studia\\4 semestr\\Platformy Technologiczne\\Platformy_Technologiczne\\Lab8");
        }
        private void PopulateTreeView(TreeViewItem item, string path)
        {
            try
            {
                string[] directories = Directory.GetDirectories(path);
                foreach (string directory in directories)
                {
                    TreeViewItem subItem = new TreeViewItem
                    {
                        Header = Path.GetFileName(directory),
                        Tag = directory
                    };
                    item.Items.Add(subItem);
                    PopulateTreeView(subItem, directory);
                }
                string[] files = Directory.GetFiles(path);
                foreach (string file in files)
                {
                    TreeViewItem subItem = new TreeViewItem
                    {
                        Header = Path.GetFileName(file),
                        Tag = file
                    };
                    item.Items.Add(subItem);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while populating the tree: {ex.Message}", "Error", (MessageBoxButtons)MessageBoxButton.OK, (MessageBoxIcon)MessageBoxImage.Error);
            }
        }
        private void OpenMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (treeView.SelectedItem != null)
            {
                TreeViewItem selectedItem = (TreeViewItem)treeView.SelectedItem;
                string filePath = (string)selectedItem.Tag;
                if (File.Exists(filePath))
                {
                    try
                    {
                        string fileContent = File.ReadAllText(filePath);
                        txtFileContents.Text = fileContent;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"An error occurred while opening the file: {ex.Message}", "Error", (MessageBoxButtons)MessageBoxButton.OK, (MessageBoxIcon)MessageBoxImage.Error);
                    }
                }
                UpdateAttributesStatusBar(filePath);
            }
        }
        private void DeleteMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (treeView.SelectedItem != null)
            {
                TreeViewItem selectedItem = (TreeViewItem)treeView.SelectedItem;
                string path = (string)selectedItem.Tag;
                try
                {
                    if (File.Exists(path))
                    {
                        File.Delete(path);
                    }
                    else if (Directory.Exists(path))
                    {
                        Directory.Delete(path, true);
                    }
                    TreeViewItem parentItem = (TreeViewItem)selectedItem.Parent;
                    parentItem.Items.Remove(selectedItem);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred while deleting the item: {ex.Message}", "Error", (MessageBoxButtons)MessageBoxButton.OK, (MessageBoxIcon)MessageBoxImage.Error);
                }
            }
        }

        private void CreateMenuItem_Click(object sender, RoutedEventArgs e)
        {
            CreateItemWindow createWindow = new CreateItemWindow();
            if (createWindow.ShowDialog() == true)
            {
                string itemName = createWindow.ItemName;
                bool isFolder = createWindow.IsFolder;
                string attributes = createWindow.Attributes;

                string newPath = Path.Combine(selectedFolder, itemName);
                try
                {
                    if (isFolder)
                    {
                        DirectoryInfo directoryInfo = Directory.CreateDirectory(newPath);
                        if (!string.IsNullOrEmpty(attributes))
                        {
                            ApplyAttributes(directoryInfo, attributes);
                        }
                    }
                    else
                    {
                        File.WriteAllText(newPath, "");
                        if (!string.IsNullOrEmpty(attributes))
                        {
                            File.SetAttributes(newPath, GetFileAttributes(attributes));
                        }
                    }

                    TreeViewItem newItem = new TreeViewItem
                    {
                        Header = itemName,
                        Tag = newPath
                    };
                    TreeViewItem selectedTreeViewItem = (TreeViewItem)treeView.SelectedItem;
                    selectedTreeViewItem.Items.Add(newItem);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred while creating the item: {ex.Message}", "Error", (MessageBoxButtons)MessageBoxButton.OK, (MessageBoxIcon)MessageBoxImage.Error);
                }
            }
        }

        private void ApplyAttributes(FileSystemInfo info, string attributes)
        {
            // Set DOS attributes for directory
            foreach (char attribute in attributes)
            {
                switch (attribute)
                {
                    case 'r':
                        info.Attributes |= FileAttributes.ReadOnly;
                        break;
                    case 'a':
                        info.Attributes |= FileAttributes.Archive;
                        break;
                    case 's':
                        info.Attributes |= FileAttributes.System;
                        break;
                    case 'h':
                        info.Attributes |= FileAttributes.Hidden;
                        break;
                }
            }
        }

        private FileAttributes GetFileAttributes(string attributes)
        {
            FileAttributes fileAttributes = FileAttributes.Normal;
            foreach (char attribute in attributes)
            {
                switch (attribute)
                {
                    case 'r':
                        fileAttributes |= FileAttributes.ReadOnly;
                        break;
                    case 'a':
                        fileAttributes |= FileAttributes.Archive;
                        break;
                    case 's':
                        fileAttributes |= FileAttributes.System;
                        break;
                    case 'h':
                        fileAttributes |= FileAttributes.Hidden;
                        break;
                }
            }
            return fileAttributes;
        }


        private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            TreeViewItem selectedItem = (TreeViewItem)treeView.SelectedItem;
            if (selectedItem != null && Directory.Exists((string)selectedItem.Tag))
            {
                selectedFolder = (string)selectedItem.Tag;
            }
        }
        private void UpdateAttributesStatusBar(string filePath)
        {
            if (File.Exists(filePath) || Directory.Exists(filePath))
            {
                string attributes = GetDosAttributes(filePath);
                attributesTextBlock.Text = "Attributes: " + attributes;
            }
        }

        private string GetDosAttributes(string path)
        {
            FileAttributes attributes = File.GetAttributes(path);

            StringBuilder sb = new StringBuilder();
            sb.Append((attributes & FileAttributes.ReadOnly) != 0 ? "r" : "-");
            sb.Append((attributes & FileAttributes.Archive) != 0 ? "a" : "-");
            sb.Append((attributes & FileAttributes.System) != 0 ? "s" : "-");
            sb.Append((attributes & FileAttributes.Hidden) != 0 ? "h" : "-");

            return sb.ToString();
        }

    }
    public partial class CreateItemWindow : Window
    {
        public string ItemName { get; private set; }
        public bool IsFolder { get; private set; }
        public string Attributes { get; private set; }

        public CreateItemWindow()
        {
            InitializeComponent();
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            ItemName = ItemNameTextBox.Text;
            IsFolder = (ItemTypeComboBox.SelectedIndex == 0); // 0 for folder, 1 for file
            StringBuilder sb = new StringBuilder();
            if (ReadOnlyCheckBox.IsChecked == true)
                sb.Append("r");
            if (ArchiveCheckBox.IsChecked == true)
                sb.Append("a");
            if (SystemCheckBox.IsChecked == true)
                sb.Append("s");
            if (HiddenCheckBox.IsChecked == true)
                sb.Append("h");

            Attributes = sb.ToString();

            if (string.IsNullOrWhiteSpace(ItemName))
            {
                MessageBox.Show("Please enter a valid name for the item.", "Error", (MessageBoxButtons)MessageBoxButton.OK, (MessageBoxIcon)MessageBoxImage.Error);
                return;
            }

            string pattern = @"^[\w\-.~]{1,8}\.(txt|php|html)$";
            bool isValidName = Regex.IsMatch(ItemName, pattern, RegexOptions.IgnoreCase);

            if (!isValidName && !IsFolder)
            {
                MessageBox.Show("Please enter a valid name for the file (basename.extension).\nValid extensions are: txt, php, html.", "Error", (MessageBoxButtons)MessageBoxButton.OK, (MessageBoxIcon)MessageBoxImage.Error);
                return;
            }

            DialogResult = true;
        }
    }
}
