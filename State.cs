using System.Collections.Generic;

namespace SimpleTgBot
{
    internal enum TaskCreationStateType
    {
        DoingNothing,
        SelectingOption,
        Creating,
        Deleting,
        Editing
    }

    internal class TaskCreationState
    {
        public TaskCreationStateType TaskCreationStateType { get; set; }

        string TaskName { get; set; }
        string Description { get; set; }
    }

    internal class GlobalState 
    {
        Dictionary<long, State> states;
        public GlobalState ()
        {
           Dictionary<long, State> states = new Dictionary<long, State>();
        }
    } 


    internal class State
    {
        public long ChatId { get; }

        public int MessageCount { get; set; }

        public TaskCreationStateType TaskCreationState { get; set; }

        public State(long chatId)
        {
            ChatId = chatId;
            MessageCount = 0;
            TaskCreationState = TaskCreationStateType.DoingNothing;
        }
    }
}