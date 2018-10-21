using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TPresenter.Render.GeometryStage.Model;

namespace TPresenter.Game.Interfaces
{
    public interface IObjectEntity: IEntity, IDrawableEntity
    {
        MyModel Model { get; }

        #region Render

        bool Visible { get; set; }
        bool SkipIfTooSmall { get; set; }

        #endregion

        #region Physics



        #endregion
    }
}
