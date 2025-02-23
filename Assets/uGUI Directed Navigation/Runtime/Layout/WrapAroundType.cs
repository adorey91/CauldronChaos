using System;

namespace IEVO.UI.uGUIDirectedNavigation.Layout
{
    public static class WrapAroundType
    {
        [Serializable] public enum Value : int { Disabled, Direct,  Prev, Next }

        public static readonly string[] Names = Enum.GetNames(typeof(Value));

        public static int ToInt(this Value type)
        {
            return (int)type;
        }
    }
}
