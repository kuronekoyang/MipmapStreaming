using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace kuro
{
    public abstract class MipmapStreamingHookBase : MonoBehaviour
    {
        protected List<MipmapStreamingInfo> m_TextureList = null;
        protected bool m_Visible = false;
        protected bool m_DidAwake = false;
        protected bool m_DidOnDestroy = false;


        protected abstract void SpawnImpl();

        protected virtual void DeSpawnImpl()
        {
            if (m_TextureList != null)
            {
                OnBecameInvisibleImpl();
                MipmapStreamingManager.UnRegister(m_TextureList);
                ListPool<MipmapStreamingInfo>.Release(m_TextureList);
            }
        }

        public void Spawn()
        {
            if (m_DidAwake)
                return;
            m_DidAwake = true;
            SpawnImpl();
        }

        public void DeSpawn()
        {
            if (m_DidOnDestroy)
                return;
            m_DidOnDestroy = false;
            DeSpawnImpl();
        }

        public void AddVisibleCount(int value)
        {
            if (m_TextureList == null)
                return;
            foreach (var info in m_TextureList)
                info.visibleCount += value;
        }

        protected void OnBecameVisibleImpl()
        {
            if (m_Visible)
                return;
            m_Visible = true;
            if (m_TextureList == null)
                return;
            foreach (var info in m_TextureList)
                info.visibleCount += 1;
        }

        protected void OnBecameInvisibleImpl()
        {
            if (!m_Visible)
                return;
            m_Visible = false;
            if (m_TextureList == null)
                return;
            foreach (var info in m_TextureList)
                info.visibleCount -= 1;
        }
        
        public bool IsRequestedMipmapLevelLoaded()
        {
            if (m_TextureList != null)
            {
                foreach (var tex in m_TextureList)
                {
                    if (tex.texture == null)
                        continue;
                    if (!tex.texture.IsRequestedMipmapLevelLoaded())
                        return false;
                }    
            }
                
            return true;
        }

    }
}