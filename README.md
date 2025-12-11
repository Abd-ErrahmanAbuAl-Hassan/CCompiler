# C Parser Project -- Scanner + Recursive-Descent Parser + AST

This project implements a **C-language front-end** consisting of:

1.  **Scanner (Lexer)** -- converts raw source code into a stream of
    tokens.\
2.  **Recursive-Descent Parser** -- reads tokens and builds an
    **Abstract Syntax Tree (AST)**.\
3.  **AST Node System** -- represents declarations, statements,
    expressions, and the whole program.

The system handles **declarations**, **variables**, **functions**,
**statements**, **expressions**, **control flow**, and enforces that a
valid program must contain exactly **one `main` function\`.**

------------------------------------------------------------------------

## 1. Project Overview

This project is the foundational front-end of a small C compiler.\
Its purpose is to:

-   Convert raw C source code → tokens → AST\
-   Detect syntax errors with accurate `(line, column)` positions\
-   Validate the program structure\
-   Distinguish **pre-main declarations**, the **main() function**, and
    **post-main declarations**\
-   Build a clean, high-level AST representation suitable for later
    compilation stages

------------------------------------------------------------------------

## 2. Project Structure

    /Scanner
        Scanning.cs
        Token.cs
        CDefinitions.cs

    /Parser
        Parsing.cs
        ParserCore.cs
        ParseException.cs
        StatementParser.cs
        ExpressionParser.cs
        DeclarationParser.cs
        /Nodes
            AstNode.cs
            Declaration Nodes
            Statement Nodes
            Expression Nodes
            ProgramNode.cs

------------------------------------------------------------------------

## 3. Scanner (Lexer) Overview

The scanner performs a full lexical analysis of the input. It is
responsible for:

### ✔ Tokenizing:

-   Keywords
-   Identifiers
-   Numbers
-   Strings
-   Character literals
-   Operators
-   Delimiters
-   Comments

### ✔ Tracking positions

### ✔ Error detection

### ✔ Appending EOF token

------------------------------------------------------------------------

## 4. AST Node System

The AST contains: - Declarations\
- Statements\
- Expressions\
- Program structure (PreMain, Main, PostMain)

------------------------------------------------------------------------

## 5. Parser Overview

-   Recursive-descent parser\
-   Lookahead\
-   LL(1)-style grammar\
-   Expression precedence\
-   Function parsing\
-   Statement parsing\
-   Full support for blocks `{}`

------------------------------------------------------------------------

## 6. Parsing Workflow

1.  Initialize parser\
2.  Parse pre-main declarations\
3.  Detect & require `main()`\
4.  Parse post-main declarations\
5.  Parse statements\
6.  Parse expressions\
7.  Return AST

------------------------------------------------------------------------

## 7. Error Handling

Scanner: non-fatal errors\
Parser: throws ParseException on syntax errors

------------------------------------------------------------------------

## 8. Example Flow

Input:

``` c
int x = 5;
int main() { x = x + 1; return x; }
```

Goes through: Scanner → Tokens\
Parser → AST
