using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;

namespace CameraTest.Util
{
    public static class InputUtil
    {
        /// <summary> Return true if any key in array is pressing. </summary>
        /// <param name="keys"> Target keys. </param>
        /// <returns> Any key in array is pressing. </returns>
        public static bool GetAnyKeysPressed(params KeyCode[] keys)
            => keys.Any(t => Input.GetKey(t));
    }
    // public static class QuaternionUtil
    // {
    //     /// <summary> Compare two quaternion. </summary>
    //     /// <param name="compare"> Need compared Quaternion. </param>
    //     /// <returns> The percent of target compare compare(Quaternion). </returns>
    //     public static float CompareQuaternion(this Quaternion target, Quaternion compare)
    //     {
    //         return Quaternion.Dot(target, compare);
    //     }

    //     /// <summary> Compare with a quaternion. </summary>
    //     /// <param name="target"> Target Vector3. </param>
    //     /// <param name="compare"> Need compare Quaternion. </param>
    //     /// <returns> The percent of target compare compare(Quaternion). </returns>
    //     public static float CompareQuaternion(Vector3 target, Quaternion compare)
    //     {
    //         return Quaternion.Dot(Quaternion.Euler(target), compare);
    //     }
    // }
    // public static class DGTweeningUtil
    // {
    //     /// <summary>
    //     /// EXPERIMENTAL METHOD - Tweens a Transform's rotation BY the given value (as if
    //     ///     you chained a
    //     ///     SetRelative
    //     ///     ), in a way that allows other DOBlendableRotate tweens to work together on the
    //     ///     same target, instead than fight each other as multiple DORotate would do. Also
    //     ///     stores the transform as the tween's target so it can be used for filtered operations
    //     ///  </summary>
    //     /// <param name=""> The value to tween by </param>
    //     /// <param name=""> The duration of the tween </param>
    //     /// <param name=""> Rotation mode </param>
    //     public static Tweener DOBlendableLocalRotate(
    //         this Transform target,
    //         Vector3 byValue, 
    //         float duration, 
    //         RotateMode mode = RotateMode.Fast)
    //     {
    //         Quaternion to = Quaternion.identity;
    //         TweenerCore<Quaternion, Vector3, QuaternionOptions> tweenerCore = DOTween.To(
    //         () => to,
    //         (Quaternion x) => {
    //             var rot = target.localEulerAngles + byValue * QuaternionUtil.CompareQuaternion(byValue, x);
    //             to = Quaternion.Euler(rot);
    //             target.localRotation = to;
    //         }, 
    //         byValue, 
    //         duration
    //         ).Blendable<Quaternion, Vector3, QuaternionOptions>().SetTarget(target);
    //         tweenerCore.plugOptions.rotateMode = mode;
    //         return tweenerCore;
    //     }
    // }
}
