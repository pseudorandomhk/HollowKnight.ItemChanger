using GlobalEnums;
using Modding;
using ItemChanger.Internal;

namespace ItemChanger.FsmStateActions
{
    /// <summary>
    /// FsmStateAction for triggering a scene transition from within an fsm.
    /// </summary>
    public class ChangeSceneAction : FsmStateAction
    {
        private readonly string _gateName;
        private readonly string _sceneName;

        public ChangeSceneAction(string scene, string gate)
        {
            _sceneName = scene;
            _gateName = gate;
        }

        public override void OnEnter()
        {
            if (!string.IsNullOrEmpty(_sceneName) && !string.IsNullOrEmpty(_gateName))
            {
                ChangeToScene(_sceneName, _gateName);
            }

            Finish();
        }

        private void ChangeToScene(string sceneName, string gateName, float delay = 0f)
        {
            if (string.IsNullOrEmpty(sceneName) || string.IsNullOrEmpty(gateName))
            {
                Log("Empty string passed into ChangeToScene, ignoring");
                return;
            }

            LoadScene(sceneName, gateName, delay);
        }

        private static void LoadScene(string sceneName, string gateName, float delay)
        {
            Ref.GM.StopAllCoroutines();

            Ref.GM.StartCoroutine(Ref.GM.TransitionSceneWithInfo(new GameManager.SceneLoadInfo
            {
                SceneName = sceneName,
                HeroLeaveDirection = GetGatePosition(gateName),
                EntryGateName = gateName,
                EntryDelay = delay
            }));
        }

        private static GatePosition GetGatePosition(string name)
        {
            if (name.Contains("top"))
            {
                return GatePosition.top;
            }

            if (name.Contains("bot"))
            {
                return GatePosition.bottom;
            }

            if (name.Contains("left"))
            {
                return GatePosition.left;
            }

            if (name.Contains("right"))
            {
                return GatePosition.right;
            }

            if (name.Contains("door"))
            {
                return GatePosition.door;
            }

            return GatePosition.unknown;
        }
    }
}