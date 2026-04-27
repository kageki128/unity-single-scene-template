namespace MyProject.Actor
{
    public class TitleActions : ActionsBase
    {
        readonly MainActions.TitleActions titleActions;

        public TitleActions(MainActions hostActions)
        {
            titleActions = hostActions.Title;
        }

        public override void Enable()
        {
            titleActions.Enable();
        }

        public override void Disable()
        {
            titleActions.Disable();
        }
    }
}
