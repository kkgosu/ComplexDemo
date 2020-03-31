using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.UI;

public static class TransformExtensions
{

    /// <summary>
    /// Получение размеров объекта Transform, обладающего компонентами Renderer.
    /// </summary>
    /// <returns>Размер объекта в виде Vector3.</returns>
    /// <param name="t">Компонент Transform объекта, размер которого нужно определить.</param>
    public static Vector3 GetSize(this Transform t)
    {
        Quaternion currentRotation = t.rotation;
        t.rotation = Quaternion.Euler(0, 0, 0);
        Bounds bounds = new Bounds(t.transform.position, Vector3.zero);

        foreach (Renderer renderer in t.GetComponentsInChildren<Renderer>())
        {
            bounds.Encapsulate(renderer.bounds);
        }

        Vector3 localCenter = bounds.center - t.position;
        bounds.center = localCenter;
        t.rotation = currentRotation;
        return bounds.size;
    }

    /// <summary>
    /// Получение размеров объекта Transform, обладающего компонентами Renderer.
    /// </summary>
    /// <returns>Размер объекта в виде Vector3.</returns>
    /// <param name="t">Компонент Transform объекта, размер которого нужно определить.</param>
    public static Vector3 GetLocalCenter(this Transform t)
    {
        Quaternion currentRotation = t.rotation;
        t.rotation = Quaternion.Euler(0, 0, 0);
        Bounds bounds = new Bounds(t.transform.position, Vector3.zero);

        foreach (Renderer renderer in t.GetComponentsInChildren<Renderer>())
        {
            bounds.Encapsulate(renderer.bounds);
        }
        t.rotation = currentRotation;
        return bounds.center - t.position;
    }
}

public static class VectorExtensions
{

    /// <summary>
    /// Adds the vector within direction.
    /// </summary>
    /// <returns>The vector within direction.</returns>
    /// <param name="baseVector">Base vector.</param>
    /// <param name="appendVector">Append vector.</param>
    public static Vector3 AddVectorWithinDirection(this Vector3 baseVector, Vector3 appendVector)
    {
        Vector3 resultedVector = Vector3.zero;
        appendVector = Quaternion.FromToRotation(appendVector, baseVector) * appendVector;
        resultedVector = baseVector + appendVector;
        return resultedVector;
    }
}

/*
public static class InputFieldExtensions
{

    public static void ColoriseConfirm (this InputField inputField) {

    }

    public static void ColoriseDeny (this InputField inputField)
    {

    }

}*/