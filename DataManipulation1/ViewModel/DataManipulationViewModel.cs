using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using DataManipulation1.Annotations;
using System.Windows.Input;
using DataManipulation1.Context;
using DataManipulation1.Handlers;
using DataManipulation1.Models;

namespace DataManipulation1.ViewModel
{
    public class DataManipulationViewModel : INotifyPropertyChanged
    {
        public DataManipulationViewModel()
        {
            _canExecute = true;
        }

        private ICommand _openFileDialogCommand;
        public ICommand OpenFileDialogCommand
        {
            get
            {
                return _openFileDialogCommand ?? (_openFileDialogCommand = new CommandHandler(() => OpenFileDialog(), _canExecute));
            }
        }
        private bool _canExecute;
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public void OpenFileDialog()
        {
            List<Patient> patientList = new List<Patient>();
            Stream myStream = null;
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = "c:\\";
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if ((myStream = openFileDialog1.OpenFile()) != null)
                    {
                        using (myStream)
                        {
                            using (var reader = new StreamReader(myStream))
                            {
                                StringBuilder _value = new StringBuilder();
                                
                                while (!reader.EndOfStream)
                                {
                                    byte[] s = Encoding.Convert(Encoding.Default, Encoding.UTF8,
                                        Encoding.Default.GetBytes(reader.ReadLine()));
                                    string patient = Encoding.UTF8.GetString(s);
                      
                                    List<string> value = new List<string>();

                                    foreach (var c in patient)
                                    {
                                        if (Char.IsLetterOrDigit(c))
                                        {
                                            _value.Append(c);
                                        }
                                        else
                                        {
                                            if (_value.Length > 0)
                                            {
                                                value.Add(_value.ToString());
                                                _value.Clear();
                                            }
                                        }
                                    }
                                    var pa = new Patient
                                    {
                                        FirstName = value[0],
                                        LastName = value[1],
                                        Age = (value.Count < 3) ? (int?)null : Int32.Parse(value[2])
                                    };

                                    patientList.Add(pa);
                                    value.Clear();
                                    _value.Clear();
                                }
                                using (var patientContext = new PatientContext())
                                {
                                    try
                                    {
                                        foreach (var patient in patientList)
                                        {
                                            patientContext.Patient.Add(patient);
                                        }
                                        patientContext.SaveChanges();

                                        MessageBox.Show("Patients File has been recovered!!");
                                        System.Windows.Application.Current.Shutdown();
                                    }
                                    catch (Exception ex)
                                    {
                                        MessageBox.Show("Error: SQl Server Error. Original error: " + ex.Message);
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
        }

    }
}
