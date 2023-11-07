using UnityEngine;
using UnityEngine.Pool;

namespace kuro
{
    public class MipmapStreamingController : MipmapStreamingControllerBase
    {

        protected override void AwakeImpl()
        {
            using (ListPool<Renderer>.Get(out var rendererList))
            {
                this.GetComponentsInChildren(true, rendererList);
                if (rendererList.Count > 0)
                {
                    foreach (var renderer in rendererList)
                    {
                        MipmapStreamingHookBase hook;
                        if (renderer is ParticleSystemRenderer)
                        {
                            if (renderer.TryGetComponent<ParticleSystemMipmapStreamingHook>(out var c))
                                hook = c;
                            else
                                hook = renderer.gameObject.AddComponent<ParticleSystemMipmapStreamingHook>();
                        }
                        else
                        {
                            if (renderer.TryGetComponent<MipmapStreamingHook>(out var c))
                                hook = c;
                            else
                                hook = renderer.gameObject.AddComponent<MipmapStreamingHook>();
                        }
                        hook.Spawn();
                        m_HookList.Add(hook);
                    }
                }
            }
        }
    }
}