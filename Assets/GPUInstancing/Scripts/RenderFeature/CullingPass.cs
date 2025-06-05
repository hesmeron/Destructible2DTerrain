using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

public class CullingPass : ScriptableRenderPass
{
    private class PassData
    {

    }
    
    static void ExecutePass(PassData data, ComputeGraphContext context)
    {
        
    }
    
    public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
    {
        const string passName = "Culling Pass";
            
        using (var builder = renderGraph.AddComputePass<PassData>(passName, out var passData))
        {
            builder.SetRenderFunc((PassData data, ComputeGraphContext context) => ExecutePass(data, context));
        }
    }
}
