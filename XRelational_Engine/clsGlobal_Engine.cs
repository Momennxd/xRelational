using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XRelational_DataAccess;
using System.Data;

namespace XRelational_Engine
{
    public class clsGlobal_Engine
    {

        public static void SWAP(ref int n1, ref int n2)
        {
            int temp = n1;
            n1 = n2;
            n2 = temp;
        }

        public static void GetTableName(clsQuery.AstNode AST_QueryRoot, string DbName, ref string TableName)
        {
            
            if (AST_QueryRoot.Type == clsQuery.TokenType.TableName)
            {
                if (!clsTableDA.DoesTableExist(AST_QueryRoot.Value, DbName))
                    throw new SyntaxErrorException(AST_QueryRoot.Value);
                else
                {
                    TableName = AST_QueryRoot.Value;
                    return;
                }
            }

            foreach (clsQuery.AstNode child in AST_QueryRoot.Children)
            {
                GetTableName(child, DbName, ref TableName);
            }


        }

        public static void AstTreeToString(clsQuery.AstNode AST_QueryRoot, string[] ExcludedStrings, ref StringBuilder TreeString)
        {
            if (!ExcludedStrings.Contains(AST_QueryRoot.Value))
            {
                TreeString.Append(AST_QueryRoot.Value).Append(" ");
            }

            foreach (clsQuery.AstNode child in AST_QueryRoot.Children)
            {
                AstTreeToString(child, ExcludedStrings, ref TreeString);
            }
        }




        /// <summary>
        /// adds a set of strings to a set of int by parsing
        /// </summary>
        /// <param name="MainSet"></param>
        /// <param name="setToAdd"></param>
        public static void AddSetRange(ref HashSet<long> MainSet,HashSet<string> setToAdd)
        {
            foreach(string t in setToAdd)
            {
                MainSet.Add(long.Parse(t));
            }
        }


        public  static void InsertionSort(ref int[] arr)
        {


            for (int i = 0; i < arr.Length; i++)
            {

                for (int j = i; j >= 1; j--)
                {
                    if (arr[j] < arr[j - 1])
                    {
                        SWAP(ref arr[j - 1], ref arr[j]);

                    }
                }


            }


        }



    }
}
