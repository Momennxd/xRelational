using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XRelational_DataAccess
{
    public class clsDatabaseDA
    {



        public class st_DB_InfoDA
        {
            public string DatabaseName;
         
            public List<clsTableDA.st_Table_InfoDA> Tables;

            public List<clsRelationDA.st_Relation_InfoDA> Relations;

            public clsTableGlobDA.st_TableGlob_InfoDA TableGlobInfo;

            public st_DB_InfoDA()
            {
                DatabaseName = "";
                Tables = new List<clsTableDA.st_Table_InfoDA> ();
                Relations = new List<clsRelationDA.st_Relation_InfoDA>();
                TableGlobInfo = new clsTableGlobDA.st_TableGlob_InfoDA();
            }

        }

       














    }
}
