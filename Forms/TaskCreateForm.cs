using System.Threading.Tasks;
using TelegramBotBase.Base;

namespace TaskBot.Forms
{
    class TaskCreationForm : FormBase 
    {
        string taskName = null;
        string taskDescription = null;

        public override async Task Load(MessageResult message)
        {
            await base.Load(message);
            if (!string.IsNullOrEmpty(message.MessageText))
            {
                if (taskName == null && taskDescription == null)
                {
                    taskName = message.MessageText;
                }
                else if (taskName != null && taskDescription == null)
                {
                    taskDescription = message.MessageText;
                }
            }
        }

        public override async Task Render(MessageResult message)
        {
            await base.Render(message);

            if (taskName == null && taskDescription == null)
            {
                await Device.Send("Начинаем создание новой задачи.\nВведите название:");
            }
            else if (taskName != null && taskDescription == null)
            {
                await Device.Send("Введите описание:");
            }
            else
            {
                await Device.Send($"Задача создана.\nНазвание:\n{taskName}\nОписание:\n{taskDescription}");
                await NavigateTo(new MenuForm());
            }
        }
    }
}