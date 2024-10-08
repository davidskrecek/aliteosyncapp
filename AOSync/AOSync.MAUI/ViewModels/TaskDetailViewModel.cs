﻿using System.Windows.Input;
using AOSync.BL.Services;
using AOSync.DAL.DB;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AOSync.MAUI.ViewModels;

[QueryProperty(nameof(TaskId), nameof(TaskId))]
public partial class TaskDetailViewModel : BaseViewModel
{
    [ObservableProperty] private string _commentContent = string.Empty;

    [ObservableProperty] private IEnumerable<CommentEntity> _comments;

    private IServiceProvider _serviceProvider;

    [ObservableProperty] private TaskEntity _task;

    [ObservableProperty] private string _taskId;

    public TaskDetailViewModel()
    {
        SubmitChangesCommand = new AsyncRelayCommand(OnSubmitChanges);
        CommentSelectedCommand = new AsyncRelayCommand<CommentEntity>(OnCommentSelected!);
        PostCommentCommand = new AsyncRelayCommand(OnPostComment);
    }

    public ICommand SubmitChangesCommand { get; }
    public ICommand CommentSelectedCommand { get; }
    public ICommand PostCommentCommand { get; }

    public void Initialize(IServiceProvider serviceProvider, DataReloadService dataReloadService)
    {
        _serviceProvider = serviceProvider;
        dataReloadService.RegisterReloadAction(LoadTaskDetail);
        dataReloadService.RegisterReloadAction(LoadComments);
    }

    public async Task OnAppearing()
    {
        await LoadTaskDetail();
        await LoadComments();
    }

    private async Task LoadTaskDetail()
    {
        if (_serviceProvider == null)
            throw new InvalidOperationException("ServiceProvider not initialized.");

        using var scope = _serviceProvider.CreateScope();
        var taskService = scope.ServiceProvider.GetRequiredService<ITaskService>();
        Task = await taskService.GetByIdAsync(new Guid(TaskId));
    }

    private async Task LoadComments()
    {
        if (_serviceProvider == null)
            throw new InvalidOperationException("ServiceProvider not initialized.");

        using var scope = _serviceProvider.CreateScope();
        var commentService = scope.ServiceProvider.GetRequiredService<ICommentService>();
        Comments = await commentService.GetCommentsByTaskId(new Guid(TaskId));
    }

    private async Task OnSubmitChanges()
    {
        // Implement the logic to submit changes to the task
    }

    private async Task OnCommentSelected(CommentEntity comment)
    {
        // Implement the logic to handle comment selection
    }

    private async Task OnPostComment()
    {
        if (CommentContent != string.Empty)
        {
            var commentService = _serviceProvider.GetRequiredService<ICommentService>();
            var comment = new CommentEntity
            {
                Id = new Guid(),
                ExternalId = string.Empty,
                TaskId = new Guid(TaskId),
                Text = CommentContent,
                IsCreated = true,
                UserEmail = "david.skrecek@gmail.com"
            };
            await commentService.AddAsync(comment);
            CommentContent = string.Empty;
            await LoadComments();
        }
    }
}