using System;
using System.IO;
using System.Windows.Forms;
using PdfiumViewer;

namespace PDFRenamer
{
    public class Form1 : Form
    {
        private readonly PdfViewer pdfViewer = new PdfViewer();
        private string _directoryPath;
        private string[] _pdfFiles;
        private int _currentPdfIndex;
        private PdfDocument _pdfDocument;
        private TextBox newNameTextBox;

        public Form1()
        {
            InitializeUI();
        }

        private void InitializeUI()
        {
            // PDF viewer
            pdfViewer.Dock = DockStyle.Fill;
            Controls.Add(pdfViewer);

            // Control panel
            var controlPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                FlowDirection = FlowDirection.LeftToRight,
                Height = 40,
                AutoSize = true
            };
            Controls.Add(controlPanel);

            // Open button
            var openButton = new Button { Text = "Open" };
            openButton.Click += OpenButton_Click;
            controlPanel.Controls.Add(openButton);


            // New name text box
            newNameTextBox = new TextBox { Width = 200 };
            controlPanel.Controls.Add(newNameTextBox);

            // Previous button
            var previousButton = new Button { Text = "Previous" };
            previousButton.Click += PreviousButton_Click;
            controlPanel.Controls.Add(previousButton);

            // Next button
            var nextButton = new Button { Text = "Next" };
            nextButton.Click += NextButton_Click;
            controlPanel.Controls.Add(nextButton);

            newNameTextBox.KeyDown += NewNameTextBox_KeyDown;

            // Allow drag and drop
            this.AllowDrop = true;
            this.DragEnter += Form1_DragEnter;
            this.DragDrop += Form1_DragDrop;

        }
        private void OpenButton_Click(object sender, EventArgs e)
        {
            using (var folderBrowserDialog = new FolderBrowserDialog())
            {
                if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    _directoryPath = folderBrowserDialog.SelectedPath;
                    _pdfFiles = Directory.GetFiles(_directoryPath, "*.pdf");
                    _currentPdfIndex = 0;

                    if (_pdfFiles.Length > 0)
                    {
                        OpenPdf(_pdfFiles[_currentPdfIndex]);
                    }
                }
            }
        }

        private void RenameButton_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(newNameTextBox.Text) && _pdfFiles[_currentPdfIndex] != null)
            {
                var newFileName = $"{newNameTextBox.Text}.pdf";
                var newFilePath = Path.Combine(Path.GetDirectoryName(_pdfFiles[_currentPdfIndex]), newFileName);

                if (!File.Exists(newFilePath))
                {
                    // Close the PDF document before renaming it
                    pdfViewer.Document?.Dispose();
                    _pdfDocument?.Dispose();

                    File.Move(_pdfFiles[_currentPdfIndex], newFilePath);
                    _pdfFiles[_currentPdfIndex] = newFilePath;
                    newNameTextBox.Clear();

                    // Reopen the renamed PDF document
                    OpenPdf(newFilePath);
                }
                else
                {
                    MessageBox.Show("Error: A file with that name already exists.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void NextButton_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(newNameTextBox.Text))
            {
                RenameButton_Click(sender, e);
            }

            if (_pdfFiles != null && _currentPdfIndex < _pdfFiles.Length - 1)
            {
                _currentPdfIndex++;
                OpenPdf(_pdfFiles[_currentPdfIndex]);
                newNameTextBox.Clear();
            }
            newNameTextBox.Focus();
        }

        private void PreviousButton_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(newNameTextBox.Text))
            {
                RenameButton_Click(sender, e);
            }

            if (_pdfFiles != null && _currentPdfIndex > 0)
            {
                _currentPdfIndex--;
                OpenPdf(_pdfFiles[_currentPdfIndex]);
                newNameTextBox.Clear();
            }
            newNameTextBox.Focus();
        }

        private void OpenPdf(string pdfFilePath)
        {
            _pdfDocument?.Dispose();
            _pdfDocument = PdfDocument.Load(pdfFilePath);
            pdfViewer.Document = _pdfDocument;
            this.Text = Path.GetFileName(pdfFilePath);
        }

        private void NewNameTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                NextButton_Click(sender, e);
            }
        }

        public void OpenPdfFromArgument(string filePath)
        {
            if (File.Exists(filePath) && Path.GetExtension(filePath).Equals(".pdf", StringComparison.OrdinalIgnoreCase))
            {
                OpenPdf(filePath);
            }
        }

        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            if (files.Length > 0)
            {
                string filePath = files[0];

                if (Path.GetExtension(filePath).Equals(".pdf", StringComparison.OrdinalIgnoreCase))
                {
                    OpenPdf(filePath);
                }
            }
        }
    }

}