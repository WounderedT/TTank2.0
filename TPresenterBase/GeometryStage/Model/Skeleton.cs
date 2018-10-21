using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TPresenter.Render.Resources;

namespace TPresenter.Render.GeometryStage.Model
{
    public class Skeleton
    {
        public List<Bone> Bones = new List<Bone>();
        public Dictionary<StringId, int> ListIndexByBoneId = new Dictionary<StringId, int>(StringId.Comparer);

        public void Init(string skeletonName)
        {
            ColladaModel model = new ColladaModel();
            Bones = model.LoadSkeleton(skeletonName);
            for(int ind = 0; ind< Bones.Count; ind++)
            {
                ListIndexByBoneId.Add(Bones[ind].BoneId, ind);
            }
        }
    }
}
