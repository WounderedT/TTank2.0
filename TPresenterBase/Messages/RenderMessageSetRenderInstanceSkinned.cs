using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TPresenter.Render.GeometryStage.Model;

namespace TPresenter.Render.Messages
{
    public class RenderMessageSetRenderInstanceSkinned : RenderMessageSetRenderInstance
    {
        public override RenderMessageTypeEnum MessageType { get { return RenderMessageTypeEnum.SetRenderInstanceSkinned; } }

        public Matrix[] SkinMatrices;
    }
}
