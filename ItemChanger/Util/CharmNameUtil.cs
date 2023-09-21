namespace ItemChanger.Util
{
    public static class CharmNameUtil
    {
        public static string GetCharmName(int id) => Language.Language.Get(GetCharmNameKey(id), "UI");
        public static string GetCharmNameKey(int id)
        {
            string key = "CHARM_NAME_" + id;
            switch (id)
            {
                case 36:
                    return PlayerData.instance.GetInt(nameof(PlayerData.royalCharmState)) switch
                    {
                        0 or 1 or 2 => key + "_A",
                        3 => key + "_B",
                        _ => key + "_C"
                    };
                default:
                    return key;
            }
        }
    }
}
