namespace ItemChanger.Items
{
    public class NotchItem : IntItem
    {
        public bool refillHealth = true;

        public override void GiveImmediate(GiveInfo info)
        {
            base.GiveImmediate(info);
            if (refillHealth)
            {
                HeroController.instance.CharmUpdate();
                HeroController.instance.proxyFSM.SendEvent("HeroCtrl-MaxHealth");
                PlayMakerFSM.BroadcastEvent("UPDATE BLUE HEALTH");
                GameCameras.instance.hudCanvas.transform.FindChild("Health").gameObject.LocateMyFSM("Blue Health Control").SendEvent("UPDATE BLUE HEALTH");
            }
            GameManager.instance.RefreshOvercharm();
        }
    }
}
