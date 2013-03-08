using System;

using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

class Tokenizer
{
	//internal data
    const int QUEUE_LENGTH = 12;
	private readonly Dictionary<string, StateSymbol> tokenTable;
    private TextReader inputStream;
    private Queue<StateSymbol> tokenQueue;
	string buffer;
    
	//public scanner information
	public StateSymbol currentSymbol { get; private set; }
    public string currentLine { get; private set; }
    public int lineNumber { get; private set; }
    public int colNumber { get; private set; }

	//regex for different stuff
	Regex numberRx;
	Regex validBufferRx;

    public Tokenizer(TextReader input)
    {
        inputStream = input;
        tokenQueue = new Queue<StateSymbol>(QUEUE_LENGTH);
        lineNumber = 1;
        colNumber = 0;
		buffer = String.Empty;

		tokenTable = new Dictionary<string, StateSymbol>()
		{
			{"_",new StateSymbol(StateSymbol.Type.Assign, "_")},
			{"(",new StateSymbol(StateSymbol.Type.L_Paren, "(")},
			{")", new StateSymbol(StateSymbol.Type.R_Paren, ")")},
			{"+", new StateSymbol(StateSymbol.Type.Op, "+")},
			{"-", new StateSymbol(StateSymbol.Type.Op, "-")},
			{"*", new StateSymbol(StateSymbol.Type.Op, "*")},
			{"/", new StateSymbol(StateSymbol.Type.Op, "/")},
			{"=", new StateSymbol(StateSymbol.Type.Rel, "=")},
			{">", new StateSymbol(StateSymbol.Type.Rel, ">")},
			{"<", new StateSymbol(StateSymbol.Type.Rel, "<")},
			{"IF ", new StateSymbol(StateSymbol.Type.If, "IF")},
			{"THEN ", new StateSymbol(StateSymbol.Type.Then, "THEN")},
			{"END", new StateSymbol(StateSymbol.Type.End, "END")},
			{"BEGIN ", new StateSymbol(StateSymbol.Type.Begin, "BEGIN")},
			{"GOTO ", new StateSymbol(StateSymbol.Type.Goto, "GOTO")},
			{";", new StateSymbol(StateSymbol.Type.Semicolon, ";")},
			{":", new StateSymbol(StateSymbol.Type.Colon, ":")}
		};

		numberRx = new Regex(@"^[0-9]+$", RegexOptions.Compiled);
		validBufferRx = new Regex(@"^([a-zA-Z][a-zA-Z0-9]*|[0-9]+)?$", RegexOptions.Compiled);
		//validCharRx = new Regex(@"[a-zA-Z0-9_()+-*/=><;:]", RegexOptions.Compiled);
		
		//currentLine = input.ReadLine();
		BufferTokens();
		GetNextSymbol();
    }

    public StateSymbol GetNextSymbol()
    {
        if (tokenQueue.Count == 0)
        {
			BufferTokens();
        }
        
		currentSymbol = tokenQueue.Dequeue();
		return currentSymbol;
    }

    void BufferTokens()
    {
        currentLine = inputStream.ReadLine();
        //We have reached the end of file
        if (currentLine == null)
        {
			ClearSymbolBuffer();
            tokenQueue.Enqueue(new StateSymbol(StateSymbol.Type.Eof, "$"));
            currentSymbol = new StateSymbol(StateSymbol.Type.Eof, "$");
            return;
        }

        //now go character by character
        for (colNumber = 0; colNumber < currentLine.Length; ++colNumber)
        {
            ProcessLetter(currentLine[colNumber]);
        }
        ProcessLetter(' ');

    }

	void ClearSymbolBuffer()
	{
		if (buffer.Length == 0)
			return;

		if (numberRx.IsMatch(buffer))
		{
			tokenQueue.Enqueue(new StateSymbol(StateSymbol.Type.Int, buffer));
			buffer = String.Empty;
		}
		else
		{
			tokenQueue.Enqueue(new StateSymbol(StateSymbol.Type.Id, buffer));
			buffer = String.Empty;
		}
	}

	void ProcessLetter(char c)
	{
		string newBuffer = buffer + c;
		//this one character types
		if (tokenTable.ContainsKey(c.ToString()))
		{
			ClearSymbolBuffer();
			tokenQueue.Enqueue(tokenTable[c.ToString()]);
		}
		//keywords
		else if (tokenTable.ContainsKey(newBuffer))
		{
				tokenQueue.Enqueue(tokenTable[newBuffer]);
				buffer = String.Empty;
		}
		else
		{
			//ignore whitespaces for id and numbers
			if (c != ' ' && c != '\t')
				buffer = newBuffer;
			//Make sure the buffer only holds an id or number
			else if (validBufferRx.IsMatch(buffer))
			{
				ClearSymbolBuffer();
			}
			else
				throw new System.Exception(String.Format("Error on line {0} and col {1}", lineNumber, colNumber));
		}
	}

}
