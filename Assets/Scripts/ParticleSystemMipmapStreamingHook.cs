using UnityEngine;
using UnityEngine.Pool;

namespace kuro
{
    public class ParticleSystemMipmapStreamingHook : MipmapStreamingHookBase
    {
        protected override void SpawnImpl()
        {
            var renderer = GetComponent<ParticleSystemRenderer>();
            if (renderer)
            {
                m_TextureList = ListPool<MipmapStreamingInfo>.Get();
                MipmapStreamingManager.Register(renderer, m_TextureList, true);
                if (m_TextureList.Count > 0)
                {
                    m_Visible = renderer.gameObject.activeInHierarchy;
                    // 不可见也要加0
                    var addVisible = m_Visible ? 1 : 0;
                    foreach (var info in m_TextureList)
                        info.visibleCount += addVisible;
                }
                else
                {
                    ListPool<MipmapStreamingInfo>.Release(m_TextureList);
                    m_TextureList = null;
                }
            }
        }

        private void OnDisable()
        {
            OnBecameInvisibleImpl();
        }

        private void OnEnable()
        {
            OnBecameVisibleImpl();
        }
    }
}