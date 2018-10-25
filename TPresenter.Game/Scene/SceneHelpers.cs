using SharpDX;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using TPresenter.Filesystem;
using TPresenter.Game.Builders;
using TPresenter.Serialization;
using TPresenterMath;

namespace TPresenter.Game
{
    /// <summary>
    /// Class contains scnene and object generation methods. Temporary solution befor 'Creation kit' layer is implemented.
    /// </summary>
    internal static class SceneHelpers
    {
        public static StringId GenerateSceneSingleObjectEntity()
        {
            Builder_Scene builderScene = new Builder_Scene();
            builderScene.Id = StringId.GetOrCompute("[Generated]_SingleObjectEntityScene");

            Builder_CubeEntity builderCell = new Builder_CubeEntity();
            builderCell.Id = StringId.GetOrCompute("GeneratedRenderTestCell001");
            builderCell.WorldMatrix = Matrix.Identity;

            Builder_CubeEntity_ObjectEntry builderCellEntry = new Builder_CubeEntity_ObjectEntry();
            builderCellEntry.Id = StringId.GetOrCompute("TestDoorCylinderEmpty");
            builderCellEntry.Transformations = Matrix.Scaling(0.025f) * Matrix_T.RotationX(270);
            //builderCellEntry.Transformations = Matrix.Identity;

            builderCell.Objects.Add(builderCellEntry);
            builderScene.Cells.Add(builderCell);
            WriteScene(builderScene);

            return builderScene.Id;
        }

        public static StringId GenerateSceneSingleActorEntityBruxa()
        {
            Builder_Scene builderScene = new Builder_Scene();
            builderScene.Id = StringId.GetOrCompute("[Generated]_SingleActorEntitySceneBruxa");

            Builder_CubeEntity builderCell = new Builder_CubeEntity();
            builderCell.Id = StringId.GetOrCompute("GeneratedRenderTestCell001");
            builderCell.WorldMatrix = Matrix.Identity;

            Builder_CubeEntity_ObjectEntry builderCellEntry = new Builder_CubeEntity_ObjectEntry();
            builderCellEntry.Id = StringId.GetOrCompute("Bruxa");
            builderCellEntry.Transformations = Matrix.Scaling(0.25f) * Matrix_T.RotationX(270);

            builderCell.Objects.Add(builderCellEntry);
            builderScene.Cells.Add(builderCell);
            WriteScene(builderScene);

            return builderScene.Id;
        }

        public static StringId GenerateSceneSingleActorEntityFemale()
        {
            Builder_Scene builderScene = new Builder_Scene();
            builderScene.Id = StringId.GetOrCompute("[Generated]_SingleActorEntitySceneFemale");

            Builder_CubeEntity builderCell = new Builder_CubeEntity();
            builderCell.Id = StringId.GetOrCompute("GeneratedRenderTestCell001");
            builderCell.WorldMatrix = Matrix.Identity;

            Builder_CubeEntity_ObjectEntry builderCellEntry = new Builder_CubeEntity_ObjectEntry();
            builderCellEntry.Id = StringId.GetOrCompute("Female");
            builderCellEntry.Transformations = Matrix.Scaling(0.25f) * Matrix_T.RotationX(270);

            builderCell.Objects.Add(builderCellEntry);
            builderScene.Cells.Add(builderCell);
            WriteScene(builderScene);

            return builderScene.Id;
        }

        public static StringId GenerateSceneActorEntitObjectEntity()
        {
            Builder_Scene builderScene = new Builder_Scene();
            builderScene.Id = StringId.GetOrCompute("[Generated]_ActorEntityObjectEntityScene");

            Builder_CubeEntity builderCell = new Builder_CubeEntity();
            builderCell.Id = StringId.GetOrCompute("GeneratedRenderTestCell001");
            builderCell.WorldMatrix = Matrix.Identity;

            Builder_CubeEntity_ObjectEntry builderCellEntry_ObjectEntity = new Builder_CubeEntity_ObjectEntry();
            builderCellEntry_ObjectEntity.Id = StringId.GetOrCompute("TestDoorCylinderEmpty");
            builderCellEntry_ObjectEntity.Transformations = Matrix.Scaling(0.25f) * Matrix_T.RotationX(270);

            Builder_CubeEntity_ObjectEntry builderCellEntry_ActorEntity = new Builder_CubeEntity_ObjectEntry();
            builderCellEntry_ActorEntity.Id = StringId.GetOrCompute("Bruxa");
            builderCellEntry_ActorEntity.Transformations = Matrix.Scaling(0.25f) * Matrix_T.RotationX(270);

            builderCell.Objects.Add(builderCellEntry_ActorEntity);
            builderCell.Objects.Add(builderCellEntry_ObjectEntity);

            builderScene.Cells.Add(builderCell);
            WriteScene(builderScene);

            return builderScene.Id;
        }

