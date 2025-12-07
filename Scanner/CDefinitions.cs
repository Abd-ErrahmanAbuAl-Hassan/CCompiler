namespace Scanner
{
    public static class CDefinitions
    {
        public static readonly HashSet<string> Keywords = new()
        {
            // Data types
            "auto", "bool", "char", "double", "float", "int", "long", "short",
            "signed", "unsigned", "void", "_Bool", "_Complex", "_Imaginary",
            
            // Storage classes
            "extern", "inline", "register", "restrict", "static", "typedef",
            
            // Control flow
            "break", "case", "continue", "default", "do", "else", "enum",
            "for", "goto", "if", "return", "sizeof", "struct", "switch",
            "union", "while",
            
            // Special
            "const", "volatile", "main"
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
            
            // Member access
            "->"
        };

        public static readonly HashSet<char> SingleCharOperators = new()
        {
            '+', '-', '*', '/', '%',
            '&', '|', '^', '~', '!',
            '<', '>', '=', '?', ':',
            '.', ','
        };

        public static readonly HashSet<char> Delimiters = new()
        {
            ';', '(', ')', '{', '}', '[', ']'
        };
    }
}