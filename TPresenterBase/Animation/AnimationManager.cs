using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPresenter.Render.Animation
{
    public static class AnimationManager
    {
        static Dictionary<StringId, MyAnimation> animations = new Dictionary<StringId, MyAnimation>();

        public static MyAnimation GetOrLoadAnimation(StringId name)
        {
            if (animations.ContainsKey(name))
                return animations[name];

            MyAnimation anim = new MyAnimation();
            anim.Load(name);
            animations[name] = anim;
            return anim;
        }

        public static bool IsLoaded(StringId name)
        {
            return animations.ContainsKey(name) && animations[name] != null;
        }
    }
}
