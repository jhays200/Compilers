using System;
using System.Collections.Generic;

class StateSymbol
{
    public enum Type
    {
        //Tokens
        Id=0,
		Int = 1,
		Assign = 2,
		Eof = 3,
		L_Paren = 4,
		R_Paren = 5,
		Op = 6,
		If = 7,
		Then = 8,
		End = 9,
		Begin = 10,
		Goto = 11,
		Semicolon = 12,
		Colon = 13,
		Rel = 14,
        //Symbols
		PROG = 15,
		STL = 16,
		ST = 17,
		LST = 18,
		ULST = 19,
		GOTO = 20,
		ASSIGN = 21,
		CD = 22,
		EXP = 23,
		BOOL = 24,
		UCDST = 25
    };

	public string value {get; set;}
    public Type type { get; set; }

	public StateSymbol(Type type)
	{
		this.type = type;
		this.value = String.Empty;
		//production = node;
	}

	public StateSymbol(Type type, string value)
    {
        this.type = type;
        this.value = value;
        //production = null;
    }
}
