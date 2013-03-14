using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;


class CodeGenerator
{
    StringBuilder sb = new StringBuilder();

    const int ID_START = 1000;
    const int CONST_START = 100;
    const int CODE_START = 3000;

    int id_loc;
    int const_loc;
    public int code_loc{get; private set;}
    int current_accu;
    public int around_label { get; private set; }
    int placeHolderNumber;

    Dictionary<string, int> idTable;
    Dictionary<string, int> labelTable;
    Dictionary<int, int> constTable;
    Dictionary<int, int> physicalLabelLocation;

    public CodeGenerator()
    {
        id_loc = ID_START;
        const_loc = CONST_START;
        code_loc = CODE_START;
        current_accu = 0;
        around_label = 0;
        placeHolderNumber = 0;

        idTable = new Dictionary<string, int>();
        labelTable = new Dictionary<string, int>();
        constTable = new Dictionary<int, int>();
        physicalLabelLocation = new Dictionary<int, int>();
    }

    public void WriteLine(string line)
    {
        sb.AppendFormat("{0}: ", code_loc++);
        sb.AppendLine(line);
    }
	
	public string FormattedOutputString()
	{
		//check to make sure the output is a complete range
		using(IEnumerator<KeyValuePair<string,int> > secondIter = labelTable.OrderBy(e => e.Value).GetEnumerator())
		{
			foreach(KeyValuePair<int,int> first in physicalLabelLocation.OrderBy(e => e.Key))
			{
				if(!secondIter.MoveNext())
					throw new SystemException("Label table is missing entries, so Code Generator or SrParser needs fixing");
				
				if(secondIter.Current.Value != first.Key)
					throw new SystemException("Label " + secondIter.Current.Key + " was undefined");			
			}
			
			if(secondIter.MoveNext())
					throw new SystemException("Label table has too many entries, so Code Generator or SrParser needs fixing");
		}
		
		object[] placeholdersToReplace = physicalLabelLocation.OrderBy(e => e.Key).Select(e => e.Value).ToArray().Cast<object>().ToArray();
		
		return String.Format(sb.ToString(), placeholdersToReplace);
	}

    public void OutputTest()
    {
		int notUsed;
		//Console.WriteLine("Preformatted Assembly");
		//Console.WriteLine (sb.ToString());
		
        Console.WriteLine("\nAssembly Output");
        Console.WriteLine(FormattedOutputString());
		
        Console.WriteLine("\nSymbol Table");
        Console.WriteLine("Type\t\tAddress\tId");
        foreach (KeyValuePair<string, int> symbol in idTable.Where (e => !Int32.TryParse(e.Key, out notUsed)))
        {
            Console.WriteLine("VAR\t\t{0}\t\t{1}", symbol.Value, symbol.Key);
        }
		foreach(KeyValuePair<string,int> i in (from placeholders in labelTable
		                  join physicalLocation in physicalLabelLocation on placeholders.Value equals physicalLocation.Key
		                  select new KeyValuePair<string,int>(placeholders.Key, physicalLocation.Value)))
		{
			Console.WriteLine("LABEL\t\t{0}\t\t{1}", i.Value, i.Key);
		}
		

        Console.WriteLine("\nConst Table");
        Console.WriteLine("Address\tValue");
        foreach (KeyValuePair<int, int> symbol in constTable)
        {
            Console.WriteLine("{0}\t\t{1}", symbol.Value, symbol.Key);
        }
    }

    public string GetIdAddrStr(string id)
    {
		int tmp = 0;
		
		if(!Int32.TryParse(id, out tmp)
		   && labelTable.ContainsKey(id))
		{
			throw new SystemException("id " + id + " is already defined as a label");
		}
		
		if(!idTable.ContainsKey(id))
		{
			idTable[id] = id_loc++;
		}

        return idTable[id].ToString();
    }

    public int AddLabelPlaceHolder(string label)
    {
        if (labelTable.ContainsKey(label))
            throw new SystemException("Label " + label + " is defined twice");
		if(idTable.ContainsKey(label))
			throw new SystemException("Label " + label + " can not be used as an id and a label");

        labelTable.Add(label, placeHolderNumber++);
		return labelTable[label];
    }

    public int GetLabelPlaceHolder(string label)
    {
		if (!labelTable.ContainsKey(label))
			throw new SystemException("Label " + label + " was not defined");
        return labelTable[label];
    }

    public string GetConstAddrStr(string intValue)
    {
        int value = Int32.Parse(intValue);

        if (!constTable.ContainsKey(value))
        {
            const_loc++;
            constTable[value] = const_loc;
        }

        return constTable[value].ToString();
    }

    public void SetPhysicalLabelLocation(int placeHolder)
    {
        physicalLabelLocation[placeHolder] = code_loc;
    }

    public string NewAccu()
    {
		++current_accu;
        return GetIdAddrStr(current_accu.ToString());
    }

    public void FreeAccu()
    {
        --current_accu;
    }

    public string GetAccu()
    {
        return idTable[current_accu.ToString()].ToString();
    }

    public int NewAroundLabel()
    {
        ++around_label;
		labelTable.Add (around_label.ToString(), placeHolderNumber++);
        return labelTable[around_label.ToString()];
    }
}

