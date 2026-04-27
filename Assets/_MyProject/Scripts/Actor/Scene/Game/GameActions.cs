namespace MyProject.Actor
{
    public class GameActions : ActionsBase
    {
        readonly MainActions.GameActions gameActions;

        public GameActions(MainActions hostActions)
        {
            gameActions = hostActions.Game;
        }

        public override void Enable()
        {
            gameActions.Enable();
        }

        public override void Disable()
        {
            gameActions.Disable();
        }
    }
}
