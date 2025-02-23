/* ---------------------------------------
 * Project: 'uGUI Directed Navigation'
 * Studio:          IEVO
 * Site:     https://ievo.games/
 * -------------------------------------*/
using System;
using UnityEngine;
using UnityEngine.UI;

namespace IEVO.UI.uGUIDirectedNavigation
{
    public partial class Config
    {
        [Serializable]
        public class SectorConfig
        {
            [SerializeField] private float m_MinAngle = -25f;
            [SerializeField] private float m_MaxAngle = 25f;

            [SerializeField] private float m_Radius = 500f;


            public float MinAngle { get => m_MinAngle; }
            public const float MinAngleLimit = -90f;
            public const float MinAngleLimitOmnidirectional = -180f;

            public float MaxAngle { get => m_MaxAngle; }
            public const float MaxAngleLimit = 90f;
            public const float MaxAngleLimitOmnidirectional = 180f;

            public float Radius 
            { 
                get => m_Radius; 
                set => m_Radius = value;
            }


            public void SetMinAngle(float min, bool omnidirectional = false)
            {
                if (omnidirectional)
                    m_MinAngle = Mathf.Clamp(min, MinAngleLimitOmnidirectional, MaxAngle);
                else
                    m_MinAngle = Mathf.Clamp(min, MinAngleLimit, MaxAngle);
            }

            public void SetMaxAngle(float max, bool omnidirectional = false)
            {
                if (omnidirectional)
                    m_MaxAngle = Mathf.Clamp(max, MinAngle, MaxAngleLimitOmnidirectional);
                else
                    m_MaxAngle = Mathf.Clamp(max, MinAngle, MaxAngleLimit);
            }
        }


        [Serializable]
        public class RectangleConfig
        {
            [SerializeField] private Vector3[] m_Verts = new Vector3[4];            

            public Vector3[] Verts { get => m_Verts; }
        }


        [Serializable]
        public class RectTransformConfig
        {
            [SerializeField] private RectTransform m_RectTransform;

            public RectTransform RectTransform { get => m_RectTransform; set => m_RectTransform = value; }
        }


        [Serializable]
        public class SelectableListConfig
        {
            [SerializeField] private Selectable[] m_SelectableList = new Selectable[0];
            [SerializeField] private bool m_UseListOrder;

            public Selectable[] SelectableList { get => m_SelectableList; set => m_SelectableList = value; }
            public bool UseListOrder { get => m_UseListOrder; set => m_UseListOrder = value; }
        }


        [Serializable]
        public class AnchorConfig
        {
            [SerializeField] private Anchor.Type m_Type;
            [SerializeField] private Vector3 m_Shift;
            [SerializeField] private RectTransform m_RectTransform;

            public Anchor.Type Type { get => m_Type; set => m_Type = value; }
            public Vector3 Shift { get => m_Shift; }
            public RectTransform RectTransform { get => m_RectTransform; }

            public Vector3 GetAnchoredPositionLocal(Vector3 position, Transform space)
            {
                if (m_Type == uGUIDirectedNavigation.Anchor.Type.Identity)
                    return position;
                else if (m_Type == uGUIDirectedNavigation.Anchor.Type.Shift)
                    return position + m_Shift;
                else
                    return m_RectTransform != null && m_RectTransform.gameObject.activeInHierarchy && space != null ? space.InverseTransformPoint(m_RectTransform.position) : position;
            }

            /// <summary>
            /// Point in local coordinate space.
            /// </summary>
            /// <param name="value"></param>
            public void SetShift(Vector3 value)
            {
                m_Shift = value;
            }

            public void SetRectTransform(RectTransform value)
            {
                m_RectTransform = value;
            }

            public void Reset()
            {
                m_Type = uGUIDirectedNavigation.Anchor.Type.Identity;
                m_Shift = Vector3.zero;
                m_RectTransform = null;
            }
        }


    }
}
