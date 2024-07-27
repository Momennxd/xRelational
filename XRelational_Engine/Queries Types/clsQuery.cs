using Microsoft.SqlServer.TransactSql.ScriptDom;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace XRelational_Engine
{

    ////CI ---> Clusterd Index
    ///this index applies on the primary key to accomplish a faster search by using data pages
    public abstract class clsQuery
    {


        public string TableName
        {
            get; set;
        }

        public string DatabaseName
        {
            get; set;
        }

        public enum TokenType
        {
            Keyword,
            Number,
            TableName,
            SelectedColumn,
            AddColumn,
            SelectedColumns_SubTree,
            Condition_SubTree,
            EntryPoint,
            TableNames_SubTree,

            andConditionChunck,
            orConditionChunck,

            andConditionalColumnName,
            orConditionalColumnName,

            EqualValue,
            NotEqualValue,
            GreaterThanValue,
            LessThanValue,
            GreaterOrEqualThanValue,
            LessOrEqualThanValue,

            COLUMNS_VALUES,
            COLUMNS_NAMES,
            ColumnNameToAdd,
            ColumnValToAdd,







            EndPoint

        }
   
        public class Token
        {
            public string Value { get; }
            public TokenType Type { get; }

            public Token(string value, TokenType type)
            {
                Value = value;
                Type = type;
            }
        }

        public class AstNode
        {
            public TokenType Type { get; }
            public string Value { get; }
       
            public List<AstNode> Children { get; } = new List<AstNode>();

            public AstNode(TokenType nodeType, string tokenValue = "")
            {
                Type = nodeType;
                Value = tokenValue;
            }
        }



        //abstract methods
        protected abstract AstNode CreateAST(List<string> tokens);

        public abstract DataTable Execute(List<string> PureTokens, string DbName);


        protected abstract void SetQueryData(List<string> PureTokens, string DbName);

        //

       

        //public methods

        public static string ValidateSqlSyntax(string sqlCode)
        {

            // Create a parser for T-SQL code
            TSqlParser parser = new TSql150Parser(true);

            // Parse the SQL code
            IList<ParseError> parseErrors;
            TSqlFragment sqlFragment = parser.Parse(new System.IO.StringReader(sqlCode), out parseErrors);
            StringBuilder ErrorLines = new StringBuilder();
            // Check if any syntax errors occurred
            if (parseErrors.Count > 0)
            {
                foreach (ParseError error in parseErrors)
                {
                    ErrorLines.AppendLine($"Line {error.Line}, Column {error.Column}: {error.Message}");
                }

                return ErrorLines.ToString();
            }

            return null;
        }

        public static List<string> Tokenize(string sourceCode)
        {
            List<string> tokens = new List<string>();

            string pattern = @"\s+|((?<=\W)|(?=\W))(?<!'')""([^""]|'''')*""(?<!'')((?<=\W)|(?=\W))|((?<=\W)|(?=\W))\d+((?<=\W)|(?=\W))|\w+|.";
            MatchCollection matches = Regex.Matches(sourceCode.Trim(), pattern);

            foreach (Match match in matches)
            {
                string value = match.Value.Trim().ToLower();

                if (!(string.IsNullOrEmpty(value) || value == ","  || value == "]"
                    || value == "[" || string.IsNullOrWhiteSpace(value)))
                {
                    tokens.Add(match.Value);
                }

            }

            return tokens;


        }
     
        public static List<string> RemoveAdditionalParenthisis(List<string> tokens)
        {

          
            SortedSet<int> AdditionalParenthisisIndexes = new SortedSet<int>();

            //getting the Additional Parenthisis that don't make any diffrence in the final result
            for (int i = 0; i < tokens.Count; i++)
            {
                if (tokens[i] == "(")
                {
                    bool Delete = true;
                    for (int j = i; j < tokens.Count; j++)
                    {
                        if (tokens[j].Equals("and", StringComparison.OrdinalIgnoreCase) ||
                            tokens[j].Equals("or", StringComparison.OrdinalIgnoreCase))
                        {
                            Delete = false;
                        }

                        if (tokens[j] == ")" && Delete)
                        {


                            if (!(AdditionalParenthisisIndexes.Contains(i) ||
                                     AdditionalParenthisisIndexes.Contains(j)))
                            {
                                AdditionalParenthisisIndexes.Add(i);
                                AdditionalParenthisisIndexes.Add(j);
                            }


                        }
                    }
                }
            }


            //getting the duplicated Parenthisis Indexes in the main tokens list
            if (tokens.Count > 3)
            {
                for (int i = 0; i < tokens.Count; i++)
                {
                    if (i >= tokens.Count)
                        break;

                    if (tokens[i] == "(" &&
                            tokens[i + 1] == "(")
                    {
                        for (int j = i + 2; j < tokens.Count; j++)
                        {
                            if ((j >= tokens.Count || j + 1 >= tokens.Count) ||
                                tokens[j] != "(" && tokens[j + 1] == "(")
                                break;

                            if (tokens[j] == ")" &&
                                    tokens[j + 1] == ")")
                            {

                                if (!(AdditionalParenthisisIndexes.Contains(i) ||
                                    AdditionalParenthisisIndexes.Contains(j)))
                                {
                                    AdditionalParenthisisIndexes.Add(i);
                                    AdditionalParenthisisIndexes.Add(j);
                                }
                              

                            }

                        }
                    }

                }
            }



            //removing the Additional and Duplicated Parenthisis Indexes
            for (int i = AdditionalParenthisisIndexes.Count - 1; i >= 0; i--)
            {
                tokens.RemoveAt(AdditionalParenthisisIndexes.ElementAt(i));
            }

            return tokens;
        }

        //
    }
}
