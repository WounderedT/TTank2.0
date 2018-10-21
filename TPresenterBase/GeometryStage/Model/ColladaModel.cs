using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using TPresenter.Filesystem;
using System.Xml;
using TPresenter.Render.Resources;
using System.Xml.Linq;
using SharpDX;
using Collada141;
using TPresenterMath;
using System.Reflection;
using System.Globalization;
using TPresenter.Render.Animation;

namespace TPresenter.Render.GeometryStage.Model
{
    public class ColladaModel
    {
        public Material Material;
        public Matrix BindShapeMatrix;
        public GeometryPart[] Parts;
        
        public String Name;
        public String FullName;

        List<GeometryControllerPart> ModelControllers = new List<GeometryControllerPart>();

        #region Public Methods

        public void LoadModel(String name)
        {
            SetModelNames(name);
            COLLADA model = COLLADA.Load(FullName);
            LoadMaterial(model);
            LoadGeometry(model);
            LoadBindingMatrix(model);
        }

        public void LoadSkinnedModel(String name, Skeleton skeleton)
        {
            SetModelNames(name);
            COLLADA model = COLLADA.Load(FullName);
            LoadMaterial(model);
            LoadModelControllers(model, skeleton);
            LoadGeometry(model, true);
        }

        public List<JointAnimation> LoadAnimation(String name)
        {
            SetAnimationNames(name);
            COLLADA model = COLLADA.Load(FullName);
            List<JointAnimation> jointAnimations = new List<JointAnimation>();
            var animationSources = COLLADA.FindEntryByType<library_animations>(model).animation[0].Items;
            for(var ind = 0; ind < animationSources.Length; ind += 3)
            {
                if(animationSources[ind] is Source source)
                {
                    var boneName = source.id.Split(new string[] { "_matrix-" }, StringSplitOptions.None).First();
                    var keyframes = (source.Item as float_array).Values;

                    source = animationSources[ind + 1] as Source;
                    var boneTransformMatrices = GetMatricesFromValue((source.Item as float_array).Values);

                    source = animationSources[ind + 2] as Source;
                    var transformationTypes = (source.Item as Name_array).Values;

                    JointAnimation jointAnim = new JointAnimation();
                    jointAnim.keyFrames = new KeyFrame[keyframes.Length];
                    jointAnim.jointName = StringId.GetOrCompute(boneName);
                    for (int j = 0; j < keyframes.Length; j++)
                    {
                        KeyFrame frame = new KeyFrame(boneTransformMatrices[j], transformationTypes[j]);
                        frame.timestamp = keyframes[j];
                        jointAnim.keyFrames[j] = frame;
                    }
                    jointAnimations.Add(jointAnim);
                }
                else
                {
                    break;
                }
            }
            return jointAnimations;
        }

        public List<Bone> LoadSkeleton(String name)
        {
            SetModelNames(name);
            COLLADA model = COLLADA.Load(FullName);

            //first node in library_visual_scene is EnvironmentAmbientLight. Skipping.
            var rootNode = COLLADA.FindEntryByType<library_visual_scenes>(model).visual_scene[0].node[1];

            var bonesList = new List<Bone>();
            ProcessNode(bonesList, rootNode);

            return bonesList;
        }

        #endregion

        #region Internal Load Methods

        void SetModelNames(String name)
        {
            Name = name;
            FullName = Path.Combine(FileProvider.ModelsPath, Name + ".dae");
        }

        void SetAnimationNames(String name)
        {
            Name = name;
            FullName = Path.Combine(FileProvider.AnimationsPath, Name + ".dae");
        }

        //NOTE: This method has the following assumptions:
        //  One assigned material per model.
        //  One effect per material.
        //  One render technique per effect
        void LoadMaterial(COLLADA model)
        {
            var material = COLLADA.FindEntryByType<library_materials>(model).material[0];
            Material.Name = material.name;

            var images = COLLADA.FindEntryByType<library_images>(model).image;
            Material.Textures = new String[images.Length];
            for (int ind = 0; ind < images.Length; ind++)
            {
                Material.Textures[ind] = TextureToRelativePath(images[ind].Item.ToString());
            }

            var effect = COLLADA.FindEntryByType<library_effects>(model).effect[0];
            InitMaterialEffectParameters(effect.Items[0].technique.Item);

            Matrix uv = Matrix.Identity;
            uv.M14 = 0;
            uv.M22 = -1;
            uv.M24 = 1;
            uv.M33 = 0;
            uv.M44 = 1;
            Material.UVTransform = uv;
        }

