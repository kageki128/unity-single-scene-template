using UnityEngine;

namespace MyProject.Core
{
    [CreateAssetMenu(fileName = "GameConfig", menuName = "MyProject/GameConfig")]
    public class GameConfigSO : ScriptableObject
    {
        public SceneType InitialSceneType => initialSceneType;
        [SerializeField] SceneType initialSceneType = SceneType.Title;
    }
}