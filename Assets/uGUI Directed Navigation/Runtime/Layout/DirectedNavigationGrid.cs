/* ---------------------------------------
 * Project: 'uGUI Directed Navigation'
 * Studio:          IEVO
 * Site:     https://ievo.games/
 * -------------------------------------*/
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace IEVO.UI.uGUIDirectedNavigation.Layout
{
    [HelpURL("https://ievo.games/uGUIDirectedNavigation/Downloads/uGUI_Directed_Navigation_1.4.0.pdf")]
    [AddComponentMenu("UI/DirectedNavigationGrid", 91)]
    [ExecuteAlways]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(GridLayoutGroup))]
    public class DirectedNavigationGrid : MonoBehaviour, ILayoutController
    {
        protected enum Side { Left, Right, Top, Bottom }


        [SerializeField] protected GridSideConfig m_ConfigLeft = new GridSideConfig();
        [SerializeField] protected GridSideConfig m_ConfigRight = new GridSideConfig();

        [SerializeField] protected GridSideConfig m_ConfigUp = new GridSideConfig();
        [SerializeField] protected GridSideConfig m_ConfigDown = new GridSideConfig();


        public GridSideConfig ConfigLeft { get => m_ConfigLeft; }
        public GridSideConfig ConfigRight { get => m_ConfigRight; }
        public GridSideConfig ConfigUp { get => m_ConfigUp; }
        public GridSideConfig ConfigDown { get => m_ConfigDown; }

        public List<DirectedNavigation> DirectedNavigations { get => m_DirectedNavigations; }
        public GridLayoutGroup GridLayoutGroup { get => m_GridLayoutGroup; }

        public event Action OnUpdateGrid;


        protected RectTransform m_RectTransform;
        protected GridLayoutGroup m_GridLayoutGroup;
        protected List<DirectedNavigation> m_DirectedNavigations = new List<DirectedNavigation>();

        protected List<DirectedNavigation> m_LeftMostElements = new List<DirectedNavigation>();
        protected List<DirectedNavigation> m_RightMostElements = new List<DirectedNavigation>();
        protected List<DirectedNavigation> m_TopMostElements = new List<DirectedNavigation>();
        protected List<DirectedNavigation> m_BottomMostElements = new List<DirectedNavigation>();

        protected DirectedNavigation[,] m_DirNavsTable = new DirectedNavigation[50,50];
        protected int m_DirNavsTableRowCount;
        protected int m_DirNavsTableBufferCount;


        protected void OnEnable()
        {
            UpdateGrid();
        }


        //  ILayoutController
        public void SetLayoutHorizontal()
        {
            UpdateGrid();
        }

        public void SetLayoutVertical()
        {
            UpdateGrid();
        }
        //  ILayoutController


        public void UpdateGrid()
        {
            GetMainComponents();

            if (m_GridLayoutGroup.isActiveAndEnabled)
            {
                FillDirNavsTable();
                GetOutermost();
                ResetSettings();
                ApplySettings(m_LeftMostElements, m_ConfigLeft, Side.Left);
                ApplySettings(m_RightMostElements, m_ConfigRight, Side.Right);
                ApplySettings(m_TopMostElements, m_ConfigUp, Side.Top);
                ApplySettings(m_BottomMostElements, m_ConfigDown, Side.Bottom);

                OnUpdateGrid?.Invoke();
            }
        }


        protected void GetMainComponents()
        {
            m_RectTransform = m_RectTransform == null ? GetComponent<RectTransform>() : m_RectTransform;
            m_GridLayoutGroup = m_GridLayoutGroup == null ? GetComponent<GridLayoutGroup>() : m_GridLayoutGroup;
        }


        protected void FillDirNavsTable()
        {
            GetComponentsInChildren(false, m_DirectedNavigations);

            Array.Clear(m_DirNavsTable, 0, m_DirNavsTable.Length);

            float totalWidth = m_RectTransform.rect.width;
            float totalHeight = m_RectTransform.rect.height;

            float cellWidth = m_GridLayoutGroup.cellSize.x + m_GridLayoutGroup.spacing.x;
            float cellHeight = m_GridLayoutGroup.cellSize.y + m_GridLayoutGroup.spacing.y;

            int rows = Mathf.FloorToInt((totalHeight + m_GridLayoutGroup.spacing.y) / cellHeight);
            int columns = Mathf.FloorToInt((totalWidth + m_GridLayoutGroup.spacing.x) / cellWidth);

            if (m_DirNavsTable.GetLength(0) < rows)
                Utils.ResizeArray(m_DirNavsTable, rows, m_DirNavsTable.GetLength(1));

            if (m_DirNavsTable.GetLength(1) < columns)
                Utils.ResizeArray(m_DirNavsTable, m_DirNavsTable.GetLength(0), columns);

            m_DirNavsTableRowCount = rows;
            m_DirNavsTableBufferCount = columns;

            for (int i = 0; i < m_DirectedNavigations.Count; i++)
            {
                DirectedNavigation dirNav = m_DirectedNavigations[i];

                if (dirNav.gameObject.activeInHierarchy && dirNav.gameObject.transform.parent == transform)
                {
                    RectTransform dirNavRectTransform = dirNav.GetComponent<RectTransform>();
                    Vector3 pos = dirNavRectTransform.anchoredPosition;
                    int row = Mathf.FloorToInt(-pos.y / cellHeight);
                    int column = Mathf.FloorToInt(pos.x / cellWidth);

                    if (m_DirNavsTable.GetLength(0) < rows)
                        Utils.ResizeArray(m_DirNavsTable, rows, m_DirNavsTable.GetLength(1));

                    if (m_DirNavsTable.GetLength(1) < columns)
                        Utils.ResizeArray(m_DirNavsTable, m_DirNavsTable.GetLength(0), columns);

                    m_DirNavsTable[row, column] = dirNav;
                }
            }
        }


        protected void GetOutermost()
        {
            m_LeftMostElements.Clear();
            m_RightMostElements.Clear();
            m_TopMostElements.Clear();
            m_BottomMostElements.Clear();

            for (int row = 0; row < m_DirNavsTableRowCount; row++)
            {
                for (int column = 0; column < m_DirNavsTableBufferCount; column++)
                {
                    DirectedNavigation dirNav = m_DirNavsTable[row, column];

                    if (row == 0)
                    {
                        if(dirNav != null)
                            m_TopMostElements.Add(dirNav);
                        else
                        {
                            int rowShift = row + 1;
                            DirectedNavigation dirNavShifted = null;

                            while (dirNavShifted == null && rowShift < m_DirNavsTableRowCount)
                            {
                                dirNavShifted = m_DirNavsTable[rowShift, column];
                                rowShift++;
                            }

                            if (dirNavShifted != null)
                                m_TopMostElements.Add(dirNavShifted);
                        }
                    }

                    if (row == m_DirNavsTableRowCount - 1)
                    {
                        if (dirNav != null)
                            m_BottomMostElements.Add(dirNav);
                        else
                        {
                            int rowShift = row - 1;
                            DirectedNavigation dirNavShifted = null;

                            while (dirNavShifted == null && rowShift >= 0)
                            {
                                dirNavShifted = m_DirNavsTable[rowShift, column];
                                rowShift--;
                            }

                            if (dirNavShifted != null)
                                m_BottomMostElements.Add(dirNavShifted);
                        }
                    }

                    if (column == 0)
                    {
                        if (dirNav != null)
                            m_LeftMostElements.Add(dirNav);
                        else
                        {
                            int columnShift = column + 1;
                            DirectedNavigation dirNavShifted = null;

                            while (dirNavShifted == null && columnShift < m_DirNavsTableBufferCount)
                            {
                                dirNavShifted = m_DirNavsTable[row, columnShift];
                                columnShift++;
                            }

                            if (dirNavShifted != null)
                                m_LeftMostElements.Add(dirNavShifted);
                        }
                    }

                    if (column == m_DirNavsTableBufferCount - 1)
                    {
                        if (dirNav != null)
                            m_RightMostElements.Add(dirNav);
                        else
                        {
                            int columnShift = column - 1;
                            DirectedNavigation dirNavShifted = null;

                            while (dirNavShifted == null && columnShift >= 0)
                            {
                                dirNavShifted = m_DirNavsTable[row, columnShift];
                                columnShift--;
                            }

                            if (dirNavShifted != null)
                                m_RightMostElements.Add(dirNavShifted);
                        }
                    }
                }
            }
        }


        protected void ResetSettings()
        {
            for (int i = 0; i < m_DirectedNavigations.Count; i++)
            {
                ResetDirNavConfig(m_DirectedNavigations[i].ConfigLeft);
                ResetDirNavConfig(m_DirectedNavigations[i].ConfigRight);
                ResetDirNavConfig(m_DirectedNavigations[i].ConfigUp);
                ResetDirNavConfig(m_DirectedNavigations[i].ConfigDown);
            }
        }


        protected void ResetDirNavConfig(Config config)
        {
            config.Type = DirectedNavigationType.Value.Automatic;
            config.Anchor.Reset();
            config.Directivity = 0f;
            config.WrapAround = false;
            config.Omnidirectional = false;
        }


        protected void ApplySettings(List<DirectedNavigation> dirNavs, GridSideConfig gridWrapConfig, Side side)
        {
            for (int i = 0; i < dirNavs.Count; i++)
            {
                DirectedNavigation dirNav = dirNavs[i];
                RectTransform dirNavRectTransform = dirNav.GetComponent<RectTransform>();

                Vector3 anchorOutsideShift = Vector3.zero;

                Config dirNavConfig = null;

                if (side == Side.Left)
                {
                    dirNavConfig = dirNav.ConfigLeft;

                    if (gridWrapConfig.Strict)
                        anchorOutsideShift.x = dirNav.transform.InverseTransformPoint(m_RectTransform.TransformPoint(m_RectTransform.rect.min)).x + dirNavRectTransform.rect.width / 2;
                }
                if (side == Side.Right)
                {
                    dirNavConfig = dirNav.ConfigRight;

                    if (gridWrapConfig.Strict)
                        anchorOutsideShift.x = dirNav.transform.InverseTransformPoint(m_RectTransform.TransformPoint(m_RectTransform.rect.max)).x - dirNavRectTransform.rect.width / 2;
                }
                if (side == Side.Top)
                {
                    dirNavConfig = dirNav.ConfigUp;

                    if (gridWrapConfig.Strict)
                        anchorOutsideShift.y = dirNav.transform.InverseTransformPoint(m_RectTransform.TransformPoint(m_RectTransform.rect.max)).y - dirNavRectTransform.rect.height / 2;
                }
                if (side == Side.Bottom)
                {
                    dirNavConfig = dirNav.ConfigDown;

                    if (gridWrapConfig.Strict)
                        anchorOutsideShift.y = dirNav.transform.InverseTransformPoint(m_RectTransform.TransformPoint(m_RectTransform.rect.min)).y + dirNavRectTransform.rect.height / 2;
                }

                //  OutgoingTransitions
                if (gridWrapConfig.OutgoingTransitions)
                {
                    dirNavConfig.Type = DirectedNavigationType.Value.Automatic;
                }
                else
                {
                    dirNavConfig.Type = DirectedNavigationType.Value.RectTransform;
                    dirNavConfig.RectTransform.RectTransform = m_RectTransform;
                }
                //  OutgoingTransitions

                //  WrapAround
                if (gridWrapConfig.WrapAround == WrapAroundType.Value.Direct)
                {
                    dirNavConfig.WrapAround = true;
                    dirNavConfig.Anchor.Type = Anchor.Type.Shift;
                    dirNavConfig.Anchor.SetShift(anchorOutsideShift);
                    dirNavConfig.Directivity = gridWrapConfig.Directivity;
                }
                else if (gridWrapConfig.WrapAround == WrapAroundType.Value.Prev)
                {
                    dirNavConfig.WrapAround = true;
                    dirNavConfig.Anchor.Type = Anchor.Type.Shift;

                    Vector3 anchorPos = Vector3.zero;

                    if (i > 0)
                        anchorPos = dirNav.transform.InverseTransformPoint(dirNavs[i - 1].transform.position);
                    else
                    {
                        if (gridWrapConfig.Loop)
                            anchorPos = dirNav.transform.InverseTransformPoint(dirNavs[dirNavs.Count - 1].transform.position);
                        else
                        {
                            dirNavConfig.WrapAround = false;
                            dirNavConfig.Anchor.Type = Anchor.Type.Identity;
                            dirNavConfig.Anchor.SetShift(anchorOutsideShift);
                        }
                    }

                    if (side == Side.Left || side == Side.Right)
                    {
                        anchorPos.x = 0;
                        anchorPos.z = 0;
                    }
                    else
                    {
                        anchorPos.y = 0;
                        anchorPos.z = 0;
                    }
                    
                    dirNavConfig.Anchor.SetShift(anchorPos + anchorOutsideShift);
                    dirNavConfig.Directivity = gridWrapConfig.Directivity;
                }
                else if (gridWrapConfig.WrapAround == WrapAroundType.Value.Next)
                {
                    dirNavConfig.WrapAround = true;
                    dirNavConfig.Anchor.Type = Anchor.Type.Shift;

                    Vector3 anchorPos = Vector3.zero;

                    if (i < dirNavs.Count - 1)
                        anchorPos = dirNav.transform.InverseTransformPoint(dirNavs[i + 1].transform.position);
                    else
                    {
                        if (gridWrapConfig.Loop)
                            anchorPos = dirNav.transform.InverseTransformPoint(dirNavs[0].transform.position);
                        else
                        {
                            dirNavConfig.WrapAround = false;
                            dirNavConfig.Anchor.Type = Anchor.Type.Identity;
                            dirNavConfig.Anchor.SetShift(anchorOutsideShift);
                        }
                    }

                    if (side == Side.Left || side == Side.Right)
                    {
                        anchorPos.x = 0;
                        anchorPos.z = 0;
                    }
                    else
                    {
                        anchorPos.y = 0;
                        anchorPos.z = 0;
                    }

                    dirNavConfig.Anchor.SetShift(anchorPos + anchorOutsideShift);
                    dirNavConfig.Directivity = gridWrapConfig.Directivity;
                }
                else
                {
                    dirNavConfig.WrapAround = false;
                    dirNavConfig.Anchor.Type = Anchor.Type.Identity;
                    dirNavConfig.Anchor.SetShift(Vector3.zero);
                    dirNavConfig.Directivity = 0f;
                }
                //  WrapAround
            }
        }


    }
}

