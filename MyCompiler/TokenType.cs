namespace MyCompiler.LexicalAnalyzer
{
    public enum TokenType
    {

        Unknown = 0,
        Error = -1,
        EndOfFile = -2,

        KeywordWhile = 1,

        Identifier = 2,

        DelimiterLParen = 4,      
        DelimiterRParen = 5,      
        DelimiterSemicolon = 6,    
        DelimiterLBrace = 7,       
        DelimiterRBrace = 8,       

        IntegerNumber = 9,

        OperatorIncrement = 10,
        OperatorPlus = 11,
        OperatorDecrement = 13,
        OperatorMinus = 14,
        OperatorSpace = 15,
        OperatorLessEqual = 16,
        OperatorLess = 17,
        OperatorGreaterEqual = 18,
        OperatorGreater = 19,
        OperatorNewLine = 20,
        OperatorTab = 21,
        OperatorEqual = 22,
        OperatorNotEqual = 23,
        OperatorLogicalAnd = 24,
        OperatorLogicalOr = 25
    }
}