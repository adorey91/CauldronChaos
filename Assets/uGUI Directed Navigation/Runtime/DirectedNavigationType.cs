using System;

namespace IEVO.UI.uGUIDirectedNavigation
{
    public static class DirectedNavigationType
    {
        [Serializable] public enum Value : int { Disabled, Automatic, Sector, Rectangle, RectTransform, SelectableList }

        public static readonly string[] Names = Enum.GetNames(typeof(Value));


        public static int ToInt(this Value type)
        {
            return (int)type;
        }
    }
}
