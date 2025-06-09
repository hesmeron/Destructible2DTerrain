using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;

public class CullingFrameData : ContextItem
{
    public BufferHandle CulledMatricesBuffer;
    
    public override void Reset()
    {
        CulledMatricesBuffer = BufferHandle.nullHandle;
    }
}
