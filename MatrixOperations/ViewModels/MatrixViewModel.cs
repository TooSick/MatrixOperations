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
        private Matrix _firstMatrix;
        private Matrix _secondMatrix;

        private RelayCommand _firstOpenCommand;
        private RelayCommand _secondOpenCommand;
        private RelayCommand _equalCommand;


        public DataTable FisrtDataTable { get; set; }

        public DataTable SecondDataTable { get; set; }
        
        public DataTable ResultDataTable { get; set; }

        public RelayCommand FirstOpenCommand
        {
            get {
                return _firstOpenCommand ??= new RelayCommand(async obj =>
                {
                    _firstMatrix = await GetMatrixAsync();
                    if (_firstMatrix != null)
                    {
                        FisrtDataTable = _firstMatrix.ToDataTable();
                        OnPropertyChanged("MatrixA");
                    }

                    if (ResultDataTable != null)
                    {
                        ResultDataTable.Clear();
                    }
                });
            }
        }

        public RelayCommand SecondOpenCommand
        {
            get
            {
                return _secondOpenCommand ??= new RelayCommand(async obj =>
                {
                    _secondMatrix = await GetMatrixAsync();
                    if (_secondMatrix != null)
                    {
                        SecondDataTable = _secondMatrix.ToDataTable();
                        OnPropertyChanged("MatrixB");
                    }

                    if (ResultDataTable != null)
                    {
                        ResultDataTable.Clear();
                    }
                });
            }
        }

        public RelayCommand EqualCommand
        {
            get
            {
                return _equalCommand ??= new RelayCommand(obj =>
                {
                    try
                    {
                        ResultDataTable = (_firstMatrix * _secondMatrix).ToDataTable();
                        OnPropertyChanged("ResultMatrix");
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Check the entered data.");
                    }
                });
            }
        }


        public event PropertyChangedEventHandler? PropertyChanged;
        

        private async Task<Matrix?> GetMatrixAsync()
        { 
            try
            {
                string path = GetTxtFilePathFromOpenDialog();

                if (!string.IsNullOrEmpty(path))
                {
                    List<string> data = await TxtFileWorker.GetDataAsync(path);

                    return new Matrix(ConvertToDoubleArrayOfArray(data));
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

        private double[][] ConvertToDoubleArrayOfArray(List<string> data)
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

        private string GetTxtFilePathFromOpenDialog()
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


        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
