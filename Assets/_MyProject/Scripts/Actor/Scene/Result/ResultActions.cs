namespace MyProject.Actor
{
    public class ResultActions : ActionsBase
    {
        readonly MainActions.ResultActions resultActions;

        public ResultActions(MainActions hostActions)
        {
            resultActions = hostActions.Result;
        }

        public override void Enable()
        {
            resultActions.Enable();
        }

        public override void Disable()
        {
            resultActions.Disable();
        }
    }
}
