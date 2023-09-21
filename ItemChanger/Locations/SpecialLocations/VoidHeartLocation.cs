using HutongGames.PlayMaker.Actions;
using ItemChanger.Components;
using ItemChanger.FsmStateActions;
using ItemChanger.Extensions;

namespace ItemChanger.Locations.SpecialLocations
{
    /// <summary>
    /// Location which directly gives items in place of Void Heart following the Dream_Abyss sequence.
    /// </summary>
    public class VoidHeartLocation : AutoLocation, ILocalHintLocation
    {
        public bool HintActive { get; set; } = true;

        protected override void OnLoad()
        {
            Events.AddFsmEdit(UnsafeSceneName, new("End Cutscene", "Control"), EditEndCutscene);
            Events.AddSceneChangeEdit(SceneNames.Abyss_15, EditMirror);
            Events.AddFsmEdit(SceneNames.Abyss_15, new("Dream Enter Abyss", "Control"), EditDreamEnter);
        }

        protected override void OnUnload()
        {
            Events.RemoveFsmEdit(UnsafeSceneName, new("End Cutscene", "Control"), EditEndCutscene);
            Events.RemoveSceneChangeEdit(SceneNames.Abyss_15, EditMirror);
            Events.RemoveFsmEdit(SceneNames.Abyss_15, new("Dream Enter Abyss", "Control"), EditDreamEnter);
        }

        private void EditEndCutscene(PlayMakerFSM fsm)
        {
            FsmState charmGet = fsm.GetState("Charm Get");
            FsmState removeOvercharm = fsm.GetState("Remove Overcharm");
            FsmState getMsg = fsm.GetState("Get Msg");

            FsmStateAction give = new AsyncLambda(GiveAll, "GET ITEM MSG END");

            charmGet.ClearActions();
            removeOvercharm.ClearActions();
            getMsg.SetActions(getMsg.Actions[0], give);
        }

        private void EditDreamEnter(PlayMakerFSM fsm)
        {
            FsmState init = fsm.GetState("Init");
            init.SetActions(init.Actions[0], new DelegateBoolTest(Placement.AllObtained, "INACTIVE", null));
        }

        private void EditMirror(Scene to)
        {
            GameObject mirror = to.GetRootGameObjects().FirstOrDefault(go => go.name == "Mirror");

            if (Placement.AllObtained()) UObject.Destroy(mirror);
            else if (this.GetItemHintActive()) HintBox.Create(mirror.transform, Placement);
        }
    }
}
