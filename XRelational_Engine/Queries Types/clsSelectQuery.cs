using Microsoft.SqlServer.TransactSql.ScriptDom;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XRelational_DataAccess;
using static XRelational_Engine.clsGlobal_Engine;

namespace XRelational_Engine.Queries_Types
{
    internal class clsSelectQuery : clsQuery
    {

        private class _clsSelectQueryData
        {
            //classes
            public class clsSelectedColumns
            {
                public HashSet<string> Columns { get; set; }

                public bool SelectAll { get;  set; }

                public bool AddColumnToSelect(string ColumnName)
                {
                    return Columns.Add(ColumnName);
                }

                public clsSelectedColumns()
                {
                    Columns = new HashSet<string>();
                    this.SelectAll = false;
                }
            }       

            public class clsSearchColumn
            {
             
                public string ColumnName { get; set; }

                public HashSet<string> Values { get; set; }

                public TokenType ValType {  get; set; }            

                public clsSearchColumn(string ColumnName, bool MustMatch)
                {

                    this.ColumnName = ColumnName;
                    Values = new HashSet<string>();
                }

            }

            public class clsConditionalChunck
            {
                public List<clsSearchColumn> ConditionalColumns { get; set; }

                public TokenType ConditionalChunckType { get; set; }


                public void AddConditionalColumn(clsSearchColumn searchColumn)
                {
                    ConditionalColumns.Add(searchColumn);
                }

                public clsConditionalChunck(TokenType ConditionalChunckType)
                {
                    ConditionalColumns = new List<clsSearchColumn>();
                    this.ConditionalChunckType = ConditionalChunckType;
                }

                public clsConditionalChunck()
                {
                    ConditionalColumns = new List<clsSearchColumn>();
                }

            }

            //




            //instances

            public List<clsConditionalChunck> lstConditionalChuncks { get; set; }

            public clsSelectedColumns selectedColumns { get; set; }
            //



            //constructer
            public _clsSelectQueryData()
            {
                lstConditionalChuncks = new List<clsConditionalChunck>();
                selectedColumns = new clsSelectedColumns();
            }
        }


        private _clsSelectQueryData selectQueryData = new _clsSelectQueryData();

       







        //private methods


        private bool _RecordMatch(DataRow row)
        {
            if (row == null)
                return false;

            bool MainState = true, CurrentChunckState = true;
            for (int i = 0; i < selectQueryData.lstConditionalChuncks.Count; i++)
            {

                _clsSelectQueryData.clsSearchColumn CurrentColumn =
                    selectQueryData.lstConditionalChuncks[i].ConditionalColumns[0];

                if (string.IsNullOrEmpty(CurrentColumn.ColumnName))
                    continue;



                dynamic tableColumnVal = row[CurrentColumn.ColumnName];
                dynamic ConditionVal;

                if (CurrentColumn.Values.Count > 0)
                {
                    ConditionVal = CurrentColumn.Values.ElementAt(0);
                } 

                else
                {
                    ConditionVal = " ";
                }


                if (CurrentColumn.ValType != TokenType.EqualValue &&
                    CurrentColumn.ValType != TokenType.NotEqualValue)
                {
                    //converting the table column value to int based on the first letter of its string
                    if (!int.TryParse(tableColumnVal, out int output))
                    {
                        tableColumnVal = Convert.ToInt32(row[CurrentColumn.ColumnName].ToString().ElementAt(0));
                    }
                    else
                    {
                        tableColumnVal = output;
                    }


                    //converting the coditional column value to int based on the first letter of its string
                    if (!int.TryParse(ConditionVal, out int outConditonVal))
                    {
                        if (!string.IsNullOrWhiteSpace(ConditionVal))                       
                        {
                            ConditionVal = Convert.ToInt32(CurrentColumn.Values.ElementAt(0).
                                ToString().ElementAt(0));
                        }
                    }
                    else
                    {
                        ConditionVal = outConditonVal;
                    }
                }



                //try if the column name does not exist
                try
                {
                    switch (CurrentColumn.ValType)
                    {
                        case TokenType.EqualValue:
                            {
                                if (CurrentColumn.Values.Contains(row[CurrentColumn.ColumnName].ToString()))
                                    CurrentChunckState = true;
                                else
                                    CurrentChunckState = false;

                                break;
                            }
                        case TokenType.NotEqualValue:
                            {
                                if (!CurrentColumn.Values.Contains(row[CurrentColumn.ColumnName].ToString()))
                                    CurrentChunckState = true;
                                else
                                    CurrentChunckState = false;

                                break;
                            }
                        case TokenType.GreaterThanValue:
                            {
                                if (tableColumnVal > ConditionVal)

                                    CurrentChunckState = true;
                                else
                                    CurrentChunckState = false;

                                break;
                            }

                        case TokenType.LessThanValue:
                            {
                                if (tableColumnVal < ConditionVal)
                                    CurrentChunckState = true;
                                else
                                    CurrentChunckState = false;

                                break;
                            }

                        case TokenType.GreaterOrEqualThanValue:
                            {
                                if (tableColumnVal >= ConditionVal)
                                    CurrentChunckState = true;
                                else
                                    CurrentChunckState = false;

                                break;
                            }

                        case TokenType.LessOrEqualThanValue:
                            {
                                if (tableColumnVal <= ConditionVal)
                                    CurrentChunckState = true;
                                else
                                    CurrentChunckState = false;

                                break;
                            }

                        default:
                            {
                                CurrentChunckState = false;
                                break;
                            }
                    }

                }
                catch
                {
                    return false;
                }






                if (i == 0)
                {
                    MainState = CurrentChunckState;
                    continue;
                }


                if (selectQueryData.lstConditionalChuncks[i].ConditionalChunckType == 
                    TokenType.andConditionChunck)
                {
                    MainState = MainState && CurrentChunckState;
                }

                else
                {
                    MainState = MainState || CurrentChunckState;
                }
                


            }



            return MainState;
        }


