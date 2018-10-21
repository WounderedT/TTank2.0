using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TPresenter.Render.GeometryStage.Model;
using TPresenter.Render.Resources;

namespace TPresenter
{
    // Should it be static?
    public static class ModelManager
    {
        public static Dictionary<StringId, MyModel> models = new Dictionary<StringId, MyModel>(StringId.Comparer);
        //  skinned models of one type have the same skeleton.
        public static Dictionary<StringId, Skeleton> skeletons = new Dictionary<StringId, Skeleton>(StringId.Comparer);

        public static MyModel GetOrLoadModel(StringId modelId)
        {
            if (models.ContainsKey(modelId))
                return models[modelId];

            MyModel model = new MyModel();
            model.Load(modelId.String);
            models[modelId] = model;
            return model;
        }

        public static MyModel GetOrLoadSkinnedModel(StringId modelId, Skeleton skeleton)
        {
            if (models.ContainsKey(modelId))
                return models[modelId];

            MyModel model = new MyModel();
            model.LoadSkinned(modelId.String, skeleton);
            models[modelId] = model;
            return model;
        }

        public static Skeleton GetOrLoadSkeleton(StringId modelId)
        {
            if (skeletons.ContainsKey(modelId))
                return skeletons[modelId];

            Skeleton skeleton = new Skeleton();
            skeleton.Init(modelId.String);
            skeletons[modelId] = skeleton;
            return skeleton;
        }

        public static bool IsModelLoaded(StringId modelId)
        {
            return models.ContainsKey(modelId) && models[modelId] != null;
        }
    }
}