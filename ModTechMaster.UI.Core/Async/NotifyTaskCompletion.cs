using System;
using System.ComponentModel;
using System.Threading.Tasks;

public sealed class NotifyTaskCompletion : INotifyPropertyChanged
{
    public NotifyTaskCompletion(Task task)
    {
        this.Task = task;
        if (!task.IsCompleted)
        {
            this.TaskCompletion = this.WatchTaskAsync(task);
        }
    }

    public Task TaskCompletion { get; }

    public Task Task { get; }

    //public TResult Result => this.Task.Status == TaskStatus.RanToCompletion ? this.Task.Result : default(TResult);

    public TaskStatus Status => this.Task.Status;
    public bool IsCompleted => this.Task.IsCompleted;
    public bool IsNotCompleted => !this.Task.IsCompleted;

    public bool IsSuccessfullyCompleted =>
        this.Task.Status ==
        TaskStatus.RanToCompletion;

    public bool IsCanceled => this.Task.IsCanceled;
    public bool IsFaulted => this.Task.IsFaulted;
    public AggregateException Exception => this.Task.Exception;

    public Exception InnerException => this.Exception == null ? null : this.Exception.InnerException;

    public string ErrorMessage => this.InnerException == null ? null : this.InnerException.Message;

    public event PropertyChangedEventHandler PropertyChanged;

    private async Task WatchTaskAsync(Task task)
    {
        try
        {
            await task;
        }
        catch
        {
        }

        var propertyChanged = this.PropertyChanged;
        if (propertyChanged == null)
        {
            return;
        }

        propertyChanged(this, new PropertyChangedEventArgs("Status"));
        propertyChanged(this, new PropertyChangedEventArgs("IsCompleted"));
        propertyChanged(this, new PropertyChangedEventArgs("IsNotCompleted"));
        if (task.IsCanceled)
        {
            propertyChanged(this, new PropertyChangedEventArgs("IsCanceled"));
        }
        else if (task.IsFaulted)
        {
            propertyChanged(this, new PropertyChangedEventArgs("IsFaulted"));
            propertyChanged(this, new PropertyChangedEventArgs("Exception"));
            propertyChanged(
                this,
                new PropertyChangedEventArgs("InnerException"));
            propertyChanged(this, new PropertyChangedEventArgs("ErrorMessage"));
        }
        else
        {
            propertyChanged(
                this,
                new PropertyChangedEventArgs("IsSuccessfullyCompleted"));
            propertyChanged(this, new PropertyChangedEventArgs("Result"));
        }
    }
}