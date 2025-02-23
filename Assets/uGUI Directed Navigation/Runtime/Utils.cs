/* ---------------------------------------
 * Project: 'uGUI Directed Navigation'
 * Studio:          IEVO
 * Site:     https://ievo.games/
 * -------------------------------------*/
using System;
using UnityEngine;

namespace IEVO.UI.uGUIDirectedNavigation
{
    static public class Utils
    {
        /// <summary>
        /// Converts quadrilateral vertices array to 'Rect'. If 'vertices' == null or 'vertices.Length' != 4, default 'Rect' will be returned.
        /// </summary>
        /// <param name="vertices">Vertices array with length - 4.</param>
        /// <returns></returns>
        static public Rect QuadrilateralToRect(Vector3[] vertices)
        {
            if (vertices == null || vertices.Length != 4)
                return new Rect();

            return new Rect(vertices[0].x, vertices[0].y, Mathf.Abs(vertices[3].x - vertices[0].x), Mathf.Abs(vertices[2].y - vertices[0].y));
        }


        /// <summary>
        /// Converts quadrilateral to Rectangle
        /// </summary>
        /// <param name="vertices">Vertices array with length - 4.</param>
        static public void RectangulateQuadrilateral(Vector3[] vertices)
        {
            if (vertices != null && vertices.Length == 4)
            {
                float xMin = (vertices[0].x + vertices[1].x) / 2;
                float xMax = (vertices[2].x + vertices[3].x) / 2;
                float yMin = (vertices[0].y + vertices[3].y) / 2;
                float yMax = (vertices[1].y + vertices[2].y) / 2;

                vertices[0].x = xMin;
                vertices[0].y = yMin;

                vertices[1].x = xMin;
                vertices[1].y = yMax;

                vertices[2].x = xMax;
                vertices[2].y = yMax;

                vertices[3].x = xMax;
                vertices[3].y = yMin;
            }
        }


        /// <summary>
        /// Returns position on rectangle edge according direction.
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="dir"></param>
        /// <returns></returns>
        static public Vector3 GetPointOnRectEdge(RectTransform rect, Vector2 dir)
        {
            if (rect == null)
                return Vector3.zero;

            if (dir != Vector2.zero)
                dir /= Mathf.Max(Mathf.Abs(dir.x), Mathf.Abs(dir.y));

            dir = rect.rect.center + Vector2.Scale(rect.rect.size, dir * 0.5f);

            return dir;
        }


        /// <summary>
        /// Return semi-centroid of verts.
        /// </summary>
        /// <param name="polygonVerts"></param>
        /// <returns>Semi-centroid of verts. If "polygonVerts" is "null" or "length == 0", "Vector.zero" will be returned.</returns>
        static public Vector3 GetCentroid(Vector3[] polygonVerts)
        {
            if (polygonVerts != null && polygonVerts.Length > 0)
            {
                float totalX = 0f;
                float totalY = 0f;
                float totalZ = 0f;

                for (int i = 0; i < polygonVerts.Length; i++)
                {
                    totalX += polygonVerts[i].x;
                    totalY += polygonVerts[i].y;
                    totalZ += polygonVerts[i].z;
                }

                return new Vector3(totalX / polygonVerts.Length, totalY / polygonVerts.Length, totalZ / polygonVerts.Length);
            }
            else
            {
                return Vector3.zero;
            }
        }


        /// <summary>
        /// Normalizes 'value' in exponential scale. Used in non-linear sliders.
        /// </summary>
        /// <param name="value">Must be in range from '0' to 'maxValue'.</param>
        /// <param name="midValue">Use this value control exponentiality.</param>
        /// <param name="maxValue">Maximum value of range according which will be normalized 'value'.</param>
        /// <returns>Normalized value in range '0-1'.</returns>
        static public float ExpNormalize(float value, float midValue, float maxValue)
        {
            value = Mathf.Clamp(value, 0f, 1f);
            midValue = Mathf.Clamp(midValue, 0f, maxValue);

            float M = maxValue / midValue;
            float C = Mathf.Log(Mathf.Pow(M - 1, 2));
            float B = maxValue / (Mathf.Exp(C) - 1);
            float A = -1 * B;

            return A + B * Mathf.Exp(C * value);
        }


        /// <summary>
        /// Denormalizes 'value' in exponential scale. Used in non-linear sliders.
        /// </summary>
        /// <param name="value">Normalized value in range '0-1'.</param>
        /// <param name="midValue">Use this value control exponentiality.</param>
        /// <param name="maxValue">Maximum value of range according which will be normalized 'value'.</param>
        /// <returns>Denormalized value in range '0-maxValue'.</returns>
        static public float ExpNormalizeInverse(float value, float midValue, float maxValue)
        {
            value = Mathf.Clamp(value, 0f, maxValue);
            midValue = Mathf.Clamp(midValue, 0f, maxValue);

            float M = maxValue / midValue;
            float C = Mathf.Log(Mathf.Pow(M - 1, 2));
            float B = maxValue / (Mathf.Exp(C) - 1);
            float A = -1 * B;

            return Mathf.Log((value - A) / B) / C;
        }


        static public T[,] ResizeArray<T>(T[,] original, int rows, int cols)
        {
            var newArray = new T[rows, cols];
            int minRows = Math.Min(rows, original.GetLength(0));
            int minCols = Math.Min(cols, original.GetLength(1));
            for (int i = 0; i < minRows; i++)
                for (int j = 0; j < minCols; j++)
                    newArray[i, j] = original[i, j];
            return newArray;
        }
    }
}
