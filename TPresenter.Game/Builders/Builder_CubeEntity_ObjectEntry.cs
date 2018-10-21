using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPresenter.Game.Builders
{
    [Serializable]
    public class Builder_CubeEntity_ObjectEntry
    {
        public StringId Id { get; set; }
        public Matrix Transformations { get; set; }
    }
}
