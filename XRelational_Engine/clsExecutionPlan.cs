#define TEMP_RETURN
//#undef TEMP_RETURN

using System;
using System.Collections.Generic;
using System.Data;
using XRelational_Engine.Queries_Types;

namespace XRelational_Engine
{
    public class clsExecutionPlan
    {


        private static DataTable _Execute(string SyntaxValidatedQuery, string DbName)
        {


            //Converting the (Syntax Validated Query) to a (pure tokens list)
            List<string> PureTokens = clsQuery.RemoveAdditionalParenthisis(
                clsQuery.Tokenize(SyntaxValidatedQuery));


            //getting the Entry point of the query (SELECT, ADD, DELETE, UPDATE...etc)
            clsQueryEntryPoint.enEntryPoints enEntry = clsQueryEntryPoint.GetEntryPoint(PureTokens[0]);



            switch (enEntry)
            {
                case clsQueryEntryPoint.enEntryPoints.eSelect:
                    return new clsSelectQuery().Execute(PureTokens, DbName);

                case clsQueryEntryPoint.enEntryPoints.eInsert:
                    return new clsInsertQuery().Execute(PureTokens, DbName);

                default:
                    break;
            }







#if TEMP_RETURN
            return null;
#endif
        }





        public static DataTable Execute(string RawQuery, string DbName)
        {
            if (string.IsNullOrEmpty(RawQuery))
                return null;

            //validating the syntax
            string ErrorMessage = clsQuery.ValidateSqlSyntax(RawQuery.Trim());
            if (ErrorMessage != null)
                throw new SyntaxErrorException(ErrorMessage);



            return _Execute(RawQuery, DbName);

        }



    }
}