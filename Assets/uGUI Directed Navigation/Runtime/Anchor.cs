using System;

namespace IEVO.UI.uGUIDirectedNavigation
{
    public static class Anchor
    {
        [Serializable] public enum Type : int { Identity, Shift, RectTransform }

        public static readonly string[] Names = Enum.GetNames(typeof(Type));


        public static int ToInt(this Type type)
        {
            return (int)type;
        }
    }
}
