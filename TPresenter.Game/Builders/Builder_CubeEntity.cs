using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TPresenter.Game.Entities;

namespace TPresenter.Game.Builders
{
    [Serializable]
    public class Builder_CubeEntity: Builder_Entity
    {
        public List<Builder_CubeEntity_ObjectEntry> Objects { get; set; } = new List<Builder_CubeEntity_ObjectEntry>();
        public Matrix WorldMatrix { get; set; }
    }
}
