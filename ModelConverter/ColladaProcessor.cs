using Collada141;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ModelConverter
{
    public class ColladaProcessor
    {
        private const String Prefix = "[Fixed]_";

        public static Boolean OverwriteFile { get; set; }

        public static async Task<ColladaProcessorResult> ProcessColladaFile(String modelPath)
        {
            //return await Task.Factory.StartNew(
            //    () => ProcessColladaFileInternal(modelPath), 
            //    CancellationToken.None, 
            //    TaskCreationOptions.None, 
            //    TaskScheduler.FromCurrentSynchronizationContext());
            return await Task.Run(() => ProcessColladaFileInternal(modelPath));
        }

        private static ColladaProcessorResult ProcessColladaFileInternal(String modelPath)
        {
            try
            {
                COLLADA model = COLLADA.Load(modelPath);
                var skeletonParts = UpdateSkeleton(model);
                if(skeletonParts == null)
                {
                    return new ColladaProcessorResult(modelPath, "No skeleton information found!");
                }
                UpdateControllers(model, skeletonParts);
                if (OverwriteFile)
                {
                    model.Save(modelPath);
                }
                else
                {
                    model.Save(ChangeModelName(modelPath));
                }
                return new ColladaProcessorResult(modelPath);
            }
            catch (Exception)
            {

                throw;
            }
        }

        private static Dictionary<String, String> UpdateSkeleton(COLLADA model)
        {
            Dictionary<String, String> skeletonParts = new Dictionary<string, string>();

            var visualScene = COLLADA.FindEntryByType<library_visual_scenes>(model).visual_scene[0];
            //First node in OPENCOllada file is EnvironmentAmbientLight. Skipping.
            if(visualScene.node.Length == 1)
            {
                return null;
            }
            Node skeletonNodes = visualScene.node[1];
            ProcessNode(skeletonNodes, skeletonParts);

            var environmentAmbientLightNode = visualScene.node[0];
            visualScene.node = new Node[1];
            visualScene.node[0] = environmentAmbientLightNode;

            return skeletonParts;
        }

        private static void UpdateControllers(COLLADA model, Dictionary<String, String> skeletonParts)
        {
            var skin = (COLLADA.FindEntryByType<library_controllers>(model).controller[0].Item as Skin).source[0];
            var joints = skin.Item as Name_array;
            for(UInt64 ind = 0; ind < joints.count; ind++)
            {
                joints.Values[ind] = skeletonParts[joints.Values[ind]];
            }
        }

        private static void ProcessNode(Node node, Dictionary<String, String> skeletonParts)
        {
            if (!String.IsNullOrEmpty(node.sid))
            {
                skeletonParts.Add(node.sid, node.id);
            }
            if(node.node1 != null)
            {
                foreach(Node subnode in node.node1)
                {
                    ProcessNode(subnode, skeletonParts);
                }
            }
        }

        private static String ChangeModelName(String modelPath)
        {
            var array = modelPath.Split(new String[] { @"\" }, StringSplitOptions.None);
            array[array.Length - 1] = Prefix + array[array.Length - 1];
            return String.Join(@"\", array);
        }
    }

    public class ColladaProcessorResult : IResult
    {
        public Boolean IsSuccess { get; set; }
        public String ModelPath { get; set; }
        public String Message { get; set; } 
        public Exception Exception { get; set; }

        public List<IResult> InternalResults { get; }

        public ColladaProcessorResult(String modelPath, Boolean result = true)
        {
            IsSuccess = result;
            ModelPath = modelPath;
            InternalResults = new List<IResult>();
        }

        public ColladaProcessorResult(String modelPath, String message) : this(modelPath, false)
        {
            Message = message;
        }

        public ColladaProcessorResult(String modelPath, Exception exception) : this(modelPath, false)
        {
            Message = exception.Message;
            Exception = exception;
        }
    }
}
