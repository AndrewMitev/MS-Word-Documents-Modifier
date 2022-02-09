using System;
using System.Windows;

using WpfColorFontDialog;
using DocumentServices;


namespace DictionaryPrepare
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool _isAddingHeaders = true;
        private bool _isNumberedDictionary = false;
        private bool _isSpecificFontSelect = false;

        private string _fontFamily;
        private double _fontSize;

        public MainWindow()
        {
            InitializeComponent();
            AddHeader.IsChecked = true;
            NormalDictionary.IsChecked = true;
        }

        private void Drag_Drop(object sender, DragEventArgs e)
        {
            if (e.Data != null && e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // Note that you can have more than one file.
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                FileNameDisplay.Content = files[0];
                FileNameDisplay.Visibility = Visibility.Visible;

                GenerateButton.IsEnabled = true;
            }
        }

        private void AddHeader_Toggle(object sender, RoutedEventArgs e)
        {
            RemoveHeader.IsChecked = false;
            _isAddingHeaders = true;
        }

        private void RemoveHeader_Toggle(object sender, RoutedEventArgs e)
        {
            AddHeader.IsChecked = false;
            _isAddingHeaders = false;
        }

        private void NormalDictionary_Toggle(object sender, RoutedEventArgs e)
        {
            NumberedDictionary.IsChecked = false;
            _isNumberedDictionary = false;
        }

        private void NumberedDictionary_Toggle(object sender, RoutedEventArgs e)
        {
            NormalDictionary.IsChecked = false;
            _isNumberedDictionary = true;
        }

        private void Generate_Handler(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_isAddingHeaders)
                {
                    if (!_isSpecificFontSelect)
                    {
                        Document.AddHeadings(FileNameDisplay.Content.ToString(), _isNumberedDictionary);
                    }
                    else 
                    {
                        Document.AddHeadings(FileNameDisplay.Content.ToString(), _isNumberedDictionary, _fontFamily, _fontSize);
                    }
                }
                else
                {
                    Document.RemoveHeadings(FileNameDisplay.Content.ToString());
                }

                FileNameDisplay.Content = string.Empty;
                FileNameDisplay.Visibility = Visibility.Hidden;

                GenerateButton.IsEnabled = false;

                MessageBox.Show("Success!");
            }
            catch (Exception ex)
            { 
                MessageBox.Show(ex.Message);
            }
        }

        private void FontHandler(object sender, EventArgs args)
        {
            ColorFontDialog dialog = new ColorFontDialog(true, false);
            dialog.Font = FontInfo.GetControlFont(MyTextBox);

            dialog.ShowDialog();

            FontInfo font = dialog.Font;

            if (font != null)
            {
                _fontFamily = font.Family.Source;
                _fontSize = font.Size;
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            FontPicker.IsEnabled = true;
            _isSpecificFontSelect = true;
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            FontPicker.IsEnabled = false;
            _isSpecificFontSelect = false;
        }
    }
}
