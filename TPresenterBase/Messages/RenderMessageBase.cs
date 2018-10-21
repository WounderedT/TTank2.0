using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPresenter.Render.Messages
{
    public abstract class RenderMessageBase
    {
        public abstract RenderMessageTypeEnum MessageType { get; }
    }
}
