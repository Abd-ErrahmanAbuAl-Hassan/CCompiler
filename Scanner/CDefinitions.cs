namespace Scanner
{
    public static class CDefinitions
    {
        public static readonly HashSet<string> Keywords = new()
        {
            // Data types
            "auto", "bool", "char", "double", "float", "int", "long", "short",
            "signed", "unsigned", "void",
            
            // Storage classes
            "restrict", "static",
            
            // Control flow
            "break", "case", "continue", "default", "do", "else",
            "for", "goto", "if", "return", "switch",
            "union", "while",
            
            // Special
            "const", "main"
        };

        // Multi-char operators must be ordered by length (longest first)
        public static readonly List<string> MultiCharOperators = new()
        {
            // Assignment operators
            "<<=", ">>=", "+=", "-=", "*=", "/=", "%=", "&=", "|=", "^=", "=",
            
            // Increment/Decrement
            "++", "--",
            
            // Bitwise shift
            "<<", ">>",
            
            // Relational
            "<=", ">=", "==", "!=",
            
            // Logical
            "&&", "||",
            
        };

        public static readonly HashSet<char> SingleCharOperators = new()
        {
            '+', '-', '*', '/', '%','&', '|', '^', '~', '!', '<', '>', '=','.', ','
        };

        public static readonly HashSet<char> Delimiters = new()
        {
            ';', '(', ')', '{', '}', '[', ']'
        };
    }
}