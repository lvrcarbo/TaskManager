
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;
using TaskManager.Shared;

namespace TaskManager.Client.Pages
{
    public partial class Index
    {
        [Inject] public HttpClient http { get; set; }
        private IList<TaskItem> tasks;
        private string error;
        private string newTask;


        protected override async Task OnInitializedAsync()
        {
            try
            {
                string requestUri = "TaskItems";
                tasks = await http.GetFromJsonAsync<IList<TaskItem>>(requestUri);
            }
            catch (Exception)
            {
                error = "Error Encountered";
            }
        }

        private async Task CheckboxChecked(TaskItem task)
        {
            task.IsCompleted = !task.IsCompleted;

            string requestUri = $"TaskItems/{task.TaskItemId}";
            var response = await
                http.PutAsJsonAsync<TaskItem>(requestUri, task);
            if (!response.IsSuccessStatusCode)
            {
                error = response.ReasonPhrase;
            };
        }

        private async Task DeleteTask(TaskItem taskItem)
        {
            tasks.Remove(taskItem);

            string requestUri =
                $"TaskItems/{taskItem.TaskItemId}";
            var response = await http.DeleteAsync(requestUri);
            if (!response.IsSuccessStatusCode)
            {
                error = response.ReasonPhrase;
            };
        }

        private async Task AddTask()
        {
            if (!string.IsNullOrWhiteSpace(newTask))
            {
                TaskItem newTaskItem = new TaskItem
                {
                    TaskName = newTask,
                    IsCompleted = false
                };
                tasks.Add(newTaskItem);

                string requestUri = "TaskItems";
                var response = await http.PostAsJsonAsync(requestUri, newTaskItem);
                if (response.IsSuccessStatusCode)
                {
                    newTask = string.Empty;
                    var task =
                        await response.Content.ReadFromJsonAsync
                            <TaskItem>();
                }
                else
                {
                    error = response.ReasonPhrase;
                };
            };

        }
    }
}
