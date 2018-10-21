using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TPresenter.Render.GeometryStage.Model;

namespace TPresenter.Render.GeometryStage.Rendering
{
    // This class will be responsible for render of all meshes.
    class GeometryRender
    {
        RenderPass renderPass;
        List<InstanceComponent> visibleEntities = new List<InstanceComponent>();

        internal void Init()
        {
            renderPass = new RenderPass();
            renderPass.Init();
        }

        internal void Draw(List<InstanceComponent> componets)
        {
            renderPass.Draw(componets);
        }

        internal void Dispose()
        {
            renderPass.Dispose();
        }
        //  All entities are visible now. Debug only!
        //internal void PerformFrustumCulling(List<IMyEntity> enities, List<InstanceComponent> out_list)
        //{
        //    out_list.Clear();
        //    foreach (var entity in entities)
        //        out_list.Add(new InstanceComponent(entity.Model));
        //}
    }
}
