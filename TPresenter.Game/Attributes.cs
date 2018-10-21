using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPresenter.Game
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class FactoryBaseAttribute : Attribute
    {
        public readonly Type BuilderType;
        public Type EntityType;

        public FactoryBaseAttribute(Type entutyBuilderType)
        {
            BuilderType = entutyBuilderType;
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class EntityBuilderTypeAttribute : FactoryBaseAttribute
    {
        public EntityBuilderTypeAttribute(Type entutyBuilderType) : base(entutyBuilderType)
        {
        }
    }
}
