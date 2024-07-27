using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XRelational_DataAccess
{
    public class clsRelationDA
    {
        public struct st_Relation_InfoDA
        {
            public int RelationID { get; set; }

            public string PK_TableName { get; set; }

            public string FK_TableName { get; set; }


            public string PK_ColumnName { get; set; }

            public string FK_ColumnName { get; set; }


            public string RelationName { get; set; }
        }

    }
}
