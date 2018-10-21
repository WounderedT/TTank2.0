using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TPresenter.Game.Entities;

namespace TPresenter.Game.Builders
{
    [Serializable]
    public class Builder_Scene
    {
        public StringId Id { get; set; }
        public List<Builder_CubeEntity> Cells { get; set; } = new List<Builder_CubeEntity>();
    }
}
