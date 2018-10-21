using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace TPresenter.Input
{
    public class KeyHasher
    {
        public List<Keys> Keys = new List<Keys>();

        SHA256 hashser = SHA256.Create();
        byte[] tmpHashData = new byte[256];


    }
}
