using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelConverter
{
    public interface IResult
    {
        Boolean IsSuccess { get; set; }
        String ModelPath { get; set; }
        String Message { get; set; }
        Exception Exception { get; set; }
        List<IResult> InternalResults { get; }
    }
}
