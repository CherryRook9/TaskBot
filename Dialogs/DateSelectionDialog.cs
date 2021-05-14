using System;
using System.Threading.Tasks;
using TelegramBotBase.Args;
using TelegramBotBase.Base;
using TelegramBotBase.Controls.Inline;
using TelegramBotBase.Form;

namespace TaskBot.Dialogs
{
    class DateSelectionDialog : ModalDialog
    {
        private CalendarPicker calendar;

        private int? messageId = null;

        public event Action<DateTime> Completed;

        public DateSelectionDialog() => this.Init += OnInit;

        public Task OnInit(object sender, InitEventArgs _)
        {
            this.calendar = new CalendarPicker();
            this.calendar.Title = "Выберите дату";
            
            this.AddControl(calendar);
            return Task.CompletedTask;
        }

        public override Task Action(MessageResult message)
        {
            if (message.GetData<CallbackData>() is CallbackData call)
            {
                if (call.Method == "done")
                {
                    Completed(calendar.SelectedDate);
                    this.CloseForm();
                }
            }
            return base.Action(message);
        }

        public override async Task Render(MessageResult message)
        {
            var answerText = $"Выбранна дата {this.calendar.SelectedDate.ToShortDateString()}";

            ButtonForm bf = new ButtonForm();
            bf.AddButtonRow(new ButtonBase("Готово", new CallbackData("done", "").Serialize()));

            if (messageId != null)
            {
                await this.Device.Edit(messageId.Value, answerText, bf);
            }
            else
            {
                var m = await this.Device.Send(answerText, bf);
                this.messageId = m.MessageId;
            }
        }
    }
}