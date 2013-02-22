using System;
using System.Collections.Generic;

class StateSymbol
{
    public enum Type
    {
        //Tokens
        Id,
        Int,
        Assign,
        Eof,
        L_Paren,
        R_Paren,
        Op,
        If,
        Then,
        End,
        Begin,
        Goto,
        Semicolon,
        Colon,
		Rel,
        //Symbols
        PROG,
        STL,
        ST,
        LST,
        ULST,
        GOTO,
        ASSIGN,
        CD,
        EXP,
        BOOL,
        UCDST
    };

	public string value {get; set;}
    public Type type { get; set; }

	public StateSymbol(Type type, string value)
    {
        this.type = type;
        this.value = value;
    }
}
