using System.Threading.Tasks;
using TelegramBotBase.Base;

namespace TaskBot.Forms
{
    internal class FormBase : TelegramBotBase.Form.FormBase
    {
        protected Serilog.ILogger logger;

        public FormBase() : base()
        {
            logger = Serilog.Log.Logger;
        }

        public override async Task Load(MessageResult message)
        {
            await base.Load(message);
            logger.Debug(
                "Form '{form}' loaded with because {updateType} event.", 
                this.GetType().Name, 
                message.MessageType);
        }
    }
}