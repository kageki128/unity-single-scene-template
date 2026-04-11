using R3;
using UnityEngine;

namespace MyProject.Actor
{
    public class GameActorHub : SceneActorHubBase
    {
        public Observable<Unit> ToSelectButtonClicked => ToSelectButton.Clicked;

        [SerializeField] StandardButton ToSelectButton;
    }
}