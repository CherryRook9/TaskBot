using System;
using System.Threading.Tasks;
using TaskBot.Services;
using TelegramBotBase.Base;
using TelegramBotBase.Controls.Inline;

namespace TaskBot.Forms
{
    class DateForm : FormBase
    {
        [Dependency]
        public TasksContext db { get; set; }

        private CalendarPicker picker;
        private readonly FormBase parentForm;

        public DateForm()
        {
            this.AddControl(picker);
        }

        public override async Task Action(MessageResult message)
        {
            if (picker.SelectedDate != new DateTime())
            {
                var date = new TelegramBotBase.Controls.Inline.CalendarPicker();
                await NavigateTo(DI.Resolve(parentForm));
            }
            else
            {

                await NavigateTo(DI.Resolve(new MenuForm()));
            }
        }

        public override async Task Render(MessageResult message)
        {
            var control = new TelegramBotBase.Controls.Inline.CalendarPicker();
            this.AddControl(control);
            var date = control.SelectedDate;

        }
    }
}