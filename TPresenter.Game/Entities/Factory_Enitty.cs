using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using TPresenter.Filesystem;
using TPresenter.Game.Builders;
using TPresenter.Game.Interfaces;
using TPresenter.Serialization;

namespace TPresenter.Game.Entities
{
    public static class Factory_Enitty
    {
        private static FactoryEntityBase<EntityBuilderTypeAttribute> _entityFactory;
        private static Dictionary<StringId, Builder_Entity> _buildersByEntityId = new Dictionary<StringId, Builder_Entity>(StringId.Comparer);

        static Factory_Enitty()
        {
            _entityFactory = new FactoryEntityBase<EntityBuilderTypeAttribute>();
            _entityFactory.RegisterAssemly(Assembly.GetAssembly(typeof(Factory_Enitty)));
            LoadEnityBuilders();
        }

        public static T CreateInstance<T>() where T : Entity
        {
            return _entityFactory.CreateObject<T>();
        }

        public static Entity CreateInstance(StringId id)
        {
            var builder = _buildersByEntityId[id];
            return _entityFactory.CreateObject(builder.GetType());
        }

        //TODO: Do we need to return copy of the builder here?
        public static Builder_Entity GetBuilder(StringId id)
        {
            //return _buildersByEntityId[id].Copy();
            return _buildersByEntityId[id];
        }

        private static void LoadEnityBuilders()
        {
            foreach (Type type in _entityFactory.GetRegisteredBuilderTypes())
            {
                var method = type.GetRuntimeMethod("LoadBuilderDefinitions", new Type[] { });
                if(method != null)
                {
                    _buildersByEntityId.AddRange((Dictionary<StringId, Builder_Entity>)method.Invoke(null, new object[] { }));
                }
            }
        }
    }
}
