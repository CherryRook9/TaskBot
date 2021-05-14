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
            DI.Resolve(this);
        }

        public override async Task Load(MessageResult message)
        {
            await base.Load(message);
            logger.Debug(
                "Form '{form}' loaded because of {updateType} event from {user} (DeviceId = {deviceId}, MessageId = {messageId}).", 
                this.GetType().Name, 
                message.MessageType,
                message.Message.From.Username,
                message.DeviceId,
                message.MessageId);
        }
    }
}