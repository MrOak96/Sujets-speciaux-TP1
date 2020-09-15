using FileScanner.Commands;
using FileScanner.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
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
        private ObservableCollection<Item> items = new ObservableCollection<Item>();

        public DelegateCommand<string> OpenFolderCommand { get; private set; }
        public DelegateCommand<string> ScanFolderCommand { get; private set; }

        public ObservableCollection<Item> Items
        {
            get => items;
            set
            {
                items = value;
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
            ScanFolderCommand = new DelegateCommand<string>(ScanFolderAsync, CanExecuteScanFolder);
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

        private async void ScanFolderAsync(string dir)
        {
            Items = new ObservableCollection<Item>();

            var folders = await Task.Run(() => GetDirs(dir));
            var files = await Task.Run(() => GetFiles(dir));

            if(folders != null)
                await Task.Run(() => AddItems(folders, "/Images/Folder.png"));

            if(files != null)
                await Task.Run(() => AddItems(files, "/Images/File.png"));
        }

        public List<string> GetDirs(string dir)
        {
            var folders = new List<string>();
            try
            {
                foreach (var d in Directory.EnumerateDirectories(dir, "*", SearchOption.AllDirectories))
                    folders.Add(d);
            }
            catch (UnauthorizedAccessException)
            {
                System.Windows.MessageBox.Show("Directory Access denied !");
                return null;
            }

            return folders;
        }

        public List<string> GetFiles(string dir)
        {
            var files = new List<string>();
            try
            {
                foreach (var f in Directory.EnumerateFiles(dir, "*", SearchOption.AllDirectories))
                    files.Add(f);
            }
            catch (UnauthorizedAccessException)
            {
                System.Windows.MessageBox.Show("File Access denied !");
                return null;
            }
            return files;
        }

        public void AddItems(List<string> items, string image)
        {
            System.Windows.Application.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, (Action)delegate ()
            {
                foreach (var file in items)
                    Items.Add(new Item(file, image));
            });
        }
    }
}
