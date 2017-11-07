﻿using System.Collections.Generic;
using System.Linq;
using TasksManager.Application.Models;
using TasksManager.Model.Entities;

namespace TasksManager.Application.Utility
{
    public class Converter
    {
        public TaskModel ConvertToTaskModel(Task entity)
        {
            TaskModel taskModel = new TaskModel();

            taskModel.TaskId = entity.Id;
            taskModel.Title = entity.Title;
            taskModel.Status = entity.Status;
            taskModel.Priority = entity.Priority;
            taskModel.Category = entity.Category;
            taskModel.CreationDate = entity.CreationDate;
            taskModel.FinishDate = entity.FinishDate;
            taskModel.Description = entity.Description;            

            return taskModel;
        }

        public IEnumerable<TaskModel> ConvertToTaskModel(IEnumerable<Task> tasks)
        {
            var taskModels = tasks.Select(e => ConvertToTaskModel(e));
            return taskModels;
        }

        public Task ConvertToTask(TaskModel taskModel)
        {
            var task = new Task();

            task.Id = taskModel.TaskId;
            task.Title = taskModel.Title;
            task.Status = taskModel.Status;
            task.Priority = taskModel.Priority;
            task.Category = taskModel.Category;
            task.CreationDate = taskModel.CreationDate;
            taskModel.FinishDate = taskModel.FinishDate;
            task.Description = taskModel.Description;

            return task;
        }
    }
}

