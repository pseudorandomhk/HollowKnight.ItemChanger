namespace ItemChanger.Items
{
    /// <summary>
    /// Item which gives the specified amount of blue health.
    /// </summary>
    public class LifebloodItem : AbstractItem
    {
        public int amount;

        public override void GiveImmediate(GiveInfo info)
        {
            try
            {
                PlayMakerFSM blueHealthControl = PlayMakerFSM.FsmList.FirstOrDefault(f => f != null && f.FsmName == "Blue Health Control");
                if (blueHealthControl != null) blueHealthControl.SendEvent("INVENTORY OPENED");
            }
            catch (Exception) { }

            var lifebloodControl = GameCameras.instance.hudCanvas.transform.FindChild("Health").gameObject.LocateMyFSM("Blue Health Control");
            for (int i = 0; i < amount; i++)
            {
                lifebloodControl.SendEvent("ADD BLUE HEALTH");
            }
        }
    }
}
