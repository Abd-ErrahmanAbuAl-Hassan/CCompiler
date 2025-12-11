# C Compiler Parser Project

This project implements a **parser** for a simplified C compiler. It is built on top of a custom scanner (lexer) and follows a structured recursive‑descent parsing approach.

## 📌 Project Overview

The parser reads C source code and verifies whether it follows the defined grammar rules. It also generates a parse tree or structured representation of the source program.

The project consists of:

* A **scanner** that tokenizes input.
* A **parser** that checks syntax according to grammar.
* A **set of grammar rules** for declarations, functions, statements, and expressions.
* Proper **error handling** for invalid syntax.

---

## 📁 Project Structure

```
CCompiler/
├── Scanner/            # Tokenizer (existing)
├── Parser/
│   ├── Parsing.cs       # Recursive-descent parser
│   ├── AST/            # Abstract Syntax Tree nodes
│   └── Errors.cs       # Error reporting & recovery
```

---

## 🔤 Grammar (Simplified)

Below is the grammar used by the parser:

### Program Structure

```
Program → PreMainDecl* MainFunc PostMainDecl*
```

### Main Function Constraint

```
MainFunc → "int" "main" "(" ")" CompoundStmt
```

If the `main` function does not exist → **syntax error**.

### Declarations

```
PreMainDecl → VarDecl | FuncProto | FuncDef
PostMainDecl → VarDecl | FuncDef
```

### Variable Declaration

```
VarDecl → Type IDENT ( '=' Expr )? ';'
```

### Function Prototype

```
FuncProto → Type IDENT '(' ParamList? ')' ';'
```

### Function Definition

```
FuncDef → Type IDENT '(' ParamList? ')' CompoundStmt
```

### Parameters

```
ParamList → Param (',' Param)*
Param → Type IDENT
```

### Statements

```
Stmt → ExprStmt | IfStmt | WhileStmt | ReturnStmt | CompoundStmt
```

---

## 🧱 Parser Features

### ✔ Recursive Descent

Each rule in the grammar corresponds to a parser function.

### ✔ Error Detection

Example: missing `;`, wrong parameter list, missing `main`, etc.

### ✔ Error Recovery

Skips to safe tokens (`;`, `}`, etc.) to continue parsing.

### ✔ Operator Precedence

Handles arithmetic expressions with correct precedence:

```
Term → Factor (('*' | '/') Factor)*
Expr → Term (('+' | '-') Term)*
```

---

## 🧪 Example Input

```c
int global_counter = 0;

void increment_counter(int amount);

int main() {
    increment_counter(5);
    return 0;
}

void increment_counter(int amount) {
    global_counter += amount;
}
```

---

## ⚠ Error Example

Invalid code:

```c
int x // = 5;
= 5;
```

Parser output:

```
Syntax Error: unexpected '=' at line ...
```

---

## ▶ Running the Parser

```bash
dotnet run myprogram.c
```

The parser will print:

* Pre-main declarations
* Main function block
* Post-main declarations
* Syntax errors (if any)

---

## 🚀 Future Improvements

* Add full C expression grammar
* Support arrays & pointers
* Add symbol table
* Add type checking

---

## 📄 License

MIT (or your choice)
