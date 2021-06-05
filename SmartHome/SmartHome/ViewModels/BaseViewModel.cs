using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using Xamarin.Forms;

using SmartHome.Models;
using SmartHome.Services;
using Xamarin.Essentials;

namespace SmartHome.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        private TaskService _taskService;
        public TaskService TaskService => _taskService ?? (_taskService = InitializeTaskService());


        private string _status = "Status: \n";
        public string Status
        {
            get => _status;
            set => SetProperty(ref _status, value);
        }
        private TaskService InitializeTaskService()
        {
            var taskService = new TaskService();
            return taskService;
        }

        public void ClearStatus() => Status = string.Empty;

        public void PrintStatus(string input) => Device.BeginInvokeOnMainThread(() => Status += $"{input}\n");

        public void PrintThreadCheck(bool addSpacing = true) => Status += $"{(addSpacing ? "\n" : "")}" +
            $"==={(MainThread.IsMainThread ? "Is" : "Not")} on MainThread===\n" +
            $"{(addSpacing ? "\n" : "")}";

        bool isBusy = false;
        public bool IsBusy
        {
            get { return isBusy; }
            set { SetProperty(ref isBusy, value); }
        }

        string title = string.Empty;
        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value); }
        }

        protected bool SetProperty<T>(ref T backingStore, T value,
            [CallerMemberName]string propertyName = "",
            Action onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;
            if (changed == null)
                return;

            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
