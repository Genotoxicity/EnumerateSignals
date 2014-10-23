using System;
using System.Windows;
using System.Windows.Controls;
using System.Text.RegularExpressions;
using KSPE3Lib;

namespace KSP.E3.EnumerateSignals
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class UI : Window, IDisposable
    {

        private E3ApplicationInfo applicationInfo;
        private Script script;
        private bool IsCaptured;
        private string acceptableNumber = "1";

        public UI()
        {
            applicationInfo = new E3ApplicationInfo();
            InitializeComponent();
            SetElementProperties();
            MinHeight = Height;
            MinWidth = Width;
            MaxHeight = Height;
            MaxWidth = Width;
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            if (applicationInfo.Status == SelectionStatus.Selected)
                tb_Project.Text = applicationInfo.MainWindowTitle;
            else
            {
                tb_Project.Text = applicationInfo.StatusReasonDescription;
                btn_Capture.IsEnabled = false;
            }
        }

        private void SetElementProperties()
        {
            sb_Number.Value = 1;
            sb_Number.ValueChanged += sb_Number_ValueChanged;
            tb_Number.AllowDrop = false;
            tb_Number.TextChanged += tb_Number_TextChanged;
            tb_Project.SelectionBrush = null;
        }

        private void ChangeNumber(RoutedPropertyChangedEventArgs<double> e)
        {
            if (e.NewValue < e.OldValue)
            {
                sb_Number.Maximum = sb_Number.Value + 1;
                sb_Number.Minimum = sb_Number.Value - 1;
                int number;
                if (int.TryParse(tb_Number.Text, out number))
                    tb_Number.Text = (++number).ToString();
            }
            if (e.NewValue > e.OldValue)
            {
                sb_Number.Maximum = sb_Number.Value + 1;
                sb_Number.Minimum = sb_Number.Value - 1;
                int number;
                if (int.TryParse(tb_Number.Text, out number))
                {
                    if (number > 1)
                        tb_Number.Text = (--number).ToString();
                }
            }
        }

        private void CheckEnteredNumber()
        {
            Regex regex = new Regex(@"^[1-9]+\d*$");
            string text = tb_Number.Text;
            if (regex.IsMatch(text))
                acceptableNumber = text;
            else
                tb_Number.Text = acceptableNumber;
        }

        private void CaptureProject()
        {
            if (IsCaptured)
            {
                btn_Capture.Content = "Захватить";
                script.Release();
            }
            else
            {
                btn_Capture.Content = "Освободить";
                string prefix = tb_Prefix.Text;
                int number;
                if (int.TryParse(tb_Number.Text, out number))
                    script = new Script(applicationInfo.ProcessId, prefix, number, new Action<string>(s=>tb_Number.Text=s));
            }
            IsCaptured = !IsCaptured;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
                script.Dispose();
        }

        private void sb_Number_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ChangeNumber(e);
        }

        private void tb_Number_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckEnteredNumber();
        }

        private void btn_Capture_Click(object sender, RoutedEventArgs e)
        {
            CaptureProject();
        }

    }
}
