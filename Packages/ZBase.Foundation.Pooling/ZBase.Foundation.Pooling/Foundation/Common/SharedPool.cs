using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace ZBase.Foundation.Pooling
{
    public static class SharedPool
    {
        private static readonly List<Action> s_resetActions = new();

        /// <seealso href="https://docs.unity3d.com/Manual/DomainReloading.html"/>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetAll()
        {
            for (var i = 0; i < s_resetActions.Count; i++)
            {
                s_resetActions[i].Invoke();
            }

            s_resetActions.Clear();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Of<T>() where T : IPool, IShareable, new()
            => SharedInstance<T>.Instance;

        private static class SharedInstance<T> where T : IPool, IShareable, new()
        {
            private static T s_instance;

            public static T Instance => s_instance ??= Create();

            private static T Create()
            {
                s_resetActions.Add(static () => s_instance = default);
                return new T();
            }
        }
    }
}
