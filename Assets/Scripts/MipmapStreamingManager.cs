using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace kuro
{

    public class MipmapStreamingManager : MonoBehaviour
    {
        private static MipmapStreamingManager s_Instance;
        private Dictionary<int, MipmapStreamingInfo> m_TextureStreamingMap = new();
        public Dictionary<int, MipmapStreamingInfo> textureStreamingMap => m_TextureStreamingMap;
        
        private void Awake()
        {
            if (s_Instance != null)
            {
                Destroy(this);
                return;
            }
            s_Instance = this;

            // 打开Mipmap Streaming
            QualitySettings.streamingMipmapsActive = true;
            // 将内存预算设置为特别大
            QualitySettings.streamingMipmapsMemoryBudget = 9999;
            // 裁剪等级设为最大
            QualitySettings.streamingMipmapsMaxLevelReduction = 7;
            // 设置丢弃用不到的Mipmap
            Texture.streamingTextureDiscardUnusedMips = true;
        }

        private void OnDestroy()
        {
            if (s_Instance == this)
            {
                s_Instance = null;
            }
        }
        
        public static void Register(Renderer renderer, List<MipmapStreamingInfo> textureList, bool mipmapLevelIsZero)
        {
            var manager = s_Instance;
            if (manager == null)
                return;

            using (ListPool<Material>.Get(out var materialList))
            using (ListPool<int>.Get(out var texNameList))
            {
                renderer.GetSharedMaterials(materialList);
                foreach (var mat in materialList)
                {
                    if (mat == null)
                        continue;
                    texNameList.Clear();
                    mat.GetTexturePropertyNameIDs(texNameList);
                    foreach (var texName in texNameList)
                    {
                        var tex = mat.GetTexture(texName) as Texture2D;
                        if (tex == null)
                            continue;
                        if (!tex.streamingMipmaps || tex.mipmapCount == 0)
                            continue;

                        var instanceId = tex.GetInstanceID();
                        if (!manager.m_TextureStreamingMap.TryGetValue(instanceId, out var info))
                        {
                            info = MipmapStreamingInfo.Alloc(instanceId, tex, mipmapLevelIsZero);
                            manager.m_TextureStreamingMap.Add(instanceId, info);
                        }

                        info.AddActive();
                        textureList.Add(info);
                    }
                }
            }
        }

        public static void UnRegister(List<MipmapStreamingInfo> textureList)
        {
            var manager = s_Instance;
            if (manager == null)
                return;

            foreach (var info in textureList)
            {
                if (info.SubActive())
                    continue;

                if (manager.m_TextureStreamingMap.Remove(info.id))
                    MipmapStreamingInfo.Free(info);
            }

            textureList.Clear();
        }
    }
}