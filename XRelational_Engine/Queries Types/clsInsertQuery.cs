using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XRelational_DataAccess;

namespace XRelational_Engine.Queries_Types
{
    internal class clsInsertQuery : clsQuery
    {



        private Dictionary<string, object> _InsertRowData;

        

        












        protected override AstNode CreateAST(List<string> tokens)
        {
            //setting the keyword (select , add...etc) as a root
            AstNode root = new AstNode(TokenType.EntryPoint, tokens[0].ToLower());



            Stack<AstNode> nodeStack = new Stack<AstNode>();
            nodeStack.Push(root);
            foreach (string token in tokens)
            {
                if (token == tokens[0])
                    continue;

            
                if (token.ToLower() == "into")
                {
                    AstNode newNode = new AstNode(TokenType.TableNames_SubTree,
                       TokenType.TableNames_SubTree.ToString());

                    root.Children.Add(newNode);


                    //setting the table tree for the next token that should be the table name
                    nodeStack.Push(newNode);                    
                }
                else if (nodeStack.Peek().Type == TokenType.TableNames_SubTree)
                {
                    //adding the table name to the root
                    AstNode newNode = new AstNode(TokenType.TableName, token);
                    nodeStack.Peek().Children.Add(newNode);

                    //popping the TableNames_SubTree node from the stack
                    nodeStack.Pop();


                    //starting the Columns Names sub tree
                    if (nodeStack.Peek().Type != TokenType.COLUMNS_VALUES)
                    {
                        AstNode ColumnNamesNode = new AstNode(TokenType.COLUMNS_NAMES, TokenType.COLUMNS_NAMES.ToString());
                        root.Children.Add(ColumnNamesNode);
                        nodeStack.Push(ColumnNamesNode);
                    }




                }
                //else if (token == "(")
                //{

                //    if (nodeStack.Peek().Type != TokenType.COLUMNS_VALUES)
                //    {
                //        AstNode newNode = new AstNode(TokenType.COLUMNS_NAMES, token);
                //        root.Children.Add(newNode);
                //        nodeStack.Push(newNode);
                //    }
                   
                //}
                //else if (token == ")")
                //{
                //    //popping the last opened node might be (ColumnNameToAdd, ColumnValToAdd)
                //    while (nodeStack.Peek().Type != TokenType.EntryPoint)
                //    {
                //        if (nodeStack.Count == 0)
                //            break;

                //        nodeStack.Pop();
                //    }

                //}
                else if (token.ToLower() == "values")
                {
                    //popping the last opened node might be (ColumnNameToAdd, ColumnValToAdd)
                    while (nodeStack.Peek().Type != TokenType.EntryPoint)
                    {
                        if (nodeStack.Count == 0)
                            break;

                        nodeStack.Pop();
                    }


                    AstNode newNode = new AstNode(TokenType.COLUMNS_VALUES, token);
                    root.Children.Add(newNode);
                    nodeStack.Push(newNode);
                }

                //adding columns names
                else if (nodeStack.Peek().Type == TokenType.COLUMNS_NAMES)
                {
                    AstNode newNode = new AstNode(TokenType.ColumnNameToAdd, token);
                    nodeStack.Peek().Children.Add(newNode);
                }
                //adidng columns values
                else if (nodeStack.Peek().Type == TokenType.COLUMNS_VALUES)
                {
                    AstNode newNode = new AstNode(TokenType.ColumnValToAdd, token);
                    nodeStack.Peek().Children.Add(newNode);
                }
                else if (token == ",")
                {
                    continue;
                }


                /////BUG--->>> FIX THE ' THING IN COLUMN VALUES AS U ADD THEM


                //end point IMPLEMENTATION
            }
                return root;
        }

        protected override void SetQueryData(List<string> PureTokens, string DbName)
        {
            if (PureTokens.Count == 0 || string.IsNullOrEmpty(DbName))
                return;

            base.DatabaseName = DbName;



            CreateAST(PureTokens);
        }







        //public methods

        public override DataTable Execute(List<string> PureTokens, string DbName)
        {

            SetQueryData(PureTokens, DbName);

            //validating the table name

            //if (!clsTableDA.DoesTableExist(TableName, DbName))
            //    throw new clsExceptions.TableNameDoesNotExistException(TableName);

            //ValidateColumnsNames();


            return null;
            //return _GenerateTargetTable();
        }

        //
    }
}