        public static void GenerateObjectDefinitions_ObjectEntity()
        {
            BuilderEntityCollection<Builder_ObjectEntity> entityCollection = new BuilderEntityCollection<Builder_ObjectEntity>();

            Builder_ObjectEntity builderObjectEntity = new Builder_ObjectEntity();
            builderObjectEntity.Id = StringId.GetOrCompute("TestDoorCylinderEmpty");
            builderObjectEntity.Name = "Cylinder Door Empty";
            builderObjectEntity.ModelId = StringId.GetOrCompute("test-door-cylinder-empty");

            entityCollection.BuilderEntityList.Add(builderObjectEntity);

            Builder_Entity.WriteBuilderDefinitions<Builder_ObjectEntity>(entityCollection);
        }

        public static void GenerateObjectDefinitions_ActorEntity()
        {
            BuilderEntityCollection<Builder_ActorEntity> entityCollection = new BuilderEntityCollection<Builder_ActorEntity>();

            Builder_ActorEntity builderBruxa = new Builder_ActorEntity();
            builderBruxa.Id = StringId.GetOrCompute("Bruxa");
            builderBruxa.Name = "Bruxa";
            builderBruxa.SkeletonId = StringId.GetOrCompute(@"characters\common\default-skeleton-female");
            builderBruxa.ModelParts = new List<StringId>(1);
            builderBruxa.ModelParts.Add(StringId.GetOrCompute(@"characters\bruxa\bruxa-torso-default"));
            builderBruxa.ModelParts.Add(StringId.GetOrCompute(@"characters\bruxa\bruxa-head-default"));
            builderBruxa.ModelParts.Add(StringId.GetOrCompute(@"characters\bruxa\bruxa-eyes-default"));
            builderBruxa.ModelParts.Add(StringId.GetOrCompute(@"characters\bruxa\woman-teeth-default"));
            builderBruxa.ModelParts.Add(StringId.GetOrCompute(@"characters\bruxa\bruxa-hair-default"));

            Builder_ActorEntity builderFemale = new Builder_ActorEntity();
            builderFemale.Id = StringId.GetOrCompute("Female");
            builderFemale.Name = "Female CBBE";
            builderFemale.SkeletonId = StringId.GetOrCompute(@"characters\common\default-skeleton-female");
            builderFemale.ModelParts = new List<StringId>(1);
            builderFemale.ModelParts.Add(StringId.GetOrCompute(@"characters\female\female-torso-default"));
            builderFemale.ModelParts.Add(StringId.GetOrCompute(@"characters\female\female-head-default"));
            builderFemale.ModelParts.Add(StringId.GetOrCompute(@"characters\female\female-hair-default"));

            entityCollection.BuilderEntityList.Add(builderBruxa);
            entityCollection.BuilderEntityList.Add(builderFemale);

            Builder_Entity.WriteBuilderDefinitions<Builder_ActorEntity>(entityCollection);
        }

        private static void WriteScene(Builder_Scene builderScene)
        {
            XmlSerializer serializer = XmlSerializerManager.GetOrCreateSerializer(typeof(Builder_Scene));
            using (FileStream fileStream = new FileStream(Path.Combine(FileProvider.ContentPath, builderScene.Id.String + ".xml"), FileMode.Create))
            using (XmlTextWriter xmlTextWriter = new XmlTextWriter(fileStream, Encoding.Unicode))
            {
                xmlTextWriter.Formatting = Formatting.Indented;
                xmlTextWriter.Indentation = 4;
                serializer.Serialize(xmlTextWriter, builderScene);
            }
        }
    }
}
