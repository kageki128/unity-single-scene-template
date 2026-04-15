using UnityEngine;
using UnityEngine.Serialization;

namespace MyProject.Core
{
    [CreateAssetMenu(fileName = "GameConfig", menuName = "MyProject/GameConfig")]
    public class GameConfigSO : ScriptableObject
    {
        [field: SerializeField, FormerlySerializedAs("initialSceneType")]
        public SceneType InitialSceneType { get; private set; } = SceneType.Title;
    }
}