        void LoadGeometry(COLLADA model, Boolean isSkinned = false)
        {
            var geometries = COLLADA.FindEntryByType<library_geometries>(model).geometry;
            Parts = new GeometryPart[geometries.Length];
            for (var ind = 0; ind < Parts.Length; ind++)
            {
                if (isSkinned)
                {
                    Parts[ind] = LoadVertices(geometries[ind].Item as Mesh, StringId.GetOrCompute(geometries[ind].name),
                        ModelControllers[ind]);
                    Parts[ind].Bones = ModelControllers[ind].BoneDictionary;
                }
                else
                {
                    Parts[ind] = LoadVertices(geometries[ind].Item as Mesh, StringId.GetOrCompute(geometries[ind].name));
                }
            }
            var mesh = geometries[0].Item as Mesh;
        }

        GeometryPart LoadVertices(Mesh meshObj, StringId geometryId, GeometryControllerPart geometryControllerPart = null)
        {
            var offset = meshObj.GetOffset(0);
            var positions = meshObj.source[offset.Vertex].Item as float_array;
            var normals = meshObj.source[offset.Normal].Item as float_array;
            var uvs = meshObj.source[offset.Texcoord].Item as float_array;
            UInt32 trianglesCount = (UInt32)meshObj.GetTrianglesCount(0);

            GeometryPart part = new GeometryPart(geometryId, trianglesCount * 3, geometryControllerPart != null);
            part.VertexCount = trianglesCount;
            part.StartVertex = 0;
            part.BindShapeMatrix = (geometryControllerPart != null) ? geometryControllerPart.BindShapeMatrix : Matrix.Identity;

            var trianglePointsArray = meshObj.GetTrianglePointsRaw(0);
            var step = meshObj.GetStep(0);

            int k = 0;
            if (geometryControllerPart == null)
            {
                for (int i = 0; i < trianglePointsArray.Length; i += step)
                {
                    var vertex = LoadVertex(positions, normals, uvs, trianglePointsArray, ref i, ref offset);
                    part.Vertices[k] = vertex;
                    part.Indices[k] = (ushort)k;
                    k++;
                }
            }
            else
            {
                SkinningVertex[] skinningVertices = geometryControllerPart.VertexSkinningArray;
                for (int i = 0; i < trianglePointsArray.Length; i += step)
                {
                    var vertex = LoadVertex(positions, normals, uvs, trianglePointsArray, ref i, ref offset);
                    part.Vertices[k] = vertex;
                    part.Indices[k] = (ushort)k;
                    part.Skinning[k] = skinningVertices[trianglePointsArray[i + offset.Vertex]];
                    k++;
                }
            }

            return part;
        }

        Vertex LoadVertex(
            float_array positions, float_array normals, float_array uvs,
            Int32[] trianglePointsArray, ref Int32 ind, ref Offset offset)
        {
            var vertex = new Vertex();
            vertex.Color = new Color(255, 255, 255, 255);
            vertex.Position = Vector3_T.New(
                positions.Values[trianglePointsArray[ind + offset.Vertex] * 3],
                positions.Values[trianglePointsArray[ind + offset.Vertex] * 3 + 1],
                positions.Values[trianglePointsArray[ind + offset.Vertex] * 3 + 2]);
            vertex.Normal = Vector3_T.New(
                normals.Values[trianglePointsArray[ind + offset.Normal] * 3],
                normals.Values[trianglePointsArray[ind + offset.Normal] * 3 + 1],
                normals.Values[trianglePointsArray[ind + offset.Normal] * 3 + 2]);
            var textcoordU = (float)uvs.Values[trianglePointsArray[ind + offset.Texcoord] * 3];
            var textcoordV = (float)uvs.Values[trianglePointsArray[ind + offset.Texcoord] * 3 + 1];
            vertex.UV = new Vector2(
                (textcoordU > 0) ? textcoordU : 1 + textcoordU,
                (textcoordV > 0) ? textcoordV : 1 + textcoordV);
            return vertex;
        }


