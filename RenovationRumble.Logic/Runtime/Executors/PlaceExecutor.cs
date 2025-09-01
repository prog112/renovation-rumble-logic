namespace RenovationRumble.Logic.Runtime.Executors
{
    using Data.Commands;
    using Runner;

    public class PlaceExecutor : ICommandExecutor<PlaceCommandDataModel>
    {
        public bool CanApply(in Context context, PlaceCommandDataModel command)
        {
            return true;
        }

        public void Apply(Context context, PlaceCommandDataModel command)
        {
        }
    }
}