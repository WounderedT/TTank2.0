using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TPresenter.Render.GeometryStage.Model;

namespace TPresenter.Render.Animation
{
    public class MyAnimation
    {
        public List<JointAnimation> JointAnimations = new List<JointAnimation>();

        public void Load(StringId name)
        {
            ColladaModel colladaModel = new ColladaModel();
            JointAnimations = colladaModel.LoadAnimation(name.String);
        }
    }
}