        void ProcessNode(List<Bone> bonesList, Node nodeObj, Int32 parentIndex = -1)
        {
            Bone bone = new Bone();
            bone.BoneLocalTransform = Matrix_T.New((nodeObj.Items[0] as MatrixCollada).ValuesDouble);
            bone.ParentIndex = parentIndex;
            bone.BoneId = StringId.GetOrCompute(nodeObj.id);
            parentIndex = bonesList.Count;
            bonesList.Add(bone);
            if (nodeObj.node1 == null)
                return;
            for(var ind = 0; ind < nodeObj.node1.Length; ind++)
            {
                ProcessNode(bonesList, nodeObj.node1[ind], parentIndex);
            }
        }

        void LoadModelControllers(COLLADA model, Skeleton skeleton)
        {
            var controllers = COLLADA.FindEntryByType<library_controllers>(model);
            foreach(var controllerEntry in controllers.controller)
            {
                ModelControllers.Add(LoadModelController(controllerEntry, skeleton));
            }
        }

        GeometryControllerPart LoadModelController(Controller controllerEntry, Skeleton skeleton)
        {
            var skinning = controllerEntry.Item as Skin;
            GeometryControllerPart controllerPart = new GeometryControllerPart();
            controllerPart.Id = StringId.GetOrCompute(controllerEntry.id);
            controllerPart.BindShapeMatrix = Matrix_T.New(COLLADA.ParseStringAsArrayDouble(skinning.bind_shape_matrix));
            controllerPart.SkinJointNameArray = StringId.GetOrCompute((skinning.source[0].Item as Name_array).Values);

            var invBindPosesValues = (skinning.source[1].Item as float_array).Values;
            for (var ind = 0; ind < controllerPart.SkinJointNameArray.Length; ind++)
            {
                var matrix = new Double[16];
                for (var j = 0; j < 16; j++)
                    matrix[j] = invBindPosesValues[ind * 16 + j];
                ModelBone bone = new ModelBone();
                bone.InvBindPose = Matrix_T.New(matrix);
                bone.BoneIndex = skeleton.ListIndexByBoneId[controllerPart.SkinJointNameArray[ind]];
                bone.BoneSid = controllerPart.SkinJointNameArray[ind];
                controllerPart.BoneDictionary.Add(skeleton.Bones[bone.BoneIndex].BoneId, bone);
            }

            controllerPart.VertexSkinningArray = LoadVertexSkinnig(skinning, controllerPart);

            return controllerPart;
        }

        SkinningVertex[] LoadVertexSkinnig(Skin skinning, GeometryControllerPart controllerPart)
        {
            var bonesPerVertex = COLLADA.ParseStringAsArrayInt32(skinning.vertex_weights.vcount);
            var boneIndicesAndWeights = COLLADA.ParseStringAsArrayInt32(skinning.vertex_weights.v);
            var boneWeights = (skinning.source[2].Item as float_array).Values;

            SkinningVertex[] skinningVertex = new SkinningVertex[bonesPerVertex.Length];
            int boneIndexWeightInd = 0;
            int influense = 0;
            for (int i = 0; i < skinningVertex.Length; i++)
            {
                System.Diagnostics.Debug.Assert(bonesPerVertex[i] <= 8, "Vertex is affected by " + bonesPerVertex[i] + " bones. Bone influence cannot be greated than 8!");
                SkinningVertex vertex = new SkinningVertex(bonesPerVertex[i]);
                if (bonesPerVertex[i] > influense)
                    influense = bonesPerVertex[i];
                for (int j = 0; j < bonesPerVertex[i]; j++)
                {
                    var ind = boneIndexWeightInd++;
                    vertex.BoneIndices[j] = (UInt32)controllerPart.BoneDictionary[controllerPart.SkinJointNameArray[boneIndicesAndWeights[ind]]].BoneIndex;
                    vertex.BoneWeights[j] = (Single)boneWeights[boneIndicesAndWeights[boneIndexWeightInd++]];
                }

                Single sum = vertex.BoneWeights.Sum();
                if (sum != 1)
                {
                    for (int j = 0; j < vertex.BoneWeights.Length; j++)
                        vertex.BoneWeights[j] /= sum;
                }

                skinningVertex[i] = vertex;
            }

            return skinningVertex;
        }

        void LoadBindingMatrix(COLLADA model)
        {
            var nodes = COLLADA.FindEntryByType<library_visual_scenes>(model).visual_scene[0];
            foreach(GeometryPart part in Parts)
            {
                var node = nodes.node.Where(n => n.name.Equals(part.GeometryId.String)).Single();
                if(node.Items == null)
                {
                    BindShapeMatrix = Matrix.Identity;
                    return;
                }
                Matrix outterMatrix = Matrix_T.New((node.Items[0] as MatrixCollada).ValuesDouble);
                if(node.node1 != null)
                {
                    Matrix innerMatrix = Matrix_T.New((node.node1[0].Items[0] as MatrixCollada).ValuesDouble);
                    Matrix.Multiply(ref outterMatrix, ref innerMatrix, out BindShapeMatrix);
                }
                else
                {
                    BindShapeMatrix = outterMatrix;
                }
            }
        }

