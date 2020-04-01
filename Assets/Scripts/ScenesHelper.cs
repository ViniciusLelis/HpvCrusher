using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class ScenesHelper {

    public static float VolumeScalar { get; set; }
    private static Dictionary<string, object> sceneParameters;

    public static object GetParameter(string parameterName)
    {
        return sceneParameters[parameterName];
    }

    public static void SetParameter(string parameterName, object value)
    {
        if (sceneParameters == null)
            sceneParameters = new Dictionary<string, object>();
        sceneParameters.Add(parameterName, value);
    }

    public static void ClearParameters()
    {
        sceneParameters.Clear();
    }

}
