using System;
using System.Collections.Generic;
using System.IO;

class SrParser
{
	private delegate void SrFunction(int state);
	private Dictionary<Tuple<int, StateSymbol.Type>, Tuple<SrFunction, int> > srTable;
	private Stack<StateSymbol> symbolStack;
    private Stack<int> stateStack;
	Tokenizer t;
	CodeGenerator cg;
    private bool keepParsing = false;

	public SrParser()
	{
        symbolStack = new Stack<StateSymbol>();
        stateStack = new Stack<int>();
        cg = new CodeGenerator();

		srTable = new Dictionary<Tuple<int, StateSymbol.Type>, Tuple<SrFunction, int>>()
		{
            //State 0
            {Tuple.Create(0,StateSymbol.Type.Begin), Tuple.Create<SrFunction,int>(Shift, 2)},
            {Tuple.Create(0,StateSymbol.Type.PROG), Tuple.Create<SrFunction,int>(GotoState, 1)},

            //State 1
            {Tuple.Create(1,StateSymbol.Type.Eof), Tuple.Create<SrFunction,int>(Accept, 0)},

            //State 2
            {Tuple.Create(2,StateSymbol.Type.Id), Tuple.Create<SrFunction,int>(Shift, 31)},

            {Tuple.Create(2,StateSymbol.Type.STL), Tuple.Create<SrFunction,int>(GotoState, 3)},
            {Tuple.Create(2,StateSymbol.Type.ST), Tuple.Create<SrFunction,int>(GotoState, 7)},
            {Tuple.Create(2,StateSymbol.Type.LST), Tuple.Create<SrFunction,int>(GotoState, 8)},
            {Tuple.Create(2,StateSymbol.Type.ULST), Tuple.Create<SrFunction,int>(GotoState, 9)},
            {Tuple.Create(2,StateSymbol.Type.ASSIGN), Tuple.Create<SrFunction,int>(GotoState, 10)},
            {Tuple.Create(2,StateSymbol.Type.GOTO), Tuple.Create<SrFunction,int>(GotoState, 11)},
            {Tuple.Create(2,StateSymbol.Type.CD), Tuple.Create<SrFunction,int>(GotoState, 12)},

            //State 3
            {Tuple.Create(3,StateSymbol.Type.End), Tuple.Create<SrFunction,int>(Shift, 4)},
            {Tuple.Create(3,StateSymbol.Type.Semicolon), Tuple.Create<SrFunction,int>(Shift, 5)},

            //State 4
            {Tuple.Create(4,StateSymbol.Type.Eof), Tuple.Create<SrFunction,int>(Reduce, 0)},

            //State 5
            {Tuple.Create(5,StateSymbol.Type.Goto), Tuple.Create<SrFunction,int>(Shift, 13)},
            {Tuple.Create(5,StateSymbol.Type.If), Tuple.Create<SrFunction,int>(Shift, 15)},
            {Tuple.Create(5,StateSymbol.Type.Id), Tuple.Create<SrFunction,int>(Shift, 31)},

            {Tuple.Create(5,StateSymbol.Type.ST), Tuple.Create<SrFunction,int>(GotoState, 6)},
            {Tuple.Create(5,StateSymbol.Type.LST), Tuple.Create<SrFunction,int>(GotoState, 8)},
            {Tuple.Create(5,StateSymbol.Type.ULST), Tuple.Create<SrFunction,int>(GotoState, 9)},
            {Tuple.Create(5,StateSymbol.Type.ASSIGN), Tuple.Create<SrFunction,int>(GotoState, 10)},
            {Tuple.Create(5,StateSymbol.Type.GOTO), Tuple.Create<SrFunction,int>(GotoState, 11)},
            {Tuple.Create(5,StateSymbol.Type.CD), Tuple.Create<SrFunction,int>(GotoState, 12)},

            //State 6
            {Tuple.Create(6,StateSymbol.Type.Semicolon), Tuple.Create<SrFunction,int>(Reduce, 2)},
            {Tuple.Create(6,StateSymbol.Type.End), Tuple.Create<SrFunction,int>(Reduce, 2)},

            //State 7
            {Tuple.Create(7,StateSymbol.Type.Semicolon), Tuple.Create<SrFunction,int>(Reduce, 1)},
            {Tuple.Create(7,StateSymbol.Type.End), Tuple.Create<SrFunction,int>(Reduce, 1)},

            //State 8
            {Tuple.Create(8,StateSymbol.Type.Semicolon), Tuple.Create<SrFunction,int>(Reduce, 3)},
            {Tuple.Create(8,StateSymbol.Type.End), Tuple.Create<SrFunction,int>(Reduce, 3)},

            //State 9
            {Tuple.Create(9,StateSymbol.Type.Semicolon), Tuple.Create<SrFunction,int>(Reduce, 4)},
            {Tuple.Create(9,StateSymbol.Type.End), Tuple.Create<SrFunction,int>(Reduce, 4)},

            //State 10
            {Tuple.Create(10,StateSymbol.Type.Semicolon), Tuple.Create<SrFunction,int>(Reduce, 6)},
            {Tuple.Create(10,StateSymbol.Type.End), Tuple.Create<SrFunction,int>(Reduce, 6)},

            //State 11
            {Tuple.Create(11,StateSymbol.Type.Semicolon), Tuple.Create<SrFunction,int>(Reduce, 7)},
            {Tuple.Create(11,StateSymbol.Type.End), Tuple.Create<SrFunction,int>(Reduce, 7)},

            //State 12
            {Tuple.Create(12,StateSymbol.Type.Semicolon), Tuple.Create<SrFunction,int>(Reduce, 8)},
            {Tuple.Create(12,StateSymbol.Type.End), Tuple.Create<SrFunction,int>(Reduce, 8)},

            //State 13
            {Tuple.Create(13,StateSymbol.Type.Id), Tuple.Create<SrFunction,int>(Shift, 14)},

            //State 14
            {Tuple.Create(14,StateSymbol.Type.Semicolon), Tuple.Create<SrFunction,int>(Reduce, 10)},
            {Tuple.Create(14,StateSymbol.Type.End), Tuple.Create<SrFunction,int>(Reduce, 10)},

            //State 15
            {Tuple.Create(15,StateSymbol.Type.Id), Tuple.Create<SrFunction,int>(Shift, 23)},
            {Tuple.Create(15,StateSymbol.Type.Int), Tuple.Create<SrFunction,int>(Shift, 22)},
            {Tuple.Create(15,StateSymbol.Type.L_Paren), Tuple.Create<SrFunction,int>(Shift, 24)},
            {Tuple.Create(15,StateSymbol.Type.BOOL), Tuple.Create<SrFunction,int>(GotoState, 16)},
            {Tuple.Create(15,StateSymbol.Type.EXP), Tuple.Create<SrFunction,int>(GotoState, 19)},

            //State 16
            {Tuple.Create(16,StateSymbol.Type.Then), Tuple.Create<SrFunction,int>(Shift, 17)},

            //State 17
            {Tuple.Create(17,StateSymbol.Type.Id), Tuple.Create<SrFunction,int>(Shift, 36)},
            {Tuple.Create(17,StateSymbol.Type.Goto), Tuple.Create<SrFunction,int>(Shift, 13)},

            {Tuple.Create(17,StateSymbol.Type.UCDST), Tuple.Create<SrFunction,int>(GotoState, 18)},
			{Tuple.Create(17,StateSymbol.Type.ASSIGN), Tuple.Create<SrFunction,int>(GotoState, 29)},
            {Tuple.Create(17,StateSymbol.Type.GOTO), Tuple.Create<SrFunction,int>(GotoState, 30)},

            //State 18
			//reduce
			{Tuple.Create(18,StateSymbol.Type.End), Tuple.Create<SrFunction,int>(Reduce, 11)},
			{Tuple.Create(18,StateSymbol.Type.Semicolon), Tuple.Create<SrFunction,int>(Reduce, 11)},

			//State 19
			//shift
			{Tuple.Create(19,StateSymbol.Type.Rel), Tuple.Create<SrFunction,int>(Shift, 20)},

			//State 20
			//shift
			{Tuple.Create(20,StateSymbol.Type.Id), Tuple.Create<SrFunction,int>(Shift, 23)},
			{Tuple.Create(20,StateSymbol.Type.Int), Tuple.Create<SrFunction,int>(Shift, 22)},
			{Tuple.Create(20,StateSymbol.Type.L_Paren), Tuple.Create<SrFunction,int>(Shift, 24)},
			//goto
			{Tuple.Create(20,StateSymbol.Type.EXP), Tuple.Create<SrFunction,int>(GotoState, 21)},

			//State 21
			//reduce
			{Tuple.Create(21,StateSymbol.Type.Then), Tuple.Create<SrFunction,int>(Reduce, 12)},

            //State 22
            {Tuple.Create(22,StateSymbol.Type.End), Tuple.Create<SrFunction,int>(Reduce, 13)},
            {Tuple.Create(22,StateSymbol.Type.Then), Tuple.Create<SrFunction,int>(Reduce, 13)},
            {Tuple.Create(22,StateSymbol.Type.Rel), Tuple.Create<SrFunction,int>(Reduce, 13)},
            {Tuple.Create(22,StateSymbol.Type.Op), Tuple.Create<SrFunction,int>(Reduce, 13)},
            {Tuple.Create(22,StateSymbol.Type.Semicolon), Tuple.Create<SrFunction,int>(Reduce, 13)},
            {Tuple.Create(22,StateSymbol.Type.R_Paren), Tuple.Create<SrFunction,int>(Reduce, 13)},

            //State 23
            {Tuple.Create(23,StateSymbol.Type.End), Tuple.Create<SrFunction,int>(Reduce, 14)},
            {Tuple.Create(23,StateSymbol.Type.Then), Tuple.Create<SrFunction,int>(Reduce, 14)},
            {Tuple.Create(23,StateSymbol.Type.Rel), Tuple.Create<SrFunction,int>(Reduce, 14)},
            {Tuple.Create(23,StateSymbol.Type.Op), Tuple.Create<SrFunction,int>(Reduce, 14)},
            {Tuple.Create(23,StateSymbol.Type.Semicolon), Tuple.Create<SrFunction,int>(Reduce, 14)},
            {Tuple.Create(23,StateSymbol.Type.R_Paren), Tuple.Create<SrFunction,int>(Reduce, 14)},

            //State 24
            {Tuple.Create(24,StateSymbol.Type.Id), Tuple.Create<SrFunction,int>(Shift, 23)},
            {Tuple.Create(24,StateSymbol.Type.Int), Tuple.Create<SrFunction,int>(Shift, 22)},
            {Tuple.Create(24,StateSymbol.Type.L_Paren), Tuple.Create<SrFunction,int>(Shift, 24)},
            {Tuple.Create(24,StateSymbol.Type.EXP), Tuple.Create<SrFunction,int>(GotoState, 25)},

            //State 25
            {Tuple.Create(25,StateSymbol.Type.Op), Tuple.Create<SrFunction,int>(Shift, 26)},

            //State 26
            {Tuple.Create(26,StateSymbol.Type.Id), Tuple.Create<SrFunction,int>(Shift, 23)},
            {Tuple.Create(26,StateSymbol.Type.Int), Tuple.Create<SrFunction,int>(Shift, 22)},
            {Tuple.Create(26,StateSymbol.Type.L_Paren), Tuple.Create<SrFunction,int>(Shift, 24)},
            {Tuple.Create(26,StateSymbol.Type.EXP), Tuple.Create<SrFunction,int>(GotoState, 27)},

            //State 27
            {Tuple.Create(27,StateSymbol.Type.R_Paren), Tuple.Create<SrFunction,int>(Shift, 28)},

            //State 28
			//reduce
			{Tuple.Create(28,StateSymbol.Type.End), Tuple.Create<SrFunction,int>(Reduce, 15)},
			{Tuple.Create(28,StateSymbol.Type.Then), Tuple.Create<SrFunction,int>(Reduce, 15)},
			{Tuple.Create(28,StateSymbol.Type.Rel), Tuple.Create<SrFunction,int>(Reduce, 15)},
			{Tuple.Create(28,StateSymbol.Type.Semicolon), Tuple.Create<SrFunction,int>(Reduce, 15)},
			{Tuple.Create(28,StateSymbol.Type.R_Paren), Tuple.Create<SrFunction,int>(Reduce, 15)},
			{Tuple.Create(28,StateSymbol.Type.Op), Tuple.Create<SrFunction,int>(Reduce, 15)},

			//State 29
			//reduce
			{Tuple.Create(29,StateSymbol.Type.End), Tuple.Create<SrFunction,int>(Reduce, 16)},
			{Tuple.Create(29,StateSymbol.Type.Semicolon), Tuple.Create<SrFunction,int>(Reduce, 16)},

			//State 30
			//reduce
			{Tuple.Create(30,StateSymbol.Type.End), Tuple.Create<SrFunction,int>(Reduce, 17)},
			{Tuple.Create(30,StateSymbol.Type.Semicolon), Tuple.Create<SrFunction,int>(Reduce, 17)},

			//State 31
			//shift
			{Tuple.Create(31,StateSymbol.Type.Assign), Tuple.Create<SrFunction,int>(Shift, 32)},
			{Tuple.Create(31,StateSymbol.Type.Colon), Tuple.Create<SrFunction,int>(Shift, 34)},

			//State 32
			//shift
			{Tuple.Create(32,StateSymbol.Type.Id), Tuple.Create<SrFunction,int>(Shift, 23)},
			{Tuple.Create(32,StateSymbol.Type.Int), Tuple.Create<SrFunction,int>(Shift, 22)},
			{Tuple.Create(32,StateSymbol.Type.L_Paren), Tuple.Create<SrFunction,int>(Shift, 24)},
			//goto
			{Tuple.Create(32,StateSymbol.Type.EXP), Tuple.Create<SrFunction,int>(GotoState, 33)},

			//State 33
			//reduce
			{Tuple.Create(33,StateSymbol.Type.End), Tuple.Create<SrFunction,int>(Reduce, 9)},
			{Tuple.Create(33,StateSymbol.Type.Semicolon), Tuple.Create<SrFunction,int>(Reduce, 9)},

			//State 34
			//shift
			{Tuple.Create(34,StateSymbol.Type.Id), Tuple.Create<SrFunction,int>(Shift, 36)},
			{Tuple.Create(34,StateSymbol.Type.Goto), Tuple.Create<SrFunction,int>(Shift, 13)},
			{Tuple.Create(34,StateSymbol.Type.If), Tuple.Create<SrFunction,int>(Shift, 15)},
			//goto
			{Tuple.Create(34,StateSymbol.Type.ULST), Tuple.Create<SrFunction,int>(GotoState, 35)},
			{Tuple.Create(34,StateSymbol.Type.ASSIGN), Tuple.Create<SrFunction,int>(GotoState, 10)},
			{Tuple.Create(34,StateSymbol.Type.GOTO), Tuple.Create<SrFunction,int>(GotoState, 11)},
			{Tuple.Create(34,StateSymbol.Type.CD), Tuple.Create<SrFunction,int>(GotoState, 12)},

			//State 35
			//reduce
			{Tuple.Create(35,StateSymbol.Type.End), Tuple.Create<SrFunction,int>(Reduce, 5)},
			{Tuple.Create(35,StateSymbol.Type.Semicolon), Tuple.Create<SrFunction,int>(Reduce, 5)},

			//State 36
			//shift
			{Tuple.Create(36,StateSymbol.Type.Assign), Tuple.Create<SrFunction,int>(Shift, 32)},
		};
	}

