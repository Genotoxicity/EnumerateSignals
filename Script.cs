using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Documents;
using System.Timers;
using KSPE3Lib;

namespace KSP.E3.EnumerateSignals
{
    class Script : IDisposable
    {
        E3Project project;
        Signal signal;
        private Timer timer;
        private int lastRenamedSignalId;
        private int number;
        private int processId;
        private string prefix;
        private bool isMessageViewed;
        private bool isCaptured;
        private Action<string> setTextBoxValue;

        public Script(int processId, string prefix, int number, Action<string> setTextBoxValue)
        {
            this.processId = processId;
            this.prefix = prefix;
            this.number = number;
            this.setTextBoxValue = setTextBoxValue;
            isCaptured = false;
            lastRenamedSignalId = 0;
            timer = new Timer(1000);
            timer.Elapsed += timer_Elapsed;
            timer.Start();
        }

        public void Release()
        {
            timer.Stop();
        }

        private void RenameSignal()
        {
            isCaptured = true;
            project = new E3Project(processId);
            List<int> signalIds = project.SelectedSignalIds;
            string strNumber = number.ToString();
            string name = prefix + strNumber;
            if (signalIds.Count == 1 && signalIds[0] != lastRenamedSignalId)
            {
                signal = project.GetSignalById(0);
                if (signal.GetIdByName(name) == 0)
                {
                    signal.Id = signalIds[0];
                    signal.Name = name;
                    lastRenamedSignalId = signalIds[0];
                    isMessageViewed = false;
                    number++;
                    System.Windows.Application.Current.Dispatcher.Invoke(setTextBoxValue, number.ToString());
                }
                else
                {
                    if (!isMessageViewed)
                    {
                        isMessageViewed = true;
                        MessageBox.Show(String.Format("{0} {1} {2}", "Цепь с именем", name, "уже присутствует в проекте."));
                    }
                }
            }
            project.Release();
            isCaptured = false;
        }


        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
                timer.Dispose();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (!isCaptured)
                RenameSignal();
        }
    }
}
