using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestFirebirdBlobs
{
    [Table("MEMO")]
    public class MemoDB
    {            
        public Int32? ID { get; set; }
        public String MEMO { get; set; }       
    }

}