        private DataTable _GetMatchRows(string DataPagePath)
        {

            if (string.IsNullOrEmpty(DataPagePath))
                return null;

            DataTable RawRecords = clsXML.XmlToDataTable(DataPagePath, 1);

            DataTable dtResult = RawRecords.Clone();


            foreach (DataRow Row in RawRecords.Rows)
            {
                DataRow matchRow = dtResult.NewRow();
                if (_RecordMatch(Row))
                {
                    matchRow.ItemArray = Row.ItemArray;
                    dtResult.Rows.Add(matchRow);
                }

            }



            return dtResult;
        }


        private HashSet<long> _GetControlerCiChunckVals(string CI)
        {
            HashSet<long> ControllerCI_Values = new HashSet<long>();
            for (int i = selectQueryData.lstConditionalChuncks.Count - 1; i >= 0; i--)
            {
                _clsSelectQueryData.clsSearchColumn CurrentColumn =
                    selectQueryData.lstConditionalChuncks[i].ConditionalColumns[0];

                _clsSelectQueryData.clsConditionalChunck CurrentChunck =
                  selectQueryData.lstConditionalChuncks[i];

                if (CurrentColumn.ValType != TokenType.EqualValue)
                    continue;

                if (CurrentColumn.ColumnName != CI && CurrentChunck.ConditionalChunckType 
                    != TokenType.andConditionChunck)
                {
                    return ControllerCI_Values;
                }

                if (CurrentColumn.ColumnName != CI && CurrentChunck.ConditionalChunckType
                 != TokenType.orConditionChunck)
                {
                    ControllerCI_Values.Clear();
                    return ControllerCI_Values;
                }

                if (CurrentColumn.ColumnName == CI && CurrentChunck.ConditionalChunckType
                    == TokenType.andConditionChunck)
                {
                    AddSetRange(ref ControllerCI_Values, CurrentColumn.Values);
                    return ControllerCI_Values;
                }

                if (CurrentColumn.ColumnName == CI && CurrentChunck.ConditionalChunckType
                    != TokenType.andConditionChunck)
                {
                    AddSetRange(ref ControllerCI_Values, CurrentColumn.Values);
                }

               

            }


            return ControllerCI_Values;
        }


        private List<string> _GetConditionaltokens(List<string> tokens)
        {

            List<string> ExecludedTokens = new List<string>() { "(", ")" };

            //gettin the conditional tokens
            bool Add = false;
            List<string> Conditionaltokens = new List<string>();
            foreach (string token in tokens)
            {
                if (token.Equals("where", StringComparison.OrdinalIgnoreCase))
                    Add = true;

                if (Add && !ExecludedTokens.Contains(token))
                    Conditionaltokens.Add(token);
            }

            return Conditionaltokens;
        }


