using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;

public class CullingFrameData : ContextItem
{
    public BufferHandle CulledMatricesBuffer;
    public int InstanceCount;
    
    public override void Reset()
    {
        CulledMatricesBuffer = BufferHandle.nullHandle;
        InstanceCount = 0;
    }
}
