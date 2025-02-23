/* ---------------------------------------
 * Project: 'uGUI Directed Navigation'
 * Studio:          IEVO
 * Site:     https://ievo.games/
 * -------------------------------------*/
using System;
using UnityEngine;

namespace IEVO.UI.uGUIDirectedNavigation
{
    [Serializable]
    public partial class Config
    {
        public const float DirectivityMax = 12f;

        [SerializeField] private DirectedNavigationType.Value m_Type = DirectedNavigationType.Value.Automatic;

        [SerializeField] private bool m_wrapAround = false;
        [SerializeField] private bool m_UseEdges = false;
        [SerializeField] private bool m_Omnidirectional = false;        
        [SerializeField] private float m_Directivity = 0f;

        [SerializeField] private AnchorConfig m_Anchor = new AnchorConfig();

        [SerializeField] private SectorConfig m_Sector = new SectorConfig();
        [SerializeField] private RectangleConfig m_Rectangle = new RectangleConfig();
        [SerializeField] private RectTransformConfig m_RectTransform = new RectTransformConfig();
        [SerializeField] private SelectableListConfig m_SelectableList = new SelectableListConfig();


        public DirectedNavigationType.Value Type { get => m_Type; set => m_Type = value; }

        public bool WrapAround { get => m_wrapAround; set => m_wrapAround = value; }
        public bool UseEdges { get => m_UseEdges; set => m_UseEdges = value; }
        public bool Omnidirectional { get => m_Omnidirectional; set => m_Omnidirectional = value; }
        
        public float Directivity
        {
            get => m_Directivity;
            set => m_Directivity = Mathf.Clamp(value, -DirectivityMax, DirectivityMax);
        }

        public AnchorConfig Anchor { get => m_Anchor; }

        public SectorConfig Sector { get => m_Sector; }
        public RectangleConfig Rectangle { get => m_Rectangle; }
        public RectTransformConfig RectTransform { get => m_RectTransform; }
        public SelectableListConfig SelectableList { get => m_SelectableList; }
    }
}
