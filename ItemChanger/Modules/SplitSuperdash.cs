﻿using HutongGames.PlayMaker.Actions;
using ItemChanger.Extensions;
using ItemChanger.FsmStateActions;
#pragma warning disable IDE1006 // Naming Styles

namespace ItemChanger.Modules
{
    /// <summary>
    /// Module which implements the split superdash custom skills.
    /// </summary>
    public class SplitSuperdash : Module
    {
        public bool hasSuperdashLeft { get; set; }
        public bool hasSuperdashRight { get; set; }
        public MylaDeathCondition MylaDeathHandling = MylaDeathCondition.DieAfterFullSuperdash;
        [Newtonsoft.Json.JsonIgnore] public bool hasSuperdashAny => hasSuperdashLeft ^ hasSuperdashRight || PlayerData.instance.GetBoolInternal(nameof(PlayerData.hasSuperDash));
        [Newtonsoft.Json.JsonIgnore] public bool hasSuperdashBoth => (hasSuperdashLeft && hasSuperdashRight) || PlayerData.instance.GetBoolInternal(nameof(PlayerData.hasSuperDash));

        public override void Initialize()
        {
            Events.AddFsmEdit(new("Equipment", "Build Equipment List"), EditInventory);
            Events.AddFsmEdit(SceneNames.Mines_31, new("mine_break_bridge", "Bridge Control"), PreventBridgeBreak);
            Events.AddLanguageEdit(new("UI", "INV_NAME_SUPERDASH"), EditSuperdashName);
            Events.AddLanguageEdit(new("UI", "INV_DESC_SUPERDASH"), EditSuperdashDesc);
            Modding.ModHooks.GetPlayerBoolHook += SkillBoolGetOverride;
            Modding.ModHooks.SetPlayerBoolHook += SkillBoolSetOverride;
            Events.AddFsmEdit(SceneNames.Crossroads_45, new("Miner", "Leave"), SetMylaDeathCondition);
            Events.AddFsmEdit(SceneNames.Crossroads_45, new("Zombie Myla", "FSM"), SetMylaDeathCondition);
            Events.AddFsmEdit(new("Knight", "Superdash"), CheckMidSuperdashTurnAroundDirection);
            //On.DeactivateIfPlayerdataTrue.OnEnable += SetMylaDeathCondition;
            //On.DeactivateIfPlayerdataFalse.OnEnable += SetMylaDeathCondition;
        }

        public override void Unload()
        {
            Events.RemoveFsmEdit(new("Equipment", "Build Equipment List"), EditInventory);
            Events.RemoveFsmEdit(SceneNames.Mines_31, new("mine_break_bridge", "Bridge Control"), PreventBridgeBreak);
            Events.RemoveLanguageEdit(new("UI", "INV_NAME_SUPERDASH"), EditSuperdashName);
            Events.RemoveLanguageEdit(new("UI", "INV_DESC_SUPERDASH"), EditSuperdashDesc);
            Modding.ModHooks.GetPlayerBoolHook -= SkillBoolGetOverride;
            Modding.ModHooks.SetPlayerBoolHook -= SkillBoolSetOverride;
            Events.RemoveFsmEdit(SceneNames.Crossroads_45, new("Miner", "Leave"), SetMylaDeathCondition);
            Events.RemoveFsmEdit(SceneNames.Crossroads_45, new("Zombie Myla", "FSM"), SetMylaDeathCondition);
            Events.RemoveFsmEdit(new("Knight", "Superdash"), CheckMidSuperdashTurnAroundDirection);
            //On.DeactivateIfPlayerdataTrue.OnEnable -= SetMylaDeathCondition;
            //On.DeactivateIfPlayerdataFalse.OnEnable -= SetMylaDeathCondition;
        }

        private void EditInventory(PlayMakerFSM fsm)
        {
            fsm.GetState("Super Dash").GetFirstActionOfType<PlayerDataBoolTest>().boolName.Value = nameof(hasSuperdashAny);
        }

