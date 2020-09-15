using FileScanner.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace FileScanner.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private string selectedFolder;
        private ObservableCollection<string> folderItems = new ObservableCollection<string>();
         
        public DelegateCommand<string> OpenFolderCommand { get; private set; }
        public DelegateCommand<string> ScanFolderCommand { get; private set; }

        public ObservableCollection<string> FolderItems { 
            get => folderItems;
            set 
            { 
                folderItems = value;
                OnPropertyChanged();
            }
        }

        public string SelectedFolder
        {
            get => selectedFolder;
            set
            {
                selectedFolder = value;
                OnPropertyChanged();
                ScanFolderCommand.RaiseCanExecuteChanged();
            }
        }

        public MainViewModel()
        {
            OpenFolderCommand = new DelegateCommand<string>(OpenFolder);
            ScanFolderCommand = new DelegateCommand<string>(ScanFolder, CanExecuteScanFolder);
        }

        private bool CanExecuteScanFolder(string obj)
        {
            return !string.IsNullOrEmpty(SelectedFolder);
        }

        private void OpenFolder(string obj)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    SelectedFolder = fbd.SelectedPath;
                }
            }
        }

        private void ScanFolder(string dir)
        {
            FolderItems = new ObservableCollection<string>(GetDirs(dir));
            
            foreach (var item in Directory.EnumerateFiles(dir, "*"))
            {
                FolderItems.Add(item);
            }
        }

        IEnumerable<string> GetDirs(string dir)
        {            
            foreach (var d in Directory.EnumerateDirectories(dir, "*"))
            {
                yield return d;
            }
        }

        ///TODO : Tester avec un dossier avec beaucoup de fichier
        ///TODO : Rendre l'application asynchrone
        ///TODO : Ajouter un try/catch pour les dossiers sans permission


    }
}
