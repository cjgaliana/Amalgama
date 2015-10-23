using Amalgama.PhotoAutoPicker.Views;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Windows.Input;
using System.Linq;
using Excel;
using System.Data;

namespace Amalgama.PhotoAutoPicker.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        public ICommand OpenExcelFileCommand { get; private set; }
        public ICommand PickSourceFolderCommand { get; private set; }
        public ICommand PickDestinationFolderCommand { get; private set; }
        public ICommand ImportPhotosCommand { get; private set; }
        public ICommand InfoCommand { get; private set; }

        private string _excelPath;
        private string _sourceFolderPath;
        private string _destinationFolderPath;

        private List<string> _fileNames;
        private string _firstLineMessage;
        private string _secondLineMessage;
        private bool _isBusy;

        public string ExcelPath
        {
            get
            {
                return this._excelPath;
            }
            set
            {
                this.Set(() => this.ExcelPath, ref this._excelPath, value);
            }
        }

        public string SourceFolderPath
        {
            get
            {
                return this._sourceFolderPath;
            }
            set
            {
                this.Set(() => this.SourceFolderPath, ref this._sourceFolderPath, value);
            }
        }

        public string DestinationFolderPath
        {
            get
            {
                return this._destinationFolderPath;
            }
            set
            {
                this.Set(() => this.DestinationFolderPath, ref this._destinationFolderPath, value);
            }
        }

        public string FirstLineMessage
        {
            get
            {
                return this._firstLineMessage;
            }
            set
            {
                this.Set(() => this.FirstLineMessage, ref this._firstLineMessage, value);
            }
        }

        public string SecondLineMessage
        {
            get
            {
                return this._secondLineMessage;
            }
            set
            {
                this.Set(() => this.SecondLineMessage, ref this._secondLineMessage, value);
            }
        }

        public bool IsBusy
        {
            get
            {
                return this._isBusy;
            }
            set
            {
                this.Set(() => this.IsBusy, ref this._isBusy, value);
            }
        }

        public MainViewModel()
        {
            this._fileNames = new List<string>();
            this.CreateCommans();
        }

        private void CreateCommans()
        {
            this.OpenExcelFileCommand = new RelayCommand(this.OpenExcel);
            this.PickSourceFolderCommand = new RelayCommand(this.PickSourceFolder);
            this.PickDestinationFolderCommand = new RelayCommand(this.PickDestinationFolder);
            this.ImportPhotosCommand = new RelayCommand(this.ImportPhotos);
            this.InfoCommand = new RelayCommand(this.ShowInfo);
        }

        private void ShowInfo()
        {
            var info = new InfoWindow();
            info.Show();
        }

        private void PickDestinationFolder()
        {
            var dialog = new FolderBrowserDialog();
            var result = dialog.ShowDialog();
            if (result != DialogResult.OK)
            {
                return;
            }
            this.DestinationFolderPath = dialog.SelectedPath;
        }

        private void PickSourceFolder()
        {
            var dialog = new FolderBrowserDialog();
            var result = dialog.ShowDialog();
            if (result != DialogResult.OK)
            {
                return;
            }
            this.SourceFolderPath = dialog.SelectedPath;
        }

        private void OpenExcel()
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = "Excel Files (*.xlsx, *.xls)|*.xlsx;*.xls";

            var result = dialog.ShowDialog();
            if (result != DialogResult.OK)
            {
                return;
            }
            this.ExcelPath = dialog.FileName;

            this.ImportExcelData();
        }

        private void ImportExcelData()
        {
            if (string.IsNullOrWhiteSpace(this.ExcelPath))
            {
                return;
            }
            FileStream stream = File.Open(this.ExcelPath, FileMode.Open, FileAccess.Read);
            IExcelDataReader excelReader = null;
            var extension = Path.GetExtension(this.ExcelPath);
            if (extension.Contains("xlsx"))
            {
                excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
            }
            else
            {
                excelReader = ExcelReaderFactory.CreateBinaryReader(stream);
            }

            if (excelReader != null)
            {
                DataSet result = excelReader.AsDataSet();

                //5. Data Reader methods
                while (excelReader.Read())
                {
                    var name = excelReader.GetString(0);
                    this._fileNames.Add(name);
                }

                //6. Free resources (IExcelDataReader is IDisposable)
                excelReader.Close();
            }

            this.FirstLineMessage = this._fileNames.Count + " nombres encontrados en el Excel";
        }

        private void ImportPhotos()
        {
            var copyCount = 0;

            this.IsBusy = true;

            foreach (var fileName in this._fileNames)
            {

                var allFiles = Directory.EnumerateFiles(this.SourceFolderPath, "*", SearchOption.AllDirectories).Select(x=> Path.GetFullPath(x)).ToList();
                var matchingFile = allFiles.Where(x => Path.GetFileNameWithoutExtension(x) == fileName).ToList();

                foreach (var file in matchingFile)
                {
                    var dest = Path.Combine(this.DestinationFolderPath, Path.GetFileName(file));

                    if (File.Exists(file))
                    {
                        File.Copy(file, dest, true);
                        copyCount++;
                    }
                }

                
            }




            this.IsBusy = false;
            MessageBox.Show("Se han importado " + copyCount + " archivos", "Hecho!");
        }
    }
}