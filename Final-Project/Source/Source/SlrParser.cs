

using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Script.Serialization;

class SlrParser
{
	private delegate void SlrFunction(int state);
	private Dictionary<Tuple<int, StateSymbol.Type>, Tuple<SlrFunction, int>> srTable;
	private Stack<StateSymbol> stateStack;
	private int currentState;
	Tokenizer t;
	//CodeGenerator cg;

	public SlrParser()
	{
		srTable = new Dictionary<Tuple<int, StateSymbol.Type>, Tuple<SlrFunction, int>>()
		{
			//PROG0 = 0
			{Tuple.Create(0,StateSymbol.Type.Begin), Tuple.Create<SlrFunction,int>(Shift, 1)},
			
			//S0 = 1
			//{Tuple.Create(1,StateSymbol.Type.If), Tuple.Create<SlrFunction,int>(Shift, 4)},
			{Tuple.Create(1,StateSymbol.Type.Goto), Tuple.Create<SlrFunction,int>(Shift, 3)},
			{Tuple.Create(1,StateSymbol.Type.Id), Tuple.Create<SlrFunction,int>(Shift, 2)},

			//I1=2
			{Tuple.Create(2,StateSymbol.Type.Colon), Tuple.Create<SlrFunction,int>(Shift, 1)},
			{Tuple.Create(2,StateSymbol.Type.Assign), Tuple.Create<SlrFunction,int>(Shift, 4)},
			
			//Goto1=3
			{Tuple.Create(3,StateSymbol.Type.Id), Tuple.Create<SlrFunction,int>(Exec, 2)},
			
			//E0=4
			//reduces
			{Tuple.Create(4,StateSymbol.Type.Int), Tuple.Create<SlrFunction,int>(Reduce, 13)},
			{Tuple.Create(4,StateSymbol.Type.Id), Tuple.Create<SlrFunction,int>(Reduce, 14)},
			//shift
			{Tuple.Create(4,StateSymbol.Type.Exp), Tuple.Create<SlrFunction,int>(Goto, 5)},
			//goto
			{Tuple.Create(4,StateSymbol.Type.L_Paren), Tuple.Create<SlrFunction,int>(Shift, 4)},
			
			//E1=5
			//shifts
			{Tuple.Create(5,StateSymbol.Type.Op), Tuple.Create<SlrFunction,int>(Shift, 4)},
			{Tuple.Create(5,StateSymbol.Type.Rel), Tuple.Create<SlrFunction,int>(Shift, 4)},
			//reduces
			{Tuple.Create(5,StateSymbol.Type.R_Paren), Tuple.Create<SlrFunction,int>(Reduce, )},
			//special reduce 
			{Tuple.Create(5,StateSymbol.Type.EXP), Tuple.Create<SlrFunction,int>(Reduce, )},
			{Tuple.Create(5,StateSymbol.Type.EXP), Tuple.Create<SlrFunction,int>(Reduce, )},
		};
	}

	private void Accept(int state)
	{

	}

	public void Compile(File inputFile, File outputFile)
	{
	}

	private void Reduce(int state)
	{
		switch (state)
		{
			//PROG -> BEGIN <STL> END
			case 0:
				Console.WriteLine("PROG-> BEGIN <STL> END");
				if (stateStack.Pop().type != StateSymbol.Type.End)
					throw new SystemException("END not found");

				if (stateStack.Peek().type != StateSymbol.Type.STL)
					throw new SystemException("<STL> not found");

				AstNode root = stateStack.Pop().production;

				if (stateStack.Pop().type != StateSymbol.Type.Begin)
					throw new SystemException("<Begin> not found");

				stateStack.Push(new StateSymbol(StateSymbol.Type.PROG, (AstNode)null));
				break;
			//STL -> <ST>
			case 1:
				Console.WriteLine("STL -> <ST>");

				if (stateStack.Pop().type != StateSymbol.Type.ST)
					throw new SystemException("<ST> not found");

				stateStack.Push(new StateSymbol(StateSymbol.Type.STL, (AstNode)null));

				break;
			//STL -> <STL> ; <ST>
			case 2:
				Console.WriteLine("STL -> <STL> ; <ST>");
				break;
			//ST -> LST
			case 3:
				Console.WriteLine("ST -> LST");
				break;
			//ST -> ULST
			case 4:
				Console.WriteLine("ST -> ULST");
				break;
			//LST -> id : <ULST>
			case 5:
				Console.WriteLine("LST -> id : <ULST>");
				break;
			//ULST -> <ASSIGN>
			case 6:
				Console.WriteLine("ULST -> <ASSIGN>");
				break;
			//ULST -> <GOTO>
			case 7:
				Console.WriteLine("ULST -> <GOTO>");
				break;
			//ULST -> <CD>
			case 8:
				Console.WriteLine("ULST -> <CD>");
				break;
			//ASSIGN -> id _ <EXP>
			case 9:
				Console.WriteLine("ASSIGN -> id _ <EXP>");
				break;
			//GOTO -> GOTO id
			case 10:
				Console.WriteLine("GOTO -> GOTO id");
				break;
			//CD -> IF <BOOL> THEN <UCDST>
			case 11:
				Console.WriteLine("CD -> IF <BOOL> THEN <UCDST>");
				break;
			//BOOL -> <EXP> Rel <EXP>
			case 12:
				Console.WriteLine("BOOL -> <EXP> Rel <EXP>");
				break;
			//EXP -> int
			case 13:
				Console.WriteLine("EXP -> int");
				break;
			//EXP -> id
			case 14:
				Console.WriteLine("EXP -> id");
				break;
			//EXP -> ( <EXP> op <EXP> )
			case 15:
				Console.WriteLine("EXP -> ( <EXP> op <EXP> )");
				break;
			//UCDST -> <ASSIGN>
			case 16:
				Console.WriteLine("UCDST -> <ASSIGN>");
				break;
			//UCDST -> <GOTO>
			case 17:
				Console.WriteLine("UCDST -> <GOTO>");
				break;
			default:
				Console.WriteLine("Reduce what?");
				break;
		}
	}

	private void Shift(int state)
	{
		stateStack.Push(t.currentSymbol);
		currentState = state;
	}

    public static int Main()
    {
		
		return 0;
    }


}
