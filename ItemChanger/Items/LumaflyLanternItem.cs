﻿using HutongGames.PlayMaker.Actions;
using ItemChanger.Extensions;

namespace ItemChanger.Items
{
    /// <summary>
    /// BoolItem that lights up the room the player is in when obtained.
    /// </summary>
    public class LumaflyLanternItem : BoolItem
    {
        public override void GiveImmediate(GiveInfo info)
        {
            base.GiveImmediate(info);

            if (!Internal.Ref.GM || !Internal.Ref.HC || !Internal.Ref.GM.sm) return;

            SceneManager sm = Internal.Ref.GM.sm;

            if (sm.darknessLevel == 2)
            {
                Internal.Ref.HC.SetDarkness(sm.darknessLevel);

                GameObject vignette = GameObject.FindGameObjectWithTag("Vignette");
                if (vignette)
                {
                    PlayMakerFSM vignetteFSM = FSMUtility.LocateFSM(vignette, "Darkness Control");
                    if (vignetteFSM)
                    {
                        FSMUtility.SetInt(vignetteFSM, "Darkness Level", sm.darknessLevel);
                    }
                    vignetteFSM.SendEvent("SCENE RESET");
                    //if (!sm.noLantern)
                    //{
                    //    vignetteFSM.SendEvent("RESET");
                    //}
                    //else
                    //{
                    //    vignetteFSM.SendEvent("SCENE RESET NO LANTERN");
                    //    if (Internal.Ref.HC != null)
                    //    {
                    //        Internal.Ref.HC.wieldingLantern = false;
                    //    }
                    //}
                }

                // Reactivate HazardRespawnTriggers
                //foreach (DeactivateInDarknessWithoutLantern d in UObject.FindObjectsOfType<DeactivateInDarknessWithoutLantern>(includeInactive: true))
                //{
                //    d.gameObject.SetActive(true);
                //}
                var rootGos = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
                foreach (var fsm in rootGos.SelectMany(go => go.GetComponentsInChildren<PlayMakerFSM>(true)))
                {
                    if (fsm.FsmName != "Deactivate in darkness without lantern") continue;
                    fsm.Fsm.RestartOnEnable = false;
                    fsm.Fsm.GetOwnerDefaultTarget(((ActivateGameObject)(fsm.GetState("Deactivate").Actions[0])).gameObject).SetActive(true);
                }
            }
        }
    }
}