        private bool SkillBoolGetOverride(string boolName, bool value)
        {
            return boolName switch
            {
                nameof(hasSuperdashLeft) => hasSuperdashLeft,
                nameof(hasSuperdashRight) => hasSuperdashRight,
                nameof(PlayerData.hasSuperDash) => value
                    || HeroController.instance.cState.onGround && (HeroController.instance.cState.facingRight ? hasSuperdashRight : hasSuperdashLeft)
                    || HeroController.instance.cState.wallSliding && (HeroController.instance.cState.facingRight ? hasSuperdashLeft : hasSuperdashRight),
                nameof(hasSuperdashAny) => hasSuperdashAny,
                nameof(hasSuperdashBoth) => hasSuperdashBoth,
                _ => value,
            };
        }

        private bool SkillBoolSetOverride(string boolName, bool value)
        {
            switch (boolName)
            {
                case nameof(hasSuperdashLeft):
                    hasSuperdashLeft = value;
                    goto BoolChanged;

                case nameof(hasSuperdashRight):
                    hasSuperdashRight = value;
                    goto BoolChanged;

                BoolChanged:
                    if (hasSuperdashLeft && hasSuperdashRight)
                    {
                        PlayerData.instance.SetBool(nameof(PlayerData.hasSuperDash), true);
                        PlayerData.instance.SetBool(nameof(PlayerData.canSuperDash), true);
                    }
                    break;
            }
            return value;
        }

        private void EditSuperdashName(ref string value)
        {
            if (hasSuperdashLeft && !hasSuperdashRight)
            {
                value = Language.Language.Get("ITEMCHANGER_NAME_LEFT_SUPERDASH", "UI");
            }
            else if (!hasSuperdashLeft && hasSuperdashRight)
            {
                value = Language.Language.Get("ITEMCHANGER_NAME_RIGHT_SUPERDASH", "UI");
            }
        }

        private void EditSuperdashDesc(ref string value)
        {
            if (hasSuperdashLeft && !hasSuperdashRight)
            {
                value = Language.Language.Get("ITEMCHANGER_SHOPDESC_LEFT_SUPERDASH", "UI");
            }
            else if (!hasSuperdashLeft && hasSuperdashRight)
            {
                value = Language.Language.Get("ITEMCHANGER_SHOPDESC_RIGHT_SUPERDASH", "UI");
            }
        }

        private void PreventBridgeBreak(PlayMakerFSM fsm)
        {
            fsm.GetState("Idle").GetFirstActionOfType<PlayerDataBoolTest>().boolName = nameof(hasSuperdashBoth);
        }       

        public enum MylaDeathCondition
        {
            Vanilla,
            DieAfterFullSuperdash = Vanilla,
            DieAfterEitherSuperdash,
            DieAfterLeftSuperdash,
            DieAfterRightSuperdash,
        }

        private void SetMylaDeathCondition(PlayMakerFSM fsm)
        {
            if ((fsm.gameObject.name == "Miner" && fsm.FsmVariables.GetFsmString("playerData bool").Value == nameof(PlayerData.hasSuperDash))
                || fsm.gameObject.name == "Zombie Myla")
            {
                fsm.FsmVariables.GetFsmString("playerData bool").Value = MylaDeathHandling switch
                {
                    MylaDeathCondition.DieAfterFullSuperdash => nameof(hasSuperdashBoth),
                    MylaDeathCondition.DieAfterRightSuperdash => nameof(hasSuperdashRight),
                    MylaDeathCondition.DieAfterLeftSuperdash => nameof(hasSuperdashLeft),
                    MylaDeathCondition.DieAfterEitherSuperdash => nameof(hasSuperdashAny),
                    _ => nameof(PlayerData.canSuperDash),
                };
            }
        }

        private void CheckMidSuperdashTurnAroundDirection(PlayMakerFSM fsm)
        {
            LambdaEveryFrame directionCheck = new(() =>
            {
                if (HeroController.instance.cState.facingRight && !hasSuperdashRight && !hasSuperdashBoth
                    || !HeroController.instance.cState.facingRight && !hasSuperdashLeft && !hasSuperdashBoth)
                {
                    fsm.SendEvent("SLOPE CANCEL");
                }
            });
            fsm.GetState("Dashing").AddLastAction(directionCheck);
            fsm.GetState("Cancelable").AddLastAction(directionCheck);
        }
    }
}