/* ---------------------------------------
 * Project: 'uGUI Directed Navigation'
 * Studio:          IEVO
 * Site:     https://ievo.games/
 * -------------------------------------*/
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace IEVO.UI.uGUIDirectedNavigation
{
    /// <summary>
    /// 'uGUI Directed Navigation'. The component that handles the UI navigation for the GameObject it's added to.
    /// Require components: RectTransform, Selectable.
    /// </summary>
    [HelpURL("https://ievo.games/uGUIDirectedNavigation/Downloads/uGUI_Directed_Navigation_1.4.0.pdf")]
    [AddComponentMenu("UI/DirectedNavigation", 90)]
    [ExecuteAlways]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(Selectable))]
    public class DirectedNavigation : MonoBehaviour, IMoveHandler
    {
        /// <summary>
        /// Enable/Disable UI navigation handling for current GameObject.
        /// </summary>
        public bool Active { 
            get => m_Active; 
            set
            {
                m_Active = value;
                OnActive?.Invoke(m_Active);
            }
        }

        public Selectable Selectable { get => m_Selectable; }

        public Config ConfigLeft { get => m_ConfigLeft; }
        public Config ConfigRight { get => m_ConfigRight; }
        public Config ConfigUp { get => m_ConfigUp; }
        public Config ConfigDown { get => m_ConfigDown; }

        public event Action<bool> OnActive;


        [SerializeField] private bool m_Active = true;

        [SerializeField] private Config m_ConfigLeft = new Config();
        [SerializeField] private Config m_ConfigRight = new Config();
        [SerializeField] private Config m_ConfigUp = new Config();
        [SerializeField] private Config m_ConfigDown = new Config();


        protected Selectable m_Selectable;

        // Buffer. Used for NonAlloc getting of Selectables.
        static protected Selectable[] s_SelectableBuffer = new Selectable[30];
        static protected int s_SelectableBufferCount;


        public virtual void Awake()
        {
            m_Selectable = GetComponent<Selectable>();
        }


        public virtual void OnMove(AxisEventData eventData)
        {
            if (m_Active)
            {
                switch (eventData.moveDir)
                {
                    case MoveDirection.Right:
                        Navigate(eventData, FindSelectableRight());
                        break;

                    case MoveDirection.Up:
                        Navigate(eventData, FindSelectableUp());
                        break;

                    case MoveDirection.Left:
                        Navigate(eventData, FindSelectableLeft());
                        break;

                    case MoveDirection.Down:
                        Navigate(eventData, FindSelectableDown());
                        break;
                }
            }
        }


        protected void Navigate(AxisEventData eventData, Selectable sel)
        {
            if (sel != null && sel.IsActive())
                eventData.selectedObject = sel.gameObject;
            else
                eventData.selectedObject = gameObject;
        }


        public virtual Selectable FindSelectableLeft()
        {
            return FindSelectable(transform.rotation * Vector3.left, m_ConfigLeft);
        }

        public virtual Selectable FindSelectableRight()
        {
            return FindSelectable(transform.rotation * Vector3.right, m_ConfigRight);
        }

        public virtual Selectable FindSelectableUp()
        {
            return FindSelectable(transform.rotation * Vector3.up, m_ConfigUp);
        }

        public virtual Selectable FindSelectableDown()
        {
            return FindSelectable(transform.rotation * Vector3.down, m_ConfigDown);
        }


        /// <summary>
        /// Find Selectable for direction.
        /// </summary>
        /// <param name="dir">Direction in which will be search of Selectable.</param>
        /// <param name="config">Configuration for specified direction.</param>
        /// <returns></returns>
        protected Selectable FindSelectable(Vector3 dir, Config config)
        {
            if (config.Type == DirectedNavigationType.Value.Disabled)
                return null;

            dir = dir.normalized;

            Vector3 localDir = Quaternion.Inverse(transform.rotation) * dir;
            Vector3 pointOnRectEdge = config.Anchor.GetAnchoredPositionLocal(Utils.GetPointOnRectEdge(transform as RectTransform, localDir), transform);
            Vector3 posGlobal = transform.TransformPoint(pointOnRectEdge);

            Selectable[] selectables = GetAllSelectables(config);
            Selectable target = GetSelectable(selectables, s_SelectableBufferCount, dir, posGlobal, config);

            if (target == null && config.WrapAround && 
                config.Type != DirectedNavigationType.Value.Sector)
            {
                Vector3 posGlobalWrapAroundOffsetted = GetPosWrapAroundOffsetted(selectables, dir, pointOnRectEdge, config);
                target = GetSelectable(selectables, s_SelectableBufferCount, dir, posGlobalWrapAroundOffsetted, config);
            }

            return target;
        }

        /// <summary>
        /// Getting selectables which will be used in navigation process.
        /// Returns all active selectables in the scene or selectables predefined in 'Selectable List' section, 
        /// according configuration.
        /// </summary>
        /// <param name="config">Configuration for specified direction.</param>
        /// <returns>Selectables which will be used in navigation process.</returns>
        protected Selectable[] GetAllSelectables(Config config)
        {
            Selectable[] selectables;

            if (config.Type == DirectedNavigationType.Value.SelectableList)
            {
                selectables = config.SelectableList.SelectableList;
                s_SelectableBufferCount = selectables.Length;
            }
            else
            {
                if (s_SelectableBuffer.Length < Selectable.allSelectableCount)
                    s_SelectableBuffer = new Selectable[Selectable.allSelectableCount];

                s_SelectableBufferCount = Selectable.AllSelectablesNoAlloc(s_SelectableBuffer);
                selectables = s_SelectableBuffer;
            }

            return selectables;
        }


        /// <summary>
        /// Get selectable for navigation.
        /// </summary>
        /// <param name="selectables">Selecatables from which will be selected target to navigate.</param>
        /// <param name="selectablesCount">Selectables count. Need because 'selectables' used like a buffer with extra size.</param>
        /// <param name="dir">Direction in which will be executed selection.</param>
        /// <param name="lookFromPosGlobal">Position from which search of selectable executed.</param>
        /// <param name="config">Configuration for specified direction.</param>
        /// <returns>Selectable to which will be navigated transition.</returns>
        protected Selectable GetSelectable(Selectable[] selectables, int selectablesCount, Vector3 dir, Vector3 lookFromPosGlobal, Config config)
        {
            Selectable target = null;
            float targetSqrMagnitude = Mathf.Infinity;

            for (int i = 0; i < selectablesCount; ++i)
            {
                Selectable sel = selectables[i];

                if (sel == null || sel == m_Selectable || !sel.IsInteractable() || !sel.IsActive() || sel.navigation.mode == UnityEngine.UI.Navigation.Mode.None)
                    continue;

#if UNITY_EDITOR
                if (Camera.current != null && !UnityEditor.SceneManagement.StageUtility.IsGameObjectRenderedByCamera(sel.gameObject, Camera.current))
                    continue;
#endif

                if (config.Type == DirectedNavigationType.Value.SelectableList && config.SelectableList.UseListOrder)
                    return sel;

                RectTransform selRectT = sel.transform as RectTransform;
                Vector3 selPos;
                if (config.UseEdges)
                {
                    Vector3 dirSelToPos = (lookFromPosGlobal - sel.transform.position).normalized;// Gets a vector that points from 'sel'(Selectable) position to the 'pos'(Current GameObject)
                    selPos = Utils.GetPointOnRectEdge(selRectT, dirSelToPos);
                }
                else
                {
                    selPos = selRectT != null ? (Vector3)selRectT.rect.center : Vector3.zero;
                }

                Vector3 selPosGlobal = sel.transform.TransformPoint(selPos);
                Vector3 dirPosToSel = selPosGlobal - lookFromPosGlobal;

                if (!config.Omnidirectional && Vector3.Dot(dir, dirPosToSel) <= 0)
                    continue;

                if (config.Type == DirectedNavigationType.Value.Sector)
                {
                    float distance = Vector3.Distance(selPosGlobal, lookFromPosGlobal);

                    if (distance > config.Sector.Radius * transform.lossyScale.x)
                        continue;

                    float angle = Vector3.SignedAngle(dirPosToSel, dir, Vector3.back);

                    if (!(angle >= config.Sector.MinAngle && angle <= config.Sector.MaxAngle))
                        continue;
                }
                else if (config.Type == DirectedNavigationType.Value.Rectangle)
                {
                    Rect rect = Utils.QuadrilateralToRect(config.Rectangle.Verts);

                    if (!rect.Contains(transform.InverseTransformPoint(selPosGlobal)))
                        continue;
                }
                else if (config.Type == DirectedNavigationType.Value.RectTransform)
                {
                    if (config.RectTransform.RectTransform == null ||
                        !config.RectTransform.RectTransform.gameObject.activeInHierarchy ||
                        !config.RectTransform.RectTransform.rect.Contains(config.RectTransform.RectTransform.InverseTransformPoint(selPosGlobal)))
                        continue;
                }

                float directivityCoeff = 1f;

                if (config.Directivity != 0f)
                {
                    float angle = Vector3.Angle(dirPosToSel, dir);

                    if (config.Omnidirectional && Vector3.Dot(dir, dirPosToSel) < 0)
                        angle -= 180;

                    float coeffBase;
                    const float MinCoeffBase = 0.1f;

                    if (config.Directivity <= 0)
                    {
                        coeffBase = (Mathf.Abs(angle)) / 90f;
                        coeffBase = Mathf.Clamp(coeffBase, MinCoeffBase, 1f);
                        directivityCoeff = Mathf.Pow(coeffBase, -config.Directivity);
                    }
                    else
                    {
                        coeffBase = (90f - Mathf.Abs(angle)) / 90f;
                        coeffBase = Mathf.Clamp(coeffBase, MinCoeffBase, 1f);
                        directivityCoeff = Mathf.Pow(coeffBase, config.Directivity);
                    }
                }

                float targetSqrMagnitudeNew = dirPosToSel.sqrMagnitude / directivityCoeff;

                if (targetSqrMagnitudeNew < targetSqrMagnitude)
                {
                    targetSqrMagnitude = targetSqrMagnitudeNew;
                    target = sel;
                }
            }

            return target;
        }


        /// <summary>
        /// Getting position in 'Wrap Around' state from which will be search of selectable executed.
        /// </summary>
        /// <param name="selectables">Selecatables from which will be selected target to navigate.</param>
        /// <param name="dir">Direction in which will be executed selection.</param>
        /// <param name="lookFromPosLocal">Position from which search of selectable executed.</param>
        /// <param name="config">Configuration for specified direction.</param>
        /// <returns>Position in 'Wrap Around' state from which will be search of selectable executed.</returns>
        protected Vector3 GetPosWrapAroundOffsetted(Selectable[] selectables, Vector3 dir, Vector3 lookFromPosLocal, Config config)
        {
            Rect selecatablesArea = new Rect();

            for (int i = 0; i < s_SelectableBufferCount; ++i)
            {
                Selectable sel = selectables[i];

                if (sel == null || sel == m_Selectable || !sel.IsInteractable() || !sel.IsActive() || sel.navigation.mode == UnityEngine.UI.Navigation.Mode.None)
                    continue;

#if UNITY_EDITOR
                if (Camera.current != null && !UnityEditor.SceneManagement.StageUtility.IsGameObjectRenderedByCamera(sel.gameObject, Camera.current))
                    continue;
#endif

                RectTransform selRectT = sel.transform as RectTransform;

                Vector3 centerGlobal = Vector3.zero;
                Vector3 center = Vector3.zero;
                Vector3 min = Vector3.zero;
                Vector3 max = Vector3.zero;
                
                if (selRectT != null)
                {
                    centerGlobal = sel.transform.TransformPoint(selRectT.rect.center);
                    center = transform.InverseTransformPoint(centerGlobal);
                    min = transform.InverseTransformPoint(sel.transform.TransformPoint(selRectT.rect.min));
                    max = transform.InverseTransformPoint(sel.transform.TransformPoint(selRectT.rect.max));
                }

                if (config.Type == DirectedNavigationType.Value.Rectangle)
                {
                    Rect rect = Utils.QuadrilateralToRect(config.Rectangle.Verts);

                    if (!rect.Contains(transform.InverseTransformPoint(centerGlobal)))
                        continue;
                }
                else if (config.Type == DirectedNavigationType.Value.RectTransform)
                {
                    if (config.RectTransform.RectTransform == null ||
                        !config.RectTransform.RectTransform.gameObject.activeInHierarchy ||
                        !config.RectTransform.RectTransform.rect.Contains(config.RectTransform.RectTransform.InverseTransformPoint(centerGlobal)))
                        continue;
                }

                if (selecatablesArea.center == Vector2.zero && selecatablesArea.width == 0 && selecatablesArea.height == 0)
                {
                    selecatablesArea.xMin = min.x;
                    selecatablesArea.yMin = min.y;

                    selecatablesArea.xMax = max.x;
                    selecatablesArea.yMax = max.y;
                }

                selecatablesArea.xMin = min.x < selecatablesArea.xMin ? min.x : selecatablesArea.xMin;
                selecatablesArea.xMax = max.x > selecatablesArea.xMax ? max.x : selecatablesArea.xMax;
                selecatablesArea.yMin = min.y < selecatablesArea.yMin ? min.y : selecatablesArea.yMin;
                selecatablesArea.yMax = max.y > selecatablesArea.yMax ? max.y : selecatablesArea.yMax;
            }

            Vector3 selecatablesAreaCenter = new Vector3(selecatablesArea.center.x, selecatablesArea.center.y);
            Vector3 direction = selecatablesAreaCenter - lookFromPosLocal;
            Vector3 directionNormalized = direction.normalized;

            return transform.TransformPoint(selecatablesAreaCenter + Vector3.Reflect(direction, Vector3.Cross(Vector3.Scale(transform.localToWorldMatrix.inverse * dir, transform.lossyScale), Vector3.forward)));
        }


    }
}

