﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Collections.ObjectModel;
using TasksManager.Application.Models;
using System.Windows.Input;
using TasksManager.Application.Services;
using TasksManager.Application.Utility;
using TasksManager.Model.Entities;

namespace TasksManager.Application.ViewModel
{
    public class TaskOverviewViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private TaskModel _newTask;
        private TaskModel _selectedTask;
        private TaskDataService _taskDataService;
        private ObservableCollection<TaskModel> _tasks;
        private ObservableCollection<TaskModel> _tasksInProgress;
        private ObservableCollection<TaskModel> _tasksDone;
        private ObservableCollection<TaskModel> _tasksToDo;
        private TaskStatus _filterName;

        public ICommand DetailCommand { get; set; }
        public ICommand AddTaskCommand { get; set; }
        public ICommand DeleteTaskCommand { get; set; }
        public ICommand EditTaskCommand { get; set; }
        public ICommand SaveTaskCommand { get; set; }
        public ICommand MakeDoneCommand { get; set; }
        public ICommand CancelCommand { get; set; }

        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ObservableCollection<TaskModel> Tasks
        {
            get { return _tasks; }
            set
            {
                _tasks = value;
                RaisePropertyChanged(nameof(Tasks));
            }
        }

        public ObservableCollection<TaskModel> TasksInProgress
        {
            get { return _tasksInProgress; }
            set
            {
                _tasksInProgress = value;
                RaisePropertyChanged(nameof(TasksInProgress));
            }
        }

        public ObservableCollection<TaskModel> TasksDone
        {
            get { return _tasksDone; }
            set
            {
                _tasksDone = value;
                RaisePropertyChanged(nameof(TasksDone));
            }
        }
        public ObservableCollection<TaskModel> TasksToDo
        {
            get { return _tasksToDo; }
            set
            {
                _tasksToDo = value;
                RaisePropertyChanged(nameof(TasksToDo));
            }
        }
        public TaskModel SelectedTask
        {
            get { return _selectedTask; }
            set
            {
                _selectedTask = value;
                RaisePropertyChanged(nameof(SelectedTask));
            }
        }
        public TaskStatus FilterName
        {
            get { return _filterName; }
            set
            {
                switch (value)
                {
                    case TaskStatus.ToDo:
                        Tasks = TasksToDo;
                        break;
                    case TaskStatus.InProgress:
                        Tasks = TasksInProgress;
                        break;
                    case TaskStatus.Done:
                        Tasks = TasksDone;
                        break;
                }
                _filterName = value;
                RaisePropertyChanged(nameof(FilterName));
            }
        }

        public TaskModel NewTask
        {
            get { return _newTask; }
            set
            {
                _newTask = value;
                RaisePropertyChanged(nameof(NewTask));
            }
        }

        public IEnumerable<TaskStatus> TaskStatuses
        {
            get
            {
                return Enum.GetValues(typeof(TaskStatus)).Cast<TaskStatus>();
            }
        }

        public IEnumerable<TaskPriority> TaskPriorities
        {
            get
            {
                return Enum.GetValues(typeof(TaskPriority)).Cast<TaskPriority>();
            }
        }

        public IEnumerable<TaskCategory> TaskCategories
        {
            get
            {
                return Enum.GetValues(typeof(TaskCategory)).Cast<TaskCategory>();
            }
        }

        public IEnumerable<TaskCategory> SelectTaskCategory
        {
            get
            {
                return Enum.GetValues(typeof(TaskCategory)).Cast<TaskCategory>();
            }
        }

        public RelayCommand ClearTextCommand { get; private set; }

        public TaskOverviewViewModel()
        {
            _taskDataService = new TaskDataService();
            LoadData();
            LoadCommands();
        }

        private void LoadCommands()
        {
            AddTaskCommand = new RelayCommand(AddTask, CanAddTask);
            DeleteTaskCommand = new RelayCommand(DeleteTask, CanDeleteTask);
            EditTaskCommand = new RelayCommand(EditTask, CanEditTask);
            SaveTaskCommand = new RelayCommand(SaveTask, CanSaveTask);
            MakeDoneCommand = new RelayCommand(MakeDone, CanMakeDone);
            CancelCommand = new RelayCommand(Cancel, CanCancel);
        }

        private bool CanCancel(object obj)
        {
            return true;
        }

        private void Cancel(object obj)
        {
            var task = obj as TaskModel;
            if (task.IsNew == true)
            {
                Tasks.Remove(task);
                NewTask = null;
            }
            if (task.IsModify == true)
            {
                var ind = Tasks.IndexOf(task);
                Tasks.Remove(task);
                task = _taskDataService.GetById(task.TaskId);
                Tasks.Insert(ind,task);
            }
        }

        private bool CanMakeDone(object obj)
        {
            return true;
        }

        private void MakeDone(object obj)
        {
            var task = obj as TaskModel;
            task.Status = TaskStatus.Done;
            task.FinishDate = DateTime.Now;
            _taskDataService.Update(task);

            TasksDone.Insert(0,task);
            TasksInProgress.Remove(task);
            Tasks.Remove(task);
        }

        private void AddTask(object obj)
        {
            NewTask = new TaskModel();
            NewTask.IsNew = true;
            Tasks.Insert(0, NewTask);
        }

        private bool CanAddTask(object obj)
        {
            if (NewTask != null)
            {
                return false;
            }

            return true;
        }

        private void EditTask(object obj)
        {
            SelectedTask = obj as TaskModel; ;

            foreach (var task in Tasks)
            {
                if (task.IsModify == true)
                {
                    task.IsModify = false;
                }
            }
            SelectedTask.IsModify = true;
        }
        private bool CanEditTask(object obj)
        {
            return true;
        }

        private void SaveTask(object obj)
        {
            TaskModel task = obj as TaskModel;

            if (task.IsNew == true)
            {
                _taskDataService.Add(task);
                task.IsNew = false;
                NewTask = null;
            }
            else
            {
                _taskDataService.Update(task);
                task.IsModify = false;
            }

            var ind = Tasks.IndexOf(task);

            Tasks.RemoveAt(ind);
            if (task.Status == TaskStatus.ToDo)
            {
                TasksToDo.Insert(0, task);
            }
            else if (task.Status == TaskStatus.InProgress)
            {
                TasksInProgress.Insert(0, task);
            }
            else
            {
                task.FinishDate = DateTime.Now;
                TasksDone.Insert(0, task);
            }           
        }
        private bool CanSaveTask(object obj)
        {
            return true;
        }

        private void DeleteTask(object obj)
        {
            TaskModel task = obj as TaskModel;
            if (task != null)
            {
                _taskDataService.Delete(task);
            }
            Tasks.Remove(task);
        }

        private bool CanDeleteTask(object obj)
        {
            return true;
        }

        private void LoadData()
        {
            TasksInProgress = new ObservableCollection<TaskModel>(_taskDataService.GetAllInProgress());
            TasksDone = new ObservableCollection<TaskModel>(_taskDataService.GetAllDone());
            TasksToDo = new ObservableCollection<TaskModel>(_taskDataService.GetAllToDo());
            Tasks = TasksToDo;
        }
    }
}
