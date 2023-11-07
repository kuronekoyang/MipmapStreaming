using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Pool;

namespace kuro
{

    public class MipmapStreamingInfo
    {
        private const int k_InvisibleMipmapLevel = 7;

        private static ObjectPool<MipmapStreamingInfo> s_Pool = new(
            static () => new(),
            null,
            static x =>
            {
                x.m_MipmapLevelIsZero = default;
                x.m_Id = default;
                x.m_Texture2D = default;
                x.m_ActiveCount = default;
                x.m_VisibleCount = default;
            }
        );

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static MipmapStreamingInfo Alloc(int id, Texture2D texture, bool mipmapLevelIsZero)
        {
            var r = s_Pool.Get();
            r.m_Id = id;
            r.m_Texture2D = texture;
            r.m_MipmapLevelIsZero = mipmapLevelIsZero;
            return r;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Free(MipmapStreamingInfo info)
        {
            s_Pool.Release(info);
        }

        private bool m_MipmapLevelIsZero;
        private int m_Id;

        public int id
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => m_Id;
        }

        private Texture2D m_Texture2D;

        public Texture2D texture
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => m_Texture2D;
        }

        private int m_ActiveCount;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddActive() => ++m_ActiveCount;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool SubActive()
        {
            --m_ActiveCount;
            return m_ActiveCount > 0;
        }

        private int? m_VisibleCount;

        public int visibleCount
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => m_VisibleCount ?? 0;
            set
            {
                bool updateMipmapLevel = false;
                if (m_VisibleCount.HasValue)
                {
                    var old = m_VisibleCount.Value;
                    if (old == value)
                        return;
                    if (value * old <= 0)
                        updateMipmapLevel = true;
                }
                else
                {
                    updateMipmapLevel = true;
                }

                m_VisibleCount = value;

                if (!updateMipmapLevel)
                    return;

                if (value > 0)
                {
                    if (m_MipmapLevelIsZero)
                    {
                        m_Texture2D.requestedMipmapLevel = 0;
                        Debug.Log($"{m_Texture2D.name} requestedMipmapLevel = 0");
                    }
                    else
                    {
                        m_Texture2D.ClearRequestedMipmapLevel();
                        Debug.Log($"{m_Texture2D.name} ClearRequestedMipmapLevel");
                    }
                }
                else
                {
                    m_Texture2D.requestedMipmapLevel = k_InvisibleMipmapLevel;
                    Debug.Log($"{m_Texture2D.name} requestedMipmapLevel = {k_InvisibleMipmapLevel}");
                }
            }
        }

        private MipmapStreamingInfo()
        {
        }
    }
}