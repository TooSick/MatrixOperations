using FileWorkers;
using MatrixOperations.Commands;
using MatrixOperations.Models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;

namespace MatrixOperations.ViewModels
{
    public class MatrixViewModel : INotifyPropertyChanged
    {
        private Matrix FirstMatrix;
        private Matrix SecondMatrix;
        public DataTable MatrixA { get; set; }
        public DataTable MatrixB { get; set; }
        public DataTable ResultMatrix { get; set; }

        private RelayCommand firstOpenCommand;
        public RelayCommand FirstOpenCommand
        {
            get {
                return firstOpenCommand ??= new RelayCommand(async obj =>
                {
                    FirstMatrix = await ExecuteOpenCommandAsync();
                    if (FirstMatrix != null)
                    {
                        MatrixA = FirstMatrix.ToDataTable();
                        OnPropertyChanged("MatrixA");
                    }

                    if (ResultMatrix != null)
                    {
                        ResultMatrix.Clear();
                    }
                });
            }
        }

        private RelayCommand secondOpenCommand;
        public RelayCommand SecondOpenCommand
        {
            get
            {
                return secondOpenCommand ??= new RelayCommand(async obj =>
                {
                    SecondMatrix = await ExecuteOpenCommandAsync();
                    if (SecondMatrix != null)
                    {
                        MatrixB = SecondMatrix.ToDataTable();
                        OnPropertyChanged("MatrixB");
                    }

                    if (ResultMatrix != null)
                    {
                        ResultMatrix.Clear();
                    }
                });
            }
        }

        private RelayCommand equalCommand;
        public RelayCommand EqualCommand
        {
            get
            {
                return equalCommand ??= new RelayCommand(obj =>
                {
                    try
                    {
                        ResultMatrix = (FirstMatrix * SecondMatrix).ToDataTable();
                        OnPropertyChanged("ResultMatrix");
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Check the entered data.");
                    }
                });
            }
        }

        private async Task<Matrix?> ExecuteOpenCommandAsync()
        { 
            try
            {
                string path = GetFilePathFromOpenDialog();

                if (!string.IsNullOrEmpty(path))
                {
                    List<string> data = await TxtFileWorker.GetDataAsync(path);

                    return new Matrix(ConvertTo2DDoubleArray(data));
                }
            }
            catch (FormatException)
            {
                MessageBox.Show("Incorrect format of the entered data.");
            }
            catch (ArgumentOutOfRangeException)
            {
                MessageBox.Show("The length of the columns must be the same.");
            }
            catch (Exception)
            {
                MessageBox.Show("Something's wrong.");
            }

            return null;
        }

        private double[][] ConvertTo2DDoubleArray(List<string> data)
        {
            double[][] result = new double[data.Count][];

            for (int i = 0; i < data.Count; i++)
            {
                result[i] = data[i].Split(' ').Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => double.Parse(x)).ToArray();

                if (result[i].Length != result[0].Length)
                {
                    throw new ArgumentOutOfRangeException(nameof(result));
                }
            }

            return result;
        }

        private string GetFilePathFromOpenDialog()
        {
            OpenFileDialog dialog = new OpenFileDialog();

            dialog.DefaultExt = ".txt";
            dialog.Filter = "Text documents (.txt)|*.txt";

            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                return dialog.FileName;
            }

            return string.Empty;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
