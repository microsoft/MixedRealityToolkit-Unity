using HoloToolkit.Unity;
using UnityEngine;
using MRTK.UX;
#if UNITY_EDITOR
using UnityEditor;

public class LineBaseEditor : MRTKEditor
{
    public const int DefaultDisplayLineSteps = 16;
    public static readonly Color DefaultDisplayLineColor = Color.white;

    protected override void DrawCustomSceneGUI()
    {
        LineBase line = (LineBase)target;
        
        DrawDottedLine(line);
        DrawManualUpVectorHandles(line);
    }

    protected void DrawDottedLine(LineBase line, int numSteps = DefaultDisplayLineSteps)
    {
        Vector3 firstPos = line.GetPoint(0f);
        Vector3 lastPos = firstPos;
        Handles.color = DefaultDisplayLineColor;

        for (int i = 1; i < numSteps; i++)
        {
            float normalizedLength = (1f / numSteps) * i;
            Vector3 currentPos = line.GetPoint(normalizedLength);
            Handles.DrawDottedLine(lastPos, currentPos, MRTKEditor.DottedLineScreenSpace);
            lastPos = currentPos;
        }

        if (line.Loops)
        {
            Handles.DrawDottedLine(lastPos, firstPos, MRTKEditor.DottedLineScreenSpace);
        }
    }

    protected void DrawManualUpVectorHandles(LineBase line)
    {
        /*if (line.ManualUpVectors == null || line.ManualUpVectors.Length < 2)
            line.ManualUpVectors = new Vector3[2];

        for (int i = 0; i < line.ManualUpVectors.Length; i++)
        {
            float normalizedLength = (1f / (line.ManualUpVectors.Length - 1)) * i;
            Vector3 currentPoint = line.GetPoint(normalizedLength);
            Vector3 currentUpVector = line.ManualUpVectors[i];
            float maxHandleLength = (HandleUtility.GetHandleSize(currentPoint) * rotationHandleLength);
            Vector3 upVectorPoint = currentPoint + (currentUpVector * (maxHandleLength * currentUpVector.magnitude));

            Handles.color = Color.Lerp(Color.black, Color.cyan, currentUpVector.magnitude);

            Handles.DrawDottedLine(currentPoint, upVectorPoint, rotationHandleSize);
            Handles.Label(upVectorPoint, currentUpVector.magnitude.ToString("0.00"));
            Vector3 newUpVectorPoint = Handles.FreeMoveHandle(
                upVectorPoint,
                Quaternion.identity,
                HandleUtility.GetHandleSize(currentPoint) * rotationHandleSize,
                Vector3.zero,
                Handles.RectangleHandleCap);
            if (newUpVectorPoint != upVectorPoint)
            {
                if (!recordingUndo)
                {
                    recordingUndo = true;
                    Undo.RegisterCompleteObjectUndo(line, "Edit Manual Up Vector");
                }
                Vector3 newUpVector = (newUpVectorPoint - currentPoint) / maxHandleLength;
                if (newUpVector.magnitude > 1)
                    newUpVector.Normalize();

                line.ManualUpVectors[i] = newUpVector;
            }
        }*/
    }
}
#endif