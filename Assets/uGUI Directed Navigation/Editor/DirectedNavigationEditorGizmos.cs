/* ---------------------------------------
 * Project: 'uGUI Directed Navigation'
 * Studio:          IEVO
 * Site:     https://ievo.games/
 * -------------------------------------*/
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace IEVO.UI.uGUIDirectedNavigation
{
    public partial class DirectedNavigationEditor : Editor
    {
        // Buffer. Used for NonAlloc getting of Selectables.
        static protected Selectable[] s_SelectableBuffer = new Selectable[30];
        static protected int s_SelectableBufferCount;

        // Buffer. For NonAlloc. Verts of Rect in global coordinate space.
        static protected readonly Vector3[] s_RectangleVertsGlobal = new Vector3[4];

        static private Color s_GizmosColorPrimary = new Color(1, 0.8f, 0.4f);
        static private Color s_GizmosColorSecondary = Style.Gizmos.GetGizmosSecondaryColor(s_GizmosColorPrimary);


        private void RegisterOnDuringSceneGuiGizmos()
        {
            SceneView.duringSceneGui -= OnDuringSceneGuiGizmos;

            if (s_Editors.Count > 0)
                SceneView.duringSceneGui += OnDuringSceneGuiGizmos;
        }

        static protected void OnDuringSceneGuiGizmos(SceneView view)
        {
            if (s_ShowNavigation && s_ShowGizmos)
                DrawGizmos();
        }


        static protected void DrawGizmos()
        {
            DirectedNavigation[] activeDirNavs = Selection.GetFiltered<DirectedNavigation>(SelectionMode.Editable);

            DirectedNavigation dirNav;
            RectTransform dirNavRt;
            
            for (int i = 0; i < activeDirNavs.Length; i++)
            {
                dirNav = activeDirNavs[i];

                if (!(dirNav.transform is RectTransform))
                    continue;

                dirNavRt = dirNav.transform as RectTransform;

                if (dirNav != null && dirNav.isActiveAndEnabled && dirNav.Active)
                {
                    // Anchor
                    //----------------
                    DrawSideGizmosAnchor(dirNav, dirNavRt, Vector3.left, -dirNavRt.right, dirNav.ConfigLeft);//Left
                    DrawSideGizmosAnchor(dirNav, dirNavRt, Vector3.right, dirNavRt.right, dirNav.ConfigRight);//Right

                    DrawSideGizmosAnchor(dirNav, dirNavRt, Vector3.up, dirNavRt.up, dirNav.ConfigUp);//Up
                    DrawSideGizmosAnchor(dirNav, dirNavRt, Vector3.down, -dirNavRt.up, dirNav.ConfigDown);//Down
                    //----------------
                    // Anchor

                    // Directivity
                    //----------------
                    DrawSideGizmosDirectivity(dirNav, dirNavRt, Vector3.left, -dirNavRt.right, dirNav.ConfigLeft);//Left
                    DrawSideGizmosDirectivity(dirNav, dirNavRt, Vector3.right, dirNavRt.right, dirNav.ConfigRight);//Right

                    DrawSideGizmosDirectivity(dirNav, dirNavRt, Vector3.up, dirNavRt.up, dirNav.ConfigUp);//Up
                    DrawSideGizmosDirectivity(dirNav, dirNavRt, Vector3.down, -dirNavRt.up, dirNav.ConfigDown);//Down
                    //----------------
                    // Directivity

                    // Sector
                    //----------------
                    DrawSideGizmosSector(dirNav, dirNavRt, Vector3.left, -dirNavRt.right, dirNav.ConfigLeft);//Left
                    DrawSideGizmosSector(dirNav, dirNavRt, Vector3.right, dirNavRt.right, dirNav.ConfigRight);//Right

                    DrawSideGizmosSector(dirNav, dirNavRt, Vector3.up, dirNavRt.up, dirNav.ConfigUp);//Up
                    DrawSideGizmosSector(dirNav, dirNavRt, Vector3.down, -dirNavRt.up, dirNav.ConfigDown);//Down
                    //----------------
                    // Sector

                    // Rectangle
                    //----------------
                    DrawSideGizmosRectangle(dirNav, dirNavRt, Vector3.left, -dirNavRt.right, dirNav.ConfigLeft);//Left
                    DrawSideGizmosRectangle(dirNav, dirNavRt, Vector3.right, dirNavRt.right, dirNav.ConfigRight);//Right

                    DrawSideGizmosRectangle(dirNav, dirNavRt, Vector3.up, dirNavRt.up, dirNav.ConfigUp);//Up
                    DrawSideGizmosRectangle(dirNav, dirNavRt, Vector3.down, -dirNavRt.up, dirNav.ConfigDown);//Down
                    //----------------
                    // Rectangle

                    // RectTransform
                    //----------------
                    DrawSideGizmosRectTransform(dirNav, dirNavRt, Vector3.left, -dirNavRt.right, dirNav.ConfigLeft);//Left
                    DrawSideGizmosRectTransform(dirNav, dirNavRt, Vector3.right, dirNavRt.right, dirNav.ConfigRight);//Right

                    DrawSideGizmosRectTransform(dirNav, dirNavRt, Vector3.up, dirNavRt.up, dirNav.ConfigUp);//Up
                    DrawSideGizmosRectTransform(dirNav, dirNavRt, Vector3.down, -dirNavRt.up, dirNav.ConfigDown);//Down
                    //----------------
                    // RectTransform

                    // SelectableList
                    //----------------
                    DrawSideGizmosSelectableList(dirNav, dirNavRt, Vector3.left, -dirNavRt.right, dirNav.ConfigLeft);//Left
                    DrawSideGizmosSelectableList(dirNav, dirNavRt, Vector3.right, dirNavRt.right, dirNav.ConfigRight);//Right

                    DrawSideGizmosSelectableList(dirNav, dirNavRt, Vector3.up, dirNavRt.up, dirNav.ConfigUp);//Up
                    DrawSideGizmosSelectableList(dirNav, dirNavRt, Vector3.down, -dirNavRt.up, dirNav.ConfigDown);//Down
                    //----------------
                    // SelectableList
                }
            }
        }

        static protected void DrawSideGizmosAnchor(DirectedNavigation dirNav, RectTransform dirNavRectT, Vector3 baseDirection, Vector3 selRtDirection, Config config)
        {
            if (config.Type == DirectedNavigationType.Value.Disabled || config.Type == DirectedNavigationType.Value.SelectableList && config.SelectableList.UseListOrder ||
                config.Anchor.Type == Anchor.Type.Identity)
                return;

            Vector3 posLocal = Utils.GetPointOnRectEdge(dirNavRectT, baseDirection);
            Vector3 posLocalAnchored = config.Anchor.GetAnchoredPositionLocal(posLocal, dirNavRectT);

            Vector3 posGlobal = dirNav.transform.TransformPoint(posLocal);
            Vector3 posGlobalAnchored = dirNav.transform.TransformPoint(posLocalAnchored);

            // Handle
            //----------------
            Handles.color = s_GizmosColorPrimary;

            EditorGUI.BeginChangeCheck();

            Vector3 newPosGlobalAnchored;

            if (config.Anchor.Type == Anchor.Type.Shift)
            {
                newPosGlobalAnchored = Handles.Slider2D(posGlobalAnchored, dirNavRectT.forward, dirNavRectT.right, dirNavRectT.up, HandleUtility.GetHandleSize(posGlobalAnchored) * 0.1f, Handles.CircleHandleCap, 0.1f);
            }
            else
            {
                newPosGlobalAnchored = Handles.Slider2D(posGlobalAnchored, dirNavRectT.forward, dirNavRectT.right, dirNavRectT.up, HandleUtility.GetHandleSize(posGlobalAnchored) * 0.1f, Handles.RectangleHandleCap, 0.1f);
            }

            if (EditorGUI.EndChangeCheck() && config.Anchor.Type == Anchor.Type.Shift)
            {
                Undo.RecordObject(dirNav, Texts.Undo);

                config.Anchor.SetShift(dirNav.transform.InverseTransformPoint(newPosGlobalAnchored) - posLocal);
            }
            //----------------
            // Handle

            if(config.Anchor.Type == Anchor.Type.RectTransform &&
                config.Anchor.RectTransform != null && !config.Anchor.RectTransform.gameObject.activeInHierarchy)
            {
                Handles.Slider2D(config.Anchor.RectTransform.position, dirNavRectT.forward, dirNavRectT.right, dirNavRectT.up, HandleUtility.GetHandleSize(posGlobalAnchored) * 0.1f, Handles.RectangleHandleCap, 0.1f);
                Handles.DrawDottedLine(posGlobal, config.Anchor.RectTransform.position, Style.Gizmos.AnchorDottedLineSize);
            }

            Handles.DrawSolidDisc(posGlobal, dirNavRectT.forward, 0.035f * HandleUtility.GetHandleSize(posGlobal));
            Handles.DrawAAPolyLine(4f, posGlobal, posGlobalAnchored);
        }


        static protected void DrawSideGizmosDirectivity(DirectedNavigation dirNav, RectTransform dirNavRectT, Vector3 baseDirection, Vector3 selRtDirection, Config config)
        {
            if (config.Type == DirectedNavigationType.Value.Disabled ||
                config.Type == DirectedNavigationType.Value.SelectableList && config.SelectableList.UseListOrder)
                return;

            Vector3 pointOnRectEdge = Utils.GetPointOnRectEdge(dirNavRectT, baseDirection);
            Vector3 posLocal = config.Anchor.GetAnchoredPositionLocal(pointOnRectEdge, dirNavRectT);
            Vector3 posGlobal = dirNav.transform.TransformPoint(posLocal);

            // Arcs
            //----------------
            Handles.color = s_GizmosColorSecondary;
            float arcAngle = config.Omnidirectional ? 360f : 180f;
            Handles.DrawSolidArc(posGlobal, dirNavRectT.forward, Quaternion.AngleAxis(-90, dirNavRectT.forward) * selRtDirection, arcAngle, 0.35f * HandleUtility.GetHandleSize(posGlobal));
            //----------------
            // Arcs

            // Lines
            //----------------
            Handles.color = s_GizmosColorPrimary;
            for (int i = -90; i <= 90; i += 10)
            {
                DrawLine(posGlobal, i, dirNavRectT.forward, selRtDirection, 0.35f, false, config.Directivity);

                if(config.Omnidirectional)
                    DrawLine(posGlobal, i, dirNavRectT.forward, selRtDirection * -1, 0.35f, false, config.Directivity);
            }
            //----------------
            // Lines
        }


        static protected void DrawLine(Vector3 pos, float angle, Vector3 forward, Vector3 direction, float baseLenght, bool endArrow, float directivity)
        {
            const float lineWidth = 2.5f;
            const float sharpnessAngle = 7;
            const float arrowSideLengthCoeff = 0.8f;
            float length = baseLenght * HandleUtility.GetHandleSize(pos);

            float directivityCoeff;
            float coeffBase;
            const float MinCoeffBase = 0.1f;

            if (directivity <= 0)
            {
                coeffBase = (Mathf.Abs(angle)) / 90f;
                coeffBase = Mathf.Clamp(coeffBase, MinCoeffBase, 1f);
                directivityCoeff = Mathf.Pow(coeffBase, -directivity);
            }
            else
            {
                coeffBase = (90f - Mathf.Abs(angle)) / 90f;
                coeffBase = Mathf.Clamp(coeffBase, MinCoeffBase, 1f);
                directivityCoeff = Mathf.Pow(coeffBase, directivity);
            }

            var q = Quaternion.AngleAxis(angle, forward);
            Vector3 centralPoint = pos + q * direction * length * directivityCoeff;

            Handles.DrawAAPolyLine(lineWidth, pos, centralPoint);

            if (endArrow)
            {
                q = Quaternion.AngleAxis(-sharpnessAngle + angle, forward);
                Vector3 leftPoint = pos + q * direction * length * arrowSideLengthCoeff * directivityCoeff;

                q = Quaternion.AngleAxis(sharpnessAngle + angle, forward);
                Vector3 rightPoint = pos + q * direction * length * arrowSideLengthCoeff * directivityCoeff;

                Handles.DrawAAPolyLine(lineWidth, centralPoint, leftPoint);
                Handles.DrawAAPolyLine(lineWidth, centralPoint, rightPoint);
            }
        }


        static protected void DrawSideGizmosSector(DirectedNavigation dirNav, RectTransform dirNavRectT, Vector3 baseDirection, Vector3 selRtDirection, Config config)
        {
            if (config.Type != DirectedNavigationType.Value.Sector)
                return;

            Vector3 pointOnRectEdge = Utils.GetPointOnRectEdge(dirNavRectT, baseDirection);
            Vector3 posLocal = config.Anchor.GetAnchoredPositionLocal(pointOnRectEdge, dirNavRectT);
            Vector3 posGlobal = dirNav.transform.TransformPoint(posLocal);
            float distance = config.Sector.Radius * dirNav.transform.lossyScale.x;

            // Arcs
            //----------------
            Handles.color = s_GizmosColorSecondary;
            Handles.DrawSolidArc(posGlobal, dirNavRectT.forward, Quaternion.AngleAxis(config.Sector.MinAngle, dirNavRectT.forward) * selRtDirection, config.Sector.MaxAngle - config.Sector.MinAngle, distance);

            Handles.color = s_GizmosColorPrimary;
            Handles.DrawWireArc(posGlobal, dirNavRectT.forward, Quaternion.AngleAxis(config.Sector.MinAngle, dirNavRectT.forward) * selRtDirection, config.Sector.MaxAngle - config.Sector.MinAngle, distance);
            //----------------
            // Arcs


            // Handles
            //----------------
            Handles.color = s_GizmosColorPrimary;

            // Min
            var q = Quaternion.AngleAxis(config.Sector.MinAngle, dirNavRectT.forward);
            Vector3 handlePos = posGlobal + q * selRtDirection * distance;

            EditorGUI.BeginChangeCheck();

            Vector3 newHandlePos = Handles.Slider2D(handlePos, dirNavRectT.forward, dirNavRectT.right, dirNavRectT.up, HandleUtility.GetHandleSize(handlePos) * 0.06f, Handles.DotHandleCap, 0.1f);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(dirNav, Texts.Undo);

                Vector3 targetDir = newHandlePos - posGlobal;
                Vector3 forwardVector = selRtDirection;
                float angle = Vector3.SignedAngle(forwardVector, targetDir, new Vector3(0, 0, 1));

                config.Sector.SetMinAngle(angle, config.Omnidirectional);
            }
            // Min

            // Max
            q = Quaternion.AngleAxis(config.Sector.MaxAngle, dirNavRectT.forward);
            handlePos = posGlobal + q * selRtDirection * distance;

            EditorGUI.BeginChangeCheck();

            newHandlePos = Handles.Slider2D(handlePos, dirNavRectT.forward, dirNavRectT.right, dirNavRectT.up, HandleUtility.GetHandleSize(handlePos) * 0.06f, Handles.DotHandleCap, 0.1f);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(dirNav, Texts.Undo);

                Vector3 targetDir = newHandlePos - posGlobal;
                Vector3 forwardVector = selRtDirection;
                float angle = Vector3.SignedAngle(forwardVector, targetDir, new Vector3(0, 0, 1));

                config.Sector.SetMaxAngle(angle, config.Omnidirectional);
            }
            // Max

            // Central
            Handles.color = s_GizmosColorPrimary;

            float angleBetweenMinMax = config.Sector.MaxAngle - config.Sector.MinAngle;
            float halfAngleBetweenMinMax = angleBetweenMinMax / 2;
            float middleAngle = config.Sector.MaxAngle - halfAngleBetweenMinMax;

            q = Quaternion.AngleAxis(middleAngle, dirNavRectT.forward);
            handlePos = posGlobal + q * selRtDirection * (distance + HandleUtility.GetHandleSize(handlePos) * 0.15f);

            EditorGUI.BeginChangeCheck();

            newHandlePos = Handles.Slider2D(handlePos, dirNavRectT.forward, dirNavRectT.right, dirNavRectT.up, HandleUtility.GetHandleSize(handlePos) * 0.07f, Handles.DotHandleCap, 0.1f);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(dirNav, Texts.Undo);

                Vector3 targetDir = newHandlePos - posGlobal;
                Vector3 forwardVector = selRtDirection;
                float angle = Vector3.SignedAngle(forwardVector, targetDir, new Vector3(0, 0, 1));

                if(config.Omnidirectional)
                    angle = Mathf.Clamp(angle, Config.SectorConfig.MinAngleLimitOmnidirectional + halfAngleBetweenMinMax, Config.SectorConfig.MaxAngleLimitOmnidirectional - halfAngleBetweenMinMax);
                else
                    angle = Mathf.Clamp(angle, Config.SectorConfig.MinAngleLimit + halfAngleBetweenMinMax, Config.SectorConfig.MaxAngleLimit - halfAngleBetweenMinMax);

                float newMinAngle = angle - halfAngleBetweenMinMax;
                float newMaxAngle = angle + halfAngleBetweenMinMax;

                config.Sector.SetMinAngle(newMinAngle, config.Omnidirectional);
                config.Sector.SetMaxAngle(newMaxAngle, config.Omnidirectional);
                config.Sector.SetMinAngle(newMinAngle, config.Omnidirectional);//Temporary workaround. Required to prevent the sector size from changing when moving the mouse clockwise in 'Omnidirectional' mode.

                float handleShift = HandleUtility.GetHandleSize(handlePos) * 0.15f;
                config.Sector.Radius = Mathf.Clamp((Vector3.Distance(posGlobal, newHandlePos) - handleShift)/dirNav.transform.lossyScale.x, 0, s_RadiusLimit);
            }
            // Central

            //----------------
            // Handles

            // Label
            //----------------
            DrawSideLabel(posGlobal + q * selRtDirection * (distance / 2 + HandleUtility.GetHandleSize(handlePos) * 0.15f), baseDirection);
            //----------------
            // Label
        }


        static protected void DrawSideGizmosRectangle(DirectedNavigation dirNav, RectTransform dirNavRectT, Vector3 baseDirection, Vector3 selRtDirection, Config config)
        {
            if (config.Type != DirectedNavigationType.Value.Rectangle)
                return;

            if (s_RectangleVertsGlobal.Length != config.Rectangle.Verts.Length)
                return;

            Vector3 posLocal = config.Anchor.GetAnchoredPositionLocal(Utils.GetPointOnRectEdge(dirNavRectT, baseDirection), dirNavRectT);
            Vector3 posGlobal = dirNav.transform.TransformPoint(posLocal);

            if ( config.Rectangle.Verts[0] == Vector3.zero && 
                config.Rectangle.Verts[1] == Vector3.zero &&
                config.Rectangle.Verts[2] == Vector3.zero &&
                config.Rectangle.Verts[3] == Vector3.zero)
            {
                ResetRectangleVerts(config.Rectangle.Verts, posLocal, dirNavRectT);
            }

            Utils.RectangulateQuadrilateral(config.Rectangle.Verts);

            for (int i = 0; i < s_RectangleVertsGlobal.Length; i++)
            {
                s_RectangleVertsGlobal[i] = dirNav.transform.TransformPoint(config.Rectangle.Verts[i]);
            }

            Vector3 centroid = Utils.GetCentroid(s_RectangleVertsGlobal);

            // Draw Rectangle
            Handles.DrawSolidRectangleWithOutline(s_RectangleVertsGlobal, s_GizmosColorSecondary, s_GizmosColorPrimary);
            // Draw Rectangle

            Handles.color = s_GizmosColorPrimary;

            // Handles
            //----------------

            // Corners
            for (int i = 0; i < s_RectangleVertsGlobal.Length; i++)
            {
                EditorGUI.BeginChangeCheck();

                Vector3 newVertPos = Handles.Slider2D(s_RectangleVertsGlobal[i], dirNavRectT.forward, dirNavRectT.right, dirNavRectT.up, HandleUtility.GetHandleSize(posGlobal) * 0.06f, Handles.DotHandleCap, 0.1f);

                if (EditorGUI.EndChangeCheck())
                    config.Rectangle.Verts[i] = dirNav.transform.InverseTransformPoint(newVertPos);
            }
            // Corners

            // Center
            Vector3 newCentroid = Handles.Slider2D(centroid, dirNavRectT.forward, dirNavRectT.right, dirNavRectT.up, HandleUtility.GetHandleSize(posGlobal) * 0.06f, Handles.DotHandleCap, 0.1f);
            Vector3 centroidDiff = dirNav.transform.InverseTransformPoint(centroid) - dirNav.transform.InverseTransformPoint(newCentroid);

            for (int i = 0; i < s_RectangleVertsGlobal.Length; i++)
            {
                config.Rectangle.Verts[i] -= centroidDiff;
            }
            // Center

            //----------------
            // Handles

            // Line
            Handles.DrawAAPolyLine(2f, posGlobal, centroid);
            // Line

            // Label
            DrawSideLabel(centroid, baseDirection, 0.7f);
            // Label
        }


        static protected void DrawSideGizmosRectTransform(DirectedNavigation dirNav, RectTransform dirNavRectT, Vector3 baseDirection, Vector3 selRtDirection, Config config)
        {
            if (config.Type != DirectedNavigationType.Value.RectTransform)
                return;

            if (config.RectTransform.RectTransform == null)
                return;

            Vector3 posLocal = config.Anchor.GetAnchoredPositionLocal(Utils.GetPointOnRectEdge(dirNavRectT, baseDirection), dirNavRectT);
            Vector3 posGlobal = dirNav.transform.TransformPoint(posLocal);

            config.RectTransform.RectTransform.GetWorldCorners(s_RectangleVertsGlobal);

            Vector3 centroid = Utils.GetCentroid(s_RectangleVertsGlobal);

            // Draw Rectangle
            Handles.DrawSolidRectangleWithOutline(s_RectangleVertsGlobal, s_GizmosColorSecondary, s_GizmosColorPrimary);
            // Draw Rectangle

            // Line
            if (config.RectTransform.RectTransform.gameObject.activeInHierarchy)
            {
                Handles.DrawAAPolyLine(2f, posGlobal, centroid);
            }
            else
            {
                Handles.DrawDottedLine(posGlobal, centroid, Style.Gizmos.DottedLineSize);
                Handles.DrawDottedLine(s_RectangleVertsGlobal[0], s_RectangleVertsGlobal[2], Style.Gizmos.DottedLineSize);
                Handles.DrawDottedLine(s_RectangleVertsGlobal[1], s_RectangleVertsGlobal[3], Style.Gizmos.DottedLineSize);
            }
            // Line

            // Label
            DrawSideLabel(centroid, baseDirection, 0.7f);
            // Label
        }


        static protected void DrawSideGizmosSelectableList(DirectedNavigation dirNav, RectTransform dirNavRectT, Vector3 baseDirection, Vector3 selRtDirection, Config config)
        {
            if (config.Type != DirectedNavigationType.Value.SelectableList)
                return;

            if (config.SelectableList.SelectableList == null || config.SelectableList.SelectableList.Length == 0)
                return;

            Handles.color = s_GizmosColorPrimary;

            Vector3 pointOnRectEdge = Utils.GetPointOnRectEdge(dirNavRectT, baseDirection);

            Vector3 posLocal;
            if (config.SelectableList.UseListOrder)
                posLocal = pointOnRectEdge;
            else
                posLocal = config.Anchor.GetAnchoredPositionLocal(pointOnRectEdge, dirNavRectT);

            Vector3 posGlobal = dirNav.transform.TransformPoint(posLocal);

            for(int i = 0; i < config.SelectableList.SelectableList.Length; i++)
            {
                Selectable sel = config.SelectableList.SelectableList[i];

                if (sel != null)
                {
                    (sel.transform as RectTransform).GetWorldCorners(s_RectangleVertsGlobal);

                    Vector3 centroid = Utils.GetCentroid(s_RectangleVertsGlobal);

                    // Draw Rectangle
                    Handles.DrawSolidRectangleWithOutline(s_RectangleVertsGlobal, s_GizmosColorSecondary, s_GizmosColorPrimary);
                    // Draw Rectangle

                    // Line
                    if (sel.IsActive() && sel.IsInteractable() && sel.navigation.mode != UnityEngine.UI.Navigation.Mode.None)
                    {
                        Handles.DrawAAPolyLine(2f, posGlobal, centroid);
                    }
                    else
                    {
                        Handles.DrawDottedLine(posGlobal, centroid, Style.Gizmos.DottedLineSize);
                        Handles.DrawDottedLine(s_RectangleVertsGlobal[0], s_RectangleVertsGlobal[2], Style.Gizmos.DottedLineSize);
                        Handles.DrawDottedLine(s_RectangleVertsGlobal[1], s_RectangleVertsGlobal[3], Style.Gizmos.DottedLineSize);
                    }
                    // Line

                    // Label
                    DrawSideLabel(centroid, baseDirection, 0.7f);
                    // Label

                    // Label Item Order
                    if (config.SelectableList.UseListOrder)
                        DrawOrderLabel(i, centroid, -1.7f);
                    // Label Item Order
                }
            }
        }


        static protected void DrawSideLabel(Vector3 position, Vector3 direction, float upShifCoeff = 0f)
        {
            string text = Texts.Label_Undefined;

            if (direction == Vector3.left)
                text = Texts.Label_Left;
            else if (direction == Vector3.right)
                text = Texts.Label_Right;
            else if (direction == Vector3.up)
                text = Texts.Label_Up;
            else if (direction == Vector3.down)
                text = Texts.Label_Down;

            GUIContent guiContent = new GUIContent(text);
            GUIStyle style = Style.Gizmos.LabelStyle;
            style.normal.textColor = s_GizmosColorPrimary;

            Vector2 guiContentSize = style.CalcSize(guiContent);
            Vector3 labelPosition = position + new Vector3(-guiContentSize.x / 1.7f, guiContentSize.y + guiContentSize.y * upShifCoeff) * HandleUtility.GetHandleSize(position) * 0.01f;
            Handles.Label(labelPosition, guiContent, style);
        }


        static protected void DrawOrderLabel(int order, Vector3 position, float upShifCoeff = 0f)
        {
            string text = $"({order})";

            GUIContent guiContent = new GUIContent(text);
            GUIStyle style = Style.Gizmos.LabelStyle;
            style.normal.textColor = s_GizmosColorPrimary;

            Vector2 guiContentSize = style.CalcSize(guiContent);
            Vector3 labelPosition = position + new Vector3(-guiContentSize.x / 1.7f, guiContentSize.y + guiContentSize.y * upShifCoeff) * HandleUtility.GetHandleSize(position) * 0.01f;
            Handles.Label(labelPosition, guiContent, style);
        }


        static protected void ResetRectangleVerts(Vector3[] verts, Vector3 pos, RectTransform buttonRect)
        {
            float maxSide = Mathf.Max(buttonRect.rect.width, buttonRect.rect.height);
            float halfSide = maxSide / 2;
            const float coeffDistanceFromSide = 3f;

            float xMin = -halfSide;
            float xMax = halfSide;
            float yMin = -halfSide;
            float yMax = halfSide;

            verts[0].x = xMin;
            verts[0].y = yMin;
            verts[0] += pos * coeffDistanceFromSide;

            verts[1].x = xMin;
            verts[1].y = yMax;
            verts[1] += pos * coeffDistanceFromSide;

            verts[2].x = xMax;
            verts[2].y = yMax;
            verts[2] += pos * coeffDistanceFromSide;

            verts[3].x = xMax;
            verts[3].y = yMin;
            verts[3] += pos * coeffDistanceFromSide;
        }


    }
}
