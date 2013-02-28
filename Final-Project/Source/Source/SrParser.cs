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
	//CodeGenerator cg;
    private bool keepParsing = false;

	public SrParser()
	{
        symbolStack = new Stack<StateSymbol>();
        stateStack = new Stack<int>();

		srTable = new Dictionary<Tuple<int, StateSymbol.Type>, Tuple<SrFunction, int>>()
		{
            /*
			//PROG0 = 0
			{Tuple.Create(0,StateSymbol.Type.Begin), Tuple.Create<SrFunction,int>(Shift, 1)},
			
			//S0 = 1
			//{Tuple.Create(1,StateSymbol.Type.If), Tuple.Create<SrFunction,int>(Shift, 4)},
			//{Tuple.Create(1,StateSymbol.Type.Goto), Tuple.Create<SrFunction,int>(Shift, 3)},
			{Tuple.Create(1,StateSymbol.Type.Id), Tuple.Create<SrFunction,int>(Shift, 2)},

			//I1=2
			//{Tuple.Create(2,StateSymbol.Type.Colon), Tuple.Create<SrFunction,int>(Shift, 1)},
			{Tuple.Create(2,StateSymbol.Type.Assign), Tuple.Create<SrFunction,int>(Shift, 4)},
			
			//Goto1=3
			//{Tuple.Create(3,StateSymbol.Type.Id), Tuple.Create<SrFunction,int>(Exec, 2)},
			
			//E0=4
			//reduces
			{Tuple.Create(4,StateSymbol.Type.Int), Tuple.Create<SrFunction,int>(Shift, 6)},
			{Tuple.Create(4,StateSymbol.Type.Id), Tuple.Create<SrFunction,int>(Shift, 6)},
			//shift
			//{Tuple.Create(4,StateSymbol.Type.EXP), Tuple.Create<SrFunction,int>(Shift, (int)StateSymbol.Type.EXP)},
			//goto
			{Tuple.Create(4,StateSymbol.Type.L_Paren), Tuple.Create<SrFunction,int>(Shift, 4)},
			
			//E1=5

			//shifts
			{Tuple.Create(5,StateSymbol.Type.Op), Tuple.Create<SrFunction,int>(Shift, 4)},
			{Tuple.Create(5,StateSymbol.Type.Rel), Tuple.Create<SrFunction,int>(Shift, 4)},

			//reduces
            //<EXP> -> ( <EXP> op <EXP> )
			{Tuple.Create(5,StateSymbol.Type.R_Paren), Tuple.Create<SrFunction,int>(Reduce, 15)},

			//special reduce 
            //<BOOL> -> <EXP> rel <EXP> || ST - > id _ <EXP>
			{Tuple.Create(5,StateSymbol.Type.EXP), Tuple.Create<SrFunction,int>(Reduce, 9)},
            
            //goto
            {Tuple.Create(5,StateSymbol.Type.EXP), Tuple.Create<SrFunction,int>(GotoState, 9)},

            //I0=6
            //EXP -> int
            {Tuple.Create(6,StateSymbol.Type.Int), Tuple.Create<SrFunction,int>(Reduce, 13)},
            //EXP -> Id
			{Tuple.Create(6,StateSymbol.Type.Id), Tuple.Create<SrFunction,int>(Reduce, 14)},

            //I0=goto
            {Tuple.Create(6,StateSymbol.Type.EXP), Tuple.Create<SrFunction,int>(GotoState, 5)},
            {Tuple.Create(6,StateSymbol.Type.ST), Tuple.Create<SrFunction,int>(GotoState, 7)},
            {Tuple.Create(6,StateSymbol.Type.BOOL), Tuple.Create<SrFunction,int>(GotoState, 8)},

            //S1=7
            {Tuple.Create(7,StateSymbol.Type.ST), Tuple.Create<SrFunction,int>(GotoState, 5)},
            {Tuple.Create(7,StateSymbol.Type.ST), Tuple.Create<SrFunction,int>(GotoState, 7)},
            {Tuple.Create(7,StateSymbol.Type.BOOL), Tuple.Create<SrFunction,int>(GotoState, 8)},

            //B1=8
             */

            

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

            //State 3
            {Tuple.Create(3,StateSymbol.Type.End), Tuple.Create<SrFunction,int>(Shift, 4)},
            //{Tuple.Create(3,StateSymbol.Type.Semicolon), Tuple.Create<SrFunction,int>(Shift, 5)},

            //State 4
            {Tuple.Create(4,StateSymbol.Type.Eof), Tuple.Create<SrFunction,int>(Reduce, 0)},

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
            {Tuple.Create(28,StateSymbol.Type.End), Tuple.Create<SrFunction,int>(Reduce, 15)},
            {Tuple.Create(28,StateSymbol.Type.Then), Tuple.Create<SrFunction,int>(Reduce, 15)},
            {Tuple.Create(28,StateSymbol.Type.Rel), Tuple.Create<SrFunction,int>(Reduce, 15)},
            {Tuple.Create(28,StateSymbol.Type.Op), Tuple.Create<SrFunction,int>(Reduce, 15)},
            {Tuple.Create(28,StateSymbol.Type.Semicolon), Tuple.Create<SrFunction,int>(Reduce, 15)},
            {Tuple.Create(28,StateSymbol.Type.R_Paren), Tuple.Create<SrFunction,int>(Reduce, 15)},

            //State 31
            {Tuple.Create(31,StateSymbol.Type.Assign), Tuple.Create<SrFunction,int>(Shift, 32)},
            //{Tuple.Create(31,StateSymbol.Type.Colon), Tuple.Create<SrFunction,int>(Shift, 34)},

            //State 32
            {Tuple.Create(32,StateSymbol.Type.Id), Tuple.Create<SrFunction,int>(Shift, 23)},
            {Tuple.Create(32,StateSymbol.Type.Int), Tuple.Create<SrFunction,int>(Shift, 22)},
            {Tuple.Create(32,StateSymbol.Type.L_Paren), Tuple.Create<SrFunction,int>(Shift, 24)},
            {Tuple.Create(32,StateSymbol.Type.EXP), Tuple.Create<SrFunction,int>(GotoState, 33)},

            //State 33
            {Tuple.Create(33,StateSymbol.Type.End), Tuple.Create<SrFunction,int>(Reduce, 9)}
		};
	}

	private void Accept(int state)
	{
        Console.WriteLine("Winning");
        keepParsing = false;
	}

	public void Compile(StreamReader input)
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
		switch (state)
		{
            //PROG -> BEGIN <STL> END
			case 0:
				Console.WriteLine("PROG-> BEGIN <STL> END");
				if (symbolStack.Pop().type != StateSymbol.Type.End)
					throw new SystemException("END not found");

				if (symbolStack.Pop().type != StateSymbol.Type.STL)
					throw new SystemException("<STL> not found");

				if (symbolStack.Pop().type != StateSymbol.Type.Begin)
					throw new SystemException("<Begin> not found");
			
				PopStates(3);
				symbolStack.Push(new StateSymbol(StateSymbol.Type.PROG));
				break;
			//STL -> <ST>
			case 1:
				Console.WriteLine("STL -> <ST>");

				if (symbolStack.Pop().type != StateSymbol.Type.ST)
					throw new SystemException("<ST> not found");
			
				PopStates(1);
				symbolStack.Push(new StateSymbol(StateSymbol.Type.STL));
				break;
			//STL -> <STL> ; <ST>
			case 2:
				Console.WriteLine("STL -> <STL> ; <ST>");

                if (symbolStack.Pop().type != StateSymbol.Type.ST)
                    throw new SystemException("<ST> not found");

                if (symbolStack.Pop().type != StateSymbol.Type.Semicolon)
                    throw new SystemException("; not found");

                if (symbolStack.Pop().type != StateSymbol.Type.STL)
                    throw new SystemException("<STL> not found");
			
				PopStates (3);
                symbolStack.Push(new StateSymbol(StateSymbol.Type.STL));
                
               break;
            //ST -> LST
            case 3:
                Console.WriteLine("ST -> LST");

                if(symbolStack.Pop().type != StateSymbol.Type.LST)
                    throw new SystemException("<LST> not found");
			
				PopStates(1);
                symbolStack.Push(new StateSymbol(StateSymbol.Type.ST));
                break;
            //ST -> ULST
            case 4:
                Console.WriteLine("ST -> ULST");

                if (symbolStack.Pop().type != StateSymbol.Type.ULST)
                    throw new SystemException("<ULST> not found");
			
				PopStates(1);
                symbolStack.Push(new StateSymbol(StateSymbol.Type.ST));
                break;
            //LST -> id : <ULST>
            case 5:
                Console.WriteLine("LST -> id : <ULST>");
                break;
            //ULST -> <ASSIGN>
            case 6:
                Console.WriteLine("ULST -> <ASSIGN>");

                if (symbolStack.Pop().type != StateSymbol.Type.ASSIGN)
                    throw new SystemException("<ASSIGN> not found");
			
				PopStates(1);
                symbolStack.Push(new StateSymbol(StateSymbol.Type.ULST));
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

                if(symbolStack.Pop().type != StateSymbol.Type.EXP)
                    throw new SystemException("<EXP> not found");

                if (symbolStack.Pop().type != StateSymbol.Type.Assign)
                    throw new SystemException("Assign not found");

                if (symbolStack.Pop().type != StateSymbol.Type.Id)
                    throw new SystemException("Id not found");
			
				PopStates(3);
                symbolStack.Push(new StateSymbol(StateSymbol.Type.ASSIGN));

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

                if (symbolStack.Pop().type != StateSymbol.Type.Int)
                    throw new SystemException("Int not found");
			
				PopStates(1);
                symbolStack.Push(new StateSymbol(StateSymbol.Type.EXP));
                break;
            //EXP -> id
            case 14:
                Console.WriteLine("EXP -> id");

                if (symbolStack.Pop().type != StateSymbol.Type.Id)
                    throw new SystemException("Id not found");
			
				PopStates(1);
                symbolStack.Push(new StateSymbol(StateSymbol.Type.EXP));
                break;
            //EXP -> ( <EXP> op <EXP> )
            case 15:
                Console.WriteLine("EXP -> ( <EXP> op <EXP> )");

                if (symbolStack.Pop().type != StateSymbol.Type.R_Paren)
                    throw new SystemException(") not found");

                if (symbolStack.Pop().type != StateSymbol.Type.EXP)
                    throw new SystemException("<EXP> not found");

                if (symbolStack.Pop().type != StateSymbol.Type.Op)
                    throw new SystemException("Op not found");

                if (symbolStack.Pop().type != StateSymbol.Type.EXP)
                    throw new SystemException("<EXP> not found");

                if (symbolStack.Pop().type != StateSymbol.Type.L_Paren)
                    throw new SystemException("( not found");
			
				PopStates(5);
                symbolStack.Push(new StateSymbol(StateSymbol.Type.EXP));

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
        //Console.Write("Enter Path to Tokenize:");
        string path = "/storage/dev/Compilers/Final-Project/Source/test1.txt";//Console.ReadLine();
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
