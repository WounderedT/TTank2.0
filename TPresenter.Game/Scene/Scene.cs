using SharpDX;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using TPresenter.Filesystem;
using TPresenter.Game.Builders;
using TPresenter.Game.Entities;
using TPresenter.Game.Interfaces;
using TPresenter.Serialization;
using TPresenterMath;

namespace TPresenter.Game
{
    public class Scene
    {
        private static Dictionary<StringId, Builder_Scene> _sceneBuilders = new Dictionary<StringId, Builder_Scene>(StringId.Comparer);
        public static List<IEntity> SceneObjects = new List<IEntity>();  //This will not gonna work with cells and distance render. For collada fix only!

        public static void LoadScene(StringId sceneBuilderId)
        {
            //TODO: temporary solution. Remove when 'Creation kit' is ready.
            SceneHelpers.GenerateObjectDefinitions_ActorEntity();
            SceneHelpers.GenerateObjectDefinitions_ObjectEntity();
            sceneBuilderId = SceneHelpers.GenerateSceneActorEntitObjectEntity();
            //sceneBuilderId = SceneHelpers.GenerateSceneSingleActorEntityFemale();
            //sceneBuilderId = SceneHelpers.GenerateSceneSingleActorEntityBruxa();
            //sceneBuilderId = SceneHelpers.GenerateSceneSingleObjectEntity();

            ReadScene(sceneBuilderId);
            UpdateScene(sceneBuilderId);
        }

        private static void ReadScene(StringId id)
        {
            XmlSerializer serializer = XmlSerializerManager.GetOrCreateSerializer(typeof(Builder_Scene));
            Builder_Scene sceneEntity;
            using (System.Xml.XmlReader xReader = System.Xml.XmlReader.Create(Path.Combine(FileProvider.ContentPath, id.String + ".xml")))
                sceneEntity = serializer.Deserialize(xReader) as Builder_Scene;
            _sceneBuilders.Add(sceneEntity.Id, sceneEntity);
        }

        /// <summary>
        /// Update current scene with objects from <paramref name="sceneBuilderId"/> builder.
        /// TODO: this method is buggy. Use for collada fix only!
        /// </summary>
        /// <param name="sceneBuilderId"></param>
        private static void UpdateScene(StringId sceneBuilderId)
        {
            var sceneBuilder = _sceneBuilders[sceneBuilderId];
            foreach(Builder_CubeEntity builderCellEntity in sceneBuilder.Cells)
            {
                CubeEntity cellEntity = new CubeEntity();
                cellEntity.Init(builderCellEntity);
                foreach(var entry in cellEntity.Objects)
                {
                    Matrix newWorld;
                    Matrix.Multiply(ref cellEntity.WorldMatrix, ref entry.Value.WorldMatrix, out newWorld);
                    entry.Value.SetWorldMatrix(newWorld);
                    SceneObjects.Add(entry.Value);
                }
            }
        }
    }
}
