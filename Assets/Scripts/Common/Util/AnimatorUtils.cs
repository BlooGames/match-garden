using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AnimatorUtils {

    public static bool HasParameter(string paramName, Animator animator)
    {
        foreach (AnimatorControllerParameter param in animator.parameters)
        {
            if (param.name == paramName) return true;
        }
        return false;
    }

    public static bool SetFloat(GameObject gameObject, string parameter, float value)
    {
        Animator animator = gameObject.GetComponent<Animator>();
        if (animator && AnimatorUtils.HasParameter(parameter, animator))
        {
            animator.SetFloat(parameter, value);
            return true;
        }
        return false;
    }

    public static bool SetBool(GameObject gameObject, string parameter, bool value)
    {
        Animator animator = gameObject.GetComponent<Animator>();
        if (animator && AnimatorUtils.HasParameter(parameter, animator))
        {
            animator.SetBool(parameter, value);
            return true;
        }
        return false;
    }

    public static bool Trigger(GameObject gameObject, string trigger)
    {
        Animator animator = gameObject.GetComponent<Animator>();
        if (animator && AnimatorUtils.HasParameter(trigger, animator))
        {
            animator.SetTrigger(trigger);
            return true;
        }
        return false;
    }
}