	private void Accept(int state)
	{
        Console.WriteLine("Winning");
        keepParsing = false;
	}

	public void Compile(TextReader input)
	{
        keepParsing = true;
        t = new Tokenizer(input);
        stateStack.Push(0);

        while (keepParsing)
        {
            Tuple<int, StateSymbol.Type> lookup = Tuple.Create(stateStack.Peek(), t.currentSymbol.type);
            if (!srTable.ContainsKey(lookup))
            {
                keepParsing = false;
                Console.WriteLine("Didn't parse table completely");
            }
            else
            {
                srTable[lookup].Item1(srTable[lookup].Item2);
            }
        }

        symbolStack.Peek().astNode.CodeGenCalvin(cg);
        cg.OutputTest();

	}

    private void PopStates(int count)
    {
        for(; count != 0; count--)
		{
			stateStack.Pop();
		}
    }

	private void Reduce(int state)
	{
        IAstNode leftNode = null;
        IAstNode rightNode = null;
        string op;

		switch (state)
		{
            //PROG -> BEGIN <STL> END 
			case 0:
				Console.WriteLine("PROG-> BEGIN <STL> END");
				if (symbolStack.Pop().type != StateSymbol.Type.End)
					throw new SystemException("END not found");

				if (symbolStack.Peek().type != StateSymbol.Type.STL)
					throw new SystemException("<STL> not found");

                IAstNode stlAst = symbolStack.Pop().astNode;

                if (stlAst.Right != null)
                    stlAst = new StlNode(stlAst, (IAstNode)null);

				if (symbolStack.Pop().type != StateSymbol.Type.Begin)
					throw new SystemException("<Begin> not found");
			
				PopStates(3);
				symbolStack.Push(new StateSymbol(StateSymbol.Type.PROG,stlAst));
				break;
			//STL -> <ST>
			case 1:
				Console.WriteLine("STL -> <ST>");

				if (symbolStack.Peek().type != StateSymbol.Type.ST)
					throw new SystemException("<ST> not found");
			
				PopStates(1);
				symbolStack.Push(new StateSymbol(StateSymbol.Type.STL, new StlNode(symbolStack.Pop().astNode, null)));
				break;
			//STL -> <STL> ; <ST>
			case 2:
				Console.WriteLine("STL -> <STL> ; <ST>");

                if (symbolStack.Peek().type != StateSymbol.Type.ST)
                    throw new SystemException("<ST> not found");

                rightNode = symbolStack.Pop().astNode;

                if (symbolStack.Pop().type != StateSymbol.Type.Semicolon)
                    throw new SystemException("; not found");

                if (symbolStack.Peek().type != StateSymbol.Type.STL)
                    throw new SystemException("<STL> not found");

                leftNode = symbolStack.Pop().astNode;
		
                if (leftNode.Right == null)
                    leftNode = leftNode.Left;

                PopStates (3);
                symbolStack.Push(new StateSymbol(StateSymbol.Type.STL, 
                                 new StlNode(leftNode, rightNode)));
               break;
            //ST -> LST
            case 3:
                Console.WriteLine("ST -> LST");

                if(symbolStack.Peek().type != StateSymbol.Type.LST)
                    throw new SystemException("<LST> not found");
			
				PopStates(1);
                symbolStack.Push(new StateSymbol(StateSymbol.Type.ST, symbolStack.Pop().astNode));
                break;
            //ST -> ULST
            case 4:
                Console.WriteLine("ST -> ULST");

                if (symbolStack.Peek().type != StateSymbol.Type.ULST)
                    throw new SystemException("<ULST> not found");
			
				PopStates(1);
                symbolStack.Push(new StateSymbol(StateSymbol.Type.ST, symbolStack.Pop().astNode));
                break;
            //LST -> id : <ULST>
            case 5:
                Console.WriteLine("LST -> id : <ULST>");

                if (symbolStack.Pop().type != StateSymbol.Type.ULST )
                    throw new SystemException("<ULST> not found");

                if (symbolStack.Pop().type != StateSymbol.Type.Colon)
                    throw new SystemException(": not found");

                if (symbolStack.Pop().type != StateSymbol.Type.Id)
                    throw new SystemException("Id not found");

                PopStates(3);
                symbolStack.Push(new StateSymbol(StateSymbol.Type.LST, (IAstNode)null));

                break;
            //ULST -> <ASSIGN>
            case 6:
                Console.WriteLine("ULST -> <ASSIGN>");

                if (symbolStack.Peek().type != StateSymbol.Type.ASSIGN)
                    throw new SystemException("<ASSIGN> not found");
			
				PopStates(1);
                symbolStack.Push(new StateSymbol(StateSymbol.Type.ULST, symbolStack.Pop().astNode));
                break;
            //ULST -> <GOTO>
            case 7:
                Console.WriteLine("ULST -> <GOTO>");

                if (symbolStack.Peek().type != StateSymbol.Type.GOTO)
                    throw new SystemException("<GOTO> not found");

                PopStates(1);
                symbolStack.Push(new StateSymbol(StateSymbol.Type.ULST, symbolStack.Pop().astNode));
                break;
            //ULST -> <CD>
            case 8:
                Console.WriteLine("ULST -> <CD>");

                if (symbolStack.Peek().type != StateSymbol.Type.CD)
                    throw new SystemException("<CD> not found");

                PopStates(1);
                symbolStack.Push(new StateSymbol(StateSymbol.Type.ULST, symbolStack.Pop().astNode));

                break;
            //ASSIGN -> id _ <EXP>
            case 9:
                Console.WriteLine("ASSIGN -> id _ <EXP>");

                if(symbolStack.Peek().type != StateSymbol.Type.EXP)
                    throw new SystemException("<EXP> not found");

                IAstNode astNode = symbolStack.Pop().astNode;

                if (symbolStack.Pop().type != StateSymbol.Type.Assign)
                    throw new SystemException("Assign not found");

                if (symbolStack.Peek().type != StateSymbol.Type.Id)
                    throw new SystemException("Id not found");

                string id = symbolStack.Pop().value;
			
				PopStates(3);
                symbolStack.Push(new StateSymbol(StateSymbol.Type.ASSIGN,
                                 new AssignNode(id, astNode)));

                break;
            //GOTO -> GOTO id
            case 10:
                Console.WriteLine("GOTO -> GOTO id");

                if (symbolStack.Pop().type != StateSymbol.Type.Id)
                    throw new SystemException("id not found");

                if (symbolStack.Pop().type != StateSymbol.Type.Goto)
                    throw new SystemException("GOTO not found");

                PopStates(2);
                symbolStack.Push(new StateSymbol(StateSymbol.Type.GOTO, (IAstNode)null));
                break;
            //CD -> IF <BOOL> THEN <UCDST>
            case 11:
                IAstNode rel = null;
                IAstNode ucdst = null;
                Console.WriteLine("CD -> IF <BOOL> THEN <UCDST>");

                if (symbolStack.Peek().type != StateSymbol.Type.UCDST)
                    throw new SystemException("<UCDST> not found");

                ucdst = symbolStack.Pop().astNode;

                if (symbolStack.Pop().type != StateSymbol.Type.Then)
                    throw new SystemException("THEN not found");

                if (symbolStack.Peek().type != StateSymbol.Type.BOOL)
                    throw new SystemException("<BOOL> not found");

                rel = symbolStack.Pop().astNode;

                if (symbolStack.Pop().type != StateSymbol.Type.If)
                    throw new SystemException("IF not found");

                PopStates(4);
                symbolStack.Push(new StateSymbol(StateSymbol.Type.CD, new IfNode(rel, ucdst)));
                break;
            //BOOL -> <EXP> Rel <EXP>
            case 12:
                Console.WriteLine("BOOL -> <EXP> Rel <EXP>");

                if (symbolStack.Peek().type != StateSymbol.Type.EXP)
                    throw new SystemException("<EXP> not found");

                rightNode = symbolStack.Pop().astNode;

                if (symbolStack.Peek().type != StateSymbol.Type.Rel)
                    throw new SystemException("Rel not found");

                op = symbolStack.Pop().value;

                if (symbolStack.Peek().type != StateSymbol.Type.EXP)
                    throw new SystemException("<EXP> not found");

                leftNode = symbolStack.Pop().astNode;

                PopStates(3);
                symbolStack.Push(new StateSymbol(StateSymbol.Type.BOOL, new OpNode(leftNode, op, rightNode)));
                
                break;
            //EXP -> int
            case 13:
                Console.WriteLine("EXP -> int");

                if (symbolStack.Peek().type != StateSymbol.Type.Int)
                    throw new SystemException("Int not found");
			
				PopStates(1);
                symbolStack.Push(new StateSymbol(StateSymbol.Type.EXP,new LoadNode(symbolStack.Pop())));
                break;
            //EXP -> id
            case 14:
                Console.WriteLine("EXP -> id");

                if (symbolStack.Peek().type != StateSymbol.Type.Id)
                    throw new SystemException("Id not found");
			
				PopStates(1);
                symbolStack.Push(new StateSymbol(StateSymbol.Type.EXP, new LoadNode(symbolStack.Pop())));
                break;
            //EXP -> ( <EXP> op <EXP> )
            case 15:
                IAstNode leftExp = null;
                IAstNode rightExp = null;

                Console.WriteLine("EXP -> ( <EXP> op <EXP> )");

                if (symbolStack.Pop().type != StateSymbol.Type.R_Paren)
                    throw new SystemException(") not found");

                if (symbolStack.Peek().type != StateSymbol.Type.EXP)
                    throw new SystemException("<EXP> not found");

                rightExp = symbolStack.Pop().astNode;

                if (symbolStack.Peek().type != StateSymbol.Type.Op)
                    throw new SystemException("Op not found");

                op = symbolStack.Pop().value;

                if (symbolStack.Peek().type != StateSymbol.Type.EXP)
                    throw new SystemException("<EXP> not found");

                leftExp = symbolStack.Pop().astNode;

                if (symbolStack.Pop().type != StateSymbol.Type.L_Paren)
                    throw new SystemException("( not found");
			
				PopStates(5);
                symbolStack.Push(new StateSymbol(StateSymbol.Type.EXP, new OpNode(leftExp, op, rightExp)));

                break;
            //UCDST -> <ASSIGN>
            case 16:
                Console.WriteLine("UCDST -> <ASSIGN>");

                if (symbolStack.Peek().type != StateSymbol.Type.ASSIGN)
                    throw new SystemException("<ASSIGN> not found");
			
				PopStates(1);
                symbolStack.Push(new StateSymbol(StateSymbol.Type.UCDST, symbolStack.Pop().astNode));

                break;
            //UCDST -> <GOTO>
            case 17:
                Console.WriteLine("UCDST -> <GOTO>");

                if (symbolStack.Peek().type != StateSymbol.Type.GOTO)
                    throw new SystemException("<UCDST> not found");
			
				PopStates(1);
                symbolStack.Push(new StateSymbol(StateSymbol.Type.UCDST, symbolStack.Pop().astNode));

                break;
            default:
                throw new SystemException("Reduce what?");
		}
		
		//Goto the next symbol
		Tuple<int,StateSymbol.Type> lookup = Tuple.Create(stateStack.Peek(),symbolStack.Peek().type);
		if(srTable.ContainsKey(lookup))
		{
			srTable[lookup].Item1(srTable[lookup].Item2);
		}
		else
			throw new SystemException("No Idea what state to goto");
	}

	private void Shift(int state)
	{
		symbolStack.Push(t.currentSymbol);
		stateStack.Push(state);
        t.GetNextSymbol();
	}

    private void GotoState(int state)
    {
        stateStack.Push(state);
    }

    public static void Main()
    {
        Console.Write("Enter Path to Tokenize:");
        string path = Console.ReadLine();
        SrParser sp = new SrParser();
		
		try {
			using(StreamReader sr = new StreamReader(path))
			{
	        	sp.Compile(new StreamReader(path));
			}
		}
		catch(SystemException se)
		{
			Console.WriteLine ("Fatal Error: " + se.Message);
		}
		
        Console.ReadLine();
    }


}
