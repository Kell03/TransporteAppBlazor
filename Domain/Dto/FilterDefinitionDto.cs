using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Dto
{
    public class FilterDefinitionDto
    {
        public string PropertyName { get; set; }
        public string Operator { get; set; }
        public object Value { get; set; }
        public string Title { get; set; }
    }
}
