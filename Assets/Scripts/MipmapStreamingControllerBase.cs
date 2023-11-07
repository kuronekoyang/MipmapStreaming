using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace kuro
{

    public abstract class MipmapStreamingControllerBase : MonoBehaviour
    {
        private static UnityEngine.Profiling.CustomSampler s_AwakeSampler = UnityEngine.Profiling.CustomSampler.Create($"{nameof(MipmapStreamingControllerBase)}.{nameof(Awake)}");
        private static UnityEngine.Profiling.CustomSampler s_OnDestroySampler = UnityEngine.Profiling.CustomSampler.Create($"{nameof(MipmapStreamingControllerBase)}.{nameof(OnDestroy)}");

        protected List<MipmapStreamingHookBase> m_HookList = null;
        protected bool m_IsLock = false;

        protected abstract void AwakeImpl();

        protected virtual void OnDestroyImpl()
        {
            UnLock();
            if (m_HookList != null)
            {
                foreach (var hook in m_HookList)
                    hook.DeSpawn();

                ListPool<MipmapStreamingHookBase>.Release(m_HookList);
                m_HookList = null;
            }
        }

        protected void Awake()
        {
            s_AwakeSampler.Begin();
            m_HookList = ListPool<MipmapStreamingHookBase>.Get();
            AwakeImpl();
            if (m_HookList.Count <= 0)
            {
                ListPool<MipmapStreamingHookBase>.Release(m_HookList);
                m_HookList = null;
            }
            s_AwakeSampler.End();
        }

        protected void OnDestroy()
        {
            s_OnDestroySampler.Begin();
            OnDestroyImpl();
            s_OnDestroySampler.End();
        }

        public void Lock()
        {
            if (m_IsLock)
                return;
            m_IsLock = true;

            if (m_HookList == null)
                return;
            foreach (var hook in m_HookList)
                hook.AddVisibleCount(1);
        }

        public void UnLock()
        {
            if (!m_IsLock)
                return;
            m_IsLock = false;

            if (m_HookList == null)
                return;
            foreach (var hook in m_HookList)
                hook.AddVisibleCount(-1);
        }

        public bool IsRequestedMipmapLevelLoaded()
        {
            if (m_HookList != null)
            {
                foreach (var hook in m_HookList)
                {
                    if (!hook.IsRequestedMipmapLevelLoaded())
                        return false;
                }
            }

            return true;
        }
    }
}