        private AstNode _CreateConditionAST(List<string> FullTokens)
        {
            //getting the conditional tokens starting from WHERE keyword
            List<string> Conditionaltokens = _GetConditionaltokens(FullTokens);

            if (Conditionaltokens.Count == 0)
                return null;

            AstNode root = new AstNode(TokenType.Condition_SubTree, Conditionaltokens[0]);

            Stack<AstNode> nodeStack = new Stack<AstNode>();

            nodeStack.Push(root);
            int TokensCounter = 0;
            string AND_OR = "";



            //adding an AndChunck at the beginning                     
            AstNode FirstNode;
            FirstNode = new AstNode(TokenType.andConditionChunck,
                TokenType.andConditionChunck.ToString());
            nodeStack.Peek().Children.Add(FirstNode);
            nodeStack.Push(FirstNode);
            TokenType ColumnValueType = TokenType.EqualValue;


            bool IsValString = false;
            List<string> lstColumnValue = new List<string>();


            foreach (string token in Conditionaltokens)
            {
                if (token == Conditionaltokens[0])
                {
                    TokensCounter++;
                    continue;
                }

                AstNode currentNode = nodeStack.Peek();

                if (token.Equals("and", StringComparison.OrdinalIgnoreCase)
                   || token.Equals("or", StringComparison.OrdinalIgnoreCase))
                {


                    AND_OR = token.ToLower();

                    //popping the last added chuncks till the main one
                    while (nodeStack.Peek().Type != TokenType.Condition_SubTree)
                    {
                        nodeStack.Pop();
                    }

                    //updating the current node
                    currentNode = nodeStack.Peek();

                    //adding a new conditional chunck                                     
                    AstNode newNode;
                    if (AND_OR.Equals("or", StringComparison.OrdinalIgnoreCase))
                    {
                        newNode = new AstNode(TokenType.orConditionChunck,
                        TokenType.orConditionChunck.ToString());
                    }
                    else
                    {
                        newNode = new AstNode(TokenType.andConditionChunck,
                        TokenType.andConditionChunck.ToString());
                    }

                    currentNode.Children.Add(newNode);
                    nodeStack.Push(newNode);


                    //resetting the token type
                    ColumnValueType = TokenType.EqualValue;
                }

                else if (token.Equals("in", StringComparison.OrdinalIgnoreCase))
                {
                    //setting the columns values after 'IN' to 'orConditionalColumnValue'
                    ColumnValueType = TokenType.EqualValue;
                    AND_OR = "or";
                    continue;
                }

                else if (token.Equals(">", StringComparison.OrdinalIgnoreCase))
                {
                    ColumnValueType = TokenType.GreaterThanValue;
                }

                else if (token.Equals("<", StringComparison.OrdinalIgnoreCase))
                {
                    ColumnValueType = TokenType.LessThanValue;
                }

                else if (token.Equals("!", StringComparison.OrdinalIgnoreCase))
                {
                    ColumnValueType = TokenType.NotEqualValue;
                }

                else if (token.Equals("=", StringComparison.OrdinalIgnoreCase))
                {
                    if (ColumnValueType == TokenType.GreaterThanValue)                  
                        ColumnValueType = TokenType.GreaterOrEqualThanValue;

                    else if (ColumnValueType == TokenType.LessThanValue)
                        ColumnValueType = TokenType.LessOrEqualThanValue;

                    else if (ColumnValueType == TokenType.NotEqualValue)
                        ColumnValueType = TokenType.NotEqualValue;

                    else                    
                        ColumnValueType = TokenType.EqualValue;

                    

                }

                else if (token.Equals("'", StringComparison.OrdinalIgnoreCase))
                {
                    if (lstColumnValue.Count != 0)
                    {
                        StringBuilder val = new StringBuilder();
                        lstColumnValue.ForEach(item=> val.Append(item)); 

                        AstNode newNode = new AstNode(ColumnValueType, val.ToString());
                        currentNode.Children.Add(newNode);

                        lstColumnValue.Clear();
                        IsValString = false;
                    }
                    else
                    {
                        IsValString = true;
                    }
                
                }


                //adding the column NAME as a node
                else if (currentNode.Type == TokenType.orConditionChunck
                    || currentNode.Type == TokenType.andConditionChunck)
                {
                    AstNode newNode = new AstNode(AND_OR.Equals("or", StringComparison.OrdinalIgnoreCase) ?
                        TokenType.orConditionalColumnName : TokenType.andConditionalColumnName, token);
                    currentNode.Children.Add(newNode);
                    nodeStack.Push(newNode);
                }

                //adding the column VALUE as a node ('=' Approach)
                else if (currentNode.Type == TokenType.orConditionalColumnName
                   || currentNode.Type == TokenType.andConditionalColumnName)
                {
                    if (!IsValString)
                    {
                        AstNode newNode = new AstNode(ColumnValueType, token);

                        currentNode.Children.Add(newNode);
                    }
                    else
                    {

                       lstColumnValue.Add(token);

                    }




                }

                TokensCounter++;


            }


            return root;


        }


