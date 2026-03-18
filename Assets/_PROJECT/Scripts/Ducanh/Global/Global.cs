using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Global 
{
    public class Motorcycle
    {
        #region COLLISION VARIABLE
        public static float MinimumCollisionForce { get; private set; } = 4f;
        #endregion

    }

}

public enum FadeOption
{
    FadeIn,
    FadeOut,
}