        #endregion

        #region Helpers

        private Matrix[] GetMatricesFromValue(Double[] values)
        {
            Matrix[] matrices = new SharpDX.Matrix[values.Length / 16];
            for(int i = 0; i < matrices.Length; i++)
            {
                double[] tmp = new double[16];
                for(int j = 0; j < 16; j++)
                    tmp[j] = values[i * 16 + j];
                matrices[i] = Matrix_T.New(tmp);
            }
            return matrices;
        }

        private String TextureToRelativePath(String fullPath)
        {
            return fullPath.Split(new string[] { @"/Content/Textures/" }, StringSplitOptions.None).Last().Replace("/", @"\");
        }

        private void InitMaterialEffectParameters(object effect)
        {
            if(effect is effectFx_profile_abstractProfile_COMMONTechniqueBlinn effectBlinn)
            {
                InitMaterialEffectParameters(effectBlinn);
                if(effectBlinn.specular.Item is common_color_or_texture_typeColor specularColor)
                {
                    var values = (effectBlinn.specular.Item as common_color_or_texture_typeColor).Values;
                    Material.Specular = new Vector4((float)values[0], (float)values[1], (float)values[2], (float)values[3]);
                } else if(effectBlinn.specular.Item is common_color_or_texture_typeTexture specularTexture)
                {
                    //TODO: Add support for specular texture
                    Material.Specular = new Vector4(0, 0, 0, 1);
                }
                Material.SpecularPower = (float)(effectBlinn.shininess.Item as common_float_or_param_typeFloat).Value;
                return;
            }
            if(effect is effectFx_profile_abstractProfile_COMMONTechniqueLambert effectLambert)
            {
                InitMaterialEffectParameters(effectLambert);
                return;
            }
            if (effect is effectFx_profile_abstractProfile_COMMONTechniquePhong effectPhong)
            {
                InitMaterialEffectParameters(effectPhong);
                var values = (effectPhong.specular.Item as common_color_or_texture_typeColor).Values;
                Material.Specular = new Vector4((float)values[0], (float)values[1], (float)values[2], (float)values[3]);
                Material.SpecularPower = (float)(effectPhong.shininess.Item as common_float_or_param_typeFloat).Value;
                return;
            }
        }

        private void InitMaterialEffectParameters(effectFx_profile_abstractProfile_COMMONTechniqueLambert effect)
        {
            var values = (effect.emission.Item as common_color_or_texture_typeColor).Values;
            Material.Emissive = new Vector4((float)values[0], (float)values[1], (float)values[2], (float)values[3]);

            values = (effect.ambient.Item as common_color_or_texture_typeColor).Values;
            Material.Ambient = new Vector4((float)values[0], (float)values[1], (float)values[2], (float)values[3]);

            Material.Diffuse = new Vector4(1.0f);
        }

        #endregion
    }

    public class GeometryPart
    {
        public Vertex[] Vertices;
        public ushort[] Indices;
        public SkinningVertex[] Skinning;
        public Dictionary<StringId, ModelBone> Bones;
        public StringId GeometryId;
        public Matrix BindShapeMatrix;
        public uint VertexCount;
        public uint StartVertex;

        public GeometryPart(StringId name, UInt64 count, Boolean isSkinned = false)
        {
            GeometryId = name;
            Vertices = new Vertex[count];
            Indices = new UInt16[count];
            if (isSkinned)
            {
                Skinning = new SkinningVertex[count];
            }
        }
    }

    public class GeometryControllerPart
    {
        private StringId[] _skinJointNameArray;

        public StringId Id;
        public SkinningVertex[] VertexSkinningArray;
        public Dictionary<StringId, ModelBone> BoneDictionary;

        public Matrix BindShapeMatrix { get; set; }
        public StringId[] SkinJointNameArray
        {
            get { return _skinJointNameArray; }
            set
            {
                _skinJointNameArray = value;
                BoneDictionary = new Dictionary<StringId, ModelBone>(_skinJointNameArray.Length, StringId.Comparer);
            }
        }
    }
}
