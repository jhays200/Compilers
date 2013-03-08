using System;
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

    public void OutputTest()
    {

        Console.WriteLine("\nAssembly Output");
        Console.WriteLine(sb.ToString());

        Console.WriteLine("\nSymbol Table");
        Console.WriteLine("Address\tId");
        foreach (KeyValuePair<string, int> symbol in idTable)
        {
            Console.WriteLine("{0}\t{1}", symbol.Value, symbol.Key);
        }

        Console.WriteLine("\nConst Table");
        Console.WriteLine("Address\tValue");
        foreach (KeyValuePair<int, int> symbol in constTable)
        {
            Console.WriteLine("{0}\t{1}", symbol.Value, symbol.Key);
        }
    }

    public string GetIdAddrStr(string id)
    {
        if (labelTable.ContainsKey(id))
            return String.Empty;

        if (!idTable.ContainsKey(id))
        {
            id_loc++;
            idTable[id] = id_loc;
        }

        return idTable[id].ToString();
    }

    public void AddLabelPlaceHolder(string label)
    {
        if (labelTable.ContainsKey(label))
            throw new SystemException("Label: " + label + "is defined twice");

        labelTable.Add(label, placeHolderNumber++);
    }

    public int GetLabelPlaceHolder(string label)
    {
        return labelTable[label]);
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
        return Int32.Parse(GetLabelPlaceholder(around_label.ToString()));
    }


}

