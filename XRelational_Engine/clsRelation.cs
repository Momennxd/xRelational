using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XRelational_Engine
{
    public class clsRelation
    {

        public int ID { get; set; }

        public string PK_TableName { get; set; }

        public string FK_TableName { get; set; }


        public string PK_ColumnName { get; set; }

        public string FK_ColumnName { get; set; }


        public string RelationName { get; set; }

        public clsRelation(int ID, string PK_TableName, string FK_TableName, string PK_ColumnName,
            string FK_ColumnName, string RelationName) 
        { 
            this.ID = ID;

            this.PK_TableName = PK_TableName;
            this.FK_TableName = FK_TableName;

            this.PK_ColumnName = PK_ColumnName;
            this.FK_ColumnName = FK_ColumnName;

            this.RelationName = RelationName;
        }







    }
}
