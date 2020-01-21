using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace Elements
{
    /// <summary>
    /// Interaction logic for Main
    /// </summary>
    public partial class Main : Window
    {
        public Main()
        {
            InitializeComponent();
        }
    }

    /// <summary>
    /// ViewModel class for Main
    /// </summary>
    public class MainViewModel : INotifyPropertyChanged
    {
        // Number of Elements in current version of the Periodic Table
        private const int NUM_ELEMENTS = 118;

        // INotifyPropertyChanged from MS Docs
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Constructor
        public MainViewModel()
        {
            // Model
            Element = new Element();

            // Button Clicks
            LoadCommand = new RelayCommand(param => Load(), param => true);
            SaveCommand = new RelayCommand(param => Save(), param => true);
            ResaveCommand = new RelayCommand(param => Resave(), param => true);
        }

        #region Commands
        // Load/Save/Resave
        public ICommand LoadCommand { get; private set; }
        public ICommand SaveCommand { get; private set; }
        public ICommand ResaveCommand { get; private set; }

        // Returns the path of the current element (assuming that it is in the current directory)
        private string GetDefaultPathOfElement()
        {
            return System.AppDomain.CurrentDomain.BaseDirectory + Element.ReturnPartialFileName();
        }

        // Indicates whether a file is being loaded
        private bool ElementLoading = false;
        // Load
        public void Load()
        {
            // Ask user to locate file and load element using filename
            OpenFileDialog fileDialog = new OpenFileDialog()
            {
                CheckFileExists = true,
                Title = "Locate File",
                Filter = "JSON files (*.json)|*.json"
            };
            if (fileDialog.ShowDialog() == true)
            {
                if (fileDialog.FileName != null)
                {
                    // Load Element
                    ElementLoading = true;
                    LoadElement(fileDialog.FileName);

                    // Refresh UI and Finish
                    NotifyPropertyChanged(string.Empty);
                    ElementLoading = false;
                }
            }
        }

        // Loads an element from file
        public void LoadElement(string fileName)
        {
            // Ensure that file exists
            if (File.Exists(fileName) == false)
            {
                MessageBox.Show("File does not exist", System.Reflection.Assembly.GetExecutingAssembly().GetName().Name, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            // Read file
            string file;
            using (StreamReader sr = new StreamReader(fileName))
            {
                file = sr.ReadToEnd();
            }

            // Populate Element object
            JsonConvert.PopulateObject(file, Element);
        }

        // Save
        public void Save()
        {
            // Ensure that an element is being saved
            if (Name == "")
            {
                MessageBox.Show("Please select an element", System.Reflection.Assembly.GetExecutingAssembly().GetName().Name, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            // Update output textbox
            NotifyPropertyChanged("Output");

            // Get Save Path
            string defaultFilePath = GetDefaultPathOfElement();
            string fileName = null;
            if (File.Exists(defaultFilePath) == true)
            {
                // If file is in current directory, use current directory as save location
                fileName = defaultFilePath;
            }
            else
            {
                // Ask user for save location and filename if otherwise
                SaveFileDialog fileDialog = new SaveFileDialog()
                {
                    InitialDirectory = System.AppDomain.CurrentDomain.BaseDirectory,
                    Title = "Locate Save Location",
                    AddExtension = true,
                    DefaultExt = ".json",
                    Filter = "JSON files (*.json)|*.json",
                    FileName = Element.ReturnPartialFileName()
                };
                if (fileDialog.ShowDialog() == true)
                {
                    fileName = fileDialog.FileName;
                }
            }

            // Exit if fileName is Empty
            if (string.IsNullOrEmpty(fileName))
            {
                return;
            }

            // Save serialized object
            using (StreamWriter sw = new StreamWriter(new FileStream(fileName, FileMode.OpenOrCreate), System.Text.Encoding.UTF8))
            {
                sw.Write(Element.GenerateSaveJson());
            }
        }

        // Load and Resave all Element Files
        public void Resave()
        {
            int i = 0;
            for (i = 0; i < NUM_ELEMENTS; i++)
            {
                // Get current element
                ElementIndex = i;
                string fileName = GetDefaultPathOfElement();

                // If element file is in current directory, load and save
                if (File.Exists(fileName))
                {
                    LoadElement(fileName);
                    Save();
                }
            }
            // Refresh UI
            NotifyPropertyChanged(null);
        }
        #endregion

        #region Properties
        // Element (Model)
        private Element Element { get; set; }

        // Output
        public string Output
        {
            get
            {
                // Return Initial Message on Load
                if (Name == "")
                {
                    return "Output";
                }
                // Return JSON
                return Element.GenerateSaveJson();
            }
        }

        // Name
        public string Name
        {
            get
            {
                return Element.Name;
            }
            set
            {
                Element.Name = value;
                NotifyPropertyChanged("Output");
            }
        }

        // Atomic Number
        public int Number
        {
            get => Element.Number;
        }
        // Index of the selected element
        private int _ElementIndex = -1;
        public int ElementIndex
        {
            get
            {
                return _ElementIndex;
            }
            set
            {
                _ElementIndex = value;

                // Atomic Number is SelectedIndex of ComboBox + 1
                if (value == int.MaxValue)
                {
                    throw new ArgumentOutOfRangeException("value", "Element number is too large.");
                }
                Element.Number = value + 1;

                // Update Number Textbox
                NotifyPropertyChanged("Number");
                NotifyPropertyChanged("Name");

                if (ElementLoading == true)
                {
                    return;
                }

                string defaultPath = GetDefaultPathOfElement();
                if (File.Exists(defaultPath))
                {
                    // Element file is in current directory, load it directly
                    LoadElement(defaultPath);
                }
                else
                {
                    // Reset Element
                    Element.Reset();
                }
                // Refresh UI
                NotifyPropertyChanged(null);
            }
        }

        // Mass/Symbol
        public string Mass
        {
            get => Element.Mass;
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    Element.Mass = null;
                }
                else
                {
                    Element.Mass = value;
                }
            }
        }
        public string Symbol
        {
            get => Element.Symbol;
            set => Element.Symbol = value;
        }

        // Location/Type
        public string Location { get => Element.Location; set => Element.Location = value; }
        public string Classification { get => Element.Classification; set => Element.Classification = value; }
        public int Period { get => Element.Period; set => Element.Period = value; }
        public object Group
        {
            get
            {
                if (!Element.Group.HasValue)
                {
                    return "none";
                }
                return Element.Group;
            }
            set
            {
                if ((string)value == "none")
                {
                    Element.Group = null;
                }
                else
                {
                    Element.Group = Convert.ToInt32(value, System.Globalization.CultureInfo.InvariantCulture);
                }
            }
        }

        // Config
        public string ShellConfig { get => Element.ShellConfig; set => Element.ShellConfig = value; }
        public string SubshellConfig { get => Element.SubshellConfig; set => Element.SubshellConfig = value; }

        // States
        public string State { get => Element.State; set => Element.State = value; }
        public string Ionisation { get => Element.Ionisation; set => Element.Ionisation = value; }
        public string Boiling { get => Element.BoilingPoint; set => Element.BoilingPoint = value; }
        public string Melting { get => Element.MeltingPoint; set => Element.MeltingPoint = value; }

        // Other Information
        public string Description { get => Element.Description; set => Element.Description = value; }
        public string Discovery { get => Element.Discovery; set => Element.Discovery = value; }
        public string Isotopes { get => Element.Isotopes; set => Element.Isotopes = value; }
    }
    #endregion
}
