using Models.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Models.Classes
{
    [Serializable]
    public class Command
    {

        // Properties

        public int Id { get; set; }
        public Car Car { get; set; }
        public HttpMethods Method { get; set; }
    }
}