        private void _SetSelectQueryParts(AstNode AST_QueryRoot)
        {

            if (AST_QueryRoot == null)
                return;
     

            if (AST_QueryRoot.Value == "*")
            {
                selectQueryData.selectedColumns.SelectAll = true;
            }

            else if (AST_QueryRoot.Type == TokenType.SelectedColumn)
            {
                selectQueryData.selectedColumns.AddColumnToSelect(AST_QueryRoot.Value);             
            }

            else if (AST_QueryRoot.Type == TokenType.TableName)
            {
                base.TableName = AST_QueryRoot.Value;
            }



            //conditional sub tree


            //adding a conditional chunck
            else if (AST_QueryRoot.Type == TokenType.andConditionChunck ||
                    AST_QueryRoot.Type == TokenType.orConditionChunck)
            {

                //adding a new chunck to the list
                selectQueryData.lstConditionalChuncks.Add(
                    new _clsSelectQueryData.clsConditionalChunck(AST_QueryRoot.Type));

                //adding the chunck to the stack              
                //this._nodeStack.Push(AST_QueryRoot);
            }
            //adding a column NAME
            else if (AST_QueryRoot.Type == TokenType.andConditionalColumnName ||
                    AST_QueryRoot.Type == TokenType.orConditionalColumnName)
            {
             
                //adding the column name in the current conditional chunck
                 selectQueryData.lstConditionalChuncks.Last().AddConditionalColumn(
                    new _clsSelectQueryData.clsSearchColumn(AST_QueryRoot.Value,
                      AST_QueryRoot.Type == TokenType.andConditionalColumnName));
                

                //making the column name last element in the stack
                //this._nodeStack.Push(AST_QueryRoot);


            }
            //adding the column VALUE
            else if (AST_QueryRoot.Type == TokenType.EqualValue ||
                    AST_QueryRoot.Type == TokenType.GreaterThanValue ||
                    AST_QueryRoot.Type == TokenType.LessThanValue ||
                    AST_QueryRoot.Type == TokenType.GreaterOrEqualThanValue ||
                    AST_QueryRoot.Type == TokenType.LessOrEqualThanValue ||
                    AST_QueryRoot.Type == TokenType.NotEqualValue)
            {

                //adding the column value 
                selectQueryData.lstConditionalChuncks.Last().
                    ConditionalColumns.Last().Values.Add(AST_QueryRoot.Value);


                //adding the column val type
                selectQueryData.lstConditionalChuncks.Last().
                  ConditionalColumns.Last().ValType = AST_QueryRoot.Type;
                    
              
            }


            foreach (AstNode child in AST_QueryRoot.Children)
            {            
                _SetSelectQueryParts(child);
            }

        }


        private DataTable _GenerateTargetTable()
        {

            //getting the CIs values that control the result set
            HashSet<long> CI_Values =
                _GetControlerCiChunckVals(clsTable.GetPrimaryKeyColumnName(TableName, DatabaseName));


            //to make sure that the result exist 
            //TRUE if there is at least (one CI path and CI_values is not empty)
            bool ResultExist = CI_Values.Count == 0;


            //getting the CI pathes
            HashSet<string> CI_Paths = new HashSet<string>();
            foreach (long i in CI_Values)
            {
                string Path = clsPaths.GetExistDataPagePath_CI(base.DatabaseName, base.TableName, i);

                if (Path != null)
                {
                    CI_Paths.Add(Path);                   
                    ResultExist = true;                   
                }
            }




            //getting all the data pages to table scan
            if (CI_Paths.Count == 0  && ResultExist) 
            {
                CI_Paths = clsPaths.GetTableDataPagesPaths(DatabaseName, base.TableName).ToHashSet();          
            }


            //getting all the target tables to merge
            List<DataTable> TargetTables = new List<DataTable>();
            foreach (string path in CI_Paths)
            {
                TargetTables.Add(_GetMatchRows(path));
            }



            //getting an empty table if the result set is empty
            if (TargetTables.Count == 0)
            {
                return clsTableDA.GetEmptyTableStruct(TableName, DatabaseName);
            }





            if (!selectQueryData.selectedColumns.SelectAll)
            {
                return clsGlobal_DA.FilterColumns(clsGlobal_DA.UnionDataTables(ref TargetTables),
               selectQueryData.selectedColumns.Columns.ToArray());

            }

            return clsGlobal_DA.UnionDataTables(ref TargetTables);


        }


