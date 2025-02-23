using System;
using UnityEngine;

namespace IEVO.UI.uGUIDirectedNavigation.Layout
{
    [Serializable]
    public class GridSideConfig
    {
        [SerializeField] private bool m_OutgoingTransitions = false;
        [SerializeField] private WrapAroundType.Value m_WrapAround;
        [SerializeField] private bool m_Strict = false;
        [SerializeField] private bool m_Loop = true;
        [SerializeField] private float m_Directivity = 0f;

        public bool OutgoingTransitions { get => m_OutgoingTransitions; set => m_OutgoingTransitions = value; }
        public WrapAroundType.Value WrapAround { get => m_WrapAround; set => m_WrapAround = value; }
        public bool Strict { get => m_Strict; set => m_Strict = value; }
        public bool Loop { get => m_Loop; set => m_Loop = value; }
        public float Directivity
        {
            get => m_Directivity;
            set => m_Directivity = Mathf.Clamp(value, -Config.DirectivityMax, Config.DirectivityMax);
        }
    }
}
