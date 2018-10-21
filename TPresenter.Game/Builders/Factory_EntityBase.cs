using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using TPresenter.Filesystem;
using TPresenter.Game.Actor;
using TPresenter.Game.Builders;
using TPresenter.Game.Entities;
using TPresenter.Game.Interfaces;
using TPresenter.Serialization;

namespace TPresenter.Game.Builders
{
    public class FactoryEntityBase<TAttribute> where TAttribute: FactoryBaseAttribute
    {
        private static Dictionary<Type, TAttribute> _attributeByBuilderType = new Dictionary<Type, TAttribute>();
        private static Dictionary<Type, TAttribute> _attributeByObjectType = new Dictionary<Type, TAttribute>();

        public T CreateObject<T>() where T : Entity
        {
            return Activator.CreateInstance(typeof(T)) as T;
        }

        public Entity CreateObject(Type type)
        {
            return Activator.CreateInstance(_attributeByBuilderType[type].EntityType) as Entity;
        }

        public void RegisterAssemly(Assembly assembly)
        {
            foreach(Type type in assembly.GetTypes())
            {
                foreach(TAttribute attribute in type.GetCustomAttributes(typeof(TAttribute)))
                {
                    attribute.EntityType = type;
                    _attributeByBuilderType.Add(attribute.BuilderType, attribute);
                    _attributeByObjectType.Add(type, attribute);
                }
            }
        }

        public IEnumerable<Type> GetRegisteredBuilderTypes()
        {
            return _attributeByBuilderType.Keys.AsEnumerable();
        }

        public Type GetBuilderType(Type objectType)
        {
            return _attributeByObjectType[objectType].BuilderType;
        }

        public Type GetEntityType(Type builderType)
        {
            return _attributeByBuilderType[builderType].EntityType;
        }
    }
}
