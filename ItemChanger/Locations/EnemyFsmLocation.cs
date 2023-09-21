using ItemChanger.Extensions;
using ItemChanger.FsmStateActions;
using ItemChanger.Util;

namespace ItemChanger.Locations
{
    /// <summary>
    /// /A variant of EnemyLocation which accounts for the fact that some enemies may not be loaded at activeSceneChanged, and are easier to locate by fsm.
    /// </summary>
    public class EnemyFsmLocation : ContainerLocation
    {
        // enemy info - look for fsm in OnEnable, rather than object on scene entry
        public string enemyFsm;
        public string? enemyObj;
        public bool removeGeo;
        private Action? _cleanupAction;

        protected override void OnLoad()
        {
            Events.AddFsmEdit(UnsafeSceneName, new(enemyObj, enemyFsm), OnEnable);
        }

        protected override void OnUnload()
        {
            Events.RemoveFsmEdit(UnsafeSceneName, new(enemyObj, enemyFsm), OnEnable);
            DoCleanup();
        }

        public override bool Supports(string containerType)
        {
            if (containerType == Container.Chest || containerType == Container.Totem) return false;
            if (Container.GetContainer(containerType) is not Container c || !c.SupportsDrop) return false;
            return base.Supports(containerType);
        }

        public void OnEnable(PlayMakerFSM fsm)
        {
            DoCleanup();

            GameObject enemy = fsm.gameObject;
            PlayMakerFSM hm = enemy.LocateMyFSM("health_manager_enemy");
            if (hm.FsmVariables.GetFsmBool("Special Death").Value)
            {
                var specialDeathTransition = hm.GetState("Pause").Transitions.First(t => t.EventName == "RETURN");
                var onDeathState = new FsmState(hm.Fsm);
                onDeathState.Name = "ItemChanger::Give Item";

                var onDeathAction = new Lambda(OnDeath);
                onDeathState.Actions = new[] { onDeathAction };
                onDeathState.AddTransition(FsmEvent.Finished, hm.Fsm.GetState(specialDeathTransition.ToState));

                specialDeathTransition.ToState = onDeathState.Name;
            }
            else
            {
                var deathControl = enemy.LocateMyFSM("death_control");
                deathControl.GetState("Destroy").AddFirstAction(new Lambda(OnDeath));
            }

            if (removeGeo)
            {
                hm.FsmVariables.GetFsmInt("Geo Small").Value = 0;
                hm.FsmVariables.GetFsmInt("Geo Medium").Value = 0;
                hm.FsmVariables.GetFsmInt("Geo Large").Value = 0;
            }

            void OnDeath()
            {
                if (flingType == FlingType.DirectDeposit)
                {
                    GiveDirectly(enemy.transform);
                    Placement.AddVisitFlag(VisitState.Dropped);
                }
                else
                {
                    GiveEarly(enemy.transform);
                    Placement.AddVisitFlag(VisitState.Dropped);
                    GetContainer(out GameObject obj, out string containerType);
                    Container c = Container.GetContainer(containerType)!;
                    c.ApplyTargetContext(obj, enemy.transform.position.x, enemy.transform.position.y, 0);
                    if (containerType == Container.Shiny && !Placement.GetPlacementAndLocationTags().OfType<Tags.ShinyFlingTag>().Any())
                    {
                        ShinyUtility.SetShinyFling(obj.LocateMyFSM("Shiny Control"), ShinyFling.RandomLR);
                    }
                }
                DoCleanup();
            }
        }

        private void GiveDirectly(Transform t)
        {
            ItemUtility.GiveSequentially(Placement.Items, Placement, new GiveInfo
            {
                Container = "Enemy",
                FlingType = flingType,
                MessageType = MessageType.Corner,
                Transform = t,
            });
        }

        private void GiveEarly(Transform t)
        {
            ItemUtility.GiveSequentially(
                Placement.Items.Where(i => i.GiveEarly("Enemy")), 
                Placement,
                new GiveInfo 
                {
                    Container = "Enemy",
                    FlingType = flingType,
                    MessageType = MessageType.Corner,
                    Transform = t,
                });
        }

        private void DoCleanup()
        {
            if (_cleanupAction != null)
            {
                try { _cleanupAction.Invoke(); } catch { }
                _cleanupAction = null;
            }
        }
    }
}