        private void ValidateColumnsNames()
        {

            List<string> ColumnsNames = clsTableDA.GetColumnsList(base.TableName, base.DatabaseName);
            
            foreach(_clsSelectQueryData.clsConditionalChunck chunck 
                in this.selectQueryData.lstConditionalChuncks)
            {
                foreach(_clsSelectQueryData.clsSearchColumn searchColumn in
                   chunck.ConditionalColumns)
                {
                    if (!ColumnsNames.Contains(searchColumn.ColumnName))
                    {
                        throw new clsExceptions.ColumnNameDoesNotExistException(searchColumn.ColumnName);
                    }
                }
            }

        }

        //










        //override methods
        protected override AstNode CreateAST(List<string> tokens)
        {

            //setting the keyword (select , add...etc) as a root
            AstNode root = new AstNode(TokenType.EntryPoint, tokens[0]);

            Stack<AstNode> nodeStack = new Stack<AstNode>();
            nodeStack.Push(root);
            foreach (string token in tokens)
            {
                if (token == tokens[0])
                    continue;

                AstNode currentNode = nodeStack.Peek();

                if (token == "*")
                {
                    AstNode newNode = new AstNode(TokenType.Keyword, token);
                    currentNode.Children.Add(newNode);
                }
                //setting the root for the selected columns if not a '*'
                else if (token != "*" && token.ToLower() != "where" &&
                    currentNode.Type == TokenType.EntryPoint && token.ToLower() != "from")
                {

                    //creating the SelectedColumns_SubTree
                    AstNode newNode = new AstNode(TokenType.SelectedColumns_SubTree,
                        TokenType.SelectedColumns_SubTree.ToString());

                    //
                    nodeStack.Peek().Children.Add(newNode);
                    nodeStack.Push(newNode);
                    //


                    //pushing the current selected column to the SelectedColumns_SubTree node as a child

                    newNode = new AstNode(TokenType.SelectedColumn, token);
                    nodeStack.Peek().Children.Add(newNode);

                }
                //adding a selected column if exists
                else if (currentNode.Type == TokenType.SelectedColumns_SubTree && token.ToLower() != "from")
                {
                    AstNode newNode = new AstNode(TokenType.SelectedColumn, token);
                    nodeStack.Peek().Children.Add(newNode);
                }
                else if (token.ToLower() == "from")
                {
                    AstNode newNode = new AstNode(TokenType.TableNames_SubTree,
                       TokenType.TableNames_SubTree.ToString());

                    //
                    //popping all the nodes till it reaches the first node (the entry point)
                    while (nodeStack.Peek().Type != TokenType.EntryPoint)
                    {
                        if (nodeStack.Count == 0)
                            break;

                        nodeStack.Pop();
                    }

                    nodeStack.Peek().Children.Add(newNode);
                    nodeStack.Push(newNode);
                    //
                }
                //adding the tables names to the tablesNames_Subtree
                else if (currentNode.Type == TokenType.TableNames_SubTree && token.ToLower() != "where")
                {
                    AstNode newNode = new AstNode(TokenType.TableName, token);
                    nodeStack.Peek().Children.Add(newNode);

                    //breaking the loop because only one table should exist.
                    break;
                }






            }


            AstNode ConditionalNode = _CreateConditionAST(tokens);

            if (ConditionalNode != null)
                root.Children.Add(ConditionalNode);

            return root;
        }


        protected override void SetQueryData(List<string> PureTokens, string DbName)
        {
            if (PureTokens.Count == 0 || string.IsNullOrEmpty(DbName))
                return;

            base.DatabaseName = DbName;

            _SetSelectQueryParts(this.CreateAST(PureTokens));
        }

        //









        //public methods

        public override DataTable Execute(List<string> PureTokens, string DbName)
        {

            SetQueryData(PureTokens, DbName);

            //validating the table name

            if (!clsTableDA.DoesTableExist(TableName, DbName))           
                throw new clsExceptions.TableNameDoesNotExistException(TableName);

            ValidateColumnsNames();


            return _GenerateTargetTable();
        }

        //



    }
}