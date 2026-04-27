namespace MyProject.Actor
{
    public class SelectActions : ActionsBase
    {
        readonly MainActions.SelectActions selectActions;

        public SelectActions(MainActions hostActions)
        {
            selectActions = hostActions.Select;
        }

        public override void Enable()
        {
            selectActions.Enable();
        }

        public override void Disable()
        {
            selectActions.Disable();
        }
    }
}
